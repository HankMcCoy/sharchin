using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour {
    public float maxHealth = 100.0f;
    private float currentHealth;

    public float getHealth() {
        return currentHealth;
    }

    public float getHealthPercentage() {
        return currentHealth/maxHealth;
    }

    public void setHealth(float health) {
        currentHealth = health;
    }

    public void decreaseHealth(float healthDelta) {
        currentHealth -= healthDelta;
    }

    public void increaseHealth(float healthDelta) {
        currentHealth += healthDelta;
    }

    void Start () {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update () {

    }
}
