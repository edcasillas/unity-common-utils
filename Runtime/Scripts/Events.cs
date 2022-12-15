using System;
using UnityEngine;
using UnityEngine.Events;

namespace CommonUtils {
	[Serializable]
	public class GameObjectEvent : UnityEvent<GameObject> { }

	[Serializable]
	public class ColliderEvent: UnityEvent<Collider> { }

	[Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	[Serializable]
	public class IntEvent : UnityEvent<int> { }

	[Serializable]
	public class FloatEvent : UnityEvent<float> { }

	[Serializable]
	public class StringEvent : UnityEvent<string> { }

	[Serializable]
	public class Vector2Event : UnityEvent<Vector2> { }

	[Serializable]
	public class Vector3Event : UnityEvent<Vector3> { }

	[Serializable]
	public class ColorEvent : UnityEvent<Color> { }
}