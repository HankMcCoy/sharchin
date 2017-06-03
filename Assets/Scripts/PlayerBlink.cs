using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerBlink : MonoBehaviour {

    public bool resetVelocityOnBlink;

    private bool CanBlink { get; set; }
    private bool m_Blink, m_Cancel;
    private bool prefabInstantiated;
    private bool blinkInitiated;
    private GameObject blinkPrefab;
    private GameObject instantiatedBlinkPrefab;
    private Rigidbody rigidBody;

	// Use this for initialization
	void Start () {
        CanBlink = true;
        blinkPrefab = (GameObject)Resources.Load("Prefabs/BlinkPrefab");
        rigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		m_Blink = CrossPlatformInputManager.GetButtonDown("Blink");
        m_Cancel = CrossPlatformInputManager.GetButtonDown("Cancel");
	}

    private void FixedUpdate()
    {
        if (blinkInitiated)
        {
            if (m_Cancel)
            {
                Destroy(instantiatedBlinkPrefab);
                ResetBlinkPrefab();
            }
        }

        if(m_Blink)
        {
            if (blinkInitiated)
            {
                Destroy(instantiatedBlinkPrefab);
                if (resetVelocityOnBlink)
                {
                    rigidBody.velocity = Vector3.zero;
                    rigidBody.angularVelocity = Vector3.zero;
                }
				transform.position = instantiatedBlinkPrefab.transform.position;
                ResetBlinkPrefab();
			}else
            {
                instantiatedBlinkPrefab = Instantiate(blinkPrefab, new Vector3(0,5,0), Quaternion.identity);
                prefabInstantiated = true;
				blinkInitiated = true;
            }
        }
    }

    private void ResetBlinkPrefab(){
		instantiatedBlinkPrefab = null;
		prefabInstantiated = false;
		blinkInitiated = false;
    }
}
