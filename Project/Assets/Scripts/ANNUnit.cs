using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using ApproachableANN.Senses;

namespace ApproachableANN
{
    /// <summary>
    /// Monobheaviour Wrapper for Artifical Neural Networks 
    /// </summary>
    public sealed class ANNUnit : MonoBehaviour
    {
        /// <summary>
        /// Invoked during once per Update loop
        /// </summary>
        public UnityEvent<Dictionary<string, float>> Decided;

        /// <summary>
        /// Trained AI that gets loaded on awake
        /// </summary>
        [SerializeField]
        public ANNObject AIToLoad;

        /// <summary>
        /// What this unit can see
        /// </summary>
        [HideInInspector]
        public ANNSense[] senses;
        
        /// <summary>
        /// What this unit can do
        /// </summary>
        [HideInInspector]
        public string[] decisions;
        
        /// <summary>
        /// How fit this unit is. Higher fitness lets it survive longer during the training.
        /// </summary>
        public float fitness { get; private set; }

        /// <summary>
        /// Artifical Neural Network that gets used for decisionmaking
        /// </summary>
        public ArtificalNeuralNetwork network { get; private set; }

        /// <summary>
        /// Wether or not debug gizmos should be drawn
        /// </summary>
        public bool showDebugInformation = true;
        private float debugActivePunishedDuration;
        private float debugActiveRewardedDuration;

        /// <summary>
        /// Replaces the active AI with a deep copy of another AI
        /// </summary>
        public void OverrideAI(ArtificalNeuralNetwork network)
        {
            this.network = new ArtificalNeuralNetwork(network);
        }

        private void Awake()
        {
            if(decisions.Length == 0)
            {
                Debug.LogError("ANN Error: Didn't register any possible decisions");
            }

            if (senses.Contains(null))
            {
                Debug.LogError("ANN Error: Senses may not contain 'Null'");
            }
        }

        /// <summary>
        /// Makes decisions based on the thing this can see and sends the result via the "Decided" event
        /// </summary>
        public void Update()
        {
            if (network == null)
            {
                return;
            }

            if(Decided.GetPersistentEventCount() == 0)
            {
                Debug.LogWarning("ANN Warning: Decisions of this ANN Unit won't get executed, because no one is listening to 'Decided' event.");
            }

            var namedResults = new Dictionary<string, float>();
            var inputs = senses.Select(vision => vision.value).ToArray();
            var result = network.FeedForward(inputs);

            for (int i = 0; i < result.Count(); i++)
            {
                namedResults.Add(decisions[i], result[i]);
            }

            Decided.Invoke(namedResults);
        }

        /// <summary>
        /// Tells this Unit that it did something good
        /// </summary>
        /// <param name="amount">how good the thing it did was</param>
        public void Reward(float amount)
        {
            if (amount <= 0) return;

            fitness += amount;
            debugActiveRewardedDuration = 10 * Time.deltaTime;
        }

        /// <summary>
        /// Tells this Unit that it did something bad
        /// </summary>
        /// <param name="amount">How bad the thing it did was</param>
        public void Punish(float amount)
        {
            if (amount <= 0) return;

            fitness -= amount;
            debugActivePunishedDuration = 10 * Time.deltaTime;
        }

        /// <summary>
        /// Disables all renderes of this gameobject and its children
        /// </summary>
        public void Hide()
        {
            foreach (var renderer in GetComponentsInChildren<Renderer>(true))
            {
                renderer.enabled = false;
            }
        }

        /// <summary>
        /// Enables all renderers of this gameobject and its children
        /// </summary>
        public void Show()
        {
            foreach(var renderer in GetComponentsInChildren<Renderer>(true))
            {
                renderer.enabled = true;
            }
        }

        public void OnValidate()
        {
            if (AIToLoad != null)
            {
                for(int i = 0; i < senses.Length && i < AIToLoad.inputTypes.Length; i++)
                {
                    if (senses[i] != null && senses[i].GetType().AssemblyQualifiedName != AIToLoad.inputTypes[i])
                    {
                        senses[i] = null;
                    }
                }

                this.decisions = AIToLoad.outputNames;

                network = new ArtificalNeuralNetwork(AIToLoad);
                return;
            }
            
            if(senses != null && decisions != null)
            {
                network = new ArtificalNeuralNetwork(senses.Length, decisions.Length);
                return;
            }

            network = null;
        }

        private void OnDrawGizmos()
        {
            if (!showDebugInformation) return;

            if (GetComponentsInChildren<Renderer>().Select(renderer => renderer.gameObject != this.gameObject && renderer.enabled).ToArray().Length == 0) return;

            if (debugActivePunishedDuration > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position + new Vector3(0,0.25f), 0.1f);
                debugActivePunishedDuration -= Time.deltaTime;
            }
            if (debugActiveRewardedDuration > 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position - new Vector3(0, 0.25f), 0.1f);
                debugActiveRewardedDuration -= Time.deltaTime;
            }

            Handles.Label(transform.position, fitness.ToString());
        }
    }
}


