using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor
{
    public enum EditorIcon
    {
		ConsoleWindow,
		GameObject,
		SceneAsset,
		PrefabNormal,
		PrefabModel,
		PrefabVariant,
		Folder,
		FolderEmpty,
		Favorite,
		AudioSource,
		Camera,
		Light,
		Terrain,
		MeshRenderer,
		ParticleSystem,
		AnimatorController,
		TextAsset,
		ScriptIcon,
		Rigidbody,
		PlayButton,
		PauseButton,
		StopButton,
		Help,
		Search,
		BuildSettings,
		PackageManager,
		Profiler
    }

	public static class EditorIconExtensions {
		public static string GetIconName(this EditorIcon icon) => icon switch {
			EditorIcon.ConsoleWindow => "d_UnityEditor.ConsoleWindow",
			EditorIcon.GameObject => "d_GameObject Icon",
			EditorIcon.SceneAsset => "d_SceneAsset Icon",
			EditorIcon.PrefabNormal => "d_Prefab Icon",
			EditorIcon.PrefabModel => "d_ModelPrefab Icon",
			EditorIcon.PrefabVariant => "d_PrefabVariant Icon",
			EditorIcon.Folder => "d_Folder Icon",
			EditorIcon.FolderEmpty => "d_FolderEmpty Icon",
			EditorIcon.Favorite => "Favorite Icon",
			EditorIcon.AudioSource => "d_AudioSource Icon",
			EditorIcon.Camera => "d_Camera Icon",
			EditorIcon.Light => "d_Light Icon",
			EditorIcon.Terrain => "d_Terrain Icon",
			EditorIcon.MeshRenderer => "d_MeshRenderer Icon",
			EditorIcon.ParticleSystem => "d_ParticleSystem Icon",
			EditorIcon.AnimatorController => "d_AnimatorController Icon",
			EditorIcon.TextAsset => "d_TextAsset Icon",
			EditorIcon.ScriptIcon => "d_cs Script Icon",
			EditorIcon.Rigidbody => "d_Rigidbody Icon",
			EditorIcon.PlayButton => "d_PlayButton",
			EditorIcon.PauseButton => "d_PauseButton",
			EditorIcon.StopButton => "d_PreMatQuad",
			EditorIcon.Help => "d_UnityEditor.InspectorWindow",
			EditorIcon.Search => "d_Search Icon",
			EditorIcon.BuildSettings => "d_BuildSettings.Editor",
			EditorIcon.PackageManager => "d_PackageManager Icon",
			EditorIcon.Profiler => "d_Profiler Icon",
			_ => null
		};

		public static void Draw(this EditorIcon icon, float width = 16f, float height = 16f)
		{
			var iconName = icon.GetIconName();
			if (string.IsNullOrEmpty(iconName)) return;

			var content = EditorGUIUtility.IconContent(iconName);
			GUILayout.Label(content, GUILayout.Width(width), GUILayout.Height(height));
		}

		public static GUIContent ToGUIContent(this EditorIcon icon, string tooltip = null) {
			var iconName = icon.GetIconName();
			if(string.IsNullOrEmpty(iconName)) return null;
			var result = EditorGUIUtility.IconContent(iconName);
			if(string.IsNullOrEmpty(tooltip)) result.tooltip = tooltip;
			return result;
		}
	}
}
