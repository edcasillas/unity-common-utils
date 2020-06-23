using System;
using CommonUtils.Inspector.HelpBox;
using UnityEngine;
using UnityEngine.Events;

namespace CommonUtils.Input {
	[AddComponentMenu("UI/Android Buttons Listener")]
	public class AndroidButtonsListener : MonoBehaviour {
		#pragma warning disable 649
		[SerializeField] private UnityEvent onBackButtonPressed;
		[SerializeField] private UnityEvent onMenuButtonPressed;
		[HelpBox("Check this box to test functionality of this component in the Editor.", HelpBoxMessageType.Info)]
		[SerializeField] private bool useOnEditor;
		#pragma warning restore 649

		#if UNITY_EDITOR
		private void Awake() {
			if(!useOnEditor) Destroy(gameObject);
		}
		#elif !UNITY_ANDROID
		private void Awake() => Destroy(gameObject);
		#endif

		private void Update() {
			if(UnityEngine.Input.GetKey(KeyCode.Escape)) {
				onBackButtonPressed.Invoke();
			}

			if(UnityEngine.Input.GetKey(KeyCode.Menu)) {
				onMenuButtonPressed.Invoke();
			}
		}
	}
}