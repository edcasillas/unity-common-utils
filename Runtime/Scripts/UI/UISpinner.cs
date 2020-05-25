using UnityEngine;

namespace CommonUtils.UI {
	[AddComponentMenu("UI/UI Spinner")]
	[RequireComponent(typeof(RectTransform))]
	public class UISpinner : MonoBehaviour {
		#pragma warning disable 649
		[SerializeField] private float speed = 300f;
		#pragma warning restore 649

		private RectTransform tr;

		private void Awake() => tr = GetComponent<RectTransform>();

		private void Update() => tr.Rotate(0,0,speed * Time.deltaTime);
	}
}
