using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Grid : MonoBehaviour
{
    [SerializeField] 
    int Rows;
    [SerializeField] 
    int Columns;
    [SerializeField] 
    Tile Prefab;

    public Color DefaultColor = Color.white;
    public Color ExpensiveColor = Color.red;
    public Color InfinityColor = Color.black;
    public Color StartColor = Color.blue;
    public Color EndColor = Color.magenta;
    public Color PathColor = new Color(0.5f, 0.0f, 1.0f);
    public Color VisitedColor = new Color(0.75f, 0.55f, 0.38f);
    public Color NeighborColor = Color.yellow;
    
    public const int Weight = 1;
    public const int Expense = 50;
    public const int RandomExpensiveValue = 20;
    public const int Infinity = int.MaxValue;
    public Vector2Int Start = Vector2Int.zero;
    public Vector2Int End = Vector2Int.zero;
    public Tile[] Tiles { get; private set; }
    private IEnumerator runningPath;

    private void Awake()
    {
        Tiles = new Tile[Rows * Columns];
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                var tile = Instantiate(Prefab, transform);
                tile.Init(this, i, j, Weight);
                var index = GetTileIndex(i, j);
                Tiles[index] = tile;

            }
        }
        CreateRandomExpensiveArea();
        ResetGrid();
    }

    private void Update()
    {
        var start = GetTile(Start.x, Start.y);
        var end = GetTile(End.x, End.y);
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StopPathFinding();
            runningPath = LookForPath(start, end, PathFinder.BFS);
            StartCoroutine(runningPath);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StopPathFinding();
            runningPath = LookForPath(start, end, PathFinder.Dijkstra);
            StartCoroutine(runningPath);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StopPathFinding();
            runningPath = LookForPath(start, end, PathFinder.AStar);
            StartCoroutine(runningPath);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StopPathFinding();
            runningPath = LookForPath(start, end, PathFinder.Greedy);
            StartCoroutine(runningPath);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopPathFinding();
            ResetGrid();
            start.SetColor(StartColor);
            end.SetColor(EndColor);
        }
    }

    private void StopPathFinding()
    {
        if (runningPath == null)
            return;
        StopCoroutine(runningPath);
        runningPath = null;
    }
    private void CreateRandomExpensiveArea()
    {
        ResetGrid();
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (UnityEngine.Random.Range(0, 100) < RandomExpensiveValue)
                {
                    var tile = GetTile(i, j);
                    if (tile != null)
                        tile.Weight = Expense;
                }
            }
        }
    }
    
    
    private void ResetGrid()
    {
        foreach (var tile in Tiles)
        {
            tile.Cost = 0;
            tile.Previous = null;
            tile.SetText(string.Empty);
            var weight = tile.Weight;
            switch (weight)
            {
                case Weight:
                    tile.SetColor(DefaultColor);
                    break;
                case Expense:
                    tile.SetColor(ExpensiveColor);
                    break;
                case Infinity:
                    tile.SetColor(InfinityColor);
                    break;
            }
        }
        GetTile(Start.x, Start.y).SetColor(StartColor);
        GetTile(End.x, End.y).SetColor(EndColor);
    }

    private IEnumerator LookForPath(Tile start, Tile end, Func<Grid, Tile, Tile, List<TileUpdater>, List<Tile>> func)
    {
        ResetGrid();
        var steps = new List<TileUpdater>();
        func(this, start, end, steps);
        foreach (var step in steps)
        {
            step.Run();
            yield return new WaitForFixedUpdate();
        }
    }

    public Tile GetTile(int row, int column)
    {
        if (!IsValid(row, column))
            return null;
        return Tiles[GetTileIndex(row, column)];
    }
    
    public IEnumerable<Tile> GetNeighbors (Tile tile)
    {
        var right = GetTile(tile.Row, tile.Column + 1);
        if (right != null)
            yield return right;
        var left = GetTile(tile.Row, tile.Column - 1);
        if (left != null)
            yield return left;
        var up = GetTile(tile.Row - 1, tile.Column);
        if (up != null)
            yield return up;
        var down = GetTile(tile.Row + 1, tile.Column);
        if (down != null)
            yield return down;
    }

    private bool IsValid(int row, int column)
    {
        return row >= 0 &&
               row < Rows &&
               column >= 0 &&
               column < Columns;
    }

    private int GetTileIndex(int row, int column)
    {
        return row * Columns + column;
    }
}