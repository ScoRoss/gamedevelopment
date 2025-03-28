using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FMS_AdvancedZombieAI
{
    public class ZombieChaseHandler : MonoBehaviour
    {
        [HideInInspector] public float loseTimer = 0;
        private AdvancedZombieAI zombieAI;
        private ZombieAttackHandler zombieAttackHandler;


        void Start()
        {
            zombieAttackHandler = GetComponent<ZombieAttackHandler>();
            zombieAI = GetComponent<AdvancedZombieAI>();
        }

        public void ChaseHandler()
        {
            if (zombieAI.motionState == AdvancedZombieAI.MotionState.Chasing)
            {
                zombieAI.animator.SetBool("Walk", false);
                zombieAI.animator.SetBool("Idle", false);
                zombieAI.animator.SetBool("Scream", false);
                zombieAI.animator.SetBool("Custom", false);
                zombieAI.canSeeThePlayer = true;
                

                zombieAI.isCrawlingChasing = true;
                zombieAI.isCrawling = false;
                Vector3 randomMoveOffset = Random.insideUnitSphere * zombieAI.GetRandomMovementOffset();

                float distanceToPlayer = Vector3.Distance(zombieAI.transform.position, zombieAI.player.transform.position);

                if (distanceToPlayer > zombieAI.attackRange + 3f)
                {
                    zombieAI.SetAgentDestination(zombieAI.player.transform.position + randomMoveOffset);
                }
                else if (distanceToPlayer <= zombieAI.attackRange + 3f)
                {
                    zombieAI.SetAgentDestination(zombieAI.player.transform.position);
                }




                if (!zombieAI.willCrawlAfterDepletingHealth)
                {
                    zombieAI.animator.SetBool("Chase", true);
                }
                else if (zombieAI.willCrawlAfterDepletingHealth)
                {
                    if (zombieAI.health > 0)
                    {
                        zombieAI.animator.SetBool("Chase", true);
                    }
                    else if (zombieAI.health <= 0)
                    {
                        if (!zombieAI.animator.GetCurrentAnimatorStateInfo(0).IsName("CrawlingAttack"))
                        {
                            zombieAI.actionState = AdvancedZombieAI.ActionState.CrawlingChase;
                        }
                        ZombieCrawlingHandler zombieCrawlingHandler = GetComponent<ZombieCrawlingHandler>();
                        if (zombieCrawlingHandler.canMove)
                        {
                            zombieAI.animator.SetBool("Crawling Chase", true);
                            zombieAI.animator.SetBool("Chase", false);
                            zombieAI.animator.SetBool("Crawl", false);
                        }
                    }
                }

                if (!zombieAI.isVaulting)
                {
                    zombieAI.SetAgentSpeed(zombieAI.chasingSpeed);
                }

                

                zombieAttackHandler.UpdateAttackAnimatorState(distanceToPlayer);
            }
        }

        public void UpdateLoseTimer()
        {
            loseTimer += Time.deltaTime;
            if (loseTimer >= zombieAI.losePlayerTime)
            {
                zombieAI.motionState = AdvancedZombieAI.MotionState.Wandering;
                loseTimer = 0;
            }
        }

    }
}
