using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using System.Collections;
using UnityEditor;


namespace FMS_AdvancedZombieAI
{
    public class AdvancedZombieAI : MonoBehaviour
    {
        public enum MotionState//
        {
            Wandering,
            Chasing,
            Attack,
            Die,
        }

        public MotionState motionState = MotionState.Wandering;//

        public enum ActionState//
        {
            None,
            Vault,
            Hit,
            ChaseHit,
            Attack,
            ChaseAttack,
            Crawling,
            CrawlingChase,
            CrawlingAttack,

        }

        public ActionState actionState = ActionState.None;//


        public enum WanderingMode//
        {
            RandomWandering,
            WaypointWandering,
            FollowPlayer
        }

        public WanderingMode wanderingMode = WanderingMode.RandomWandering;//
        public Waypoints[] waypointsToWander;//

        [Range(0, 360)]
        public float fieldOfView = 120f;//
        public float viewDistance = 10f;//
        public float wanderingRadius = 7f;//
        public float wanderingSpeed = 4f;//
        public float chasingSpeed = 7f;//
        public float crawlingWanderSpeed = 1.5f;//
        public float crawlingChaseSpeed = 3.5f;//
        public float health = 100f;//


        public float attackRange = 2f;//
        public float attackCooldown = 2f;//
        public float attackForce = 0.5f;//



        public bool willCrawlAfterDepletingHealth = true;//
        public float crawlingHealth = 150;//
        public float moveDelayAfterFall = 5f;
        public float losePlayerTime = 10f;//
        [Range(0, 10)]
        public float randomMovementOffset = 2f;//
        [Range(0, 10)]
        public float followPlayerMovementOffset = 2f;//

        public float headWeight = 0.5f;//
        public float bodyWeight = 0f;//

        public bool ShowGizmos = true;//
        public bool extraDamageIfShotOnHead = true;
        public GameObject zombieHead;
        public float headDamage;

        private ZombieSpawner zombieSpawner;
        private bool hasSpawned = false;


        private Collider[] ragdollColliders;
        private Rigidbody[] ragdollRigidbodies;
        private ZombieAttackHandler zombieAttackHandler;
        private ZombieCrawlingHandler zombieCrawlingHandler;
        private ZombieChaseHandler zombieChaseHandler;
        private ZombieWanderingHandler zombieWanderingHandler;


        [HideInInspector] public bool isCrawlingChasing = false;
        [HideInInspector] public bool canSeeThePlayer = false;
        [HideInInspector] public bool isCrawling = false;
        [HideInInspector] public NavMeshAgent agent;
        [HideInInspector] public Player player;
        [HideInInspector] public Animator animator;
        [HideInInspector] public bool isDead = false;
        [HideInInspector] public bool isVaulting = false;

        private void Awake()
        {
            animator = GetComponent<Animator>();

            if (!IsTagExists("Zombie"))
            {
;
            }
            this.gameObject.tag = "Zombie";

            Transform[] childrens = GetComponentsInChildren<Transform>();
            string layerName = "Zombie";
            bool isLayerInMask = IsLayerInMask(layerName);
            if (!isLayerInMask)
            {
                Debug.LogError("'Zombie' layer doesn't exists. Add 'Zombie' layer");
            }
            else
            {
                this.gameObject.layer = LayerMask.NameToLayer("Zombie");
                foreach (Transform child in childrens)
                {
                    child.gameObject.layer = LayerMask.NameToLayer("Zombie");
                }
            }
        }

        public void Start()
        {
            InitializeComponents();
            DeactivateRagdollComponents();
        }

        private void DeactivateRagdollComponents()
        {

            foreach (Rigidbody rigidbody in ragdollRigidbodies)
            {
                rigidbody.isKinematic = true;
            }
        }

        private void InitializeComponents()
        {
            zombieWanderingHandler = GetComponent<ZombieWanderingHandler>();
            zombieAttackHandler = GetComponent<ZombieAttackHandler>();
            zombieChaseHandler = GetComponent<ZombieChaseHandler>();
            zombieCrawlingHandler = GetComponent<ZombieCrawlingHandler>();
            zombieSpawner = FindObjectOfType<ZombieSpawner>();
            player = FindObjectOfType<Player>();
            agent = GetComponent<NavMeshAgent>();
            ragdollColliders = GetComponentsInChildren<Collider>();
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        }

        public void Update()
        {
            OffMeshLink();

            if (!isDead)
            {
                if (health > 0)
                {
                    zombieAttackHandler.AttackHandler();

                }
                else
                {
                    if (zombieCrawlingHandler.canMove)
                    {
                        zombieAttackHandler.AttackHandler();
                    }
                }

                zombieCrawlingHandler.HandleCrawling();

                zombieChaseHandler.ChaseHandler();


                DetectAndWander();

                if (motionState != MotionState.Wandering && health > 0 && !isVaulting)
                {
                    animator.SetLayerWeight(1, 1);
                    animator.SetLayerWeight(2, 1);
                }

                if (!canSeeThePlayer && motionState == MotionState.Chasing)
                {
                    zombieChaseHandler.UpdateLoseTimer();
                }

                if (canSeeThePlayer && motionState == MotionState.Chasing)
                {
                    zombieChaseHandler.loseTimer = 0;
                }
                if (player != null)
                {
                    if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
                    {
                        motionState = MotionState.Attack;
                    }
                }
                Die();
            }
            else
            {
                isDead = true;
                motionState = MotionState.Die;
                Die();
                return;
            }

        }





        public float GetRandomMovementOffset()
        {
            return randomMovementOffset;
        }



        public void SetAgentDestination(Vector3 destination)
        {
            if (!isDead)
            {
                agent.SetDestination(destination);
            }
        }




        public void SetAgentSpeed(float speed)
        {
            agent.speed = speed;
        }










        private bool IsPlayerInFieldOfView(float angleToTarget)
        {
            return angleToTarget < fieldOfView / 2f;
        }

        private bool IsPlayerInDetectionRange()
        {
            float distanceToTarget = Vector3.Distance(player.transform.position, transform.position);
            return distanceToTarget < viewDistance;
        }

        private bool IsPlayerVisible()
        {
            RaycastHit raycastHit;
            if (Physics.Linecast(transform.position, player.transform.position, out raycastHit, -1))
            {
                return raycastHit.transform.CompareTag("Player");
            }
            return false;
        }



        public class Pheromone : MonoBehaviour
        {
            public float strength = 1f; // Initial pheromone strength
            public float decayRate = 0.1f; // How fast pheromones fade

            public void Initialize(float initialStrength, float decay)
            {
                strength = initialStrength;
                decayRate = decay;
                StartCoroutine(DecayPheromone());
            }

            private IEnumerator DecayPheromone()
            {
                while (strength > 0)
                {
                    strength -= decayRate * Time.deltaTime;
                    yield return null;
                }

                Destroy(gameObject); // Remove the pheromone when it fully fades
            }
        }




        public void Die()
        {
            if (motionState == MotionState.Die)
            {
                animator.enabled = false;
                agent.enabled = false;
                agent.speed = 0;
                wanderingSpeed = 0;
                chasingSpeed = 0;
                isDead = true;

                foreach (Rigidbody rigidbody in ragdollRigidbodies)
                {
                    rigidbody.isKinematic = false;
                }
            }
        }
        public void Spawn()
        {
            if (!hasSpawned)
            {
                if (zombieSpawner != null)
                {
                    zombieSpawner.SpawnZombieOnDeath();
                    hasSpawned = true;
                }
            }
        }







        void OffMeshLink()
        {
            if (agent.isOnOffMeshLink)
            {
                var meshLink = agent.currentOffMeshLinkData;
                if (health > 0)
                {
                    if (!isVaulting && meshLink.offMeshLink.area == NavMesh.GetAreaFromName("Obstacle"))
                    {
                        isVaulting = true;
                        animator.SetTrigger("Vault");
                        actionState = ActionState.Vault;
                    }
                    else if (!isVaulting && meshLink.offMeshLink.area != NavMesh.GetAreaFromName("Obstacle"))
                    {
                        Debug.LogError(meshLink.offMeshLink.gameObject + ": Offmesh link navigation area not set to 'Obstacle'. Please set the Offmesh link component's area to 'Obstacle'.");
                    }
                }
            }
            else
            {
                isVaulting = false;
            }
        }
        private bool IsTagExists(string tag)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
            return objectsWithTag.Length > 0;
        }


        private static bool IsLayerInMask(string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            return layer != -1;
        }

    }
}