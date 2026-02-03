using UnityEngine;
using UnityEngine.InputSystem;

public enum GameMode
{
    MainMenu,
    Playing,
    Paused,
    Settings,
    Guide
}

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public InputSystem_Actions Actions { get; private set; }

    public GameMode Mode { get; private set; }

    public event System.Action PausePressed;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Actions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        if (Actions == null) return;

        Actions.Enable();

        // Always listen for Pause (System map)
        Actions.System.Pause.performed += OnPausePerformed;
    }

    private void OnDisable()
    {
        if (Actions == null) return;

        Actions.System.Pause.performed -= OnPausePerformed;
        Actions.Disable();
    }

    private void OnPausePerformed(InputAction.CallbackContext _)
    {
        PausePressed?.Invoke();
    }

    public void SetMode(GameMode mode)
    {
        Mode = mode;

        // Disable all maps first (except System, which we keep enabled)
        Actions.Gameplay.Disable();
        Actions.UI.Disable();
        Actions.System.Enable();

        switch (mode)
        {
            case GameMode.Playing:
                Actions.Gameplay.Enable();
                break;

            case GameMode.Paused:
            case GameMode.MainMenu:
            case GameMode.Settings:
            case GameMode.Guide:
                Actions.UI.Enable();
                break;
        }
    }
}
