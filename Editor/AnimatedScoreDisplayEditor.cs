using System;
using CommonUtils.UI;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor {
	[CustomEditor(typeof(AnimatedScoreDisplay))]
	public class AnimatedScoreDisplayEditor : UnityEditor.Editor {
		private AnimatedScoreDisplay animatedScoreDisplay;
		private int scoreToSet;
		private bool animated = true;

		private void OnEnable() => animatedScoreDisplay = (AnimatedScoreDisplay)target;

		public override void OnInspectorGUI() {
			if (Application.isPlaying) {
				EditorExtensions.BoxGroup(() => {
					animated = EditorGUILayout.Toggle("Animated", animated);
					EditorGUILayout.BeginHorizontal();
					scoreToSet = EditorGUILayout.IntField(scoreToSet);
					if (GUILayout.Button("Set Score")) {
						animatedScoreDisplay.SetScore(scoreToSet, animated);
					}
					EditorGUILayout.EndHorizontal();
				}, "Debug");
			}

			DrawDefaultInspector();
		}
	}
}
