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
            EditorGUILayout.IntField("Current Index", SequentialKeyboardNavigationManager.CurrentIndex);
            EditorGUILayout.ObjectField("Current Focus", SequentialKeyboardNavigationManager.CurrentlyFocusedItem.IsValid()? SequentialKeyboardNavigationManager.CurrentlyFocusedItem.gameObject : null, typeof(GameObject), true);
            _showAllItems = EditorExtensions.ReadonlyEnumerable(_showAllItems,
                SequentialKeyboardNavigationManager.AllItems.Select(i => i.gameObject), "All Items");
            _showActiveItems = EditorExtensions.ReadonlyEnumerable(_showActiveItems,
                SequentialKeyboardNavigationManager.CurrentlyActiveItems.Select(i => i.gameObject), "Active Items");
        }
    }
}