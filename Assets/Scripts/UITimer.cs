using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour {

    public float totalTime;
    private float currentTime;
    private Image uiImage;

	// Use this for initialization
	void Start () {
        uiImage = GetComponent<Image>();
        currentTime = totalTime;
	}
	
	// Update is called once per frame
	void Update () {
        currentTime -= Time.deltaTime;
        uiImage.fillAmount = currentTime / totalTime;
        DetermineUIColor();
	}

    private void DetermineUIColor(){
        float percentage = currentTime / totalTime;
        if (percentage > 0.30f){
            uiImage.color = Color.green;
        }else if (percentage > 0.15f){
            uiImage.color = Color.yellow;
        }else{
            uiImage.color = Color.red;
        }
    }
}
