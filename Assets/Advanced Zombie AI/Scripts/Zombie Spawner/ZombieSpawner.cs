using UnityEngine;
using FMS_AdvancedZombieAI;

namespace FMS_AdvancedZombieAI
{
   

    public class ZombieSpawner : MonoBehaviour
    {
        public enum SpawnMode
        {
            OverTime,
            OnZombieDeath
        }
        public GameObject[] zombiePrefabs;
        public float spawnInterval = 3f;
        public SpawnMode spawnMode = SpawnMode.OverTime;

        private float nextSpawnTime;


        void Update()
        {
            if (spawnMode == SpawnMode.OverTime && Time.time >= nextSpawnTime)
            {
                SpawnZombie();
                nextSpawnTime = Time.time + spawnInterval;
            }
        }

        void SpawnZombie()
        {
            int randomIndex = Random.Range(0, zombiePrefabs.Length);
            Vector3 spawnPosition = new Vector3(
                Random.Range(transform.position.x - transform.localScale.x / 2f, transform.position.x + transform.localScale.x / 2f),
                transform.position.y,
                Random.Range(transform.position.z - transform.localScale.z / 2f, transform.position.z + transform.localScale.z / 2f)
            );

            // Instantiate the zombie prefab and set its parent
            GameObject newZombie = Instantiate(zombiePrefabs[randomIndex], spawnPosition, Quaternion.identity);
        }

        public void SpawnZombieOnDeath()
        {
            int randomIndex = Random.Range(0, zombiePrefabs.Length);
            Vector3 spawnPosition = new Vector3(
                Random.Range(transform.position.x - transform.localScale.x / 2f, transform.position.x + transform.localScale.x / 2f),
                transform.position.y,
                Random.Range(transform.position.z - transform.localScale.z / 2f, transform.position.z + transform.localScale.z / 2f)
            );

            // Instantiate the zombie prefab and set its parent
            GameObject newZombie = Instantiate(zombiePrefabs[randomIndex], spawnPosition, Quaternion.identity);

        }

    }
}
