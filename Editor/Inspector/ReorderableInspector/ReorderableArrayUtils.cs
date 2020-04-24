using System;
using System.Collections.Generic;
using CommonUtils.Editor;
using CommonUtils.Inspector.ReorderableInspector;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SubjectNerd.Utilities {
	public static class ReorderableArrayUtils {
		public static string GetGrandParentPath(SerializedProperty property) {
			string parent = property.propertyPath;
			int firstDot = property.propertyPath.IndexOf('.');
			if (firstDot > 0) {
				parent = property.propertyPath.Substring(0, firstDot);
			}

			return parent;
		}

		private static void createListData(SerializedProperty property, List<SortableListData> listIndex) {
			string parent = GetGrandParentPath(property);

			// Try to find the grand parent in SortableListData
			SortableListData data = listIndex.Find(listData => listData.Parent.Equals(parent));
			if (data == null) {
				data = new SortableListData(parent);
				listIndex.Add(data);
			}

			data.AddProperty(property);
			var arrayAttr = getReorderableAttribute(property);
			if (arrayAttr != null) {
				HandleReorderableOptions(arrayAttr, property, data);
			}
		}

		private static ReorderableAttribute getReorderableAttribute(SerializedProperty property) {
			object[] attr = property.GetAttributes<ReorderableAttribute>();
			return attr != null && attr.Length == 1 ? (ReorderableAttribute)attr[0] : null;
		}

		private static void HandleReorderableOptions(ReorderableAttribute arrayAttr, SerializedProperty property, SortableListData data, GUIStyle styleHighlight = null) {
			// Draw property as single line
			if (arrayAttr.ElementSingleLine) {
				var list = data.GetPropertyList(property);
#if UNITY_5_3_OR_NEWER
				list.elementHeightCallback = index => EditorGUIUtility.singleLineHeight + 6;
				list.drawElementBackgroundCallback = (rect, index, active, focused) => {
					if (focused == false)
						return;
					if (styleHighlight == null)
						styleHighlight = GUI.skin.FindStyle("MeTransitionSelectHead");
					GUI.Box(rect, GUIContent.none, styleHighlight);
				};
#endif

				list.drawElementCallback = (rect, index, active, focused) => {
					var element = property.GetArrayElementAtIndex(index);
					element.isExpanded = false;

					int childCount = data.GetElementCount(property);
					if (childCount < 1)
						return;

					rect.y += 3;
					rect.height -= 6;

					if (element.NextVisible(true)) {
						float restoreWidth = EditorGUIUtility.labelWidth;
						EditorGUIUtility.labelWidth /= childCount;

						float padding = 5f;
						float width = rect.width - padding * (childCount - 1);
						width /= childCount;

						Rect childRect = new Rect(rect) { width = width };
						int depth = element.Copy().depth;
						do
						{
							if (element.depth != depth)
								break;

							if (childCount <= 2)
								EditorGUI.PropertyField(childRect, element, false);
							else
								EditorGUI.PropertyField(childRect, element, GUIContent.none, false);
							childRect.x += width + padding;
						} while (element.NextVisible(false));

						EditorGUIUtility.labelWidth = restoreWidth;
					}
				};
			}
		}

		public static void FindTargetProperties(SerializedObject serializedObject, List<SortableListData> listIndex, Dictionary<string, Editor> editableIndex, ref bool hasSortableArrays, ref bool hasEditable) {
			listIndex.Clear();
			editableIndex.Clear();
			Type typeScriptable = typeof(ScriptableObject);

			SerializedProperty iterProp = serializedObject.GetIterator();
			// This iterator goes through all the child serialized properties, looking
			// for properties that have the SortableArray attribute
			if (iterProp.NextVisible(true)) {
				do {
					if (iterProp.isArray && iterProp.propertyType != SerializedPropertyType.String) {
#if LIST_ALL_ARRAYS
						bool canTurnToList = true;
#else
						bool canTurnToList = iterProp.HasAttribute<ReorderableAttribute>();
#endif
						if (canTurnToList) {
							hasSortableArrays = true;
							createListData(serializedObject.FindProperty(iterProp.propertyPath), listIndex);
						}
					}

					if (iterProp.propertyType == SerializedPropertyType.ObjectReference) {
						Type propType = iterProp.GetTypeReflection();
						if (propType == null)
							continue;

						bool isScriptable = propType.IsSubclassOf(typeScriptable);
						if (isScriptable) {
#if EDIT_ALL_SCRIPTABLES
							bool makeEditable = true;
#else
							bool makeEditable = iterProp.HasAttribute<EditScriptableAttribute>();
#endif

							if (makeEditable) {
								Editor scriptableEditor = null;
								if (iterProp.objectReferenceValue != null) {
#if UNITY_5_6_OR_NEWER
									Editor.CreateCachedEditorWithContext(iterProp.objectReferenceValue,
										serializedObject.targetObject,
										null,
										ref scriptableEditor);
#else
									CreateCachedEditor(iterProp.objectReferenceValue, null, ref scriptableEditor);
#endif
									var reorderable = scriptableEditor as ReorderableArrayInspector;
									if (reorderable != null)
										reorderable.isSubEditor = true;
								}

								editableIndex.Add(iterProp.propertyPath, scriptableEditor);
								hasEditable = true;
							}
						}
					}
				} while (iterProp.NextVisible(true));
			}

			if (hasSortableArrays == false) {
				listIndex.Clear();
			}
		}

		public static void IterateDrawProperty(SerializedObject serializedObject, SerializedProperty property, bool isSubEditor,List<SortableListData> listIndex,Dictionary<string, Editor> editableIndex, Func<IterControl> filter = null)
		{
			if (property.NextVisible(true))
			{
				// Remember depth iteration started from
				int depth = property.Copy().depth;
				do
				{
					// If goes deeper than the iteration depth, get out
					if (property.depth != depth)
						break;
					if (isSubEditor && property.name.Equals("m_Script"))
						continue;

					if (filter != null)
					{
						var filterResult = filter();
						if (filterResult == IterControl.Break)
							break;
						if (filterResult == IterControl.Continue)
							continue;
					}

					DrawPropertySortableArray(serializedObject, property, listIndex, editableIndex, isSubEditor);
				} while (property.NextVisible(false));
			}
		}

		/// <summary>
		/// Draw a SerializedProperty as a ReorderableList if it was found during
		/// initialization, otherwise use EditorGUILayout.PropertyField
		/// </summary>
		public static void DrawPropertySortableArray(SerializedObject serializedObject, SerializedProperty property, List<SortableListData> listIndex, Dictionary<string, Editor> editableIndex, bool isSubEditor)
		{
			// Try to get the sortable list this property belongs to
			SortableListData listData = null;
			if (listIndex.Count > 0)
				listData = listIndex.Find(data => property.propertyPath.StartsWith(data.Parent));

			Editor scriptableEditor;
			bool isScriptableEditor = editableIndex.TryGetValue(property.propertyPath, out scriptableEditor);

			// Has ReorderableList
			if (listData != null)
			{
				// Try to show the list
				if (!listData.DoLayoutProperty(property))
				{
					EditorGUILayout.PropertyField(property, false);
					if (property.isExpanded)
					{
						EditorGUI.indentLevel++;
						SerializedProperty targetProp = serializedObject.FindProperty(property.propertyPath);
						IterateDrawProperty(serializedObject, targetProp, isSubEditor, listIndex, editableIndex);
						EditorGUI.indentLevel--;
					}
				}
			}
			// Else try to draw ScriptableObject editor
			else if (isScriptableEditor)
			{
				bool hasHeader = property.HasAttribute<HeaderAttribute>();
				bool hasSpace = property.HasAttribute<SpaceAttribute>();

				float foldoutSpace = hasHeader ? 24 : 7;
				if (hasHeader && hasSpace)
					foldoutSpace = 31;

				hasSpace |= hasHeader;

				// No data in property, draw property field with create button
				if (scriptableEditor == null)
				{
					bool doCreate;
					using (new EditorGUILayout.HorizontalScope())
					{
						EditorGUILayout.PropertyField(property, GUILayout.ExpandWidth(true));
						using (new EditorGUILayout.VerticalScope(GUILayout.Width(50)))
						{
							if (hasSpace) GUILayout.Space(10);
							doCreate = GUILayout.Button(new GUIContent("Create"), EditorStyles.miniButton);
						}
					}

					if (doCreate)
					{
						Type propType = property.GetTypeReflection();
						var createdAsset = ScriptableObjectUtility.CreateAssetWithSavePrompt(propType, "Assets");
						if (createdAsset != null)
						{
							property.objectReferenceValue = createdAsset;
							property.isExpanded = true;
						}
					}
				}
				// Has data in property, draw foldout and editor
				else
				{
					EditorGUILayout.PropertyField(property);

					Rect rectFoldout = GUILayoutUtility.GetLastRect();
					rectFoldout.width = 20;
					if (hasSpace) rectFoldout.yMin += foldoutSpace;

					property.isExpanded = EditorGUI.Foldout(rectFoldout, property.isExpanded, GUIContent.none);

					if (property.isExpanded)
					{
						EditorGUI.indentLevel++;
						using (new EditorGUILayout.VerticalScope(new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(5, 5, 5, 5) }))
						{
							var restoreIndent = EditorGUI.indentLevel;
							EditorGUI.indentLevel = 1;
							scriptableEditor.serializedObject.Update();
							scriptableEditor.OnInspectorGUI();
							scriptableEditor.serializedObject.ApplyModifiedProperties();
							EditorGUI.indentLevel = restoreIndent;
						}
						EditorGUI.indentLevel--;
					}
				}
			}
			else
			{
				SerializedProperty targetProp = serializedObject.FindProperty(property.propertyPath);

				bool isStartProp = targetProp.propertyPath.StartsWith("m_");
				using (new EditorGUI.DisabledScope(isStartProp))
				{
					EditorGUILayout.PropertyField(targetProp, targetProp.isExpanded);
				}
			}
		}

		public static void DrawSortableArray(SerializedProperty property) {
			var reorderableList = GetReorderableList(property);
			if (reorderableList == null) {
				Debug.LogError($"Property {property.name} is not a reorderable list.");
				return;
			}

			// Draw the header
			var headerText = $"{property.displayName} [{property.arraySize}]";
			EditorGUILayout.PropertyField(property, new GUIContent(headerText), false);

			// Save header rect for handling drag and drop
			Rect dropRect = GUILayoutUtility.GetLastRect();

			// Draw the reorderable list for the property
			if (property.isExpanded) {
				int newArraySize = EditorGUILayout.IntField("Size", property.arraySize);
				if (newArraySize != property.arraySize)
					property.arraySize = newArraySize;
				reorderableList.DoLayoutList();
			}

			// Handle drag and drop into the header
			Event evt = Event.current;
			if (evt == null)
				return;

			if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform) {
				if (dropRect.Contains(evt.mousePosition) == false)
					return;

				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				if (evt.type == EventType.DragPerform) {
					DragAndDrop.AcceptDrag();
					//if (propDropHandlers.TryGetValue(property.propertyPath, out Action<SerializedProperty, Object[]> handler)) {
					//	handler?.Invoke(property, DragAndDrop.objectReferences);
					//} else {
						foreach (var dragged_object in DragAndDrop.objectReferences) {
							if (dragged_object.GetType() != property.GetType())
								continue;

							int newIndex = property.arraySize;
							property.arraySize++;

							var target = property.GetArrayElementAtIndex(newIndex);
							target.objectReferenceInstanceIDValue = dragged_object.GetInstanceID();
						}
					//}

					evt.Use();
				}
			}
		}

		public static ReorderableList GetReorderableList(SerializedProperty property) {
			var reorderableAttribute = getReorderableAttribute(property);
			if (reorderableAttribute == null) return null;

			Func<int, string> defaultElementHeaderCallback= i => $"{reorderableAttribute.ElementHeader} {(reorderableAttribute.HeaderZeroIndex ? i : i + 1)}";

			ReorderableList propList = new ReorderableList(property.serializedObject,
				property,
				draggable: true,
				displayHeader: false,
				displayAddButton: true,
				displayRemoveButton: true) {
				headerHeight = 5
			};

			propList.drawElementCallback = delegate(Rect rect, int index, bool active, bool focused) {
				SerializedProperty targetElement = property.GetArrayElementAtIndex(index);

				bool isExpanded = targetElement.isExpanded;
				rect.height = EditorGUI.GetPropertyHeight(targetElement, GUIContent.none, isExpanded);

				if (targetElement.hasVisibleChildren)
					rect.xMin += 10;

				// Get Unity to handle drawing each element
				GUIContent propHeader = new GUIContent(targetElement.displayName);

				if (!string.IsNullOrEmpty(reorderableAttribute.ElementHeader)) {
					propHeader.text = defaultElementHeaderCallback(index);
				}

				EditorGUI.PropertyField(rect, targetElement, propHeader, isExpanded);
			};

			propList.elementHeightCallback = index => ElementHeightCallback(property, index);

			propList.drawElementBackgroundCallback = (rect, index, active, focused) => {
				var styleHighlight = GUI.skin.FindStyle("MeTransitionSelectHead");
				if (focused == false)
					return;
				rect.height = ElementHeightCallback(property, index);
				GUI.Box(rect, GUIContent.none, styleHighlight);
			};

			return propList;
		}

		public static float ElementHeightCallback(SerializedProperty property, int index) {
			SerializedProperty arrayElement = property.GetArrayElementAtIndex(index);
			float calculatedHeight = EditorGUI.GetPropertyHeight(arrayElement, GUIContent.none, arrayElement.isExpanded);
			calculatedHeight += 3;
			return calculatedHeight;
		}
	}
}