using UnityEngine;

public interface IUnityComponent {
	GameObject gameObject { get; }
	Transform transform { get; }
	string name { get; }
}