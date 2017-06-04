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
    public float speed = 1.0f; // In units per second.
    public float attackForce = 400.0f;

    private void Start() {

    }

    private void Update() {
        float distanceFromTarget = (target.transform.position - transform.position).magnitude;

        // If within range, walk towards player (don't change y).
        if (distanceFromTarget < aggroDistance) {
            transform.LookAt(target.transform);
            float step = speed * Time.deltaTime;
            Vector3 newPosition = Vector3.MoveTowards(transform.position, target.transform.position, step);
            newPosition.y = transform.position.y;
            transform.position = newPosition;
        }

        // Punch player if nearbye.
        if (distanceFromTarget < attackDistance) {
            target.GetComponent<Rigidbody>().AddForce(attackForce * (target.transform.position - transform.position));
        }
    }
}
