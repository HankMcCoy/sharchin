using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/**
  * Basic Enemy AI behavior.  In the future we'll have different types of these.
  */
public class EnemyAi : MonoBehaviour {
    public GameObject target; // Where the AI will try to go.
    public float aggroDistance = 5.0f;
    public float attackDistance = 1.0f; // How far away the AI can push you.
    public float speed = 1.0f; // In units per second.

    private void Start() {

    }

    private void Update() {
        float distanceFromTarget = (target.transform.position - transform.position).magnitude;

        if (distanceFromTarget < aggroDistance) {
            transform.LookAt(target.transform);
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
        }
        if (distanceFromTarget < attackDistance) {

        }
    }
}
