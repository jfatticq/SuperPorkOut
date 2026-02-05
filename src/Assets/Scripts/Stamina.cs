using System;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    public float maxStamina = 100f;
    public float currentStamina = 100f;

    public float drainRate = 20f;

    private void Update()
    {
        currentStamina -= drainRate * Time.deltaTime;
        
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }
}
