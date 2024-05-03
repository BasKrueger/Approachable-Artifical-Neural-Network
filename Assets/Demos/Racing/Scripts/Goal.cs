using System.Collections.Generic;
using UnityEngine;

namespace ApproachableANN.Demo.Race
{
    /// <summary>
    /// Whenever a ANN triggers this, remove it from the Hashmap of all referenced reward boxes
    /// </summary>
    public sealed class Goal : MonoBehaviour
    {
        [SerializeField]
        private List<RewardBox> boxes;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var unit = collision.GetComponent<ANNUnit>();
            if (unit == null) return;

            foreach (var box in boxes)
            {
                box.rewarded.Remove(unit);
            }
        }
    }
}
