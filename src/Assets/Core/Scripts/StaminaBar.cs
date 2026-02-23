using SuperPorkOut.Characters.Player;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Stamina stamina;
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f; // normalized
    }

    void Update()
    {
        slider.value = stamina.Normalized();
    }
}