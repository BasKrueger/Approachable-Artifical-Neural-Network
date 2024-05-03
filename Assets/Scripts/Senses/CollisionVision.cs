using UnityEngine;

namespace ApproachableANN.Senses
{
    /// <summary>
    /// ANNsense that returns wether or not something is currently colliding with this GameObject
    /// </summary>
    public sealed class CollisionVision : ANNSense
    {
        private bool isIn = false;

        public override float GetSenseValue()
        {
            return isIn ? 1.0f : 0.0f;
        }

        private void OnCollisionStay(Collision collision)
        {
            isIn = true;
        }

        private void OnCollisionExit(Collision collision)
        {
            isIn = false;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            isIn = true;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            isIn = false;            
        }
    }
}

