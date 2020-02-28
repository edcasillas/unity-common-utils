using CommonUtils.Effects.CamShaking;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.CustomEditors {
    [CustomEditor(typeof(CamShaker))]
    public class CamShakerEditor : UnityEditor.Editor {
        private CamShakeMode testCamShakeMode;
        private float testDuration = 1f;
        
        public override void OnInspectorGUI() {
            var camShaker = (CamShaker) target;

            var helpBoxType = camShaker.transform.parent ? MessageType.Info : MessageType.Warning;
            EditorGUILayout.HelpBox("Please make sure this camera is a children of another Game Object to preserve the original position of the camera when and after shaking.", helpBoxType);
            
            if (GUILayout.Button("Create parent")) {
                var parent = new GameObject("ShakingCamParent");
                parent.transform.position = camShaker.transform.position;
                Undo.RegisterCreatedObjectUndo(parent, "create ShakingCamParent");
                if(camShaker.transform.parent)Undo.SetTransformParent(parent.transform, camShaker.transform.parent, "set new parent");
                Undo.SetTransformParent(camShaker.transform, parent.transform,  "set new parent");
            }
            
            EditorGUILayout.Space();

            if (!Application.isPlaying) {
                EditorGUILayout.HelpBox("Enter play mode to test this component.", MessageType.Info);   
            } else {
                testCamShakeMode = (CamShakeMode) EditorGUILayout.EnumPopup("Shake Mode", testCamShakeMode);
                testDuration = EditorGUILayout.FloatField("Duration", testDuration);
                
                if (GUILayout.Button("Start shaking")) {
                    camShaker.Shake(testCamShakeMode, testDuration);
                }

                if (GUILayout.Button("Stop shaking")) {
                    camShaker.StopShaking();
                }
            }
        }
    }
}