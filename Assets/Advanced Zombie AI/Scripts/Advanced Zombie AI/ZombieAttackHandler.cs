using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FMS_AdvancedZombieAI
{
    public class ZombieAttackHandler : MonoBehaviour
    {
        private bool canAttack = true;
        private AdvancedZombieAI zombieAI;

        private PlayerMovement player;

        void Start()
        {
            player = FindObjectOfType<PlayerMovement>();
            zombieAI = GetComponent<AdvancedZombieAI>();
        }


        public void AttackHandler()
        {
            if (!zombieAI.isDead)
            {
                if (zombieAI.motionState == AdvancedZombieAI.MotionState.Attack)
                {
                    if (canAttack)
                    {
                        Attack();
                        StartCoroutine(AttackCooldown());
                    }
                    else if (!canAttack)
                    {
                        zombieAI.motionState = AdvancedZombieAI.MotionState.Chasing;
                    }
                }
            }
            
        }

        public void UpdateAttackAnimatorState(float distanceToPlayer)
        {
            if (distanceToPlayer <= zombieAI.attackRange && !player.isRunning && !player.isWalking && !player.isCrouching)
            {
                if (!zombieAI.willCrawlAfterDepletingHealth)
                {
                    zombieAI.animator.SetBool("Cooldown", true);
                }
                else if (zombieAI.willCrawlAfterDepletingHealth)
                {
                    if (zombieAI.health > 0)
                    {
                        zombieAI.animator.SetBool("Cooldown", true);
                    }
                    else if (zombieAI.health <= 0)
                    {
                        zombieAI.animator.SetBool("Cooldown", false);
                    }
                }

                zombieAI.animator.SetBool("Chase", false);

                zombieAI.SetAgentSpeed(zombieAI.attackForce);
            }
            else if (distanceToPlayer > zombieAI.attackRange)
            {
                zombieAI.animator.SetBool("Cooldown", false);

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
                        zombieAI.animator.SetBool("Chase", false);
                    }
                }
            }
        }


        private void Attack()
        {
            if (!zombieAI.isDead)
            {
                Vector3 lookAtPosition = new Vector3(zombieAI.player.transform.position.x, zombieAI.transform.position.y, zombieAI.player.transform.position.z);

                transform.LookAt(lookAtPosition);

                AttackAnimationHandler();

                // You can add visual/audio effects or other actions related to the attack here
            }
        }

        private void AttackAnimationHandler()
        {
            if (!player.isRunning && !player.isWalking && !player.isCrouching)
            {
                if (!zombieAI.willCrawlAfterDepletingHealth)
                {
                    zombieAI.actionState = AdvancedZombieAI.ActionState.Attack;

                    zombieAI.animator.SetTrigger("Attack");

                }
                else if (zombieAI.willCrawlAfterDepletingHealth)
                {
                    if (zombieAI.health > 0)
                    {
                        zombieAI.actionState = AdvancedZombieAI.ActionState.Attack;

                        zombieAI.animator.SetTrigger("Attack");


                    }
                    else if (zombieAI.health <= 0)
                    {
                        zombieAI.animator.SetTrigger("Crawling Attack");
                        zombieAI.actionState = AdvancedZombieAI.ActionState.CrawlingAttack;
                    }
                }
            }
            else if (player.isRunning || player.isWalking || player.isCrouching)
            {

                if (!zombieAI.willCrawlAfterDepletingHealth)
                {
                    zombieAI.actionState = AdvancedZombieAI.ActionState.ChaseAttack;

                    zombieAI.animator.SetTrigger("ChaseAttack");


                }
                else if (zombieAI.willCrawlAfterDepletingHealth)
                {
                    if (zombieAI.health > 0)
                    {
                        zombieAI.actionState = AdvancedZombieAI.ActionState.ChaseAttack;

                        zombieAI.animator.SetTrigger("ChaseAttack");


                    }
                    else if (zombieAI.health <= 0)
                    {
                        zombieAI.animator.SetTrigger("Crawling Attack");
                        zombieAI.actionState = AdvancedZombieAI.ActionState.CrawlingAttack;

                    }
                }
            }
        }

        private IEnumerator AttackCooldown()
        {
            canAttack = false;
            yield return new WaitForSeconds(zombieAI.attackCooldown);
            canAttack = true;
        }

    }
}
