using System;
using UnityEngine;

namespace ApproachableANN
{
    /// <summary>
    /// Neuron that stores a bias and activation function.
    /// </summary>
    [System.Serializable]
    public class Neuron
    {
        private static int nextNeuronID = 1;

        /// <summary>
        /// Influences how easy or hard it is to activate this Neuron
        /// </summary>
        [SerializeField]
        public float bias;

        /// <summary>
        /// ID of this Neuron
        /// </summary>
        [field:SerializeField]
        public int id { get; private set; }

        /// <summary>
        /// default Constructor
        /// </summary>
        public Neuron()
        {

        }

        /// <summary>
        /// Initates the neuron with a unique id
        /// </summary>
        public Neuron(bool generateID)
        {
            this.bias = 0;

            if(!generateID) return;
            id = nextNeuronID;
            nextNeuronID++;
        }

        /// <summary>
        /// Deep Copy Constructor
        /// </summary>
        public Neuron(Neuron other)
        {
            this.bias = other.bias;
            this.id = other.id;

            if(nextNeuronID <= other.id)
            {
                nextNeuronID += other.id;
            }
        }

        /// <summary>
        /// Returns the activation value of this neuron based on the sigmoid curve
        /// </summary>
        public float CalculateActivationValue(float weightedSum)
        {
            return 1.0f / (1.0f + (float)Math.Exp(-weightedSum));
        }

        /// <summary>
        /// Randomly mutates the bias
        /// </summary>
        public void Mutate()
        {
            bias += UnityEngine.Random.Range(-0.25f, 0.25f);
        }
    }
}
