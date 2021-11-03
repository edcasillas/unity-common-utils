using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.UI {
    /// <summary>
    /// Controls a <see cref="Text"/> component to show a numeric score that can be changed with an animation.
    /// </summary>
    [AddComponentMenu("UI/Animated Score Display")]
    [RequireComponent(typeof(Text))]
    public class AnimatedScoreDisplay : AbstractAnimatedScoreDisplay {

        #region Properties
        private Text _textComponent;
        private Text textComponent {
            get {
                if (_textComponent == null) _textComponent = GetComponent<Text>();
                return _textComponent;
            }
        }
        #endregion

        protected override void OnAnimationUpdated(int updatedValue) => textComponent.text = LabelDelegate.Invoke(updatedValue);
    }
}