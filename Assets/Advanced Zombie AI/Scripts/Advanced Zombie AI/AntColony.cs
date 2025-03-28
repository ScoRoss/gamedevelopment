using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FMS_AdvancedZombieAI;

namespace FMS_AdvancedZombieAI
{
    public class AntColonyZombieAI : BaseAIMovement
    {
        [Header("Pheromone Settings")]
        public GameObject pheromonePrefab; // Assign in Inspector
        public float pheromoneDropInterval = 2f; // Time between pheromone drops
        public float pheromoneStrength = 1f; // Strength of deposited pheromone
        public float pheromoneDecayRate = 0.1f; // How fast pheromones fade
        public float pheromoneDetectionRadius = 5f; // How far zombies detect pheromones

        private float pheromoneDropTimer = 0f;

        protected override void Update()
        {
            base.Update();

            pheromoneDropTimer += Time.deltaTime;
            Debug.Log("⏳ Timer: " + pheromoneDropTimer); // Debug timer

            if (CanSeePlayer())
            {
                agent.SetDestination(player.transform.position);
            }
            else
            {
                FollowPheromoneTrailOrWander();
            }

            if (pheromoneDropTimer >= pheromoneDropInterval)
            {
                Debug.Log("✅ Dropping pheromone");
                DepositPheromone();
                pheromoneDropTimer = 0f;
            }
        }

        private void DepositPheromone()
        {
            if (pheromonePrefab == null)
            {
                Debug.LogError("🚨 ERROR: Pheromone Prefab is missing! Assign it in the Inspector.");
                return;
            }

            Vector3 spawnPosition = transform.position;
            spawnPosition.y += 0.5f; // Prevents spawning inside terrain

            Vector3? detectedPlayerPosition = CanSeePlayer() ? player.transform.position : (Vector3?)null;

            GameObject pheromone = Instantiate(pheromonePrefab, spawnPosition, Quaternion.identity);
            pheromone.GetComponent<Pheromone>().Initialize(pheromoneStrength, pheromoneDecayRate, detectedPlayerPosition);

            Debug.Log($"🔵 Pheromone dropped at {spawnPosition}, PlayerLocation: {detectedPlayerPosition}");
        }


        private void FollowPheromoneTrailOrWander()
        {
            Collider[] detectedPheromones = Physics.OverlapSphere(transform.position, pheromoneDetectionRadius);
            Pheromone strongestPheromone = null;
            float highestStrength = 0f;
            Vector3? playerPosition = null;

            foreach (Collider col in detectedPheromones)
            {
                Pheromone pheromone = col.GetComponent<Pheromone>();
                if (pheromone != null && pheromone.strength > highestStrength)
                {
                    highestStrength = pheromone.strength;
                    strongestPheromone = pheromone;
                    playerPosition = pheromone.playerPosition;
                }
            }

            if (playerPosition.HasValue)
            {
                Debug.Log("⚠️ Zombie detected player pheromone! Moving to last known location.");
                SetDestination(playerPosition.Value);
            }
            else if (strongestPheromone != null)
            {
                SetDestination(strongestPheromone.transform.position);
            }
            else
            {
                WanderRandomly();
            }
        }


        private void WanderRandomly()
        {
            Vector3 randomDirection = transform.position + Random.insideUnitSphere * 5f;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 5f, NavMesh.AllAreas))
            {
                SetDestination(hit.position);
            }
        }
    }
}
