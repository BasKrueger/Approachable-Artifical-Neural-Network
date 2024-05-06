using UnityEngine;

namespace ApproachableANN.Senses
{
    /// <summary>
    /// ANN Sense that returns the value of an forward ray cast 3D
    /// </summary>
    public sealed class RayVision3D : ANNSense
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
            if(point == new Vector3())
            {
                return length;
            }

            return Vector3.Distance(point, transform.position);
        }

        private void OnDrawGizmos()
        {
            if (!debugDrawRaycast) return;

            var point = GetRayCastHitPoint();

            if (point == new Vector3())
            {
                point = transform.position + transform.forward * length;
            }

            Debug.DrawLine(transform.position, point, Color.red);
        }

        private Vector3 GetRayCastHitPoint()
        {
            var ray = new Ray(transform.position, transform.forward);
            Physics.Raycast(ray,out var hit ,length, raycastLayer);

            return hit.point;
        }
    }

}
