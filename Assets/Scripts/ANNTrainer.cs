using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace ApproachableANN
{
    /// <summary>
    /// Trains ANN Units with the NEAT Algorithm
    /// </summary>
    public sealed class ANNTrainer : MonoBehaviour
    {
        /// <summary>
        /// Wether or not the training automatically begins during the "Start" Function
        /// </summary>
        [SerializeField]
        private bool autoStart = true;

        /// <summary>
        /// ANNUnit that will be trained
        /// </summary>
        [SerializeField]
        public ANNUnit templateUnit;

        /// <summary>
        /// Setting for wether or not only the champion should be visible.
        /// </summary>
        public bool isChampionHighlighted { get; private set; } = false;

        /// <summary>
        /// Length of each individual Trainingsessions in seconds
        /// </summary>
        [SerializeField]
        private float sessionDuration = 30;

        /// <summary>
        /// How many ANNUnits are in each trainingsSession
        /// </summary>
        [SerializeField]
        private float participants = 50;

        /// <summary>
        /// Invoked after each training session
        /// </summary>
        [SerializeField]
        private UnityEvent SessionEnded;

        /// <summary>
        /// The collection of ANNUnits that try to learn
        /// </summary>
        private List<ANNUnit> population;

        /// <summary>
        /// The training that is currently in process
        /// </summary>
        private Coroutine ongoingTraining;

        #region Generation Stats to display in the inspector
        /// <summary>
        /// The current ANNUnit that uses the brain of the top performer of the previous generation
        /// </summary>
        public ANNUnit champion { get; private set; }
        /// <summary>
        /// How many trainingsessions have past
        /// </summary>
        public float generation { get; private set; } = 0;
        /// <summary>
        /// The highest fitness of all time
        /// </summary>
        public float highestFitness { get; private set; } = 0;
        /// <summary>
        /// The average fitness of the last generation
        /// </summary>
        public float lastTopAverage { get; private set; }
        /// <summary>
        /// How much the average fitness improved since the last generation
        /// </summary>
        public float lastImprovement { get; private set; } = 0;
        /// <summary>
        /// Wether or not the top average reached the highscore
        /// </summary>
        public bool stagnated { get; private set; } = false;
        #endregion

        private void Start()
        {
            if(templateUnit == null)
            {
                Debug.LogError("Error: Requires a template for training");
                return;
            }

            isChampionHighlighted = false;

            if (autoStart)
            {
                Train();
            }
        }

        /// <summary>
        /// Starts the training process
        /// </summary>
        public void Train()
        {
            ongoingTraining = StartCoroutine(Training());
            
            IEnumerator Training()
            {
                highestFitness = Mathf.NegativeInfinity;

                population = CreatePopulation();

                while (true)
                {
                    yield return new WaitForSeconds(sessionDuration);

                    SessionEnded?.Invoke();

                    UpdateGenerationStats();

                    Breed();

                    HighlightChampion(isChampionHighlighted);
                }
            }
        }

        /// <summary>
        /// Destroys the current population and ends the training
        /// </summary>
        public void Stop()
        {
            if (ongoingTraining == null) return;

            StopCoroutine(ongoingTraining);
            
            foreach(var unit in population)
            {
                Destroy(unit.gameObject);
            }
            population = new List<ANNUnit>();
        }

        /// <returns> The created population</returns>
        private List<ANNUnit> CreatePopulation()
        {
            if(templateUnit == null)
            {
                Debug.Log("Requires a template unit to train");
                return new List<ANNUnit>();
            }

            templateUnit.gameObject.SetActive(false);

            var population = new List<ANNUnit>();
            if (templateUnit.AIToLoad != null)
            {
                champion = Instantiate(templateUnit, templateUnit.transform.parent);
                champion.OverrideAI(new ArtificalNeuralNetwork(champion.AIToLoad));
                population.Add(champion);
            }

            while(population.Count < participants)
            {
                var newUnit = Instantiate(templateUnit, templateUnit.transform.parent);

                var brain = new ArtificalNeuralNetwork(templateUnit.network);
                brain.Mutate();

                newUnit.OverrideAI(brain);
                newUnit.gameObject.SetActive(true);

                population.Add(newUnit);
            }

            return population;
        }

        /// <summary>
        /// Crosses the units with similar high fitness with each other
        /// </summary>
        private void Breed()
        {
            var nextGenerationBrains = new List<ArtificalNeuralNetwork>();
            var results = RankPopulation();

            //Generate and save the brains for the next generation
            for (int i = 0; i < results.Length / 2; i++)
            {
                var dominent = results[i];
                var recessive = results[i + 1];

                var childBrain = new ArtificalNeuralNetwork(dominent.network, recessive.network);
                if(Random.Range(0f,1f) <= 0.75f) childBrain.Mutate();

                nextGenerationBrains.Add(dominent.network);
                nextGenerationBrains.Add(childBrain);
            }

            //Destroy the original population
            while (population.Count > 0)
            {
                Destroy(population[0].gameObject);
                population.RemoveAt(0);
            }

            //Replace it with the new generation
            foreach (var brain in nextGenerationBrains)
            {
                var newUnit = Instantiate(templateUnit, templateUnit.transform.parent);
                newUnit.OverrideAI(brain);
                newUnit.gameObject.SetActive(true);

                population.Add(newUnit);
            }

            champion = population[0];
        }

        /// <summary>
        /// Updates the stats for inspector displaying purposes
        /// </summary>
        private void UpdateGenerationStats()
        {
            var results = RankPopulation();
            var topHalf = results.Take(results.Length / 2).ToArray();
            var average = topHalf.Sum(unit => unit.fitness) / topHalf.Length;

            lastImprovement = Mathf.Abs(lastTopAverage - average) * (lastTopAverage > average ? -1 : 1);
            lastTopAverage = average;
            generation++;

            if (results[0].fitness > highestFitness)
            {
                highestFitness = results[0].fitness;
            }
            stagnated = Mathf.RoundToInt(average) == highestFitness;

            //Update Inspector
            var inspectorWindowType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            var targetInspector = EditorWindow.GetWindow(inspectorWindowType, false, null, false);
            targetInspector.Repaint();
        }

        /// <summary>
        /// Ranks the population by descending fitness. Ties are sorted by their ascending complexity.
        /// </summary>
        /// <returns></returns>
        private ANNUnit[] RankPopulation()
        {
            return population
                .OrderByDescending(m => m.fitness)
                .ThenBy(m => m.network.links.Sum(link => link.Value.active ? 1 : 0))
                .ToArray();
        }

        /// <summary>
        /// Toggle wether or not all members of the population, except the champion, should be hidden
        /// </summary>
        public void HighlightChampion(bool state)
        {
            if (population == null) return;

            foreach(var unit in population)
            {
                if (state)
                {
                    if (unit == champion) continue;
                    unit.Hide();
                    unit.showDebugInformation = false;
                    continue;
                }

                unit.Show();
                unit.showDebugInformation = true;
            }

            isChampionHighlighted = state;
        }
    }
}