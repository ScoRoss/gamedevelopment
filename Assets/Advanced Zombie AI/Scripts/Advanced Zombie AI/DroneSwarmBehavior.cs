using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FMS_AdvancedZombieAI
{
    public class DroneSwarmBehavior : BaseAIMovement
    {
        [Header("Swarm Settings")]
        public GameObject dronePrefab;  // Assign this in the Inspector (The drone prefab)
        public int numberOfFollowers = 10;  // Number of followers to spawn
        public float spawnRadius = 5f;  // Spawn distance around leader

        [Header("Swarm Behavior")]
        public float separationDistance = 2f;
        public float alignmentWeight = 1f;
        public float cohesionWeight = 1f;
        public float followLeaderWeight = 2f;

        private static DroneSwarmBehavior leader; // The main leader of the swarm
        private List<DroneSwarmBehavior> nearbyDrones = new List<DroneSwarmBehavior>();

        protected override void Start()
        {
            base.Start();

            // If this is the first spawned drone, make it the leader and spawn followers
            if (leader == null)
            {
                leader = this;
                SpawnFollowers();
            }
        }

        protected override void Update()
        {
            base.Update();

            if (this == leader)
            {
                // The leader moves towards the player
                SetDestination(player.position);
            }
            else
            {
                // Followers move based on the leader’s position
                Vector3 separation = CalculateSeparation();
                Vector3 alignment = CalculateAlignment();
                Vector3 cohesion = CalculateCohesion();
                Vector3 leaderFollow = (leader.transform.position - transform.position).normalized;

                Vector3 finalDirection =
                    (separation * separationDistance) +
                    (alignment * alignmentWeight) +
                    (cohesion * cohesionWeight) +
                    (leaderFollow * followLeaderWeight);

                SetDestination(transform.position + finalDirection);
            }
        }

        private void SpawnFollowers()
        {
            if (dronePrefab == null)
            {
                Debug.LogError("Drone Prefab is not assigned in the Inspector!");
                return;
            }

            for (int i = 0; i < numberOfFollowers; i++)
            {
                // Generate a random position around the leader
                Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
                spawnPosition.y = transform.position.y;  // Keep the height the same as the leader

                GameObject follower = Instantiate(dronePrefab, spawnPosition, Quaternion.identity);
                follower.GetComponent<DroneSwarmBehavior>();  // Ensure it has the behavior script
            }
        }

        private Vector3 CalculateSeparation()
        {
            Vector3 separation = Vector3.zero;
            int count = 0;

            foreach (var drone in nearbyDrones)
            {
                if (drone != this && Vector3.Distance(transform.position, drone.transform.position) < separationDistance)
                {
                    separation += (transform.position - drone.transform.position).normalized;
                    count++;
                }
            }

            return count > 0 ? separation / count : separation;
        }

        private Vector3 CalculateAlignment()
        {
            Vector3 averageDirection = Vector3.zero;
            int count = 0;

            foreach (var drone in nearbyDrones)
            {
                if (drone != this)
                {
                    averageDirection += drone.agent.velocity.normalized;
                    count++;
                }
            }

            return count > 0 ? (averageDirection / count).normalized : averageDirection;
        }

        private Vector3 CalculateCohesion()
        {
            Vector3 centerOfMass = Vector3.zero;
            int count = 0;

            foreach (var drone in nearbyDrones)
            {
                if (drone != this)
                {
                    centerOfMass += drone.transform.position;
                    count++;
                }
            }

            if (count > 0)
            {
                centerOfMass /= count;
                return (centerOfMass - transform.position).normalized;
            }

            return Vector3.zero;
        }

        private void OnTriggerEnter(Collider other)
        {
            var drone = other.GetComponent<DroneSwarmBehavior>();
            if (drone != null && !nearbyDrones.Contains(drone))
            {
                nearbyDrones.Add(drone);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var drone = other.GetComponent<DroneSwarmBehavior>();
            if (drone != null && nearbyDrones.Contains(drone))
            {
                nearbyDrones.Remove(drone);
            }
        }
    }
}
