using SuperPorkOut.Gameplay.Pickups;
using UnityEngine;

public class FoodPopupSpawner : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform layer;
    [SerializeField] private FloatingPopupText popupPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float verticalOffset = 2f;
    



    [Header("Placement")]
    [SerializeField] private Vector2 screenOffsetPixels = new(0f, 40f);

    private void OnEnable()
    {
        PowerUp.PickedUp += OnPickedUp;
    }

    private void OnDisable()
    {
        PowerUp.PickedUp -= OnPickedUp;
    }

    private void OnPickedUp(PickupEventData data)
    {
        // Format however you like (whole numbers, decimals, etc.)
        string msg = $"+{Mathf.RoundToInt(data.StaminaAmount)}";
        Vector3 popupworldPos = playerTransform.position + Vector3.up * verticalOffset;
        SpawnAtWorldPoint(popupworldPos, msg);
    }

    private void SpawnAtWorldPoint(Vector3 worldPos, string message)
    {
        Camera cam = null;

        // Overlay canvases ignore camera for screen conversion.
        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
        
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
       
        // Convert screen -> local anchored pos in layer
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            layer, screenPos, cam, out Vector2 localPoint);

        var popup = Instantiate(popupPrefab, layer);
        popup.Init(message, localPoint + screenOffsetPixels);
    }

#if UNITY_EDITOR
    private void Reset()
    {
        if (!canvas) canvas = GetComponentInParent<Canvas>();
        if (!layer && canvas) layer = canvas.GetComponent<RectTransform>();
    }
#endif
}