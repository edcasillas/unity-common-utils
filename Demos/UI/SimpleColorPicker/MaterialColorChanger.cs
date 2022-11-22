using UnityEngine;

public class MaterialColorChanger : MonoBehaviour {
	public void SetMaterialColor(Color color) => GetComponent<MeshRenderer>().material.color = color;
}
