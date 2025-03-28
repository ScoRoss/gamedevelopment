using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FMS_AdvancedZombieAI
{
    public class ZombieCrawlingHandler : MonoBehaviour
    {
        private AdvancedZombieAI zombieAI;
        [HideInInspector] public bool canMove = false;

        void Start()
        {
            zombieAI = GetComponent<AdvancedZombieAI>();
        }

        public void HandleCrawling()
        {
            if (!zombieAI.isDead)
            {
                if (zombieAI.health <= 0)
                {
                    if (!zombieAI.willCrawlAfterDepletingHealth)
                    {
                        zombieAI.isDead = true;
                        zombieAI.Spawn();
                        zombieAI.animator.enabled = false;
                        zombieAI.motionState = AdvancedZombieAI.MotionState.Die;
                        
                        return;
                    }
                    else if (zombieAI.willCrawlAfterDepletingHealth)
                    {

                        zombieAI.isCrawlingChasing = false;
                        zombieAI.animator.SetLayerWeight(1, 0);
                        zombieAI.animator.SetLayerWeight(2, 0);
                        zombieAI.animator.SetBool("Fall", true);

                        zombieAI.agent.speed = 0f;
                        zombieAI.agent.areaMask &= ~(1 << UnityEngine.AI.NavMesh.GetAreaFromName("Obstacle"));

                        zombieAI.headWeight = 0f;
                        zombieAI.bodyWeight = 0f;

                        

                        if (zombieAI.animator.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
                        {
                            zombieAI.chasingSpeed = 0f;
                            zombieAI.wanderingSpeed = 0f;
                            if (!canMove)
                            {
                                StartCoroutine(FallOver());
                            }

                        }
                        if (canMove)
                        {
                            if (!zombieAI.animator.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
                            {
                                zombieAI.chasingSpeed = zombieAI.crawlingChaseSpeed;
                            }
                            zombieAI.animator.SetBool("Fall", false);

                        }


                        if (zombieAI.crawlingHealth <= 0)
                        {
                            //zombieAI.capsuleCollider.isTrigger = true;
                            zombieAI.Spawn();
                            zombieAI.isDead = true;
                            zombieAI.animator.enabled = false;
                            zombieAI.motionState = AdvancedZombieAI.MotionState.Die;
                            return;
                        }
                    }
                }
            }
        }


        private IEnumerator FallOver()
        {
            canMove = false;
            yield return new WaitForSeconds(zombieAI.moveDelayAfterFall);
            canMove = true;
        }


    }
}