using CommonUtils.Inspector.SceneRefs;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.Inspector.SceneRefs {
	/// <summary>
	/// Display a Scene Reference object in the editor.
	/// If scene is valid, provides basic buttons to interact with the scene's role in Build Settings.
	/// </summary>
	[CustomPropertyDrawer(typeof(SceneReference))]
	public class SceneReferencePropertyDrawer : AbstractBoxedPropertyDrawer {
		// The exact name of the asset Object variable in the SceneReference object
		private const string sceneAssetPropertyString = "sceneAsset";

		// The exact name of  the scene Path variable in the SceneReference object
		private const string scenePathPropertyString = "scenePath";

		/// <summary>
		/// Drawing the 'SceneReference' property
		/// </summary>
		protected override void DrawBoxContents(Rect position, SerializedProperty property, GUIContent label) {
			var sceneAssetProperty = GetSceneAssetProperty(property);

			// Draw the main Object field
			label.tooltip = "The actual Scene Asset reference.\nOn serialize this is also stored as the asset's path.";

			EditorGUI.BeginProperty(position, GUIContent.none, property);
			EditorGUI.BeginChangeCheck();
			var sceneControlID = GUIUtility.GetControlID(FocusType.Passive);
			var selectedObject = EditorGUI.ObjectField(position,
													   label,
													   sceneAssetProperty.objectReferenceValue,
													   typeof(SceneAsset),
													   false);
			var buildScene = BuildUtils.GetBuildScene(selectedObject);

			if (EditorGUI.EndChangeCheck()) {
				sceneAssetProperty.objectReferenceValue = selectedObject;

				// If no valid scene asset was selected, reset the stored path accordingly
				if (buildScene.scene == null)
					GetScenePathProperty(property).stringValue = string.Empty;
			}

			position.y += PaddedLine;

			if (buildScene.assetGUID.Empty() == false) {
				// Draw the Build Settings Info of the selected Scene
				DrawSceneInfoGUI(position, buildScene, sceneControlID + 1);
			}

			EditorGUI.EndProperty();
		}

		protected override int GetLineCount(SerializedProperty property, GUIContent label) {
			var lines              = 2;
			var sceneAssetProperty = GetSceneAssetProperty(property);
			if (sceneAssetProperty.objectReferenceValue == null)
				lines = 1;
			return lines;
		}

		/// <summary>
		/// Draws info box of the provided scene
		/// </summary>
		private void DrawSceneInfoGUI(Rect position, BuildUtils.BuildScene buildScene, int sceneControlID) {
			var readOnly = BuildUtils.IsReadOnly();
			var readOnlyWarning =
				readOnly ? "\n\nWARNING: Build Settings is not checked out and so cannot be modified." : "";

			// Label Prefix
			var iconContent  = new GUIContent();
			var labelContent = new GUIContent();

			// Missing from build scenes
			if (buildScene.buildIndex == -1) {
				iconContent          = EditorGUIUtility.IconContent("d_winbtn_mac_close");
				labelContent.text    = "NOT In Build";
				labelContent.tooltip = "This scene is NOT in build settings.\nIt will be NOT included in builds.";
			}

			// In build scenes and enabled
			else if (buildScene.scene.enabled) {
				iconContent       = EditorGUIUtility.IconContent("d_winbtn_mac_max");
				labelContent.text = "BuildIndex: " + buildScene.buildIndex;
				labelContent.tooltip = "This scene is in build settings and ENABLED.\nIt will be included in builds." +
									   readOnlyWarning;
			}

			// In build scenes and disabled
			else {
				iconContent       = EditorGUIUtility.IconContent("d_winbtn_mac_min");
				labelContent.text = "BuildIndex: " + buildScene.buildIndex;
				labelContent.tooltip =
					"This scene is in build settings and DISABLED.\nIt will be NOT included in builds.";
			}

			// Left status label
			using (new EditorGUI.DisabledScope(readOnly)) {
				var labelRect = PropertyDrawerUtils.GetLabelRect(position, PadSize);
				var iconRect  = labelRect;
				iconRect.width  =  iconContent.image.width + PadSize;
				labelRect.width -= iconRect.width;
				labelRect.x     += iconRect.width;
				EditorGUI.PrefixLabel(iconRect,  sceneControlID, iconContent);
				EditorGUI.PrefixLabel(labelRect, sceneControlID, labelContent);
			}

			// Right context buttons
			var buttonRect = PropertyDrawerUtils.GetFieldRect(position);
			buttonRect.width = (buttonRect.width) / 3;

			var tooltipMsg = "";
			using (new EditorGUI.DisabledScope(readOnly)) {
				// NOT in build settings
				if (buildScene.buildIndex == -1) {
					buttonRect.width *= 2;
					var addIndex = EditorBuildSettings.scenes.Length;
					tooltipMsg =
						"Add this scene to build settings. It will be appended to the end of the build scenes as buildIndex: " +
						addIndex                                                                                               +
						"."                                                                                                    + readOnlyWarning;
					if (PropertyDrawerUtils.ButtonHelper(buttonRect,
											   "Add...",
											   "Add (buildIndex " + addIndex + ")",
											   EditorStyles.miniButtonLeft,
											   tooltipMsg))
						BuildUtils.AddBuildScene(buildScene);
					buttonRect.width /= 2;
					buttonRect.x     += buttonRect.width;
				}

				// In build settings
				else {
					var isEnabled   = buildScene.scene.enabled;
					var stateString = isEnabled ? "Disable" : "Enable";
					tooltipMsg = stateString + " this scene in build settings.\n" +
								 (isEnabled ?
									  "It will no longer be included in builds" :
									  "It will be included in builds") + "." + readOnlyWarning;

					if (PropertyDrawerUtils.ButtonHelper(buttonRect,
											   stateString,
											   stateString + " In Build",
											   EditorStyles.miniButtonLeft,
											   tooltipMsg))
						BuildUtils.SetBuildSceneState(buildScene, !isEnabled);
					buttonRect.x += buttonRect.width;

					tooltipMsg =
						"Completely remove this scene from build settings.\nYou will need to add it again for it to be included in builds!" +
						readOnlyWarning;
					if (PropertyDrawerUtils.ButtonHelper(buttonRect,
											   "Remove...",
											   "Remove from Build",
											   EditorStyles.miniButtonMid,
											   tooltipMsg))
						BuildUtils.RemoveBuildScene(buildScene);
				}
			}

			buttonRect.x += buttonRect.width;

			tooltipMsg = "Open the 'Build Settings' Window for managing scenes." + readOnlyWarning;
			if (PropertyDrawerUtils.ButtonHelper(buttonRect,
									   "Settings",
									   "Build Settings",
									   EditorStyles.miniButtonRight,
									   tooltipMsg)) {
				BuildUtils.OpenBuildSettings();
			}

		}

		private static SerializedProperty GetSceneAssetProperty(SerializedProperty property) => property.FindPropertyRelative(sceneAssetPropertyString);

		private static SerializedProperty GetScenePathProperty(SerializedProperty property) => property.FindPropertyRelative(scenePathPropertyString);
	}
}