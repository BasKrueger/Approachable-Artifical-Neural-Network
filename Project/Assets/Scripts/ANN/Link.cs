using UnityEngine;

namespace ApproachableANN
{
    /// <summary>
    /// Weighted Link that stores informations about two connected Neurons. 
    /// </summary>
    [System.Serializable]
    public sealed class Link
    {
        private static int nextLinkID = 1;

        /// <summary>
        /// Id of this link
        /// </summary>
        [field: SerializeField]
        public int id { get; private set; }

        /// <summary>
        /// wether or not this link is currently active
        /// </summary>
        [SerializeField]
        public bool active;

        /// <summary>
        /// setting to influence how impactful this link is on the decision making
        /// </summary>
        [field: SerializeField]
        public float weight { get; private set; }

        /// <summary>
        /// id from the neuron this link comes from
        /// </summary>
        [field: SerializeField]
        public int fromID { get; private set; }

        /// <summary>
        /// id to the neuron this link goes to
        /// </summary>
        [field: SerializeField]
        public int toID { get; private set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Link()
        {

        }

        /// <summary>
        /// Initializes Link between 2 neurons
        /// </summary>
        public Link(int fromID, int toID, float weight = 1)
        {
            this.fromID = fromID;
            this.toID = toID;
            this.weight = weight;
            active = true;

            this.id = nextLinkID;
            nextLinkID++;
        }

        /// <summary>
        /// Deep Copy Constructor
        /// </summary>
        public Link(Link other)
        {
            this.active = other.active;
            this.weight = other.weight;
            this.fromID = other.fromID;
            this.toID = other.toID;
            this.id = other.id;

            if(nextLinkID <= other.id)
            {
                nextLinkID = other.id + 1;
            }
        }

        /// <summary>
        /// Randomly adjusts the weights of this link
        /// </summary>
        public void MutateWeight()
        {
            weight += UnityEngine.Random.Range(-5f, 5f);
        }
    }
}
