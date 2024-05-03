#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ApproachableANN
{
    [CustomEditor(typeof(ANNObject))]
    public sealed class ANNObjectInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            var target = (ANNObject)base.target;
            if (target == null) return;

            ANNVisualizer.Visualize(target);

            DrawStats(target);
        }

        private void DrawStats(ANNObject target)
        {
            GUILayout.Label($"Last saved at: {target.formatedSaveDate}");
            GUILayout.Label($"Reached a high score of: {target.fitness}");

            GUILayout.Label("");
            GUILayout.Label("Gameobjects that this AI saw with during training:");
            for (int i = 0;i < target.inputNames.Length; i++)
            {
                GUILayout.Label($"   -{target.inputNames[i]}");
            }
           
            GUILayout.Label("");
            GUILayout.Label("Decisions this AI could make during training:");
            foreach (var name in target.outputNames)
            {
                GUILayout.Label($"   -{name}");
            }
        }
    }
}
#endif
