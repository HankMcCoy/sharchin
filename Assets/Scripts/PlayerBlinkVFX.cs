using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlinkVFX : MonoBehaviour {

    private LineRenderer lineRenderer;
    public float initialVelocity = 10.0f;
    private float timeResolution = 0.02f, maxTime = 10.0f;
    public LayerMask layerMask;

    // Use this for initialization
    void Start () {
        lineRenderer = GetComponent<LineRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 velocityVector = initialVelocity * transform.forward;
        lineRenderer.positionCount = (int)(maxTime / timeResolution);
        int index = 0;
        Vector3 currentPosition = transform.position;

        for (float t = 0.0f; t < maxTime; t += timeResolution)
        {
            lineRenderer.SetPosition(index, currentPosition);

            RaycastHit hit;
            if(Physics.Raycast(currentPosition, velocityVector, out hit, velocityVector.magnitude, layerMask))
            {
                lineRenderer.positionCount = (index + 2);
                lineRenderer.SetPosition(index + 1, hit.point);
                //Set blink prefab to here
                break;
            }
            currentPosition += velocityVector * timeResolution;
            velocityVector += Physics.gravity;
            index++;
        }
    }
}
