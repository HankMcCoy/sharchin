﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerBlink : MonoBehaviour
{

    public bool resetVelocityOnBlink;
    public float timeStep = 0.05f;
    public float lookSensitivity = 7f;
    public float initialBlinkVelocity = 20f;
    public Camera playerCamera;

    private bool CanBlink { get; set; }
    private bool m_Blink, m_Cancel;
    private bool prefabInstantiated;
    private bool blinkInitiated;
    private int blinkLayer = 1 << 9;
    private GameObject blinkPrefab;
    private GameObject instantiatedBlinkPrefab;
    private Rigidbody rigidBody;
    private Vector3 blinkVelocity;
    private LineRenderer lineRenderer;

    // Use this for initialization
    void Start()
    {
        CanBlink = true;
        blinkPrefab = (GameObject)Resources.Load("Prefabs/BlinkPrefab");
        rigidBody = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
    }

	// Update is called once per frame
	void Update () {
            m_Blink = CrossPlatformInputManager.GetButtonDown("Blink");
            m_Cancel = CrossPlatformInputManager.GetButtonDown("Cancel");
        }

    private void FixedUpdate()
    {
        blinkVelocity = playerCamera.transform.forward * initialBlinkVelocity;

        blinkVelocity.y = -playerCamera.transform.forward.y * lookSensitivity;

        if (blinkInitiated)
        {
            if (m_Cancel)
            {
                ResetBlinkPrefab();
            }else{
                PlotTrajectory(transform.position, blinkVelocity, timeStep, 5f);
            }
        }

        if (m_Blink)
        {
            if (blinkInitiated)
            {
                TeleportToDestination();
            }
            else
            {
                InstantiateBlinkPrefab(Vector3.zero);
            }
        }
    }

    private void TeleportToDestination() {
        Vector3 destination = instantiatedBlinkPrefab.transform.position + new Vector3(0, 1, 0);
        ResetBlinkPrefab();
        if (resetVelocityOnBlink)
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
        transform.position = destination;
    }

    private void InstantiateBlinkPrefab(Vector3 position)
    {
        prefabInstantiated = true;
        blinkInitiated = true;
        lineRenderer.enabled = true;
        instantiatedBlinkPrefab = Instantiate(blinkPrefab, position, Quaternion.identity);
    }

    private void ResetBlinkPrefab()
    {
        Destroy(instantiatedBlinkPrefab);
        instantiatedBlinkPrefab = null;
        prefabInstantiated = false;
        blinkInitiated = false;
        lineRenderer.enabled = false;
    }

	public void PlotTrajectory(Vector3 start, Vector3 startVelocity, float timestep, float maxTime)
	{
        lineRenderer.positionCount = 0;
        Vector3 prev = start;
		for (int i = 1; ; i++)
		{
			float t = timestep * i;
			if (t > maxTime) break;
			Vector3 pos = PlotTrajectoryAtTime(start, startVelocity, t);
            lineRenderer.positionCount = i;
            lineRenderer.SetPosition(i - 1, pos);
            if (Physics.Linecast(prev, pos, blinkLayer))
            {
                if (prefabInstantiated)
                {
                    instantiatedBlinkPrefab.transform.position = pos;
                }else
                {
                    InstantiateBlinkPrefab(pos);
                }
                break;
            }
			Debug.DrawLine(prev, pos, Color.red);
			prev = pos;
		}
	}

	public Vector3 PlotTrajectoryAtTime(Vector3 start, Vector3 startVelocity, float time)
	{
		return start + startVelocity * time + Physics.gravity * time * time * 0.5f;
	}
}
