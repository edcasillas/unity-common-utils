using CommonUtils.Extensions;
using UnityEngine;

namespace CommonUtils {
	public class MaterialRandomizer : MonoBehaviour {
		[SerializeField] private Material[] materials;

		private void Awake() => GetComponent<Renderer>().material = materials.PickRandom();
	}
}