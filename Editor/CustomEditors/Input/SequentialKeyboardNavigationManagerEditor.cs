using CommonUtils.Editor.DebuggableEditors;
using System.Linq;
using CommonUtils.Input.ButtonExternalControllers.SequentialKeyboardNavigation;
using CommonUtils.UnityComponents;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.CustomEditors {
    [CustomEditor(typeof(SequentialKeyboardNavigationManager))]
    public class SequentialKeyboardNavigationManagerEditor : AbstractDebuggableEditor<SequentialKeyboardNavigationManager> {
        private bool _showAllItems;
        private bool _showActiveItems;
        
        protected override void RenderDebug() {
            if (GUILayout.Button("Request active items refresh")) {
                Subject.OnItemEnabledOrDisabled();
            }
            
            EditorGUILayout.IntField("Current Index", Subject.CurrentIndex);
            EditorGUILayout.ObjectField("Current Focus", Subject.CurrentlyFocusedItem.IsValid()? Subject.CurrentlyFocusedItem.gameObject : null, typeof(GameObject), true);
            _showAllItems = EditorExtensions.ReadonlyEnumerable(_showAllItems,
                Subject.AllItems.Select(i => i.gameObject), "All Items");
            _showActiveItems = EditorExtensions.ReadonlyEnumerable(_showActiveItems,
                Subject.CurrentlyActiveItems.Select(i => i.gameObject), "Active Items");
            
            EditorUtility.SetDirty(target);
        }
    }
}