using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PipeBoard : MonoBehaviour
{
    public int rows = 5;
    public int cols = 5;

    public Transform gridParent;
    private PipeTile[,] grid;

    public PipeTile sourceTile;
    public PipeTile targetTile;

    public FuseBox fuseBox; 

    void Awake()
    {
        BuildGridFromChildren();
        RecomputePower();
    }

    void BuildGridFromChildren()
    {
        if (gridParent == null)
        {
            Debug.LogError("PipeBoard: gridParent not assigned.");
            return;
        }

        int expected = rows * cols;
        if (gridParent.childCount < expected)
        {
            Debug.LogError($"PipeBoard: Expected {expected} children, found {gridParent.childCount}.");
            return;
        }

        grid = new PipeTile[rows, cols];
        sourceTile = null;
        targetTile = null;

        for (int i = 0; i < expected; i++)
        {
            int r = i / cols;
            int c = i % cols;

            Transform child = gridParent.GetChild(i);
            Debug.Log($"Processing child {i}: {child.name} at grid[{r},{c}]");
            PipeTile tile = child.GetComponent<PipeTile>();

            if (tile == null)
            {
                Debug.LogError($"PipeBoard: Child {i} ({child.name}) missing PipeTile.");
                continue;
            }

            grid[r, c] = tile;

            if (tile.pipeType == PipeTile.PipeType.Source) {sourceTile = tile; continue;}
            if (tile.pipeType == PipeTile.PipeType.Target) {targetTile = tile; continue;}
            
            // IMPORTANT: Button must be on the SAME GameObject as PipeTile
            Button btn = child.GetComponent<Button>();
            if (btn != null)
            {
                PipeTile localTile = tile; // ✅ closure fix
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    localTile.RotateCW();
                    RecomputePower();
                });
            }
        }

        if (sourceTile == null) Debug.LogWarning("PipeBoard: No Source tile found.");
        if (targetTile == null) Debug.LogWarning("PipeBoard: No Target tile found.");
    }

    void RecomputePower()
    {
        if (grid == null) return;
        transform.GetComponent<AudioSource>().Play(); // play sound on every recompute (can be moved to only rotate if desired)
        // clear
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                if (grid[r, c] != null)
                    grid[r, c].SetPoweredVisual(false);

        if (sourceTile == null) return;

        (int sr, int sc) = FindTile(sourceTile);
        if (sr < 0) return;

        var q = new Queue<(int r, int c)>();
        q.Enqueue((sr, sc));
        grid[sr, sc].SetPoweredVisual(true);

        while (q.Count > 0)
        {
            var (r, c) = q.Dequeue();
            bool[] openings = grid[r, c].GetOpenings();

            // Up, Right, Down, Left
            TryFlow(r, c, r - 1, c, 0, 2, openings, q);
            TryFlow(r, c, r, c + 1, 1, 3, openings, q);
            TryFlow(r, c, r + 1, c, 2, 0, openings, q);
            TryFlow(r, c, r, c - 1, 3, 1, openings, q);
        }

        if (targetTile != null && targetTile.powered)
            fuseBox.completePuzzle();
    }

    void TryFlow(
        int r1, int c1, int r2, int c2,
        int dirOut, int dirIn,
        bool[] openings,
        Queue<(int r, int c)> q)
    {
        if (!openings[dirOut]) return;
        if (r2 < 0 || r2 >= rows || c2 < 0 || c2 >= cols) return;

        PipeTile neighbor = grid[r2, c2];
        if (neighbor == null) return;
        if (neighbor.powered) return;

        bool[] nOpen = neighbor.GetOpenings();
        if (!nOpen[dirIn]) return; // must connect back

        neighbor.SetPoweredVisual(true);
        q.Enqueue((r2, c2));
    }

    (int r, int c) FindTile(PipeTile t)
    {
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                if (grid[r, c] == t) return (r, c);

        return (-1, -1);
    }
}