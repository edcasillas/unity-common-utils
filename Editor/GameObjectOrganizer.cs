using System.Collections.Generic;
using System.Linq;
using CommonUtils.Extensions;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor {
    public static class GameObjectOrganizer {
        [MenuItem("GameObject/Organize/Remove inactive children", priority = -100)]
        private static void removeInactiveChildren(MenuCommand command) {
            var selection = Selection.gameObjects;
            if (selection.IsNullOrEmpty()) {
                EditorUtility.DisplayDialog("Remove inactive children", "Please select an object from the hierarchy", "Ok");
                return;
            }

            if (!EditorUtility.DisplayDialog("Remove inactive children",
                                             $"Are you sure you want to remove all inactive children of {(selection.Length == 1 ? $"\"{selection[0].name}\"" : $"{selection.Length} game objects")}?",
                                             "Yes",
                                             "No"))
                return;

            var count = 0;
            foreach (var go in selection) {
                foreach (Transform child in go.transform) {
                    if (!child.gameObject.activeSelf) {
                        Undo.DestroyObjectImmediate(child.gameObject);
                        count++;
                    }
                }
            }

            EditorUtility.DisplayDialog("Remove inactive children",
                                        count > 0 ?
                                            $"{count} objects have been removed." :
                                            $"No inactive children were found.",
                                        "Ok");
        }

        [MenuItem("GameObject/Organize/Sort children/By name", priority = -100)]
        private static void sortChildrenByName() {
            var selection = Selection.gameObjects;
            if (selection.IsNullOrEmpty()) {
                EditorUtility.DisplayDialog("Sort children", "Please select an object from the hierarchy.", "Ok");
                return;
            }

            foreach (var go in selection) {
                var list = new List<Transform>();
                foreach (Transform child in go.transform) {
                    list.Add(child);
                }

                list = list.OrderBy(t => t.name).ToList();

                for (var i = 0; i < list.Count; i++)
                    list[i].SetSiblingIndex(i);
            }
        }

        [MenuItem("GameObject/Organize/Group children/By name", priority = -100)]
        private static void groupChildrenByName() {
            var selection = Selection.gameObjects;
            if (selection.IsNullOrEmpty()) {
                EditorUtility.DisplayDialog("Group children", "Please select an object from the hierarchy.", "Ok");
                return;
            }
        
            var groups = new Dictionary<string, List<Transform>>();
            foreach (var go in selection) {
                foreach (Transform child in go.transform) {
                    var sanitizedName = removeTrailingParenthesis(child.name);
                    if (!groups.ContainsKey(sanitizedName)) groups.Add(sanitizedName, new List<Transform>());
                    groups[sanitizedName].Add(child);
                }

                foreach (var group in groups) {
                    if(group.Value.Count == 1) continue;
                    var groupParent = new GameObject(group.Key);
                    Undo.RegisterCreatedObjectUndo(groupParent, "Created group parent");
                    Undo.SetTransformParent(groupParent.transform, go.transform, "Set new parent");

                    var parentTransform = groupParent.transform;
                    foreach (var groupMember in group.Value) {
                        Undo.SetTransformParent(groupMember, parentTransform, "Set new parent");
                    }
                }
            }
        }

        [MenuItem("GameObject/Organize/Sanitize name", priority = -100)]
        private static void sanitizeNames() {
            var selection = Selection.gameObjects;
            if (selection.IsNullOrEmpty()) {
                EditorUtility.DisplayDialog("Sanitize name", "Please select an object from the hierarchy.", "Ok");
                return;
            }

            foreach (var go in selection) {
                Undo.RecordObject(go, "Sanitize name");
                go.name = removeTrailingParenthesis(go.name);
            }
        }
        
        [MenuItem("GameObject/Organize/Remove inactive children", validate = true)]
        [MenuItem("GameObject/Organize/Sort children/By name", validate    = true)]
        [MenuItem("GameObject/Organize/Group children/By name", validate = true)]
        [MenuItem("GameObject/Organize/Sanitize name", validate = true)]
        private static bool selectionValidate( MenuCommand command ) => Selection.objects.Length > 0;

        private static string removeTrailingParenthesis(string goName) {
            if (string.IsNullOrWhiteSpace(goName)) return goName;

            goName = goName.Trim();
            var start = goName.LastIndexOf('(');
            var end   = goName.LastIndexOf(')');
            if (start >= end || !string.IsNullOrWhiteSpace(goName.Substring(end + 1))) return goName;

            var strInside = goName.Substring(start + 1, end - start - 1);
            if (int.TryParse(strInside, out _)) {
                return goName.Substring(0, start).Trim();
            }

            return goName;
        }
    }
}