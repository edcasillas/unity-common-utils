using System.Collections.Generic;
using CommonUtils.UnityComponents;
using UnityEngine;

namespace CommonUtils.Input.ButtonExternalControllers {
    public interface IButtonExternalControllerBlocker : IUnityComponent {
        void Subscribe(IButtonExternalController button);
        void Unsubscribe(IButtonExternalController button);
    }
    
    public class ButtonExternalControllerBlocker : MonoBehaviour, IButtonExternalControllerBlocker {
        private readonly HashSet<IButtonExternalController> _subscribers = new HashSet<IButtonExternalController>();
        public IEnumerable<IButtonExternalController> Subscribers => _subscribers;

        private void OnEnable() {
            foreach (var subscriber in _subscribers) {
                subscriber.OnBlockerBecameActive(this);
            }
        }

        private void OnDisable() {
            foreach (var subscriber in _subscribers) {
                subscriber.OnBlockerBecameInactive(this);
            }
        }

        public void Subscribe(IButtonExternalController button) {
            if (!_subscribers.Contains(button)) {
                _subscribers.Add(button);
                if (gameObject.activeInHierarchy) button.OnBlockerBecameActive(this);
            }
        }

        public void Unsubscribe(IButtonExternalController button) {
            if (_subscribers.Contains(button)) {
                button.OnBlockerBecameInactive(this);
                _subscribers.Remove(button);
            }
        }
    }
}