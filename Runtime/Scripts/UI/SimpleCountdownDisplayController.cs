using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.UI {
	/// <summary>
	/// Starts a countdown and shows its remaining minutes and seconds in the UI.
	/// </summary>
	/// <remarks>
	/// This component serves as a ready-to-use-in-any-project example of usage of the <see cref="CountdownTimer"/> class.
	/// </remarks>
	[AddComponentMenu("UI/Simple Countdown Display")]
	[RequireComponent(typeof(Text))]
	public class SimpleCountdownDisplayController : MonoBehaviour {
		#pragma warning disable 649
		[SerializeField] private float totalTime;
		#pragma warning restore 649

		private Text label;
		private CountdownTimer timer;
		private float elapsed;

		private void Awake() {
			label = GetComponent<Text>();
			timer = new CountdownTimer(totalTime); // Initialize the timer with the total amount of seconds required.
		}

		private void Update() {
			/*
			 * Increase the elapsed time. Please note that the CountdownTimer does not automatically keeps track of
			 * this elapsed time because this elapsed time might be
			 * an input from a remote server or something else, different than Time.deltaTime.
			 */
			elapsed += Time.deltaTime;
			// Call timer.Update inside the Update method of a MonoBehaviour, and send the elapsed time in that frame (Time.deltaTime)
			var timeLeft = timer.Update(elapsed, (minutes, seconds) => {
				// This callback is received when either minutes or seconds changed, so the string for the label can be
				// built less frequently than if we did this every frame (i.e. every execution of the Update method).
				label.text = $"{minutes:D2}:{seconds:D2}";
			});

			// The CountdownTimer.Update method returns the time left so we can act accordingly. For the sake of
			// this example, we only deactivate this component when the timer reaches zero to avoid the Update
			// method to continue running.
			if (timeLeft <= 0) {
				enabled = false;
			}
		}
	}
}