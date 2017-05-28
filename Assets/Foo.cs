using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Foo : MonoBehaviour {
	private Transform m_Cam;                  // A reference to the main camera in the scenes transform
	private Vector3 m_CamForward;             // The current forward direction of the camera
	private Vector3 m_Move;
	private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
	public float rotationSpeed = 2.0f;
	public float jumpPower = 100.0f;
	private Rigidbody rigidBody;

	private void Start()
	{
		rigidBody = GetComponent<Rigidbody> ();
		// get the transform of the main camera
		if (Camera.main != null)
		{
			m_Cam = Camera.main.transform;
		}
		else
		{
			Debug.LogWarning(
				"Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
			// we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
		}
	}


	private void Update()
	{
		if (!m_Jump)
		{
			m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
		}
	}


	// Fixed update is called in sync with physics
	private void FixedUpdate()
	{
		// read inputs
		float h = CrossPlatformInputManager.GetAxis("Horizontal");
		float v = CrossPlatformInputManager.GetAxis("Vertical");

		// calculate camera relative direction to move:
		transform.Rotate (Vector3.up * rotationSpeed * h * Time.deltaTime);

		//CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
        //m_Move = v * m_CamForward;
		m_Move.z = v;


		Debug.Log (Physics.Raycast (transform.position, new Vector3 (0, -1.0f, 0), 1.0f));
		if (m_Jump && Physics.Raycast (transform.position, new Vector3 (0, -1.0f, 0), 1.0f)) {
			rigidBody.AddForce (new Vector3 (0, jumpPower, 0));
		} 

		// pass all parameters to the character control script
		m_Jump = false;
		transform.Translate (m_Move, Space.Self);
	}
}

