using CommonUtils.UI;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.CustomEditors {
    [CustomEditor(typeof(TiledSpriteFromTexture))]
    public class TiledSpriteFromTextureEditor : AbstractDebuggableEditor<TiledSpriteFromTexture> {
        private Texture2D textureOverride;
        
        protected override void RenderDebug() {

            //textureOverride =
              //  EditorGUILayout.ObjectField("Texture override", textureOverride, typeof(Texture2D), false);
            if (GUILayout.Button("Refresh")) {
                Debug.Log("Refreshing");
            }
        }
    }
}