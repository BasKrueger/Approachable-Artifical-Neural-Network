#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace ApproachableANN
{
    [CustomEditor(typeof(ANNTrainer))]
    public sealed class ANNTrainerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var trainer = (ANNTrainer)target;
            if (trainer == null) return;

            DrawHelpBoxes(trainer);
            DrawStats(trainer);

            var centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            
            VisualizeChampionNetwork(trainer);
            FocusButton(trainer);
            SaveButton(trainer);

            centeredStyle.alignment = TextAnchor.LowerLeft;
        }

        private void DrawHelpBoxes(ANNTrainer trainer)
        {
            if (trainer.templateUnit == null)
            {
                EditorGUILayout.HelpBox("Requires an ANN Unit as a Template for training", MessageType.Error);
            }
            EditorGUILayout.HelpBox("You may continue the training by giving the template ANN Unit a previously saved AI.", MessageType.Info);
        }

        private void VisualizeChampionNetwork(ANNTrainer trainer)
        {
            if (trainer.champion != null)
            {
                ANNVisualizer.Visualize(trainer.champion.network);
            }
            else
            {
                ANNVisualizer.VisualizeNull();
            }
        }

        private void FocusButton(ANNTrainer trainer)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Best performing ANNUnit of last Generation");

            var state = !trainer.isChampionHighlighted ? "Off" : "On";

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);

            if (GUILayout.Button($"Focus: {state}"))
            {
                trainer.HighlightChampion(!trainer.isChampionHighlighted);
            }

            EditorGUI.EndDisabledGroup();

            GUILayout.EndHorizontal();
        }

        private void SaveButton(ANNTrainer trainer)
        {
            EditorGUI.BeginDisabledGroup(!Application.isPlaying || trainer.champion == null);

            if (GUILayout.Button("Save best performing AI of last generation"))
            {
                var path = EditorUtility.SaveFilePanelInProject("Choose where you want to to save the neural network", "my trained Neural Network", "asset", "save this thing");
                if (path == "") return;

                if (File.Exists(path))
                {
                    var save = (ANNObject)AssetDatabase.LoadAssetAtPath(path, typeof(ANNObject));
                    save.Copy(trainer.champion, trainer.highestFitness);
                }
                else
                {
                    var save = ScriptableObject.CreateInstance<ANNObject>();
                    save.Copy(trainer.champion, trainer.highestFitness);
                    AssetDatabase.CreateAsset(save, path);
                    AssetDatabase.SaveAssets();
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        private void DrawStats(ANNTrainer trainer)
        {
            GUILayout.Label("");
            GUILayout.Label("Training Statistics", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUILayout.Label("Current generation: " + trainer.generation);
            GUILayout.Label("High Score: " + (trainer.highestFitness != Mathf.NegativeInfinity ? Mathf.RoundToInt(trainer.highestFitness) : 0));
            GUILayout.Label("Top performing average: " + Mathf.RoundToInt(trainer.lastTopAverage));
            GUILayout.Label("Learnings: " + Mathf.RoundToInt(trainer.lastImprovement));

            if (trainer.stagnated)
            {
                EditorGUILayout.HelpBox("Stagnated: The average participant score has reached the high score. " +
                    "It is unlikely that the AI will improve any further. " +
                    "Consider Changing the way you punish and reward the ANN Unit, if it hasn't learned the desired behaviour yet."
                    , MessageType.Warning);
            }
        }
    }
}
# endif