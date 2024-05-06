using UnityEngine;

namespace ApproachableANN.Senses
{
    /// <summary>
    /// ANN Sense that returns the value of an up ray cast 2D
    /// </summary>
    public sealed class RayVision2D : ANNSense
    {
        /// <summary>
        /// Layermask that gets used for the raycast
        /// </summary>
        public LayerMask raycastLayer;
        /// <summary>
        /// How long the raycast is
        /// </summary>
        public float length = 10;
        /// <summary>
        /// Wether or not a debug line along the raycast should be drawn
        /// </summary>
        public bool debugDrawRaycast = false;

        public override float GetSenseValue()
        {
            var point = GetRayCastHitPoint();
            if(point == new Vector2())
            {
                return length;
            }

            return Vector2.Distance(point, transform.position);
        }

        private void OnDrawGizmos()
        {
            if (!debugDrawRaycast) return;

            var point = GetRayCastHitPoint();

            if (point == new Vector2())
            {
                point = transform.position + transform.up * length;
            }

            Debug.DrawLine(transform.position, point, Color.red);
        }

        private Vector2 GetRayCastHitPoint()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, length, raycastLayer);
            return hit.point;
        }
    }

}
