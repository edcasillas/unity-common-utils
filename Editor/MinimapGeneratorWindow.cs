using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace CommonUtils.Editor {
	public class MinimapGeneratorWindow : EditorWindow {
		private static MinimapGeneratorWindow instance = null;

		private static int minimapLayer;
		private static int maxTextureWidth = 2048;
		private static bool upsample2x;

		private readonly Quaternion camRotation = Quaternion.Euler(90,0,0);
		private readonly BoxBoundsHandle boundsHandle = new BoxBoundsHandle();

		private bool editingBounds;
		private Bounds minimapBounds = new Bounds(Vector3.zero, Vector3.one * 10);

		private Vector3 camPosition => new Vector3(minimapBounds.center.x, minimapBounds.center.y + minimapBounds.extents.y, minimapBounds.center.z);

		[MenuItem("Tools/Generate minimap...")]
		private static void OpenActiveWindow() {
			if (!instance) {
				Debug.Log("creating");
				instance = GetWindow<MinimapGeneratorWindow>();
				instance.titleContent = new GUIContent("Minimap Generator");
				instance.minSize = new Vector2(300,220);
				// TODO instance.maxSize
			}
			instance.Show();
			instance.Focus();
		}
/*
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
			SceneView.onSceneGUIDelegate -= this.onSceneGUI;

			// Add (or re-add) the delegate.
			SceneView.onSceneGUIDelegate += this.onSceneGUI;
#endif
		}

		private void OnDestroy() {
			// When the window is destroyed, remove the delegate
			// so that it will no longer do any drawing.
#if UNITY_2019_1_OR_NEWER
			SceneView.duringSceneGui -= this.onSceneGUI;
#else
			SceneView.onSceneGUIDelegate -= this.onSceneGUI;
#endif
		}
*/
		private void OnGUI() {
			minimapLayer = EditorGUILayout.LayerField("Minimap Layer", minimapLayer);
			maxTextureWidth = EditorGUILayout.IntField("Max Texture Width", maxTextureWidth);
			upsample2x = EditorGUILayout.Toggle("Upsample 2x", upsample2x);

			EditorGUI.BeginChangeCheck();
			//camPosition = EditorGUILayout.Vector3Field("Camera position", camPosition);
			//camRotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Camera rotation", camRotation.eulerAngles));
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
				/*var previewCam = createCamera();
				previewCam.name = "MinimapPreviewCamera";*/
			}

			EditorGUILayout.Space();
			if (GUILayout.Button("Generate minimap")) {
				//generateMinimap();
			}
		}
/*
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

		}

		private Camera createCamera() {
			var result = new GameObject().AddComponent<Camera>();
			result.orthographic = true;
			result.farClipPlane = minimapBounds.size.y;
			result.orthographicSize = minimapBounds.extents.z; //as orthographic size is based on height of camera frustum
			result.transform.position = camPosition;
			result.transform.rotation = camRotation;
			return result;
		}
		*/
	}
}