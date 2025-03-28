using UnityEngine;
using UnityEngine.AI;

namespace FMS_AdvancedZombieAI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BaseAIMovement : MonoBehaviour
    {
        protected NavMeshAgent agent;
        protected Transform player;

        [Header("Movement Settings")]
        public float movementSpeed = 3.5f;
        public float detectionRange = 15f;

        protected virtual void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (agent != null)
            {
                agent.speed = movementSpeed;
            }
        }

        protected virtual void Update()
        {
            if (player != null && agent.enabled)
            {
                agent.SetDestination(player.position);
            }
        }

        public void SetDestination(Vector3 destination)
        {
            if (agent != null && agent.enabled)
            {
                agent.SetDestination(destination);
            }
        }

        public bool CanSeePlayer()
        {
            if (player == null) return false;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= detectionRange)
            {
                RaycastHit hit;
                if (Physics.Linecast(transform.position, player.position, out hit))
                {
                    return hit.transform.CompareTag("Player");
                }
            }
            return false;
        }

        public void PauseMovement() => agent.isStopped = true;
        public void ResumeMovement() => agent.isStopped = false;
    }
}
