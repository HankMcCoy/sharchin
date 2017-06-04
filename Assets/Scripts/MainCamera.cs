using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class MainCamera : MonoBehaviour
{
    GameObject player;
    float horizAngle = 0;
    float vertAngle = 0.3f;
    Vector3 offset;
    float distance;

    void Start()
    {
        player = GameObject.Find("Player");
        offset = player.transform.position - transform.position;
        distance = offset.magnitude;
        horizAngle = player.transform.rotation.y;
    }

    void LateUpdate()
    {
        horizAngle = player.transform.rotation.eulerAngles.y + 180;
        vertAngle -= CrossPlatformInputManager.GetAxis("Mouse Y") / 50f;
        float horizAngleRad = horizAngle * Mathf.PI / 180f;
        float y = Mathf.Sin(vertAngle) * distance;
        float xyDist = Mathf.Cos(vertAngle) * distance;
        float x = Mathf.Sin(horizAngleRad) * xyDist;
        float z = Mathf.Cos(horizAngleRad) * xyDist;
        transform.position = player.transform.position + new Vector3(x, y, z);
        transform.LookAt(player.transform);
    }
}
