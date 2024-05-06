using UnityEngine;

namespace ApproachableANN.Senses
{
    /// <summary>
    /// ANNsense that returns wether or not something is currently triggering this GameObject
    /// </summary>
    public sealed class TriggerVision : ANNSense
    {
        private bool isIn = false;

        public override float GetSenseValue()
        {
            return isIn? 1.0f : 0.0f;
        }

        private void OnTriggerStay(Collider other)
        {
            isIn = true;
        }

        private void OnTriggerExit(Collider other)
        {
            isIn = false;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            isIn = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            isIn = false;
        }
    }

}
