using CommonUtils.LegacyGUI;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Demo : MonoBehaviour {
	[SerializeField] private List<Rect> nodes;
	[SerializeField] private Color lineColor = new Color(0.3f, 0.7f, 0.4f);
	[SerializeField] private Color shadowColor = new Color(0.4f, 0.4f, 0.5f);

	private void OnGUI() {
		for (var i = 0; i < nodes.Count - 1; i++) curveFromTo(nodes[i], nodes[i + 1], lineColor, shadowColor);

		//BeginWindows();
		for (var i = 0; i < nodes.Count; i++) nodes[i] = GUI.Window(i, nodes[i], doWindow, $"Node {i}");
		//EndWindows();
	}

	private static void doWindow(int id) {
		GUI.Button(new Rect(0, 30, 100, 50), "Wee!");
		GUI.DragWindow();
	}

	private void curveFromTo(Rect from, Rect to, Color color, Color shadow) {
		GUILines.DrawBezier(
			new Vector2(from.x + from.width, from.y + 3 + from.height / 2),
			new Vector2(from.x + from.width + Mathf.Abs(to.x - (from.x + from.width)) / 2, from.y + 3 + from.height / 2),
			new Vector2(to.x, to.y + 3 + to.height / 2),
			new Vector2(to.x - Mathf.Abs(to.x - (from.x + from.width)) / 2, to.y + 3 + to.height / 2), shadow, 5, true,20);
		GUILines.DrawBezier(
			new Vector2(from.x + from.width, from.y + from.height / 2),
			new Vector2(from.x + from.width + Mathf.Abs(to.x - (from.x + from.width)) / 2, from.y + from.height / 2),
			new Vector2(to.x, to.y + to.height / 2),
			new Vector2(to.x - Mathf.Abs(to.x - (from.x + from.width)) / 2, to.y + to.height / 2), color, 2, true,20);
	}
}
