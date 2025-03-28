using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMS_AdvancedZombieAI;

namespace FMS_AdvancedZombieAI
{
    public class GunScript : MonoBehaviour
    {
        [Header("Values")]
        [Space]
        public float damage = 30f;
        public float range = 100f;
        public float fireRate = 0.2f;
        [Space]
        [Header("Effects")]
        [Space]
        public ParticleSystem Muzzle;
        public AudioClip shootSound;
        public float soundIntensity = 10f;
        [Space]
        [Tooltip("Set the zombie's layermask")]
        public LayerMask zombieLayer;

        private bool canShoot = true;
        private float hitCooldown = 1f;
        private bool canHit = true;
        private AudioSource audioS;
        private Camera cam;
        private Animator anim;
        public Animator camHolderAnim;

        public ParticleSystem blood;

        void Start()
        {
            anim = GetComponent<Animator>();
            cam = GetComponentInParent<Camera>();
            audioS = GetComponent<AudioSource>();
        }

        void Update()
        {
            // Check for mouse click and whether the gun can shoot
            if (Input.GetKey(KeyCode.Mouse0) && canShoot)
            {
                StartCoroutine(ShootWithDelay());
            }
            if (Input.GetKey(KeyCode.Mouse0))
            {
                anim.SetBool("Shoot", true);
                camHolderAnim.SetBool("CameraShake", true);
            }
            else
            {
                anim.SetBool("Shoot", false);
                camHolderAnim.SetBool("CameraShake", false);

            }
        }

        // Coroutine for shooting with a delay
        private IEnumerator ShootWithDelay()
        {
            Shoot();
            canShoot = false;
            yield return new WaitForSeconds(fireRate);
            canShoot = true;
        }

        // Method for shooting
        public void Shoot()
        {
            FireZombie();
            // Play shooting sound and muzzle flash
            if (shootSound != null)
            {
                audioS.PlayOneShot(shootSound);
            }
            Muzzle.Play();

            RaycastHit raycast;

            // Check if the raycast hits a zombie on the specified layer
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out raycast, range,zombieLayer))
            {
                // Set the blood particle system position on the hit point
                if (blood != null)
                {
                    blood.transform.SetParent(raycast.transform);
                    blood.transform.position = raycast.point;
                    blood.Play();
                }
                else
                {
                    Debug.LogError("Blood Particle System Not Assigned. Assign It To The Gun Script From Prefab Folder");
                }

                // Get the AdvancedZombieAI component from the hit zombie
                AdvancedZombieAI zombieAI = raycast.transform.GetComponentInParent<AdvancedZombieAI>();

                // Check if the zombieAI is valid and has health remaining
                if (zombieAI != null && zombieAI.health > 0)
                {
                    if (!zombieAI.extraDamageIfShotOnHead)
                    {
                        zombieAI.health -= damage;
                    }
                    else
                    {
                        if (raycast.transform.gameObject != zombieAI.zombieHead)
                        {
                            zombieAI.health -= damage;
                        }
                        else if (raycast.transform.gameObject == zombieAI.zombieHead)
                        {
                            zombieAI.health -= zombieAI.headDamage;
                        }

                    }
                    if (canHit)
                    {
                        if (zombieAI.animator.GetBool("Chase"))
                        {
                            zombieAI.animator.SetTrigger("RunHit");
                            zombieAI.actionState = AdvancedZombieAI.ActionState.ChaseHit;
                            StartCoroutine(HitCooldown());
                        }
                        if (!zombieAI.animator.GetBool("Chase"))
                        {
                            zombieAI.motionState = AdvancedZombieAI.MotionState.Chasing;
                            zombieAI.actionState = AdvancedZombieAI.ActionState.Hit;
                            zombieAI.animator.SetTrigger("Hit");
                            StartCoroutine(HitCooldown());
                        }
                    }
                }

                // Check if the zombieAI is valid and has specific functionality
                if (zombieAI != null)
                {
                    // Reduce crawling health if the zombie will crawl after depleting health
                    if (zombieAI.willCrawlAfterDepletingHealth && zombieAI.health <= 0)
                    {
                        if (!zombieAI.extraDamageIfShotOnHead)
                        {
                            zombieAI.crawlingHealth -= damage;
                        }
                        else
                        {

                            if (raycast.transform.gameObject == zombieAI.zombieHead)
                            {
                                zombieAI.crawlingHealth -= zombieAI.headDamage;
                            }
                            else
                            {
                                zombieAI.crawlingHealth -= damage;

                            }
                        }
                    }
                }
            }
        }

        public void FireZombie()
        {
            // Find zombies in the vicinity and make them chase the player
            Collider[] zombies = Physics.OverlapSphere(transform.position, soundIntensity, zombieLayer);
            for (int i = 0; i < zombies.Length; i++)
            {
                AdvancedZombieAI zombieAI = zombies[i].GetComponent<AdvancedZombieAI>();

                if (zombieAI != null)
                {
                    zombieAI.motionState = AdvancedZombieAI.MotionState.Chasing;
                }
            }
        }

        private IEnumerator HitCooldown()
        {
            canHit = false;
            yield return new WaitForSeconds(hitCooldown);
            canHit = true;
        }
    }
}
