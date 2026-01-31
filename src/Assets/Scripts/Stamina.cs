using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    public float maxStamina = 100f;
    public float currentStamina;

    public float drainRate = 20f;
    public float regenRate = 15f;

    public Image staminaFill;

    void Start()
    {
        currentStamina = maxStamina;
    }

    void Update()
    {
        // Hold Shift to drain stamina (example)
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0)
        {
            currentStamina -= drainRate * Time.deltaTime;
        }
        else
        {
            currentStamina += regenRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        staminaFill.fillAmount = currentStamina / maxStamina;
    }
}
