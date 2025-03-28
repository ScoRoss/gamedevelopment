using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMS_AdvancedZombieAI;

namespace FMS_AdvancedZombieAI
{
    public class Player : MonoBehaviour
    {
        [Header("Footsteps")]
       
        [Space]

        public float walkingFootstepsSoundIntensity = 2f;
        

        public float runningFootstepsSoundIntensity = 5f;
        

        public float crouchingFootstepsSoundIntensity = 0.5f;


        private PlayerMovement player;
        private AudioSource audioSource;
        private SphereCollider sphereCollider;

        public void Start()
        {
            this.gameObject.tag = "Player";

            player = GetComponent<PlayerMovement>();
            sphereCollider = GetComponent<SphereCollider>();
        }

        public void Update()
        {
            if (player.isWalking)
            {
                sphereCollider.radius = walkingFootstepsSoundIntensity;
            }
            else if (player.isRunning)
            {
                sphereCollider.radius = runningFootstepsSoundIntensity;
            }
            else if (player.isCrouching)
            {
                sphereCollider.radius = crouchingFootstepsSoundIntensity;
            }
            else
            {
                sphereCollider.radius = 0f;
            }
        }



        public void OnTriggerEnter(Collider other)
        {
            // Check if the collided object is on the "Zombie" layer
            if (other.gameObject.layer == LayerMask.NameToLayer("Zombie"))
            {

                AdvancedZombieAI zombieAI = other.GetComponent<AdvancedZombieAI>();

                if (zombieAI != null)
                {
                    // If the AdvancedZombieAI component is found, initiate the chase behavior
                    zombieAI.motionState = AdvancedZombieAI.MotionState.Chasing;
                }
            }
        }


    }
}
