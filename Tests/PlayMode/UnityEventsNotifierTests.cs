using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace CommonUtils.Tests.PlayMode {
	public class UnityEventsNotifierTests {
		[UnityTest]
		public IEnumerator SubscribeIsSuccessful_WhenAddingComponentAtRuntime() {
			// Arrange
			var gameObject = Object.Instantiate(new GameObject());
			var success = false;

			// Act
			var eventsNotifier = gameObject.AddComponent<UnityEventsNotifier>();
			eventsNotifier.SubscribeOnDestroy(() => success = true);
			yield return null;
			Object.Destroy(gameObject);
			yield return null;

			// Assert
			Assert.IsTrue(success);
		}
	}
}