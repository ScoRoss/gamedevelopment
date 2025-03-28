using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FMS_AdvancedZombieAI
{
    [RequireComponent(typeof(Animator))]
    public class HeadIK : MonoBehaviour
    {
        private Transform player;
        private AdvancedZombieAI zombieAI;

        void Start()
        {
            zombieAI = GetComponent<AdvancedZombieAI>();

            player = GameObject.FindWithTag("Player").GetComponentInChildren<Camera>().transform;

        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (zombieAI.motionState == AdvancedZombieAI.MotionState.Chasing || zombieAI.motionState == AdvancedZombieAI.MotionState.Attack)
            {
                if (player != null)
                {
                    zombieAI.animator.SetLookAtPosition(player.position);
                    zombieAI.animator.SetLookAtWeight(1, zombieAI.bodyWeight, zombieAI.headWeight);
                }
                else
                {
                    Debug.Log("Player Not Found");
                }
            }

        }
    }
}
