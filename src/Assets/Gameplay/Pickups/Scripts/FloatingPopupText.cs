using TMPro;
using UnityEngine;

namespace SuperPorkOut.Gameplay.Pickups
{
    public class FloatingPopupText : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        [Header("Motion")]
        [SerializeField] private Vector2 driftPixels = new(0f, 60f);

        [Header("Timing")]
        [SerializeField] private float lifetime = 0.8f;
        [SerializeField, Range(0f, 1f)] private float fadeStartNormalized = 0.35f;

        private RectTransform rt;
        private Vector2 startPos;
        private float t;
        private Color startColor;

        public void Init(string message, Vector2 anchoredPos)
        {
            if (!rt) rt = (RectTransform)transform;

            text.text = message;

            startPos = anchoredPos;
            rt.anchoredPosition = anchoredPos;

            startColor = text.color;
            t = 0f;
        }

        private void Awake()
        {
            rt = (RectTransform)transform;
        }

        private void Update()
        {
            t += Time.unscaledDeltaTime;
            float n = Mathf.Clamp01(t / lifetime);

            // Drift upward
            rt.anchoredPosition = startPos + driftPixels * n;

            // Fade text alpha
            float fadeT = Mathf.InverseLerp(fadeStartNormalized, 1f, n);
            float alpha = 1f - fadeT;

            Color c = startColor;
            c.a = alpha;
            text.color = c;

            if (t >= lifetime)
                Destroy(gameObject);
        }

#if UNITY_EDITOR
        private void Reset()
        {
            if (!text) text = GetComponentInChildren<TMP_Text>();
        }
#endif
    }
}