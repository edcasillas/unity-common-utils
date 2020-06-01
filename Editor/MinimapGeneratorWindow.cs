using System;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace CommonUtils.Editor {
	public class MinimapGeneratorWindow : EditorWindow {
		private static MinimapGeneratorWindow instance = null;

		private static int cullingMask = ~0;
		private static CameraClearFlags clearFlags;
		private static Color backgroundColor = new Color(.2f, .3f, .47f, 0f);
		private static int minimapLayer;
		private static int maxTextureWidth = 2048;
		private static bool upsample2x;

		private readonly Quaternion camRotation = Quaternion.Euler(90,0,0);
		private readonly BoxBoundsHandle boundsHandle = new BoxBoundsHandle();

		private bool editingBounds;
		private Bounds minimapBounds = new Bounds(Vector3.zero, Vector3.one * 10);
		private string savePath;
		private string minimapName;

		private Vector3 camPosition => new Vector3(minimapBounds.center.x, minimapBounds.center.y + minimapBounds.extents.y, minimapBounds.center.z);

		[MenuItem("Tools/Generate minimap...")]
		private static void OpenActiveWindow() {
			if (!instance) {
				instance = GetWindow<MinimapGeneratorWindow>();
				instance.titleContent = new GUIContent("Minimap Generator");
				instance.minSize = new Vector2(300,300);
				instance.savePath = Application.dataPath;

				// TODO instance.maxSize
			}
			instance.Show();
			instance.Focus();
		}

		// Window has been selected
		private void OnFocus() {
#if UNITY_2019_1_OR_NEWER
            // Remove delegate listener if it has previously
			// been assigned.
            SceneView.duringSceneGui  -= this.onSceneGUI;

			// Add (or re-add) the delegate.
			SceneView.duringSceneGui  += this.onSceneGUI;
#else
			// Remove delegate listener if it has previously
			// been assigned.
			SceneView.onSceneGUIDelegate -= onSceneGUI;

			// Add (or re-add) the delegate.
			SceneView.onSceneGUIDelegate += onSceneGUI;
#endif
		}

		private void OnDestroy() {
			// When the window is destroyed, remove the delegate
			// so that it will no longer do any drawing.
#if UNITY_2019_1_OR_NEWER
			SceneView.duringSceneGui -= this.onSceneGUI;
#else
			SceneView.onSceneGUIDelegate -= onSceneGUI;
#endif
		}

		private void OnGUI() {
			clearFlags = (CameraClearFlags)EditorGUILayout.EnumPopup("Clear Flags", clearFlags);
			backgroundColor = EditorGUILayout.ColorField("Background color", backgroundColor);
			cullingMask = EditorGUILayout.MaskField(new GUIContent("Culling Mask", "Culling mask to be used by the camera that will take the picture of the map."),  cullingMask, InternalEditorUtility.layers);
			minimapLayer = EditorGUILayout.LayerField(new GUIContent("Minimap Layer", "Layer to be applied to the minimap."), minimapLayer);
			maxTextureWidth = EditorGUILayout.IntField("Max Texture Width", maxTextureWidth);
			upsample2x = EditorGUILayout.Toggle("Upsample 2x", upsample2x);

			EditorGUI.BeginChangeCheck();
			minimapBounds = EditorGUILayout.BoundsField("Bounds", minimapBounds);

			var boundsSource = (Renderer)EditorGUILayout.ObjectField("Copy bounds from object", null, typeof(Renderer), true);
			if (boundsSource) {
				minimapBounds = boundsSource.bounds;
			}

			if (GUILayout.Button("Edit bounds")) { editingBounds = true; }
			if (GUILayout.Button("Hide bounds in Scene View")) { editingBounds = false; }

			if (editingBounds && EditorGUI.EndChangeCheck()) {
				SceneView.RepaintAll();
			}

			if (GUILayout.Button("Create preview camera")) {
				var previewCam = createCamera();
				previewCam.name = "MinimapPreviewCamera";
			}

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Save to:", getSaveRelativePath());
			if (GUILayout.Button("Select save location")) {
				var newSavePath = EditorUtility.SaveFolderPanel("Choose save location", savePath, SceneManager.GetActiveScene().name);
				if(string.IsNullOrEmpty(newSavePath)) return;
				if (!newSavePath.Contains(Application.dataPath)) {
					EditorUtility.DisplayDialog("Error", "Save location must be within the Assets folder of this project", "Ok");
					return;
				}

				savePath = newSavePath;
			}

			minimapName = EditorGUILayout.TextField("Minimap name", string.IsNullOrEmpty(minimapName) ? SceneManager.GetActiveScene().name : minimapName).Trim();

			if (!string.IsNullOrWhiteSpace(savePath)) {
				EditorGUILayout.Space();
				if (GUILayout.Button("Generate minimap")) {
					generateMinimap();
				}
			}
		}

		private void onSceneGUI(SceneView sceneView) {
			if(!editingBounds) return;
			Handles.PositionHandle(camPosition, camRotation);

			boundsHandle.center = minimapBounds.center;
			boundsHandle.size = minimapBounds.size;

			EditorGUI.BeginChangeCheck();
			Handles.color = Color.green;
			boundsHandle.DrawHandle();
			if (EditorGUI.EndChangeCheck()) {
				// copy the handle's updated data back to the target object
				minimapBounds.center = boundsHandle.center;
				minimapBounds.size = boundsHandle.size;
			}
		}

		private void generateMinimap() {
			if (string.IsNullOrEmpty(savePath) || !savePath.Contains(Application.dataPath)) return;
			var relativePath = getSaveRelativePath();

			var b = minimapBounds;
			var boundsOG = new Vector2((b.center.x + b.extents.x) - (b.center.x - b.extents.x), (b.center.z + b.extents.z) - (b.center.z - b.extents.z));
			var bounds = boundsOG;
			var boundsCenter = new Vector2((b.center.x - b.extents.x) + bounds.x * 0.5f, (b.center.z - b.extents.z) + bounds.y * 0.5f);

			var maxTextureWidthToUse = maxTextureWidth;

			if (upsample2x) {
				maxTextureWidthToUse *= 2;
			}

			if (bounds.x > bounds.y) {
				bounds.y = bounds.y / bounds.x * maxTextureWidthToUse;
				bounds.x = maxTextureWidthToUse;
			} else {
				bounds.x = bounds.x / bounds.y * maxTextureWidthToUse;
				bounds.y = maxTextureWidthToUse;
			}

			var intBounds = new Vector2Int((int)bounds.x, (int)bounds.y);

			var rt = new RenderTexture(intBounds.x, intBounds.y, 24);

			var newCamera = createCamera();
			newCamera.targetTexture = rt;

			int oldMaxLODLevel = QualitySettings.maximumLODLevel;
			var oldShadowsValue = QualitySettings.shadows;
			QualitySettings.maximumLODLevel = 0;
			QualitySettings.shadows = ShadowQuality.Disable;
			newCamera.Render();
			QualitySettings.maximumLODLevel = oldMaxLODLevel;
			QualitySettings.shadows = oldShadowsValue;

			RenderTexture.active = rt;
			Texture2D virtualPhoto = new Texture2D(intBounds.x, intBounds.y, TextureFormat.RGB24, false);

			// false, meaning no need for mipmaps
			virtualPhoto.ReadPixels(new Rect(0, 0, intBounds.x, intBounds.y), 0, 0); // you get the center section

			if (upsample2x) {
				Vector2Int newBounds = new Vector2Int(intBounds.x / 2, intBounds.y / 2);
				//Debug.Log("bilinear scale to " + newBounds);
				TextureScale.Bilinear(virtualPhoto, newBounds.x, newBounds.y);
			}

			RenderTexture.active = null; // "just in case"
			newCamera.targetTexture = null;
			DestroyImmediate(rt);

			var bytes = virtualPhoto.EncodeToPNG();

			var textureSavePath = $"{savePath}/{minimapName}.png";

			File.WriteAllBytes(textureSavePath, bytes);

			DestroyImmediate(newCamera.gameObject);

			var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
			go.name = minimapName;
			go.layer = minimapLayer;
			go.transform.position = new Vector3(boundsCenter.x, minimapBounds.center.y - minimapBounds.extents.y, boundsCenter.y);
			go.transform.localScale = new Vector3(boundsOG.x, boundsOG.y, 1);
			go.transform.rotation = Quaternion.Euler(90, 0, 0);

			var materialPath = $"{relativePath}/{minimapName}.mat";
			var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
			if (material == null) {
				material = new Material(Shader.Find("Unlit/Texture"));
				AssetDatabase.CreateAsset(material, materialPath);
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>($"{relativePath}/{minimapName}.png");

			var meshRenderer = go.GetComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = material;
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			meshRenderer.receiveShadows = false;

			EditorUtility.DisplayDialog("Success!", "Minimap was successfully generated.", "Ok");
		}

		private Camera createCamera() {
			var result = new GameObject().AddComponent<Camera>();
			result.orthographic = true;
			result.farClipPlane = minimapBounds.size.y;
			result.orthographicSize = minimapBounds.extents.z; //as orthographic size is based on height of camera frustum
			result.cullingMask = cullingMask;
			result.clearFlags = clearFlags;
			result.backgroundColor = backgroundColor;
			result.transform.position = camPosition;
			result.transform.rotation = camRotation;
			return result;
		}

		private string getSaveRelativePath() {
			try {
				return !string.IsNullOrEmpty(savePath) ? savePath.Substring(savePath.IndexOf("/Assets", StringComparison.Ordinal) +1) : string.Empty;
			} catch {
				return savePath;
			}
		}
	}
}