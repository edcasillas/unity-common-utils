using UnityEngine;

namespace CommonUtils.Inspector.ReorderableInspector {
	/// <summary>
	/// Display a List/Array as a sortable list in the inspector
	/// </summary>
	public class ReorderableAttribute : PropertyAttribute
	{
		public string ElementHeader { get; protected set; }
		public bool HeaderZeroIndex { get; protected set; }
		public bool ElementSingleLine { get; protected set; }

		/// <summary>
		/// Display a List/Array as a sortable list in the inspector
		/// </summary>
		public ReorderableAttribute()
		{
			ElementHeader = string.Empty;
			HeaderZeroIndex = false;
			ElementSingleLine = false;
		}

		/// <summary>
		/// Display a List/Array as a sortable list in the inspector
		/// </summary>
		/// <param name="headerString">Customize the element name in the inspector</param>
		/// <param name="isZeroIndex">If false, start element list count from 1</param>
		/// <param name="isSingleLine">Try to fit the array elements in a single line</param>
		public ReorderableAttribute(string headerString = "", bool isZeroIndex = true, bool isSingleLine = false)
		{
			ElementHeader = headerString;
			HeaderZeroIndex = isZeroIndex;
			ElementSingleLine = isSingleLine;
		}
	}
}
