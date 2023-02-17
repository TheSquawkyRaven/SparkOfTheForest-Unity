using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainManager : MonoBehaviour
{

    public Grid Grid;
    public Grid ChunkGrid;
    public Tilemap Tilemap;

    public Tile GrassTile;

    public Transform TreesContainerTR;
    public GameObject TreeObj;

    public float treeSpawnLimit;
    public float perlinSize;    //Inverse
    public float renderViewportOffset;

    public HashSet<Vector2Int> SpawnedChunks = new();

    private void Awake()
    {
        SpawnedChunks.Clear();
    }

    public void CameraUpdate(Camera cam)
    {
        Vector2 minWorld = cam.ViewportToWorldPoint(new(-renderViewportOffset, -renderViewportOffset, 0));
        Vector2 maxWorld = cam.ViewportToWorldPoint(new(1 + renderViewportOffset, 1 + renderViewportOffset, 0));

        Vector3Int min = ChunkGrid.WorldToCell(minWorld);
        Vector3Int max = ChunkGrid.WorldToCell(maxWorld);

        SpawnChunksSeenByCamera(min, max);
    }

    private void SpawnChunksSeenByCamera(Vector3Int min, Vector3Int max)
    {
        for (int y = min.y; y <= max.y; y++)
        {
            for (int x = min.x; x <= max.x; x++)
            {
                SpawnChunk(new(x, y));
            }
        }
    }

    private void SpawnChunk(Vector3Int chunk)
    {
        if (SpawnedChunks.Contains((Vector2Int)chunk))
        {
            return;
        }
        SpawnedChunks.Add((Vector2Int)chunk);
        SpawnInChunk(chunk);
    }

    private void SpawnInChunk(Vector3Int chunk)
    {
        Vector3 min = ChunkGrid.CellToWorld(chunk);
        Vector3 max = min + ChunkGrid.cellSize;

        Vector3Int minSpawnCell = Grid.WorldToCell(min);
        Vector3Int maxSpawnCell = Grid.WorldToCell(max);

        for (int y = minSpawnCell.y; y <= maxSpawnCell.y; y++)
        {
            for (int x = minSpawnCell.x; x <= maxSpawnCell.x; x++)
            {
                Vector3Int cell = new(x, y);
                SpawnTreesInCell(cell);
                SpawnTile(cell);
            }
        }
    }

    private void SpawnTile(Vector3Int cell)
    {
        Tilemap.SetTile(cell, GrassTile);
    }

    private void SpawnTreesInCell(Vector3Int cell)
    {
        Vector2 min = Grid.CellToWorld(cell);
        bool spawnTree = Mathf.PerlinNoise(min.x / perlinSize, min.y / perlinSize) > treeSpawnLimit;
        if (spawnTree)
        {
            Vector2 max = min + (Vector2)Grid.cellSize;
            SpawnTreeIn(min, max);
        }
    }

    private void SpawnTreeIn(Vector2 min, Vector2 max)
    {
        Vector2 pos = new(Random.Range(min.x, max.x), Random.Range(min.y, max.y));

        Instantiate(TreeObj, pos, Quaternion.identity, TreesContainerTR);
    }

}
