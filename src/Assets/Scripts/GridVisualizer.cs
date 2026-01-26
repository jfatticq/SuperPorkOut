using UnityEngine;

[ExecuteAlways]
public class GridVisualizer : MonoBehaviour
{
    [Header("Grid Dimensions (in cells)")]
    public int gridWidth = 50;
    public int gridDepth = 5;

    [Header("Cell Settings")]
    public float cellSize = 5f;
    public float heightOffset = 0.5f;

    [Header("Gizmo Appearance")]
    public Color gizmoColor = new(0f, 1f, 0f, 0.4f);

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // THIS IS THE IMPORTANT PART
        Gizmos.matrix = transform.localToWorldMatrix;

        float halfWidth = gridWidth * cellSize * 0.5f;
        float halfDepth = gridDepth * cellSize * 0.5f;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                float xPos = (x * cellSize) - halfWidth + (cellSize * 0.5f);
                float zPos = (z * cellSize) - halfDepth + (cellSize * 0.5f);

                Vector3 localCellCenter = new(
                    xPos,
                    heightOffset,
                    zPos
                );

                Gizmos.DrawWireCube(
                    localCellCenter,
                    Vector3.one * cellSize
                );
            }
        }

        // Optional safety reset (good habit)
        Gizmos.matrix = Matrix4x4.identity;
    }
}
