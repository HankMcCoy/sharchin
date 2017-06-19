using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controller for moving platforms.
 */
public class ConveyorBeltController : MonoBehaviour {
	public Vector3 conveyorVelocity;

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {

    }

	void OnCollisionStay(Collision collision) {
		Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>();
		rigidbody.velocity = conveyorVelocity;
	}
}
