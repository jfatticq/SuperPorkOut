using UnityEngine;
using TMPro;

public class PopupText : MonoBehaviour
{
    public TMP_Text text;
    public float lifetime = 1f;
    public float floatSpeed = 1f;

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
            Destroy(gameObject);
    }

    public void SetText(string value)
    {
        text.text = value;
    }
}