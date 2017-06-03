using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {

    public bool alwaysOnGround = true;
    public Vector3 cameraViewOffset = Vector3.zero;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane) + cameraViewOffset);
        if(alwaysOnGround){
			RaycastHit hit;
			Physics.Raycast(transform.position, new Vector3(0, -1.0f, 0), out hit);

			if (hit.transform != null)
			{
				transform.position = new Vector3(transform.position.x, hit.transform.position.y + 1, transform.position.z);
			}
        }
	}
}
