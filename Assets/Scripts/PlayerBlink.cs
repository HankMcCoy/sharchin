﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerBlink : MonoBehaviour
{

    public bool resetVelocityOnBlink;
    public Vector3 blinkVelocity = new Vector3(0, 0, 50.0f);
    public float timeStep = 0.2f;

    private bool CanBlink { get; set; }
    private bool m_Blink, m_Cancel;
    private bool prefabInstantiated;
    private bool blinkInitiated;
    private int blinkLayer = 1 << 9;
    private GameObject blinkPrefab;
    private GameObject instantiatedBlinkPrefab;
    private Rigidbody rigidBody;

    // Use this for initialization
    void Start()
    {
        CanBlink = true;
        blinkPrefab = (GameObject)Resources.Load("Prefabs/BlinkPrefab");
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        m_Blink = CrossPlatformInputManager.GetButtonDown("Blink");
        m_Cancel = CrossPlatformInputManager.GetButtonDown("Cancel");
    }

    private void FixedUpdate()
    {
        if (blinkInitiated)
        {
            if (m_Cancel)
            {
                ResetBlinkPrefab();
            }else{
                PlotTrajectory(transform.position, blinkVelocity, timeStep, 1.5f);
            }
        }

        if (m_Blink)
        {
            if (blinkInitiated)
            {
                Vector3 destination = new Vector3(instantiatedBlinkPrefab.transform.position.x, instantiatedBlinkPrefab.transform.position.y + 1, instantiatedBlinkPrefab.transform.position.z);
                ResetBlinkPrefab();
                if (resetVelocityOnBlink)
                {
                    rigidBody.velocity = Vector3.zero;
                    rigidBody.angularVelocity = Vector3.zero;
                }
                transform.position = destination;
            }
            else
            {
                instantiatedBlinkPrefab = Instantiate(blinkPrefab, new Vector3(0, transform.position.y + 10, 0), Quaternion.identity);
                prefabInstantiated = true;
                blinkInitiated = true;
            }
        }
    }

    private void ResetBlinkPrefab(){

        Destroy(instantiatedBlinkPrefab);
        instantiatedBlinkPrefab = null;
        prefabInstantiated = false;
        blinkInitiated = false;
    }

	public void PlotTrajectory(Vector3 start, Vector3 startVelocity, float timestep, float maxTime)
	{
		Vector3 prev = start;
		for (int i = 1; ; i++)
		{
			float t = timestep * i;
			if (t > maxTime) break;
			Vector3 pos = PlotTrajectoryAtTime(start, startVelocity, t);
			if (Physics.Linecast(prev, pos, blinkLayer)) break;
			Debug.DrawLine(prev, pos, Color.red);
			prev = pos;
		}
	}

	public Vector3 PlotTrajectoryAtTime(Vector3 start, Vector3 startVelocity, float time)
	{
        Debug.Log("Drawing something");
		return start + startVelocity * time + Physics.gravity * time * time * 0.5f;
	}
}
