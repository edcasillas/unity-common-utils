using CommonUtils.Editor.DebuggableEditors;
using CommonUtils.UI;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.CustomEditors {
    [CustomEditor(typeof(TiledSpriteFromTexture))]
    public class TiledSpriteFromTextureEditor : AbstractMonoBehaviourDebuggableEditor<TiledSpriteFromTexture> {
        private Texture2D textureOverride;
        private float borderOverride;
        private float pixelsPerUnitOverride;

        protected override void OnEnable() {
            base.OnEnable();
            borderOverride = Subject.Border;
            pixelsPerUnitOverride = Subject.PixelsPerUnit;
        }

        protected override void RenderDebug() {
            textureOverride = EditorExtensions.TextureField("Texture Override", textureOverride);
            borderOverride = EditorGUILayout.FloatField("Border Override", borderOverride);
            pixelsPerUnitOverride = EditorGUILayout.FloatField("Pixels per unit override", pixelsPerUnitOverride);
            
            if (textureOverride) {
                Subject.SetTextureTiled(textureOverride, borderOverride, pixelsPerUnitOverride);
            }
        }
    }
}