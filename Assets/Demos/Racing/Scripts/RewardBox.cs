using System.Collections.Generic;
using UnityEngine;

namespace ApproachableANN.Demo.Race
{
    /// <summary>
    /// Rewards the any ANN Unit that triggers this once
    /// </summary>
    public sealed class RewardBox : MonoBehaviour
    {
        [SerializeField]
        private float rewardOnFirstEnter;
        
        [SerializeField]
        private float rewardOnStay;

        /// <summary>
        /// Contains all ANN Units that were rewarded by this
        /// </summary>
        public HashSet<ANNUnit> rewarded = new HashSet<ANNUnit>();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var AI = collision.gameObject.GetComponent<ANNUnit>();
            if (AI != null && !rewarded.Contains(AI))
            {
                AI.Reward(rewardOnFirstEnter);
                rewarded.Add(AI);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            var AI = collision.gameObject.GetComponent<ANNUnit>();
            if (AI != null)
            {
                AI.Reward(rewardOnStay);
            }
        }
    }
}
