using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/**
  * Shoots water out its butt.
  */
public class WaterSpewer : MonoBehaviour {
    public Vector3 waterForce = new Vector3(10.0f, 0, 0);
    private bool enabled = true;

    private void Update() {
       if (enabled &&transform.CompareTag("ice")) {
               enabled = false;
               this.gameObject.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
       } else if (!enabled && transform.CompareTag("water")) {
               enabled = true;
               this.gameObject.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
       }
    }

    /** Trigger on collision with enemy. */
    void OnTriggerStay (Collider collider) {
        if(enabled && collider.gameObject.tag.Equals("Player")) {
          collider.gameObject.GetComponent<Rigidbody>().AddForce(waterForce);
        }
    }
}
