using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
	public Vector3 initialDestination = new Vector3(0,0,1.0f);
	private Vector3 destination;
	public float period = 3.0f;
	public Vector3 startPosition;
	private float speed;

	// Use this for initialization
	void Start () {
		destination = initialDestination;
		startPosition = this.transform.localPosition;
		speed = ((startPosition - destination).magnitude / period) * Time.deltaTime;
		//Debug.Log ("speed: " + speed);
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = Vector3.MoveTowards (transform.localPosition, destination, speed);
		if(transform.localPosition == destination){
			SwitchDirection ();
		}

	}

	private void SwitchDirection(){
		if (destination == startPosition) {
			destination = initialDestination;
		} else {
			destination = startPosition;
		}
	}
}
