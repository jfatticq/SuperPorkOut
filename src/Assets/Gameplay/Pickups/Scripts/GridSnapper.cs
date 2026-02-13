using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SuperPorkOut.Gameplay.Pickups
{
    [ExecuteAlways]
    public class GridSnapper : MonoBehaviour
    {
        public Vector3 size = new(5f, 5f, 5f);
        public Color color = new(0f, 1f, 0f, 0.25f);

        [Header("Snapping")]
        [SerializeField] private bool snapInEditMode = true;
        [SerializeField] private bool snapYToGridHeight = true;
        [SerializeField] private bool clampInsideGridBounds = true;

        private void Update()
        {
            if (!snapInEditMode) return;
            if (Application.isPlaying) return;

            // Only do work when the designer actually moved/rotated the object
            if (!transform.hasChanged) return;

            TrySnapToNearestGrid();
            transform.hasChanged = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, size);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, size);
        }

        private void TrySnapToNearestGrid()
        {
            GridVisualizer grid = FindBestGridForThisObject();
            if (grid == null) return;

            // Convert to the grid's local space
            Vector3 local = grid.transform.InverseTransformPoint(transform.position);

            float cell = Mathf.Max(0.0001f, grid.cellSize);

            float halfWidth = grid.gridWidth * cell * 0.5f;
            float halfDepth = grid.gridDepth * cell * 0.5f;

            // Grid cell centers (matches your GridVisualizer math):
            // xCenter = (x * cell) - halfWidth + cell*0.5
            // zCenter = (z * cell) - halfDepth + cell*0.5
            float snapX = Mathf.Round((local.x + halfWidth - (cell * 0.5f)) / cell) * cell - halfWidth + (cell * 0.5f);
            float snapZ = Mathf.Round((local.z + halfDepth - (cell * 0.5f)) / cell) * cell - halfDepth + (cell * 0.5f);

            if (clampInsideGridBounds)
            {
                float minX = -halfWidth + (cell * 0.5f);
                float maxX = halfWidth - (cell * 0.5f);
                float minZ = -halfDepth + (cell * 0.5f);
                float maxZ = halfDepth - (cell * 0.5f);

                snapX = Mathf.Clamp(snapX, minX, maxX);
                snapZ = Mathf.Clamp(snapZ, minZ, maxZ);
            }

            float snapY = snapYToGridHeight ? grid.heightOffset : local.y;

            Vector3 snappedLocal = new Vector3(snapX, snapY, snapZ);
            Vector3 snappedWorld = grid.transform.TransformPoint(snappedLocal);

#if UNITY_EDITOR
            // Makes snapping play nicely with Undo in the editor
            Undo.RecordObject(transform, "Snap To Grid");
#endif

            transform.position = snappedWorld;
        }

        private GridVisualizer FindBestGridForThisObject()
        {
            // Prefer the nearest grid that "contains" us in X/Z bounds.
            // If none contain us, fall back to the closest grid by distance.
            GridVisualizer[] grids = Object.FindObjectsByType<GridVisualizer>(FindObjectsSortMode.None);
            if (grids == null || grids.Length == 0) return null;

            GridVisualizer bestContaining = null;
            float bestContainingDist = float.MaxValue;

            GridVisualizer bestAny = null;
            float bestAnyDist = float.MaxValue;

            Vector3 p = transform.position;

            foreach (var g in grids)
            {
                float cell = Mathf.Max(0.0001f, g.cellSize);
                float halfWidth = g.gridWidth * cell * 0.5f;
                float halfDepth = g.gridDepth * cell * 0.5f;

                Vector3 local = g.transform.InverseTransformPoint(p);

                bool inside =
                    local.x >= (-halfWidth) && local.x <= (halfWidth) &&
                    local.z >= (-halfDepth) && local.z <= (halfDepth);

                // Use planar distance in world (usually what designers “feel”)
                Vector3 gw = g.transform.position;
                float dist = Vector2.Distance(new Vector2(p.x, p.z), new Vector2(gw.x, gw.z));

                if (inside && dist < bestContainingDist)
                {
                    bestContainingDist = dist;
                    bestContaining = g;
                }

                if (dist < bestAnyDist)
                {
                    bestAnyDist = dist;
                    bestAny = g;
                }
            }

            return bestContaining != null ? bestContaining : bestAny;
        }
    }
}
