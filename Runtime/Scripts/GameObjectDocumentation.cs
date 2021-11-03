using UnityEngine;

namespace ExaGames.Common {
	/// <summary>
	/// Provides a string field to comment on a Game Object.
	/// </summary>
	public class GameObjectDocumentation : MonoBehaviour {
		[Tooltip("Description of this game object.")]
		[TextArea]
		public string Documentation;
	}
}