using CommonUtils.UI;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.CustomEditors {
    [CustomEditor(typeof(TiledSpriteFromTexture))]
    public class TiledSpriteFromTextureEditor : AbstractDebuggableEditor<TiledSpriteFromTexture> {
        private Texture2D textureOverride;
        private float borderOverride;
        private float pixelsPerUnitOverride = 100;
        
        protected override void RenderDebug() {
            textureOverride = EditorExtensions.TextureField("Texture Override", textureOverride);
            borderOverride = EditorGUILayout.FloatField("Border Override", borderOverride);
            pixelsPerUnitOverride = EditorGUILayout.FloatField("Pixels per unit override", pixelsPerUnitOverride);
            
            if (GUILayout.Button("Refresh")) {
                Subject.SetTextureTiled(textureOverride, borderOverride, pixelsPerUnitOverride);
            }
        }
    }
}