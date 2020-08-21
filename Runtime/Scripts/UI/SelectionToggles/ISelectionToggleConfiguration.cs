namespace CommonUtils.SelectionToggles {
	public interface ISelectionToggleConfiguration<TSelectionValue> {
		TSelectionValue SelectionToggleValue { get; }
		string SelectionToggleText { get; }
	}
}