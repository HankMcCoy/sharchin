using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/**
  * Basic Enemy AI behavior.  In the future we'll have different types of these.
  */
public class EnemyController : MonoBehaviour {
    public GameObject target; // Where the AI will try to go.
    public float aggroDistance = 5.0f;
    public float attackDistance = 1.0f; // How far away the AI can push you.
    public float damage = 1.0f; // How much damage enemy does.
    public float speed = 1.0f; // In units per second.
    public float attackForce = 400.0f;
    public float projectileLifeTime = 3.0f;
    public float projectileSpeed = 10.0f;
    public float projectileCoolDown = 1.0f;
    public int projectileCount = 1;
    public float projectileAccuracy = 0.0f;
    public bool fireRandomly = false;
    private PlayerController playerController;
    private float timeSinceLastProjectileFired = 0.0f;
    public int health = 5;
    private int startingHealth;

    private void Start() {
        playerController = target.GetComponent<PlayerController>();
        startingHealth = health;
    }

    private void Update() {
        if (health <= 0) {
            GameObject deathEffect = Instantiate(Resources.Load("Prefabs/EnemyDeathEffect"), transform.position+transform.forward, Quaternion.identity) as GameObject;
            Destroy(deathEffect, 1.0f);
            Destroy(this.gameObject);
        }
        float distanceFromTarget = (target.transform.position - transform.position).magnitude;

        // If within range, walk towards player (don't change y).
        if (distanceFromTarget < aggroDistance || health < startingHealth) {
            transform.LookAt(target.transform);
            float step = speed * Time.deltaTime;
            Vector3 newPosition = Vector3.MoveTowards(transform.position, target.transform.position, step);
            newPosition.y = transform.position.y;
            transform.position = newPosition;
            if (Time.time - timeSinceLastProjectileFired > projectileCoolDown) {
                fireProjectile();
                timeSinceLastProjectileFired = Time.time;
            }
        }

        // Punch player if nearbye.
        if (distanceFromTarget < attackDistance) {
            playerController.damagePlayer(damage);
            playerController.pushPlayer(attackForce * (target.transform.position - transform.position));
        }
    }

    void fireProjectile() {
        // Shoot projectile.
        for(int i=0; i<projectileCount; i++) {
            GameObject projectile = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), transform.position+transform.forward, Quaternion.identity) as GameObject;
            projectile.tag = "player_damaging";
            projectile.AddComponent<Rigidbody>();
            projectile.GetComponent<Rigidbody>().useGravity = false;
            projectile.GetComponent<Rigidbody>().mass = 0.001f;
            projectile.GetComponent<Rigidbody>().isKinematic = false;
            if (fireRandomly) {
                float randomAngle = Random.Range ( 0, 2 * Mathf.PI );
                projectile.GetComponent<Rigidbody>().velocity =   new Vector3(
                        3.0f * Mathf.Cos( randomAngle ),
                        3.0f * Mathf.Sin( randomAngle ),
                        transform.forward.z
             )* projectileSpeed;
            } else {
                projectile.GetComponent<Rigidbody>().velocity = transform.forward * projectileSpeed;
            }
            Destroy(projectile, projectileLifeTime);
        }
   }

    /** Trigger on collision with enemy. */
    void OnCollisionEnter (Collision collision) {
        if(collision.gameObject.tag.Equals("enemy_damaging")) {
            // Calculate Angle Between the collision point and the player
            Vector3 forceDirection = collision.contacts[0].point - transform.position;
            forceDirection = -forceDirection.normalized;

            health--;
            Destroy(collision.gameObject);

            // Show Explosion
            GameObject explosion = Instantiate(Resources.Load("Prefabs/EnemyHitEffect"), transform.position+transform.forward, Quaternion.identity) as GameObject;
            transform.Rotate(-forceDirection);
            Destroy(explosion, 1.0f);
        }
    }
}
