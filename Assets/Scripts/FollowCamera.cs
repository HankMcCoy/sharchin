using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {

    public bool alwaysOnGround = true;
    public Vector3 cameraViewOffset = Vector3.zero;

    private GameObject player;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane) + cameraViewOffset);
        //Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane) + cameraViewOffset);
        //transform.position =  new Vector3(pos.x, pos.y+ player.transform.position.y, pos.z);
        if(alwaysOnGround){
			RaycastHit hit;
			Physics.Raycast(transform.position, new Vector3(0, -1.0f, 0), out hit);

			if (hit.transform != null)
			{
                Renderer rend = hit.transform.GetComponent<Renderer>();
                Debug.Log(rend.bounds.size.y);
				transform.position = new Vector3(transform.position.x, hit.transform.GetComponent<Renderer>().bounds.size.y , transform.position.z);
			}
        }
	}
}
