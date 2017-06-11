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
    private PlayerController playerController;
    private float timeSinceLastProjectileFired = 0.0f;
	private int health = 5;

    private void Start() {
		playerController = target.GetComponent<PlayerController>();
    }

    private void Update() {
		if (health <= 0) {
			Destroy(this.gameObject);
		}
        float distanceFromTarget = (target.transform.position - transform.position).magnitude;

        // If within range, walk towards player (don't change y).
        if (distanceFromTarget < aggroDistance) {
            transform.LookAt(target.transform);
            float step = speed * Time.deltaTime;
            Vector3 newPosition = Vector3.MoveTowards(transform.position, target.transform.position, step);
            newPosition.y = transform.position.y;
            transform.position = newPosition;
            if (Time.time - timeSinceLastProjectileFired > 1.0f) {
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
        GameObject projectile = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), transform.position+transform.forward, Quaternion.identity) as GameObject;
		projectile.tag = "player_damaging";
        projectile.AddComponent<Rigidbody>();
        projectile.GetComponent<Rigidbody>().useGravity = false;
        projectile.GetComponent<Rigidbody>().isKinematic = false;
        projectile.GetComponent<Rigidbody>().velocity = transform.forward * 10.0f;
        Destroy(projectile, 3.0f);
   }

	/** Trigger on collision with enemy. */
    void OnCollisionEnter (Collision collision) {
		/* Console.log(col.gameObject.tag); */
        if(collision.gameObject.tag.Equals("enemy_damaging")) {
			// Calculate Angle Between the collision point and the player
			Vector3 forceDirection = collision.contacts[0].point - transform.position;
			forceDirection = -forceDirection.normalized;

			health--;
            Destroy(collision.gameObject);

			// Show Explosion
			GameObject explosion = Instantiate(Resources.Load("Prefabs/Explosion"), transform.position+transform.forward, Quaternion.identity) as GameObject;
			Destroy(explosion, 1.0f);
        }
    }
}
