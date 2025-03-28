using System.Collections;
using UnityEngine;

namespace FMS_AdvancedZombieAI
{
    public class Pheromone : MonoBehaviour
    {
        public float strength = 1f; // Initial pheromone strength
        public float decayRate = 0.1f; // How fast pheromones fade
        public Vector3? playerPosition = null; // Stores player position if known

        public void Initialize(float initialStrength, float decay, Vector3? detectedPlayerPosition = null)
        {
            strength = initialStrength;
            decayRate = decay;
            playerPosition = detectedPlayerPosition; // Store player position
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
}
