using CommonUtils.Extensions;
using CommonUtils.Inspector.HelpBox;
using System.Collections.Generic;
using UnityEngine;

namespace CommonUtils.DebuggableEditors {
	[ExecuteInEditMode]
	public class DebuggableEditorGameView : MonoBehaviour {
		private const float DEFAULT_HEIGHT_PCT = 0.3f;

		[HelpBox("Please note this component will only work in the Unity Editor and Development Builds.", HelpBoxMessageType.Warning)]
		[SerializeField] [Range(0f, 1f)] private float positionX;
		[SerializeField] [Range(0f, 1f)] private float positionY;
		[SerializeField] [Range(0f, 1f)] private float width = 1f;
		[SerializeField] private Color backgroundColor = Color.black;
		[SerializeField] private Color fontColor = Color.white;
		[SerializeField] private int fontSize = 15;
		[SerializeField] private MonoBehaviour debuggableComponent;
		[SerializeField] private bool debugAllProperties;

		private void Awake() {
			if (!Debug.isDebugBuild) enabled = false;
		}

		private void OnGUI() {
			GUI.color = fontColor;
			GUI.backgroundColor = backgroundColor;
			GUI.skin.box.fontSize = GUI.skin.label.fontSize = Mathf.Min(Mathf.FloorToInt(Screen.width * fontSize/(float)1000), Mathf.FloorToInt(Screen.height * fontSize/(float)1000)); // This adjusts the font size to different screen sizes

			var componentName = "";
			ICollection<ReflectedProperty> reflectedProperties = null;
			if (debuggableComponent) {
				var debuggableComponentType = debuggableComponent.GetType();
				componentName = debuggableComponentType.Name.PascalToTitleCase();
				reflectedProperties = debuggableComponentType.GetDebuggableProperties(debugAllProperties);
			}

			var rowSize = GUI.skin.box.CalcSize(new GUIContent(componentName));
			var actualRect = new Rect(
				widthPercentageToPixels(positionX),
				heightPercentageToPixels(positionY),
				widthPercentageToPixels(width),
				(reflectedProperties is { Count: > 0 } ? (reflectedProperties.Count + 1) * rowSize.y : heightPercentageToPixels(DEFAULT_HEIGHT_PCT)));

			GUI.Box(actualRect, componentName);

			if (reflectedProperties != null) {
				var i = 1;
				foreach (var reflectedProperty in reflectedProperties) {
					var val = "-";
					if (Application.isPlaying) {
						val = reflectedProperty.GetValue(debuggableComponent)?.ToString() ?? "<null>";
					}

					GUI.Label(new Rect(actualRect.x, actualRect.y + (rowSize.y * i), Screen.width, Screen.height),
						$"{reflectedProperty.DisplayName}: {val}");
					i++;
				}
			}
		}

		private float widthPercentageToPixels(float percent) => Screen.width * percent;
		private float heightPercentageToPixels(float percent) => Screen.height * percent;

		private Rect rectPercentageToPixels(Rect percents) => new Rect(
			widthPercentageToPixels(percents.x),
			heightPercentageToPixels(percents.y),
			widthPercentageToPixels(percents.width),
			heightPercentageToPixels(percents.height));
	}
}