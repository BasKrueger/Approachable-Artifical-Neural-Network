#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ApproachableANN
{
    public static class ANNVisualizer
    {
        private const int PANEL_HEIGHT = 175;
        private const int NEURON_SIZE = 10;
        private const float INPUTNEURON_POSITION_X = 0.1f;
        private const float OUTPUTNEURON_POSITION_X = 0.9f;
        private const float HIDDENNEURON_MIN_X = 0.15f;
        private const float HIDDENNEURON_MAX_X = 0.85f;

        public static void VisualizeNull()
        {
            GUILayout.Box(GetEmptyTexture());
        }

        /// <summary>
        /// Draws a GUILayout.Box with a texture that displays the ANN
        /// </summary>
        public static void Visualize(ArtificalNeuralNetwork network)
        {
            if(network == null)
            {
                VisualizeNull();
                return;
            }

            var texture = GetEmptyTexture();
            var neuronPositions = new Dictionary<int, Vector2>();

            AddNeuronPositionsColumn(texture, in neuronPositions, network.inputNeurons.Values.ToList(), INPUTNEURON_POSITION_X);
            AddNeuronPositionsColumn(texture, in neuronPositions, network.outputNeurons.Values.ToList(), OUTPUTNEURON_POSITION_X);
            AddNeuronPositionsRandom(texture, in neuronPositions, network.neurons.Values.ToList(), HIDDENNEURON_MIN_X, HIDDENNEURON_MAX_X);

            DrawLinks(in texture, neuronPositions, network.links.Values.ToList());
            DrawNeurons(in texture, neuronPositions);

            texture.Apply();
            GUILayout.Box(texture);
        }

        /// <summary>
        /// Draws a GUILayout.Box with a texture that displays an ANN from the informations saved in an ANNObject
        /// </summary>
        public static void Visualize(ANNObject toVisualize)
        {
            var texture = GetEmptyTexture();
            var neuronPositions = new Dictionary<int, Vector2>();

            AddNeuronPositionsColumn(texture, in neuronPositions, toVisualize.inputNeurons, INPUTNEURON_POSITION_X);
            AddNeuronPositionsColumn(texture, in neuronPositions, toVisualize.outputNeurons, OUTPUTNEURON_POSITION_X);
            AddNeuronPositionsRandom(texture, in neuronPositions, toVisualize.neurons, HIDDENNEURON_MIN_X, HIDDENNEURON_MAX_X);

            DrawLinks(in texture, neuronPositions, toVisualize.links);
            DrawNeurons(in texture, neuronPositions);

            DrawLinks(in texture, neuronPositions, toVisualize.links);
            DrawNeurons(in texture, neuronPositions);

            texture.Apply();
            GUILayout.Box(texture);
        }

        /// <returns> A texture with the correct width and height</returns>
        private static Texture2D GetEmptyTexture()
        {
            var tex = new Texture2D(Mathf.RoundToInt(Screen.width * 0.90f), PANEL_HEIGHT);
            FillColor(tex, new Color32(0, 0, 0, 0));
            tex.Apply();

            return tex;
        }

        /// <summary>
        /// Calculates the positions of the neurons on a vertical line within a texture and adds the result to the given dictionary if possible
        /// </summary>
        private static void AddNeuronPositionsColumn(Texture2D reference, in Dictionary<int, Vector2> positions, List<Neuron> neurons, float widthPercent)
        {
            for (int i = 1; i <= neurons.Count; i++)
            {
                if (positions.ContainsKey(neurons[i - 1].id)) continue;

                float percent = i / (float)neurons.Count;

                int x = Mathf.RoundToInt(reference.width * widthPercent);
                int y = Mathf.RoundToInt(percent * (reference.height));
                int yOffset = -1 * Mathf.RoundToInt(0.5f * ((1 / (float)neurons.Count) * reference.height));

                positions.Add(neurons.ToList()[i - 1].id, new Vector2(x, y + yOffset));
            }
        }

        /// <summary>
        /// Calculates the random positions of the neurons and adds the result to the given dictionary if possible
        /// </summary>
        private static void AddNeuronPositionsRandom(Texture2D reference, in Dictionary<int, Vector2> positions,List<Neuron> neurons, float minPercent, float maxPercent)
        {
            foreach (var neuron in neurons)
            {
                if (positions.ContainsKey(neuron.id)) continue;

                Random.InitState(neuron.id);
                var x = Random.Range(reference.width * minPercent + NEURON_SIZE, reference.width * maxPercent - NEURON_SIZE);
                var y = Random.Range(reference.height * minPercent + NEURON_SIZE, reference.height * maxPercent - NEURON_SIZE);
                positions.Add(neuron.id, new Vector2(x, y));
            }
        }

        #region Texture draw functions
        /// <summary>
        /// Draws links between the given neurons
        /// </summary>
        /// <param name="inspectorNeurons"> (id, position)</param>
        private static void DrawLinks(in Texture2D tex, Dictionary<int, Vector2> inspectorNeurons, List<Link> links)
        {
            var gradient = new Gradient();

            var colorKeys = new GradientColorKey[2];
            colorKeys[0].color = new Color(0 / 255f, 76 / 255f, 153 / 255f);
            colorKeys[0].time = -2;
            colorKeys[1].color = new Color(153 / 255f, 204 / 255f, 255 / 255f);
            colorKeys[1].time = 2;

            var alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0].alpha = 1;
            alphaKeys[0].time = -2;
            alphaKeys[1].alpha = 1;
            alphaKeys[1].time = 2;
            
            gradient.SetKeys(colorKeys, alphaKeys);

            foreach (var link in links)
            {
                if (inspectorNeurons.ContainsKey(link.fromID) && inspectorNeurons.ContainsKey(link.toID))
                {
                    var drawColor = link.active ? gradient.Evaluate(link.weight / 2) : Color.red;
                    DrawLine(in tex, inspectorNeurons[link.fromID], inspectorNeurons[link.toID], drawColor);
                }
            }
        }

        /// <summary>
        /// Draws the neurons as circle on the texture
        /// </summary>
        private static void DrawNeurons(in Texture2D tex, Dictionary<int, Vector2> inspectorNeurons)
        {
            foreach (var neuron in inspectorNeurons)
            {
                DrawCircle(tex, (int)neuron.Value.x, (int)neuron.Value.y, NEURON_SIZE, Color.blue);
            }
        }

        private static void FillColor(in Texture2D tex, Color32 col)
        {
            var fillColorArray = tex.GetPixels32();

            for (var i = 0; i < fillColorArray.Length; ++i)
            {
                fillColorArray[i] = col;
            }

            tex.SetPixels32(fillColorArray);
        }

        //https://discussions.unity.com/t/create-line-on-a-texture/41000
        private static void DrawLine(in Texture2D tex, Vector2 p1, Vector2 p2, Color32 col)
        {
            Vector2 t = p1;
            float frac = 1 / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
            float ctr = 0;

            while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y)
            {
                t = Vector2.Lerp(p1, p2, ctr);
                ctr += frac;
                tex.SetPixel((int)t.x, (int)t.y, col);
            }
        }

        //https://stackoverflow.com/questions/30410317/how-to-draw-circle-on-texture-in-unity
        private static void DrawCircle(in Texture2D tex, int x, int y, int radius, Color32 color)
        {
            float rSquared = radius * radius;

            for (int u = x - radius; u < x + radius + 1; u++)
                for (int v = y - radius; v < y + radius + 1; v++)
                    if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                        tex.SetPixel(u, v, color);
        }
        #endregion
    }
}

#endif