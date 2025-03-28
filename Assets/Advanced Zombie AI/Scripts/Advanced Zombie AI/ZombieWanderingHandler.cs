using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FMS_AdvancedZombieAI
{
    public class ZombieWanderingHandler : MonoBehaviour
    {
        private AnimatorOverrideController animatorOverrideController;
        private int waypointIndex = 0;
        private float timeLeft;
        private Vector3 wanderPoint;
        private AdvancedZombieAI zombieAI;
        [HideInInspector] public bool nullWander = false;

        void Start()
        {
            zombieAI = GetComponent<AdvancedZombieAI>();
            wanderPoint = RandomWanderPoint();

            var overrideController = zombieAI.animator.runtimeAnimatorController as AnimatorOverrideController;
            animatorOverrideController = Instantiate(overrideController) as AnimatorOverrideController;
            zombieAI.animator.runtimeAnimatorController = animatorOverrideController;
        }


        public void Wander()
        {

            WanderingHandler();
            zombieAI.isCrawling = true;
            if (zombieAI.wanderingMode == AdvancedZombieAI.WanderingMode.RandomWandering)
            {
                if (Vector3.Distance(transform.position, wanderPoint) < 3f)
                {
                    wanderPoint = RandomWanderPoint();
                }
                else
                {
                    zombieAI.agent.SetDestination(wanderPoint);
                    NavMeshPath path = new NavMeshPath();
                    NavMesh.CalculatePath(transform.position, wanderPoint, NavMesh.AllAreas, path);
                    if (path.status != NavMeshPathStatus.PathComplete)
                    {
                        wanderPoint = RandomWanderPoint();
                    }
                }
            }
            else if (zombieAI.wanderingMode == AdvancedZombieAI.WanderingMode.WaypointWandering)
            {
                if (zombieAI.waypointsToWander.Length >= 2)
                {
                    float distanceToWaypoint = Vector3.Distance(zombieAI.waypointsToWander[waypointIndex].transform.position, zombieAI.transform.position);
                    Waypoints waypoint = zombieAI.waypointsToWander[waypointIndex].GetComponent<Waypoints>();

                    if (distanceToWaypoint < 1f)
                    {
                        if (waypoint.toDo == Waypoints.ToDo.Nothing)
                        {
                            waypointIndex = (waypointIndex == zombieAI.waypointsToWander.Length - 1) ? 0 : waypointIndex + 1;
                        }
                        else if (waypoint.toDo != Waypoints.ToDo.Nothing)
                        {
                            if (!zombieAI.willCrawlAfterDepletingHealth || zombieAI.willCrawlAfterDepletingHealth && zombieAI.health > 0)
                            {
                                nullWander = true;


                                timeLeft -= Time.deltaTime;

                                if (timeLeft > 0)
                                {
                                    zombieAI.agent.speed = 0f;

                                    zombieAI.animator.SetLayerWeight(1, 0);
                                    zombieAI.animator.SetLayerWeight(2, 0);
                                    if (waypoint.toDo == Waypoints.ToDo.Scream)
                                    {
                                        zombieAI.animator.SetBool("Scream", true);
                                    }
                                    else if (waypoint.toDo == Waypoints.ToDo.Idle)
                                    {
                                        zombieAI.animator.SetBool("Idle", true);
                                    }
                                    else if (waypoint.toDo == Waypoints.ToDo.Custom)
                                    {
                                        animatorOverrideController["Custom"] = waypoint.customAnimation;
                                        zombieAI.animator.SetBool("Custom", true);

                                    }

                                }
                                else if (timeLeft <= 0)
                                {

                                    zombieAI.animator.SetLayerWeight(1, 1);
                                    zombieAI.animator.SetLayerWeight(2, 1);
                                    if (zombieAI.motionState == AdvancedZombieAI.MotionState.Wandering)
                                    {
                                        zombieAI.agent.speed = zombieAI.wanderingSpeed;
                                        waypointIndex = (waypointIndex == zombieAI.waypointsToWander.Length - 1) ? 0 : waypointIndex + 1;
                                    }
                                }
                            }
                            else
                            {
                                waypointIndex = (waypointIndex == zombieAI.waypointsToWander.Length - 1) ? 0 : waypointIndex + 1;
                            }
                        }

                    }
                    else
                    {
                        if (!zombieAI.isDead)
                        {
                            zombieAI.animator.SetBool("Idle", false);
                            zombieAI.animator.SetBool("Scream", false);
                            zombieAI.animator.SetBool("Custom", false);

                            timeLeft = waypoint.timeToWait;
                            zombieAI.agent.SetDestination(zombieAI.waypointsToWander[waypointIndex].transform.position);
                            nullWander = false;
                        }
                    }
                }
            }
            else if (zombieAI.wanderingMode == AdvancedZombieAI.WanderingMode.FollowPlayer)
            {
                if (!zombieAI.isDead)
                {
                    Vector3 followPlayerMoveOffset = Random.insideUnitSphere * zombieAI.followPlayerMovementOffset;
                    zombieAI.agent.SetDestination(zombieAI.player.transform.position + followPlayerMoveOffset);
                }
            }
        }

        public void WanderingHandler()
        {
            if (!nullWander)
            {
                if (!zombieAI.willCrawlAfterDepletingHealth)
                {
                    if (!zombieAI.isVaulting)
                    {
                        zombieAI.actionState = AdvancedZombieAI.ActionState.None;
                    }


                    zombieAI.animator.SetBool("Walk", true);

                }
                else if (zombieAI.willCrawlAfterDepletingHealth)
                {
                    if (zombieAI.health > 0)
                    {
                        if (!zombieAI.isVaulting)
                        {
                            zombieAI.actionState = AdvancedZombieAI.ActionState.None;
                        }

                        zombieAI.animator.SetBool("Walk", true);

                    }
                    else if (zombieAI.health <= 0 && zombieAI.isCrawling && !zombieAI.isCrawlingChasing && !zombieAI.isDead)
                    {
                        ZombieCrawlingHandler zombieCrawlingHandler = GetComponent<ZombieCrawlingHandler>();
                        zombieAI.actionState = AdvancedZombieAI.ActionState.Crawling;
                        if (zombieCrawlingHandler.canMove)
                        {
                            zombieAI.animator.SetBool("Walk", false);
                            zombieAI.animator.SetBool("Crawling Chase", false);
                            zombieAI.animator.SetBool("Crawl", true);
                            zombieAI.wanderingSpeed = zombieAI.crawlingWanderSpeed;

                        }



                    }
                }
            }
            else
            {
                zombieAI.animator.SetBool("Walk", false);

            }

        }

        public Vector3 RandomWanderPoint()
        {
            Vector3 randomPoint = (Random.insideUnitSphere * zombieAI.wanderingRadius) + transform.position;
            NavMeshHit navMeshHit;
            NavMesh.SamplePosition(randomPoint, out navMeshHit, zombieAI.wanderingRadius, -1);
            return new Vector3(navMeshHit.position.x, transform.position.y, navMeshHit.position.z);
        }

    }
}
