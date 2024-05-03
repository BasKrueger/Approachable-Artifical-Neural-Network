using UnityEngine;

namespace ApproachableANN.Demo.Race
{
    /// <summary>
    /// Punishes ANN Units for colliding with this
    /// </summary>
    public sealed class Boundary : MonoBehaviour
    {
        [SerializeField]
        private float punishOnCollisionStay;

        [SerializeField]
        private float punishOnCollisionEnter;

        [HideInInspector]
        public bool active = true;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!active) return;

            var unit = collision.gameObject.GetComponent<ANNUnit>();

            if (unit == null) return;

            unit.Punish(punishOnCollisionEnter);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!active) return;

            var unit = collision.gameObject.GetComponent<ANNUnit>();

            if (unit == null) return;

            unit.Punish(punishOnCollisionStay);
        }
    }
}
