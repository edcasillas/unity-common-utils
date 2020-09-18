using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CommonUtils.SelectionToggles {
    public abstract class AbstractTMPSelectionToggle<TSelectionValue> : AbstractSelectionToggle<TSelectionValue> {
#pragma warning disable 649
        [SerializeField] private TextMeshProUGUI label;
#pragma warning restore 649
        protected override void SetLabelText(ISelectionToggleConfiguration<TSelectionValue> configuration) {
            label.text = configuration.SelectionToggleText;
        }
    }
}
