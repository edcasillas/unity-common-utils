using System.Collections.Generic;
using CommonUtils.UnityComponents;
using UnityEngine;

namespace CommonUtils.Input.ButtonExternalControllers {
    public interface IButtonExternalController : IUnityComponent {
        bool IsInteractable();
        void AddBlockers(IEnumerable<GameObject> blockers);
        void OnBlockerBecameActive(IButtonExternalControllerBlocker blocker);
        void OnBlockerBecameInactive(IButtonExternalControllerBlocker blocker);
    }
}