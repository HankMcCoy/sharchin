using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controller for causing a game object to rotate continuously.
 */
public class RotatingPlatformController : MonoBehaviour {
	public Vector3 rotationSpeed; // Vector to rotate by.  ex: (0,0,1)

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
        transform.localEulerAngles = transform.localEulerAngles + rotationSpeed;
    }
}
