using UnityEngine;

namespace CommonUtils.Inspector.SceneRefs {
    public class SceneReferenceTest : MonoBehaviour {
        public SceneReference exampleNull;
        public SceneReference exampleMissing;
        public SceneReference exampleDisabled;
        public SceneReference exampleEnabled;
        
        private void OnGUI() {
            displayLevel(exampleNull);
            displayLevel(exampleMissing);
            displayLevel(exampleDisabled);
            displayLevel(exampleEnabled);
        }

        private static void displayLevel(SceneReference scene) {
            GUILayout.Label(new GUIContent("Scene name Path: " + scene));
            if (GUILayout.Button("Load " + scene)) {
                UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
            }
        }
    }
}