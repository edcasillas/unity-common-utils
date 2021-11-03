using System;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ExaGames.Common.UI
{
	[AddComponentMenu("UI/TiledSpriteFromTexture")]
	public class TiledSpriteFromTexture : MonoBehaviour
	{
		public Texture2D Texture;
		public float Border;

		private Image _image;
		private Image image
		{
			get
			{
				if (_image == null) _image = GetComponent<Image>();
				if (_image == null) _image = gameObject.AddComponent<Image>();
				return _image;
			}
		}

		private void Awake()
		{
			image.type = Image.Type.Tiled;
			if (Texture != null) SetTextureTiled(Texture, Border);
		}

		public void SetTextureTiled(Texture2D texture, float border = 0f, float pixelsPerUnit = 100f)
		{
			if(texture == null)
			{
				Debug.LogErrorFormat("Se ha llamado a SetTextureTiled en {0} con una textura en null.", name);
				return;
			}
			texture.wrapMode = TextureWrapMode.Repeat;
			try
			{
				image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, pixelsPerUnit, 0, SpriteMeshType.Tight, new Vector4(border, border, border, border));
			} catch(Exception ex)
			{
				Debug.LogErrorFormat("No se pudo establecer la textura tileada en {0}: {1}", name, ex.Message);
			}
		}

#if UNITY_EDITOR
		// Add a menu item to create custom GameObjects.
		// Priority 1 ensures it is grouped with the other menu items of the same kind
		// and propagated to the hierarchy dropdown and hierarch context menus.
		[MenuItem("GameObject/UI/Tiled Sprite from Texture", false, 10)]
		static void CreateCustomGameObject(MenuCommand menuCommand)
		{
			// Create a custom game object
			GameObject go = new GameObject("TiledSpriteFromTexture", typeof(RectTransform), typeof(TiledSpriteFromTexture));
			// Ensure it gets reparented if this was a context click (otherwise does nothing)
			GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
			// Register the creation in the undo system
			Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
			Selection.activeObject = go;
		}
#endif
	}
}