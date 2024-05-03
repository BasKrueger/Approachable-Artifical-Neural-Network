using UnityEngine;

namespace ApproachableANN.Senses
{
    /// <summary>
    /// Base class for creating ANNsenses.
    /// </summary>
    public abstract class ANNSense : MonoBehaviour
    {
        /// <summary>
        /// Gets the sense value or 0 if the Gameobject is inactive.
        /// </summary>
        public float value
        {
            get
            {
                if (!this.enabled)
                {
                    return 0;
                }

                return GetSenseValue();
            }
            set
            {
                
            }
        }
        
        /// <summary>
        /// Returns the current value of the sense
        /// </summary>
        public abstract float GetSenseValue();
    }

}
