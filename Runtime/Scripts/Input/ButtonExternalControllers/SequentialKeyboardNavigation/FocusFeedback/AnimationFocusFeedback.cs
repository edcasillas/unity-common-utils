using UnityEngine;

namespace CommonUtils.Input.ButtonExternalControllers.SequentialKeyboardNavigation.FocusFeedback {
    [RequireComponent(typeof(Animation))]
    public class AnimationFocusFeedback : MonoBehaviour, IFocusFeedback {
        [Tooltip("OPTIONAL. If empty, it will retrieve the Animation component using GetComponent.")]
        [SerializeField] private Animation feedbackAnimation;
        
        private Animation anim {
            get {
                if (!feedbackAnimation) feedbackAnimation = GetComponent<Animation>();
                return feedbackAnimation;
            }
        }
        
        public void SetFocus(bool hasFocus) {
            anim.enabled = hasFocus;
            if(!hasFocus) transform.localScale = Vector3.one;
        }
    }
}