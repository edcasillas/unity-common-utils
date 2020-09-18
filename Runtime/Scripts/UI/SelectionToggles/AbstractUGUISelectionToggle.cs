using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.SelectionToggles {
    public abstract class AbstractUGUISelectionToggle<TSelectionValue> : AbstractSelectionToggle<TSelectionValue> {
#pragma warning disable 649
        [SerializeField] private Text label;
#pragma warning restore 649

        protected override void SetLabelText(string text) {
            label.text = text;
        }
    }
}
