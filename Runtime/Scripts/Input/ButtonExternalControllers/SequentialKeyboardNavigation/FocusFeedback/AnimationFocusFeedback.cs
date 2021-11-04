using UnityEngine;

namespace CommonUtils.Input.ButtonExternalControllers.SequentialKeyboardNavigation.FocusFeedback {
    [RequireComponent(typeof(Animation))]
    public class AnimationFocusFeedback : MonoBehaviour, IFocusFeedback {
        private Animation _anim;
        private Animation anim {
            get {
                if (!_anim) _anim = GetComponent<Animation>();
                return _anim;
            }
        }
        
        public void SetFocus(bool hasFocus) {
            anim.enabled = hasFocus;
            if(!hasFocus) transform.localScale = Vector3.one;
        }
    }
}