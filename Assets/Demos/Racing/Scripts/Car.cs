using System.Collections.Generic;
using UnityEngine;

namespace ApproachableANN.Demo.Race
{
    /// <summary>
    /// Simple top down car that gets controlled by an ANN Unit
    /// </summary>
    public sealed class Car : MonoBehaviour
    {
        [SerializeField]
        private float maxSpeed;

        private float speed;

        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            rb.velocity = transform.up * speed;
        }

        /// <summary>
        /// Called by ANN Unit "Decided" event
        /// </summary>
        public void OnDecision(Dictionary<string, float> decisions)
        {
            speed = decisions["Speed"] * maxSpeed;

            if (decisions["Turn Left"] > 0)
            {
                transform.Rotate(Vector3.forward * -150 * decisions["Turn Left"] * Time.deltaTime);
            }

            if (decisions["Turn Right"] > 0)
            {
                transform.Rotate(Vector3.forward * 150 * decisions["Turn Right"] * Time.deltaTime);
            }
        }
    }
}
