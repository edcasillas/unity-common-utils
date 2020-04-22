using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SubjectNerd.Utilities {
	public partial class ReorderableArrayInspector {
		/// <summary>
		/// Internal class that manages ReorderableLists for each reorderable
		/// SerializedProperty in a SerializedObject's direct child
		/// </summary>
		public class SortableListData
		{
			public string            Parent { get; private set; }
			public Func<int, string> ElementHeaderCallback = null;

			private readonly Dictionary<string, ReorderableList> propIndex = new Dictionary<string, ReorderableList>();
			private readonly Dictionary<string, Action<SerializedProperty, Object[]>> propDropHandlers = new Dictionary<string, Action<SerializedProperty, Object[]>>();
			private readonly Dictionary<string, int> countIndex = new Dictionary<string, int>();

			public SortableListData(string parent)
			{
				Parent = parent;
			}

			public void AddProperty(SerializedProperty property)
			{
				// Check if this property actually belongs to the same direct child
				if (ReorderableArrayUtils.GetGrandParentPath(property).Equals(Parent) == false)
					return;

				ReorderableList propList = new ReorderableList(
					property.serializedObject, property,
					draggable: true, displayHeader: false,
					displayAddButton: true, displayRemoveButton: true)
				{
					headerHeight = 5
				};

				propList.drawElementCallback = delegate (Rect rect, int index, bool active, bool focused)
				{
					SerializedProperty targetElement = property.GetArrayElementAtIndex(index);

					bool isExpanded = targetElement.isExpanded;
					rect.height = EditorGUI.GetPropertyHeight(targetElement, GUIContent.none, isExpanded);

					if (targetElement.hasVisibleChildren)
						rect.xMin += 10;

					// Get Unity to handle drawing each element
					GUIContent propHeader = new GUIContent(targetElement.displayName);
					if (ElementHeaderCallback != null)
						propHeader.text = ElementHeaderCallback(index);
					EditorGUI.PropertyField(rect, targetElement, propHeader, isExpanded);

					// If drawing the selected element, use it to set the element height
					// Element height seems to control selected background
#if UNITY_5_1 || UNITY_5_2
					if (index == propList.index)
					{
						// Height might have changed when dealing with serialized class
						// Call the select callback when height changes to reset the list elementHeight
						float newHeight = EditorGUI.GetPropertyHeight(targetElement, GUIContent.none, targetElement.isExpanded);
						if (rect.height != newHeight)
							propList.elementHeight = Mathf.Max(propList.elementHeight, newHeight);
					}
#endif
				};

				// Unity 5.3 onwards allows reorderable lists to have variable element heights
#if UNITY_5_3_OR_NEWER
				propList.elementHeightCallback = index => ElementHeightCallback(property, index);

				propList.drawElementBackgroundCallback = (rect, index, active, focused) =>
				{
					if (styleHighlight == null)
						styleHighlight = GUI.skin.FindStyle("MeTransitionSelectHead");
					if (focused == false)
						return;
					rect.height = ElementHeightCallback(property, index);
					GUI.Box(rect, GUIContent.none, styleHighlight);
				};
#endif
				propIndex.Add(property.propertyPath, propList);
			}

			private float ElementHeightCallback(SerializedProperty property, int index)
			{
				SerializedProperty arrayElement = property.GetArrayElementAtIndex(index);
				float calculatedHeight = EditorGUI.GetPropertyHeight(arrayElement,
					GUIContent.none,
					arrayElement.isExpanded);
				calculatedHeight += 3;
				return calculatedHeight;
			}

			public bool DoLayoutProperty(SerializedProperty property)
			{
				if (propIndex.ContainsKey(property.propertyPath) == false)
					return false;

				// Draw the header
				string headerText = string.Format("{0} [{1}]", property.displayName, property.arraySize);
				EditorGUILayout.PropertyField(property, new GUIContent(headerText), false);

				// Save header rect for handling drag and drop
				Rect dropRect = GUILayoutUtility.GetLastRect();

				// Draw the reorderable list for the property
				if (property.isExpanded)
				{
					int newArraySize = EditorGUILayout.IntField("Size", property.arraySize);
					if (newArraySize != property.arraySize)
						property.arraySize = newArraySize;
					propIndex[property.propertyPath].DoLayoutList();
				}

				// Handle drag and drop into the header
				Event evt = Event.current;
				if (evt == null)
					return true;

				if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
				{
					if (dropRect.Contains(evt.mousePosition) == false)
						return true;

					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					if (evt.type == EventType.DragPerform)
					{
						DragAndDrop.AcceptDrag();
						Action<SerializedProperty, Object[]> handler = null;
						if (propDropHandlers.TryGetValue(property.propertyPath, out handler))
						{
							if (handler != null)
								handler(property, DragAndDrop.objectReferences);
						}
						else
						{
							foreach (Object dragged_object in DragAndDrop.objectReferences)
							{
								if (dragged_object.GetType() != property.GetType())
									continue;

								int newIndex = property.arraySize;
								property.arraySize++;

								SerializedProperty target = property.GetArrayElementAtIndex(newIndex);
								target.objectReferenceInstanceIDValue = dragged_object.GetInstanceID();
							}
						}
						evt.Use();
					}
				}
				return true;
			}

			public int GetElementCount(SerializedProperty property)
			{
				if (property.arraySize <= 0)
					return 0;

				int count;
				if (countIndex.TryGetValue(property.propertyPath, out count))
					return count;

				var element      = property.GetArrayElementAtIndex(0);
				var countElement = element.Copy();
				int childCount   = 0;
				if (countElement.NextVisible(true))
				{
					int depth = countElement.Copy().depth;
					do
					{
						if (countElement.depth != depth)
							break;
						childCount++;
					} while (countElement.NextVisible(false));
				}

				countIndex.Add(property.propertyPath, childCount);
				return childCount;
			}

			public ReorderableList GetPropertyList(SerializedProperty property)
			{
				if (propIndex.ContainsKey(property.propertyPath))
					return propIndex[property.propertyPath];
				return null;
			}

			public void SetDropHandler(SerializedProperty property, Action<SerializedProperty, Object[]> handler)
			{
				string path = property.propertyPath;
				if (propDropHandlers.ContainsKey(path))
					propDropHandlers[path] = handler;
				else
					propDropHandlers.Add(path, handler);
			}
		}
	}
}