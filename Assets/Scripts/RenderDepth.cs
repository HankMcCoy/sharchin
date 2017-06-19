using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class RednerDepth : MonoBehaviour {

    void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;;
    }
}
