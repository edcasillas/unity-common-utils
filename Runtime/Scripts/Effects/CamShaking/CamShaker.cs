using System.Collections;
using UnityEngine;

namespace CommonUtils.Effects.CamShaking {
	/// <summary>
	/// When attached to a camera, allows to play shaking animations.
	/// Please make sure this camera is a children of another Game Object to preserve the original position of the camera when and after shaking.
	/// </summary>
	[AddComponentMenu("Effects/Cam Shaker")]
	[RequireComponent(typeof(Animation))]
	public class CamShaker : MonoBehaviour {
		private Vector3 originalPosition;
		private Animation camAnimation;

		private void Awake() {
			camAnimation = GetComponent<Animation>();
		}

		public void Shake(CamShakeMode mode = CamShakeMode.Soft, float duration = 0f) {
			originalPosition = transform.position;
			if(duration < 0f) {
				Debug.LogError("Shake duration cannot be less than zero.");
				return;
			}
			//camAnimation.Play(string.Format("{0}CamShake", mode));
			camAnimation.Play();
			if(duration > 0f) {
				StartCoroutine(waitAndStop(duration));
			}
		}

		public void StopShaking() {
			camAnimation.Stop();
			transform.position = originalPosition;
		}

		private IEnumerator waitAndStop(float duration) {
			yield return new WaitForSeconds(duration);
			StopShaking();
		}
	}
}