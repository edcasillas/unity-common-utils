using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.UI {
    /// <summary>
    /// Controls a <see cref="Text"/> component to show a numeric score that can be changed with an animation.
    /// </summary>
    [AddComponentMenu("UI/TMP Animated Score Display")]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPAnimatedScoreDisplay : AbstractAnimatedScoreDisplay {

        #region Properties
        private TextMeshProUGUI _textComponent;
        private TextMeshProUGUI textComponent {
            get {
                if (!_textComponent) _textComponent = GetComponent<TextMeshProUGUI>();
                return _textComponent;
            }
        }
        #endregion

        protected override void UpdateLabel(int score) => textComponent.text = LabelDelegate.Invoke(score);
    }
}