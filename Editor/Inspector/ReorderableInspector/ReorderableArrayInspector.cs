/*MIT License

Copyright(c) 2017 Jeiel Aranal

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

// Uncomment the line below to turn all arrays into reorderable lists
//#define LIST_ALL_ARRAYS

// Uncomment the line below to make all ScriptableObject fields editable
//#define EDIT_ALL_SCRIPTABLES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using CommonUtils.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SubjectNerd.Utilities {
	[CustomEditor(typeof(Object), true, isFallback = true)]
	[CanEditMultipleObjects]
	public partial class ReorderableArrayInspector : Editor {
		protected static bool FORCE_INIT = false;
		[DidReloadScripts]
		private static void HandleScriptReload()
		{
			FORCE_INIT = true;

			EditorApplication.delayCall = () => { EditorApplication.delayCall = () => { FORCE_INIT = false; }; };
		}

		public bool isSubEditor;

		private readonly List<SortableListData> listIndex = new List<SortableListData>();
		private readonly Dictionary<string, Editor> editableIndex = new Dictionary<string, Editor>();

		protected bool alwaysDrawInspector = false;
		protected bool isInitialized = false;
		protected bool hasSortableArrays = false;
		protected bool hasEditable = false;

		protected Dictionary<string, ContextMenuData> contextData = new Dictionary<string, ContextMenuData>();

		~ReorderableArrayInspector()
		{
			listIndex.Clear();
			//hasSortableArrays = false;
			editableIndex.Clear();
			//hasEditable = false;
			isInitialized = false;
		}

		#region Initialization
		private void OnEnable() => InitInspector();

		protected virtual void InitInspector(bool force)
		{
			if (force)
				isInitialized = false;
			InitInspector();
		}

		protected virtual void InitInspector() {
			if (isInitialized && FORCE_INIT == false)
				return;
			
			ReorderableArrayUtils.FindTargetProperties(serializedObject, listIndex, editableIndex, ref hasSortableArrays, ref hasEditable);
			isInitialized = true;
			target.GetContextMenuData(contextData);
		}

		/// <summary>
		/// Given a SerializedProperty, return the automatic ReorderableList assigned to it if any
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected ReorderableList GetSortableList(SerializedProperty property)
		{
			if (listIndex.Count == 0)
				return null;

			string parent = ReorderableArrayUtils.GetGrandParentPath(property);

			SortableListData data = listIndex.Find(listData => listData.Parent.Equals(parent));
			if (data == null)
				return null;

			return data.GetPropertyList(property);
		}

		/// <summary>
		/// Set a drag and drop handler function on a SerializedObject's ReorderableList, if any
		/// </summary>
		/// <param name="property"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		protected bool SetDragDropHandler(SerializedProperty property, Action<SerializedProperty, Object[]> handler)
		{
			if (listIndex.Count == 0)
				return false;

			string parent = ReorderableArrayUtils.GetGrandParentPath(property);

			SortableListData data = listIndex.Find(listData => listData.Parent.Equals(parent));
			if (data == null)
				return false;

			data.SetDropHandler(property, handler);
			return true;
		}
#endregion

		protected bool InspectorGUIStart(bool force = false)
		{
			// Not initialized, try initializing
			if (hasSortableArrays && listIndex.Count == 0)
				InitInspector();
			if (hasEditable && editableIndex.Count == 0)
				InitInspector();

			// No sortable arrays or list index unintialized
			bool cannotDrawOrderable = (hasSortableArrays == false || listIndex.Count == 0);
			bool cannotDrawEditable = (hasEditable == false || editableIndex.Count == 0);
			if (cannotDrawOrderable && cannotDrawEditable && force == false)
			{
				if (isSubEditor)
					DrawPropertiesExcluding(serializedObject, "m_Script");
				else
					base.OnInspectorGUI();

				DrawContextMenuButtons();
				return false;
			}

			serializedObject.Update();
			return true;
		}

		protected virtual void DrawInspector() => DrawPropertiesAll();

		public override void OnInspectorGUI()
		{
			if (InspectorGUIStart(alwaysDrawInspector) == false)
				return;

			EditorGUI.BeginChangeCheck();

			DrawInspector();

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				InitInspector(true);
			}

			DrawContextMenuButtons();
		}

		#region Helper functions
		/// <summary>
		/// Draw the default inspector, with the sortable arrays
		/// </summary>
		public void DrawPropertiesAll()
		{
			SerializedProperty iterProp = serializedObject.GetIterator();
			ReorderableArrayUtils.IterateDrawProperty(serializedObject, iterProp, isSubEditor, listIndex, editableIndex);
		}

		/// <summary>
		/// Draw the default inspector, except for the given property names
		/// </summary>
		/// <param name="propertyNames"></param>
		public void DrawPropertiesExcept(params string[] propertyNames)
		{
			SerializedProperty iterProp = serializedObject.GetIterator();

			ReorderableArrayUtils.IterateDrawProperty(serializedObject, iterProp, isSubEditor, listIndex, editableIndex,
				filter: () =>
				{
					if (propertyNames.Contains(iterProp.name))
						return IterControl.Continue;
					return IterControl.Draw;
				});
		}

		/// <summary>
		/// Draw the default inspector, starting from a given property
		/// </summary>
		/// <param name="propertyStart">Property name to start from</param>
		public void DrawPropertiesFrom(string propertyStart)
		{
			bool canDraw = false;
			SerializedProperty iterProp = serializedObject.GetIterator();
			ReorderableArrayUtils.IterateDrawProperty(serializedObject, iterProp, isSubEditor, listIndex, editableIndex,
				filter: () =>
				{
					if (iterProp.name.Equals(propertyStart))
						canDraw = true;
					if (canDraw)
						return IterControl.Draw;
					return IterControl.Continue;
				});
		}

		/// <summary>
		/// Draw the default inspector, up to a given property
		/// </summary>
		/// <param name="propertyStop">Property name to stop at</param>
		public void DrawPropertiesUpTo(string propertyStop)
		{
			SerializedProperty iterProp = serializedObject.GetIterator();
			ReorderableArrayUtils.IterateDrawProperty(serializedObject, iterProp, isSubEditor, listIndex, editableIndex,
				filter: () =>
				{
					if (iterProp.name.Equals(propertyStop))
						return IterControl.Break;
					return IterControl.Draw;
				});
		}

		/// <summary>
		/// Draw the default inspector, starting from a given property to a stopping property
		/// </summary>
		/// <param name="propertyStart">Property name to start from</param>
		/// <param name="propertyStop">Property name to stop at</param>
		public void DrawPropertiesFromUpTo(string propertyStart, string propertyStop)
		{
			bool canDraw = false;
			SerializedProperty iterProp = serializedObject.GetIterator();
			ReorderableArrayUtils.IterateDrawProperty(serializedObject, iterProp, isSubEditor, listIndex, editableIndex,
				filter: () =>
				{
					if (iterProp.name.Equals(propertyStop))
						return IterControl.Break;

					if (iterProp.name.Equals(propertyStart))
						canDraw = true;

					if (canDraw == false)
						return IterControl.Continue;

					return IterControl.Draw;
				});
		}

		public void DrawContextMenuButtons()
		{
			if (contextData.Count == 0) return;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Context Menu", EditorStyles.boldLabel);
			foreach (KeyValuePair<string, ContextMenuData> kv in contextData)
			{
				bool enabledState = GUI.enabled;
				bool isEnabled = true;
				if (kv.Value.validate != null)
					isEnabled = (bool)kv.Value.validate.Invoke(target, null);

				GUI.enabled = isEnabled;
				if (GUILayout.Button(kv.Key) && kv.Value.function != null)
				{
					kv.Value.function.Invoke(target, null);
				}
				GUI.enabled = enabledState;
			}
		}
#endregion
	}
}
