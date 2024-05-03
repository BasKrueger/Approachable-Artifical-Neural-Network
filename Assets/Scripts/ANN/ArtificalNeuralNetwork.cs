using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ApproachableANN
{
    /// <summary>
    /// Artifical Neural Network build to work with the NEAT learning algorithm
    /// </summary>
    public sealed class ArtificalNeuralNetwork
    {
        #region Mutation Odds
        private const float MUTATION_LINK_WEIGHT_CHANCE_PERCENT = 0.15f;
        private const float MUTATION_DISABLE_LINK_CHANCE_PERCENT = 0.001f;
        private const float MUTATION_ENABLE_LINK_CHANCE_PERCENT = 0.001f;
        private const float MUTATION_ADD_NEURON_CHANCE_PERCENT = 0.1f;
        private const float MUTATION_NEURON_BIAS_CHANCE_PERCENT = 0.01f;
        #endregion

        /// <summary>
        /// Neurons in the input layer. Keys are Neuron IDs.
        /// </summary>
        public readonly Dictionary<int, Neuron> inputNeurons = new Dictionary<int, Neuron>();
        /// <summary>
        /// Neurons in the output layer. Keys are Neuron IDs.
        /// </summary>
        public readonly Dictionary<int, Neuron> outputNeurons = new Dictionary<int, Neuron>();

        /// <summary>
        /// Input, Output and hidden Neurons. Keys are Neuron IDs.
        /// </summary>
        public Dictionary<int, Neuron> neurons { get; private set; } = new Dictionary<int, Neuron>();
        /// <summary>
        /// Links between the neurons. Keys are Link IDs.
        /// </summary>
        public Dictionary<int, Link> links { get; private set; } = new Dictionary<int, Link>();

        /// <summary>
        /// Initializes the neural network with a set number of input and output neurons
        /// </summary>
        public ArtificalNeuralNetwork(int inputs, int outputs)
        {
            inputNeurons = new Dictionary<int, Neuron>();
            for (int i = 0; i < inputs; i++)
            {
                var neuron = new Neuron(true);

                inputNeurons.Add(neuron.id, neuron);
                neurons.Add(neuron.id, neuron);
            }

            outputNeurons = new Dictionary<int, Neuron>();
            for (int i = 0; i < outputs; i++)
            {
                var neuron = new Neuron(true);

                outputNeurons.Add(neuron.id, neuron);
                neurons.Add(neuron.id, neuron);
            }

            foreach (var inputPair in inputNeurons)
            {
                foreach (var outputPair in outputNeurons)
                {
                    var link = new Link(inputPair.Key, outputPair.Key);
                    links.Add(link.id, link);
                }
            }
        }

        /// <summary>
        /// Deep Copy Constructor
        /// </summary>
        public ArtificalNeuralNetwork(ArtificalNeuralNetwork other)
        {
            //Deep Copy all inputs
            foreach (var inputPair in other.inputNeurons)
            {
                var neuron = new Neuron(inputPair.Value);

                inputNeurons.Add(inputPair.Key, neuron);
                neurons.Add(inputPair.Key, neuron);
            }

            //Deep Copy all outputs
            foreach (var outputPair in other.outputNeurons)
            {
                var neuron = new Neuron(outputPair.Value);

                outputNeurons.Add(outputPair.Key, neuron);
                neurons.Add(outputPair.Key, neuron);
            }

            //Deep Copy all hidden neurons
            foreach (var hiddenPair in other.neurons)
            {
                if (neurons.ContainsKey(hiddenPair.Key)) continue;

                var neuron = new Neuron(hiddenPair.Value);
                neurons.Add(hiddenPair.Key, neuron);
            }

            //Deep Copy all Links
            foreach (var linkPair in other.links)
            {
                var link = new Link(linkPair.Value);
                links.Add(linkPair.Key, link);
            }
        }

        /// <summary>
        /// Loads the network structure from an Scriptable Object
        /// </summary>
        public ArtificalNeuralNetwork(ANNObject other)
        {
            //Deep Copy all inputs
            foreach (var input in other.inputNeurons)
            {
                var neuron = new Neuron(input);

                inputNeurons.Add(neuron.id, neuron);
                neurons.Add(neuron.id, neuron);
            }

            //Deep Copy all outputs
            foreach (var output in other.outputNeurons)
            {
                var neuron = new Neuron(output);

                outputNeurons.Add(output.id, neuron);
                neurons.Add(output.id, neuron);
            }

            //Deep Copy all hidden neurons
            foreach (var hidden in other.neurons)
            {
                if (neurons.ContainsKey(hidden.id)) continue;

                var neuron = new Neuron(hidden);
                neurons.Add(hidden.id, neuron);
            }

            //Deep Copy all Links
            foreach (var otherLink in other.links)
            {
                var link = new Link(otherLink);
                links.Add(link.id, link);
            }
        }

        /// <summary>
        /// Combines the informations of two different Artifical Neural Networks
        /// </summary>
        public ArtificalNeuralNetwork(ArtificalNeuralNetwork dominent, ArtificalNeuralNetwork recessive)
        {
            #region copy all Input and output neurons from the dominent network
            foreach(var neuronPair in dominent.inputNeurons)
            {
                var neuron = new Neuron(neuronPair.Value);
                inputNeurons.Add(neuron.id, neuron);
                neurons.Add(neuron.id, neuron);
            }

            foreach(var neuronPair in recessive.outputNeurons)
            {
                var neuron = new Neuron(neuronPair.Value);
                outputNeurons.Add(neuron.id, neuron);
                neurons.Add(neuron.id, neuron);
            }
            #endregion

            #region if both have the same links, randomly copy one from either of them, else copy it from the dominent parent only
            foreach (var linkPair in dominent.links)
            {
                var id = linkPair.Key;
                var selectedLink = linkPair.Value;

                if (UnityEngine.Random.Range(0f, 1f) > 0.5f && recessive.links.ContainsKey(id))
                {
                    selectedLink = recessive.links[id];
                }

                var link = new Link(selectedLink);
                links.Add(link.id, link);
            }
            #endregion

            #region Copy all neurons that are referenced by the links. If both have the same neuron, copy a random one.
            foreach (var linkPair in links)
            {
                var id = linkPair.Value.fromID;
                if (neurons.ContainsKey(id)) continue;

                var possibleNeurons = new List<Neuron>();

                if (dominent.neurons.ContainsKey(id))
                {
                    possibleNeurons.Add(dominent.neurons[id]);
                }
                if (recessive.neurons.ContainsKey(id))
                {
                    possibleNeurons.Add(recessive.neurons[id]);
                }

                var neuron = new Neuron(possibleNeurons[Random.Range(0, possibleNeurons.Count)]);
                neurons.Add(neuron.id, neuron);
            }
            #endregion

        }

        /// <summary>
        /// Tries to randomly mutate the Network based on chance. Potentially changing the link weight(s), adding a new Link or adding a new Neuron 
        /// </summary>
        public void Mutate()
        {
            //Mutate the bias of random neurons
            foreach(var neuron in neurons)
            {
                if(MUTATION_NEURON_BIAS_CHANCE_PERCENT >= UnityEngine.Random.Range(0f, 1f))
                {
                    neuron.Value.Mutate();
                }
            }

            //Mutate the weight of random links
            foreach(var linkpair in links)
            {
                if (MUTATION_LINK_WEIGHT_CHANCE_PERCENT >= UnityEngine.Random.Range(0f, 1f))
                {
                    linkpair.Value.MutateWeight();
                }
            }

            //Randomly enable Link(s)
            foreach (var linkPair in links)
            {
                if (linkPair.Value.active) continue;

                if (MUTATION_ENABLE_LINK_CHANCE_PERCENT >= Random.Range(0f, 1f))
                {
                    linkPair.Value.active = true;
                }
            }

            //Randomly disable Link(s)
            foreach (var linkPair in links)
            {
                if (!linkPair.Value.active) continue;

                if (MUTATION_DISABLE_LINK_CHANCE_PERCENT >= Random.Range(0f, 1f))
                {
                    linkPair.Value.active = false;
                }
            }

            //Disable the link between neuron A and C and adds a new neuron B between them.
            if (MUTATION_ADD_NEURON_CHANCE_PERCENT >= UnityEngine.Random.Range(0f, 1f))
            {
                var activeLinks = links.Values.ToArray().Where(link => link.active).ToArray();
                if (activeLinks.Length == 0) return;

                var link_AC = activeLinks[UnityEngine.Random.Range(0, activeLinks.Length)];

                var neuronA = neurons[link_AC.fromID];
                var neuronB = new Neuron(true);
                var neuronC = neurons[link_AC.toID];

                var link_AB = new Link(neuronA.id, neuronB.id);
                var link_BC = new Link(neuronB.id, neuronC.id);

                links[link_AC.id].active = false;
                links.Add(link_AB.id, link_AB);
                links.Add(link_BC.id, link_BC);
                neurons.Add(neuronB.id, neuronB);
            }
        }

        /// <summary>
        /// Sets the values of the input neurons and returns the update output values
        /// </summary>
        public float[] FeedForward(float[] newInput)
        {
            newInput = NormalizeData(newInput);

            var result = new float[outputNeurons.Count];

            var inputList = inputNeurons.Values.ToArray();
            for (int i = 0; i < inputList.Length; i++)
            {
                inputList[i].bias = newInput[i];
            }

            var outputList = outputNeurons.Values.ToArray();
            for (int i = 0; i < outputNeurons.Count; i++)
            {
                var sum = CalculateWeightedSum(outputList[i]);
                result[i] = outputList[i].CalculateActivationValue(sum);
            }

            return result;
        }

        /// <returns>
        /// The raw value of a Neuron
        /// </returns>
        private float CalculateWeightedSum(Neuron neuron)
        {
            //the value of a neuron consists of
            var value = 0f;

            foreach(var linkPair in links)
            {
                if (!linkPair.Value.active) continue;

                //the sum of all neuron values that connect to this neuron
                if (linkPair.Value.toID == neuron.id)
                {
                    var previousNeuron = neurons[linkPair.Value.fromID];
                    var previousNeuronValue = CalculateWeightedSum(previousNeuron);

                    //multiplied by the weight the link has towards the neuron
                    value += linkPair.Value.weight * previousNeuronValue;
                }
            }

            //plus its own bias
            value += neuron.bias;

            return value;
        }

        private static float[] NormalizeData(IEnumerable<float> data)
        {
            var s = data.Sum();

            return data.Select(v => v / s).ToArray();
        }
    }
}
