using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class MainCamera : MonoBehaviour {
    GameObject player;
    float horizAngle = 180;
    float vertAngle = 0.3f;
    Vector3 offset;
    float distance;

	void Start () {
        player = GameObject.Find("Player");
        offset = player.transform.position - transform.position;
        distance = offset.magnitude;
        horizAngle = 0;
	}
	
	void LateUpdate () {
        transform.LookAt(player.transform);
		horizAngle += CrossPlatformInputManager.GetAxis("Mouse X") / 50f;
		vertAngle -= CrossPlatformInputManager.GetAxis("Mouse Y") / 50f;
        float y = Mathf.Sin(vertAngle) * distance;
        float xyDist = Mathf.Cos(vertAngle) * distance;
        float x = Mathf.Sin(horizAngle) * xyDist;
        float z = Mathf.Cos(horizAngle) * xyDist;
        transform.position = player.transform.position + new Vector3(x, y, z);
	}
}
