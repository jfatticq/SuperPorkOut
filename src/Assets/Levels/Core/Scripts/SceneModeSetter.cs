using UnityEngine;

public class SceneModeSetter : MonoBehaviour
{
    [SerializeField] private GameMode mode;

    private void Start()
    {
        InputManager.Instance.SetMode(mode);
    }
}
