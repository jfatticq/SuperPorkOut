using SuperPorkOut.Characters.Player;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Stamina stamina;   // Assign Oinkle here
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();

        if (stamina != null)
        {
            slider.maxValue = stamina.Max;
        }
    }

    void Update()
    {
        if (stamina != null)
        {

            slider.value = stamina.Current;
        }
    }
}
