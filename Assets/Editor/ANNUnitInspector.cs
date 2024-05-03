using UnityEngine.Windows;
using UnityEditor;
using UnityEngine;
using ApproachableANN.Senses;
using System;

#if UNITY_EDITOR
namespace ApproachableANN
{
    [CustomEditor(typeof(ANNUnit))]
    public sealed class ANNUnitInspector : Editor
    {
        bool expandSenses = false;

        public override void OnInspectorGUI()
        {
            var unit = (ANNUnit)target;
            if (unit == null) return;

            DrawProperties(unit);
            DrawAIPreview(unit);
            SaveButton(unit);

            serializedObject.ApplyModifiedProperties();
        }

        private void SaveButton(ANNUnit unitToSave)
        {
            EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying || unitToSave == null);

            if (GUILayout.Button("Save the current AI"))
            {
                var path = EditorUtility.SaveFilePanelInProject("Choose where you want to to save the neural network", 
                    "my trained Neural Network", "asset", "save this thing");
                if (path == "") return;

                if (File.Exists(path))
                {
                    var save = (ANNObject)AssetDatabase.LoadAssetAtPath(path, typeof(ANNObject));
                    save.Copy(unitToSave, unitToSave.fitness);
                }
                else
                {
                    var save = ScriptableObject.CreateInstance<ANNObject>();
                    save.Copy(unitToSave, unitToSave.fitness);
                    AssetDatabase.CreateAsset(save, path);
                    AssetDatabase.SaveAssets();
                }
            }

            EditorGUI.EndDisabledGroup();
        }

        private void DrawProperties(ANNUnit unit)
        {
            var decisionProperty = serializedObject.FindProperty("Decided");
            var inputNameProperty = serializedObject.FindProperty("senses");
            var outputNameProperty = serializedObject.FindProperty("decisions");

            EditorGUI.BeginChangeCheck();
            unit.AIToLoad = (ANNObject)EditorGUILayout.ObjectField("Use already trained AI", unit.AIToLoad, typeof(ANNObject), true);
            if (EditorGUI.EndChangeCheck())
            {
                unit.OnValidate();
            }

            if(unit.AIToLoad == null)
            {
                EditorGUILayout.HelpBox("Save an trained AI with an 'ANN Trainer', or use an already trained AI.", MessageType.Warning);
                EditorGUILayout.PropertyField(inputNameProperty, new GUIContent("What this can see:"));
            }
            else
            {
                EditorGUILayout.HelpBox("You may only use the senses and decisions that the AI used when it was saved.", MessageType.Info);
                expandSenses = EditorGUILayout.Foldout(expandSenses, "What this can see:", true);

                if (expandSenses)
                {
                    var result = new ANNSense[unit.AIToLoad.inputNames.Length];

                    for (int i = 0; i < unit.AIToLoad.inputNames.Length; i++)
                    {
                        var labelName = unit.AIToLoad.inputNames[i];
                        var sense = unit.senses[i];
                        var validType = Type.GetType(unit.AIToLoad.inputTypes[i]);

                        result[i] = (ANNSense)EditorGUILayout.ObjectField(labelName, sense, validType, true);
                    }

                    unit.senses = result;
                    GUILayout.Label("");
                }
            }

            EditorGUI.BeginDisabledGroup(unit.AIToLoad != null);
            EditorGUILayout.PropertyField(outputNameProperty, new GUIContent("Decisions"));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(decisionProperty, new GUIContent("Decided"));
        }

        private void DrawAIPreview(ANNUnit unit)
        {
            GUILayout.Label("AI Preview", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            ANNVisualizer.Visualize(unit.network);
        }
    }
}
#endif