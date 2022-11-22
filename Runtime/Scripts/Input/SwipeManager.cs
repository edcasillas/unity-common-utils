using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonUtils.Input {
	/// <summary>
	/// Swipe manager. Detects swipes and taps.
	/// </summary>
	public interface ISwipeManager {
		/// <summary>
		/// Gets or sets a value indicating whether this instance is initialized.
		/// </summary>
		/// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
		bool IsInitialized { get; }

		/// <summary>
		/// Gets a value indicating whether a touch has started with any finger.
		/// </summary>
		/// <value><c>true</c> if touch started; otherwise, <c>false</c>.</value>
		bool TouchStarted { get; }

		/// <summary>
		/// Gets a value indicating whether a touch has ended with any finger.
		/// </summary>
		/// <value><c>true</c> if touch ended; otherwise, <c>false</c>.</value>
		bool TouchEnded { get; }

		/// <summary>
		/// Indicates the current swiping distance on X axis for each of the supported fingers.
		/// </summary>
		IEnumerable<float> SwipeHorizontal { get; }

		/// <summary>
		/// Indicates the current swiping distance on Y axis for each of the supported fingers.
		/// </summary>
		IEnumerable<float> SwipeVertical { get; }

		/// <summary>
		/// Gets a value indicating whether the user tapped with any finger.
		/// </summary>
		/// <value><c>true</c> if tapped; otherwise, <c>false</c>.</value>
		bool Tapped { get; }

		/// <summary>
		/// Gets a value indicating whether the user is swiping with any finger.
		/// </summary>
		/// <value><c>true</c> if swiping; otherwise, <c>false</c>.</value>
		bool Swiping { get; }

		/// <summary>
		/// Gets a value indicating whether the user is swiping horizontal with any finger.
		/// </summary>
		/// <value><c>true</c> if swiping horizontal; otherwise, <c>false</c>.</value>
		bool SwipingHorizontal { get; }

		/// <summary>
		/// Gets a value indicating whether the user is swiping vertical with any finger.
		/// </summary>
		/// <value><c>true</c> if swiping vertical; otherwise, <c>false</c>.</value>
		bool SwipingVertical { get; }

		/// <summary>
		/// Gets a value indicating whether the user is swiping left with any finger.
		/// </summary>
		/// <value><c>true</c> if swiping left; otherwise, <c>false</c>.</value>
		bool SwipingLeft { get; }

		/// <summary>
		/// Gets a value indicating whether the user is swiping right with any finger.
		/// </summary>
		/// <value><c>true</c> if swiping right; otherwise, <c>false</c>.</value>
		bool SwipingRight { get; }

		/// <summary>
		/// Gets a value indicating whether the user is swiping up with any finger.
		/// </summary>
		/// <value><c>true</c> if swiping up; otherwise, <c>false</c>.</value>
		bool SwipingUp { get; }

		/// <summary>
		/// Gets a value indicating whether the user is swiping down with any finger.
		/// </summary>
		/// <value><c>true</c> if swiping down; otherwise, <c>false</c>.</value>
		bool SwipingDown { get; }

		/// <summary>
		/// Gets the swipe amount value on the horizontal axis for a specific finger.
		/// </summary>
		/// <returns>The swipe amount.</returns>
		/// <param name="fingerId">Finger identifier, zero based.</param>
		/// <param name="relative">If set to <c>true</c> the value returned will be from 0 to 1,
		/// else the total amount of pixels moved is returned.</param>
		float GetSwipeAmountHorizontalForFinger(int fingerId, bool relative = true);

		/// <summary>
		/// Gets the swipe amount value on the vertical axis for a specific finger.
		/// </summary>
		/// <param name="fingerId">Finger identifier, zero based.</param>
		/// <param name="relative">If set to <c>true</c> the value returned will be from 0 to 1,
		/// else the total amount of pixels moved is returned.</param>
		/// <returns>The swipe amount.</returns>
		float GetSwipeAmountVerticalForFinger(int fingerId, bool relative = true);
	}

	/// <summary>
	/// Swipe manager. Detects swipes and taps.
	/// </summary>
	/// <author>Ed Casillas</author>
	[AddComponentMenu("Input/Swipe Manager")]
	public class SwipeManager : MonoBehaviour, ISwipeManager {
		#region Singleton

		/// <summary>
		/// Private instance of the singleton.
		/// </summary>
		private static SwipeManager instance;

		/// <summary>
		/// Gets the singleton instance of the Swipe Manager.
		/// </summary>
		/// <value>The instance.</value>
		public static ISwipeManager Instance {
			get {
				if(!instance) {
					// If instance doesn't exist, creates a new one, initializes it and set DontDestroyOnLoad on it.
					instance = FindObjectOfType<SwipeManager>();
					if(!instance) {
						var gameObject = new GameObject(nameof(SwipeManager));
						instance = gameObject.AddComponent<SwipeManager>();
						DontDestroyOnLoad(gameObject);
					}
					if(!instance.IsInitialized) {
						instance.Initialize();
						instance.IsInitialized = true;
					}
				}
				return instance;
			}
		}

		/// <summary>
		/// If a duplicated instance of the object has been created, is destroyed on awake.
		/// </summary>
		private void Awake() {
			touchStarted = new bool[MaxFingers];
			swipeHorizontal = new float[MaxFingers];
			swipeVertical = new float[MaxFingers];
			touchEnded = new bool[MaxFingers];
			tapped = new bool[MaxFingers];
			swipeStartArray = new Vector2[MaxFingers];
			SmoothRewindHorizontalCoroutines = new Coroutine[MaxFingers];
			SmoothRewindVerticalCoroutines = new Coroutine[MaxFingers];
			if(instance != null) {
				DestroyImmediate(gameObject);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is initialized.
		/// </summary>
		/// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
		public bool IsInitialized{ get; private set; }

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		protected virtual void Initialize() {
		}

		#endregion

		#region Fields

		#region Public Fields

		/// <summary>
		/// Indicates the maximum number of fingers that should be supported for interaction.
		/// </summary>
		public int MaxFingers = 1;
		/// <summary>
		/// Indicates the sensibility of swipes on X and Y axes as a percentage (from 0 to 1) of the screen size.
		/// </summary>
		public Vector2 Sensibility = new Vector2(0.1f, 0.1f);
		/// <summary>
		/// Specifies the time it takes to rewind values to zero for the horizontal axis. If this value is itself zero, smooth rewind will be deactivated.
		/// </summary>
		public float SmoothRewindHorizontalTime;
		/// <summary>
		/// Specifies the time it takes to rewind values to zero for the vertical axis. If this value is itself zero, smooth rewind will be deactivated.
		/// </summary>
		public float SmoothRewindVerticalTime;

		#endregion

		/// <summary>
		/// Array of values indicating the starting point of a swipe for each finger. There's one slot per supported finger.
		/// </summary>
		private Vector2[] swipeStartArray;
		/// <summary>
		/// The swipe sensibility on X axis, expressed as a concrete float number.
		/// </summary>
		private float swipeSensibilityX;
		/// <summary>
		/// The swipe sensibility on Y axis, expressed as a concrete float number.
		/// </summary>
		private float swipeSensibilityY;

		/// <summary>
		/// Indicates whether each of the supported number of fingers has started a touch.
		/// </summary>
		private bool[] touchStarted;
		/// <summary>
		/// Indicates whether each of the supported number of fingers has ended a touch.
		/// </summary>
		private bool[] touchEnded;
		/// <summary>
		/// Indicates the current swiping distance on X axis for each of the supported fingers.
		/// </summary>
		private float[] swipeHorizontal;
		/// <summary>
		/// Indicates the current swiping distance on Y axis for each of the supported fingers.
		/// </summary>
		private float[] swipeVertical;
		/// <summary>
		/// Indicates whether each of the supported fingers has a tap scheduled.
		/// </summary>
		private bool[] tapped;

		/// <summary>
		/// The smooth rewind coroutines for the horizontal axis.
		/// </summary>
		private Coroutine[] SmoothRewindHorizontalCoroutines;
		/// <summary>
		/// The smooth rewind coroutines for the vertical axis.
		/// </summary>
		private Coroutine[] SmoothRewindVerticalCoroutines;

		#endregion

		#region Properties
		/// <summary>
		/// Gets a value indicating whether a touch has started with any finger.
		/// </summary>
		/// <value><c>true</c> if touch started; otherwise, <c>false</c>.</value>
		[ShowInInspector] public bool TouchStarted => touchStarted.Contains(true);

		/// <summary>
		/// Gets a value indicating whether a touch has ended with any finger.
		/// </summary>
		/// <value><c>true</c> if touch ended; otherwise, <c>false</c>.</value>
		[ShowInInspector] public bool TouchEnded => touchEnded.Contains(true);

		/// <summary>
		/// Gets a value indicating whether the user is swiping with any finger.
		/// </summary>
		/// <value><c>true</c> if swiping; otherwise, <c>false</c>.</value>
		[ShowInInspector] public bool Swiping => (SwipingHorizontal || SwipingVertical);

		/// <summary>
		/// Gets a value indicating whether the user is swiping horizontal with any finger.
		/// </summary>
		/// <value><c>true</c> if swiping horizontal; otherwise, <c>false</c>.</value>
		[ShowInInspector] public bool SwipingHorizontal => (SwipingLeft || SwipingRight);

		/// <summary>
		/// Gets a value indicating whether the user is swiping vertical with any finger.
		/// </summary>
		/// <value><c>true</c> if swiping vertical; otherwise, <c>false</c>.</value>
		[ShowInInspector] public bool SwipingVertical => (SwipingUp || SwipingDown);

		/// <summary>
		/// Gets a value indicating whether the user is swiping left with any finger.
		/// </summary>
		/// <value><c>true</c> if swiping left; otherwise, <c>false</c>.</value>
		[ShowInInspector] public bool SwipingLeft => swipeHorizontal.Count(c => c < 0f) > 0;

		/// <summary>
		/// Gets a value indicating whether the user is swiping right with any finger.
		/// </summary>
		/// <value><c>true</c> if swiping right; otherwise, <c>false</c>.</value>
		[ShowInInspector] public bool SwipingRight => swipeHorizontal.Count(c => c > 0f) > 0;

		/// <summary>
		/// Gets a value indicating whether the user is swiping up with any finger.
		/// </summary>
		/// <value><c>true</c> if swiping up; otherwise, <c>false</c>.</value>
		[ShowInInspector] public bool SwipingUp => swipeVertical.Count(c => c > 0f) > 0;

		/// <summary>
		/// Gets a value indicating whether the user is swiping down with any finger.
		/// </summary>
		/// <value><c>true</c> if swiping down; otherwise, <c>false</c>.</value>
		[ShowInInspector] public bool SwipingDown => swipeVertical.Count(c => c < 0f) > 0;

		/// <summary>
		/// Gets a value indicating whether the user tapped with any finger.
		/// </summary>
		/// <value><c>true</c> if tapped; otherwise, <c>false</c>.</value>
		[ShowInInspector] public bool Tapped {
			get {
				var result = false;
				for(int i = 0; i < MaxFingers; i++) {
					if(tapped[i] && touchEnded[i]) {
						result = true;
						break;
					}
				}
				return result;
			}
		}

		/// <summary>
		/// Indicates the current swiping distance on X axis for each of the supported fingers.
		/// </summary>
		[ShowInInspector] public IEnumerable<float> SwipeHorizontal => swipeHorizontal ?? Enumerable.Empty<float>();

		/// <summary>
		/// Indicates the current swiping distance on Y axis for each of the supported fingers.
		/// </summary>
		[ShowInInspector] public IEnumerable<float> SwipeVertical => swipeVertical ?? Enumerable.Empty<float>();
		#endregion

		#region Unity Behaviour
		/// <summary>
		/// Initialize swipe sensibility values.
		/// </summary>
		private void Start() {
			swipeSensibilityX = Screen.width * Sensibility.x;
			swipeSensibilityY = Screen.height * Sensibility.y;
		}

		/// <summary>
		/// Detects swipes and taps.
		/// </summary>
		private void Update() {
			/* Taps are detected only when finger hasn't moved AND a tap ended is detected. These taps should only be
			 * available for one cycle (namely, one LateUpdate). For that reason, the "tapped" array is set to a new
			 * array here, with all their values to the default (false).
			 */
			tapped = new bool[MaxFingers];

			// Swipe and tap detection is only executed when there's one or more touches.
			if(UnityEngine.Input.touchCount > 0) {
				Vector2[] swipeCurrent = new Vector2[MaxFingers];	// Stores the current position of fingers touching.

				foreach(Touch touch in UnityEngine.Input.touches) {
					// Restrict touches to MaxFingers.
					if(touch.fingerId >= MaxFingers) {
						break;
					}

					if(touch.phase == TouchPhase.Began) {
						// On touch begin, store initial finger position in both swipeStartArray and swipeCurrent.
						swipeStartArray[touch.fingerId] = swipeCurrent[touch.fingerId] = touch.position;
						touchStarted[touch.fingerId] = true;

						if((SmoothRewindHorizontalTime > 0) && SmoothRewindHorizontalCoroutines[touch.fingerId] != null) {
							StopCoroutine(SmoothRewindHorizontalCoroutines[touch.fingerId]);
						}

						if((SmoothRewindVerticalTime > 0) && SmoothRewindVerticalCoroutines[touch.fingerId] != null) {
							StopCoroutine(SmoothRewindVerticalCoroutines[touch.fingerId]);
						}

						SmoothRewindHorizontalCoroutines[touch.fingerId] = null;
						SmoothRewindVerticalCoroutines[touch.fingerId] = null;
					} else if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Ended) {
						// On any other touch phase, store only current touch position in swipeCurrent. This way we are able to
						// calculate finger movement as swipeCurrent - swipeStart.
						touchStarted[touch.fingerId] = false;
						swipeCurrent[touch.fingerId] = touch.position;
					}

					tapped[touch.fingerId] = false; // Double check for tapped inited to false.

					// If finger movement overcame sensibility on X axis, then the user is swiping horizontal (left or right).
					if(Mathf.Abs(swipeCurrent[touch.fingerId].x - swipeStartArray[touch.fingerId].x) > swipeSensibilityX) {
						swipeHorizontal[touch.fingerId] = (swipeCurrent[touch.fingerId].x - swipeStartArray[touch.fingerId].x);
					} else {
						swipeHorizontal[touch.fingerId] = 0f;
					}

					// If finger movement overcame sensibility on Y axis, then the user is swiping vertical (up or down).
					if(Mathf.Abs(swipeCurrent[touch.fingerId].y - swipeStartArray[touch.fingerId].y) > swipeSensibilityY) {
						swipeVertical[touch.fingerId] = (swipeCurrent[touch.fingerId].y - swipeStartArray[touch.fingerId].y);
					} else {
						swipeVertical[touch.fingerId] = 0f;
					}

					// If finger movement didn't overcome sensibility on any axis, then we have a tap.
					if(Mathf.Abs(swipeCurrent[touch.fingerId].x - swipeStartArray[touch.fingerId].x) < swipeSensibilityX &&
					   Mathf.Abs(swipeCurrent[touch.fingerId].y - swipeStartArray[touch.fingerId].y) < swipeSensibilityY) {
						tapped[touch.fingerId] = true;
					}

					// When touching is ended, variables are cleaned.
					if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
						touchEnded[touch.fingerId] = true;
						if(SmoothRewindHorizontalTime > 0) {
							SmoothRewindHorizontalCoroutines[touch.fingerId] = StartCoroutine(rewindHorizontalSwipe(touch.fingerId));
						} else {
							swipeHorizontal[touch.fingerId] = 0f;
						}

						if(SmoothRewindVerticalTime > 0) {
							SmoothRewindVerticalCoroutines[touch.fingerId] = StartCoroutine(rewindVerticalSwipe(touch.fingerId));
						} else {
							swipeVertical[touch.fingerId] = 0f;
						}
						swipeStartArray[touch.fingerId] = swipeCurrent[touch.fingerId] = Vector2.zero;
					} else {
						touchEnded[touch.fingerId] = false;
					}
				}
			}
		}
		#endregion

		/// <summary>
		/// Determines whether a touch is started by the finger with the specified fingerId.
		/// </summary>
		/// <returns><c>true</c> if a touch is touch started with specified fingerId; otherwise, <c>false</c>.</returns>
		/// <param name="fingerId">Finger identifier.</param>
		public bool IsTouchStarted(int fingerId) => touchStarted[fingerId];

		/// <summary>
		/// Gets the swipe amount value on the horizontal axis for a specific finger.
		/// </summary>
		/// <returns>The swipe amount.</returns>
		/// <param name="fingerId">Finger identifier, zero based.</param>
		/// <param name="relative">If set to <c>true</c> the value returned will be from 0 to 1,
		/// else the total amount of pixels moved is returned.</param>
		public float GetSwipeAmountHorizontalForFinger(int fingerId, bool relative = true) {
			var result = swipeHorizontal[fingerId];
			if(relative) {
				result /= Screen.width;
			}
			return result;
		}

		/// <summary>
		/// Gets the swipe amount value on the vertical axis for a specific finger.
		/// </summary>
		/// <param name="fingerId">Finger identifier, zero based.</param>
		/// <param name="relative">If set to <c>true</c> the value returned will be from 0 to 1,
		/// else the total amount of pixels moved is returned.</param>
		/// <returns>The swipe amount.</returns>
		public float GetSwipeAmountVerticalForFinger(int fingerId, bool relative = true) {
			var result = swipeVertical[fingerId];
			if (relative) {
				result /= Screen.height;
			}

			return result;
		}

		/// <summary>
		/// Rewinds the horizontal swipe amount for the specified finger to zero.
		/// </summary>
		/// <returns>A coroutine.</returns>
		/// <param name="fingerId">Finger identifier.</param>
		private IEnumerator rewindHorizontalSwipe(int fingerId) {
			float timer = Time.time;
			while(Mathf.Abs(swipeHorizontal[fingerId]) > 0) {
				swipeHorizontal[fingerId] = Mathf.Lerp(swipeHorizontal[fingerId], 0f, (Time.time - timer) / SmoothRewindHorizontalTime);
				yield return null;
			}
		}

		/// <summary>
		/// Rewinds the vertical swipe amount for the specified finger to zero.
		/// </summary>
		/// <returns>A coroutine.</returns>
		/// <param name="fingerId">Finger identifier.</param>
		private IEnumerator rewindVerticalSwipe(int fingerId) {
			float timer = Time.time;
			while(Mathf.Abs(swipeVertical[fingerId]) > 0) {
				swipeVertical[fingerId] = Mathf.Lerp(swipeVertical[fingerId], 0f, (Time.time - timer) / SmoothRewindVerticalTime);
				yield return null;
			}
		}
	}
}