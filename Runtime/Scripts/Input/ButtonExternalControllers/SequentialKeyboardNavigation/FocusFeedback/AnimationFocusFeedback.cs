using UnityEngine;

namespace CommonUtils.Input.ButtonExternalControllers.SequentialKeyboardNavigation.FocusFeedback {
    [RequireComponent(typeof(Animation))]
    public class AnimationFocusFeedback : MonoBehaviour, IFocusFeedback {
        private Animation _animation;
        private Animation animation {
            get {
                if (!_animation) _animation = GetComponent<Animation>();
                return _animation;
            }
        }
        
        public void SetFocus(bool hasFocus) {
            animation.enabled = hasFocus;
            if(!hasFocus) transform.localScale = Vector3.one;
        }
    }
}