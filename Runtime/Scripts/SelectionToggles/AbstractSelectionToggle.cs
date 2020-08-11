﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.SelectionToggles {
	/// <summary>
	/// Base class for selection toggle controllers, which can be used to control basic selection toggles and
	/// set them up in runtime using the <see cref="Init"/> method.
	/// </summary>
	/// <typeparam name="TSelectionValue"></typeparam>
	public abstract class AbstractSelectionToggle<TSelectionValue> : MonoBehaviour {
#pragma warning disable 649
		[SerializeField] private Text label;
		[SerializeField] private Toggle toggle;
#pragma warning restore 649

		/// <summary>
		/// Initializes this selection toggle controller.
		/// </summary>
		/// <param name="selectionValue">Value to be passed as argument of the <paramref name="onSelected"/> callback when the toggle is selected.</param>
		/// <param name="labelText">Text to be shown in the toggle.</param>
		/// <param name="onSelected">Callback to be executed when the toggle is selected.</param>
		public void Init(TSelectionValue selectionValue, string labelText, Action<TSelectionValue> onSelected) {
			label.text = labelText;
			toggle.onValueChanged.RemoveAllListeners();
			toggle.onValueChanged.AddListener(val => {
				if (val) onSelected(selectionValue);
			});
		}

		public void SetToggleGroup(ToggleGroup toggleGroup) => toggle.group = toggleGroup;

		public void SetToggleValue(bool value) => toggle.isOn = value;
	}
}