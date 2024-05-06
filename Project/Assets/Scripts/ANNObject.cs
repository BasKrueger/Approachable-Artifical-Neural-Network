using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ApproachableANN
{
    /// <summary>
    /// Scriptable Object that stores all relevant informations about an ANN Unit
    /// </summary>
    public sealed class ANNObject : ScriptableObject
    {
        public List<Neuron> inputNeurons;
        public List<Neuron> outputNeurons; 
        public List<Neuron> neurons;
        public List<Link> links;

        public string formatedSaveDate;
        public float fitness;
        public string[] inputNames;
        public string[] inputTypes;
        public string[] outputNames;

        /// <summary>
        /// Copies informations from an ANN Unit, as well as a highscore value
        /// </summary>
        public void Copy(ANNUnit unit, float highScore)
        {
            inputNeurons = new List<Neuron>();
            foreach(var neuron in unit.network.inputNeurons)
            {
                inputNeurons.Add(new Neuron(neuron.Value));
            }

            outputNeurons = new List<Neuron>();
            foreach (var neuron in unit.network.outputNeurons)
            {
                outputNeurons.Add(new Neuron(neuron.Value));
            }

            neurons = new List<Neuron>();
            foreach(var neuron in unit.network.neurons)
            {
                neurons.Add(new Neuron(neuron.Value));
            }

            links = new List<Link>();
            foreach(var link in unit.network.links)
            {
                links.Add(new Link(link.Value));
            }

            this.fitness = highScore;
            this.inputNames = unit.senses.Select(sense => sense.gameObject.name).ToArray();
            this.inputTypes = unit.senses.Select(sense => sense.GetType().AssemblyQualifiedName).ToArray();
            this.outputNames = unit.decisions;

            this.formatedSaveDate = GetFormatedCurrentDateTime();
        }

        private string GetFormatedCurrentDateTime()
        {
            var hour = DateTime.Now.Hour.ToString();
            if (DateTime.Now.Hour < 10)
            {
                hour = "0" + hour;
            }
            var minute = DateTime.Now.Minute.ToString();
            if (DateTime.Now.Minute < 10)
            {
                minute = "0" + minute;
            }
            return $"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year} | {hour}:{minute}";
        }
    }

}
