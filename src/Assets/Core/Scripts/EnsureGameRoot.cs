using UnityEngine;

public static class GameRootBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureGameRoot()
    {
        if (InputManager.Instance != null)
            return;

        var prefab = Resources.Load<GameObject>("GameRoot");
        if (prefab == null)
        {
            Debug.LogError("GameRootBootstrap: Could not find Resources/GameRoot.prefab");
            return;
        }

        Object.Instantiate(prefab);
    }
}
