using System;
using System.Collections.Generic;

public class PathFinder
{
    public static List<Tile> BFS(Grid grid, Tile start, Tile end, List<TileUpdater> steps)
    {
        steps.Add(new StartTileUpdater(start));
        steps.Add(new EndTileUpdater(end));
        var visited = new HashSet<Tile>();
        visited.Add(start);
        var surroundingArea = new Queue<Tile>();
        surroundingArea.Enqueue(start);
        start.Previous = null;
        while (surroundingArea.Count > 0)
        {
            var current = surroundingArea.Dequeue();
            if (current != start && current != end)
                steps.Add(new VisitedTileUpdater(current));
            if (current == end)
                break;
            foreach (var neighbor in grid.GetNeighbors(current))
            {
                if (visited.Contains(neighbor))
                    continue;
                visited.Add(neighbor);
                surroundingArea.Enqueue(neighbor);
                neighbor.Previous = current;
                if (neighbor != end)
                    steps.Add(new NeighborTileUpdater(neighbor, 0));
            }
        }

        var path = BacktrackToPath(end);
        foreach (var tile in path)
        {
            if (tile == start || tile == end)
                continue;
            steps.Add(new MarkPathTileUpdater(tile));
        }
        return path;
    }

    public static List<Tile> Dijkstra(Grid grid, Tile start, Tile end, List<TileUpdater> steps)
    {
        steps.Add(new StartTileUpdater(start));
        steps.Add(new EndTileUpdater(end));
        foreach (var tile in grid.Tiles)
            tile.Cost = Grid.Infinity;
        start.Cost = 0;
        var visited = new HashSet<Tile>();
        visited.Add(start);
        var surroundingArea = new MinHeap<Tile>((a,b) => a.Cost.CompareTo(b.Cost));
        surroundingArea.Add(start);
        start.Previous = null;
        while (surroundingArea.Count > 0)
        {
            var current = surroundingArea.Remove();
            if (current != start && current != end)
                steps.Add(new VisitedTileUpdater(current));
            if (current == end)
                break;
            foreach (var neighbor in grid.GetNeighbors(current))
            {
                int neighborCost = current.Cost + neighbor.Weight;
                if (neighborCost < neighbor.Cost)
                {
                    neighbor.Cost = neighborCost;
                    neighbor.Previous = current;
                }
                if (visited.Contains(neighbor))
                    continue;
                visited.Add(neighbor);
                surroundingArea.Add(neighbor);
                if (neighbor != end)
                    steps.Add((new NeighborTileUpdater(neighbor, neighbor.Cost)));
            }
        }
        var path = BacktrackToPath(end);
        foreach (var tile in path)
        {
            if (tile == start || tile == end)
                continue;
            steps.Add(new MarkPathTileUpdater(tile));
        }
        return path;
    }
    
    public static List<Tile> AStar(Grid grid, Tile start, Tile end, List<TileUpdater> steps)
        {
            steps.Add(new StartTileUpdater(start));
            steps.Add(new EndTileUpdater(end));
            foreach (var tile in grid.Tiles)
                tile.Cost = int.MaxValue;
            start.Cost = 0;
            Comparison<Tile> comparison = (a, b) =>
            {
                float aCost = a.Cost + GetLocationCost(a, end);
                float bCost = b.Cost + GetLocationCost(b, end);
                return aCost.CompareTo(bCost);
            };
            var surroundingArea = new MinHeap<Tile>(comparison);
            surroundingArea.Add(start);
            var visited = new HashSet<Tile>();
            visited.Add(start);
            start.Previous = null;
            while (surroundingArea.Count > 0)
            {
                var current = surroundingArea.Remove();
                if (current != start && current != end)
                    steps.Add(new VisitedTileUpdater(current));

                if (current == end)
                    break;
                foreach (var neighbor in grid.GetNeighbors(current))
                {
                    int neighborCost = current.Cost + neighbor.Weight;
                    if (neighborCost < neighbor.Cost)
                    {
                        neighbor.Cost = neighborCost;
                        neighbor.Previous = current;
                    }
                    if (visited.Contains(neighbor))
                        continue;
                    surroundingArea.Add(neighbor);
                    visited.Add(neighbor);

                    if (neighbor != end)
                        steps.Add(new NeighborTileUpdater(neighbor, neighbor.Cost));
                }
            }
            var path = BacktrackToPath(end);
            foreach (var tile in path)
            {
                if (tile == start || tile == end)
                    continue;
                steps.Add(new MarkPathTileUpdater(tile));
            }
            return path;
        }

        public static List<Tile> Greedy(Grid grid, Tile start, Tile end, List<TileUpdater> outSteps)
        {
            outSteps.Add(new StartTileUpdater(start));
            outSteps.Add(new EndTileUpdater(end));
            Comparison<Tile> comparison = (a, b) =>
            {
                float aCost = GetLocationCost(a, b);
                float bCost = GetLocationCost(b, b);

                return aCost.CompareTo(bCost);
            };
            var surroundingArea = new MinHeap<Tile>(comparison);
            surroundingArea.Add(start);
            var visited = new HashSet<Tile>();
            visited.Add(start);
            start.Previous = null;
            while (surroundingArea.Count > 0)
            {
                var current = surroundingArea.Remove();
                if (current != start && current != end)
                    outSteps.Add(new VisitedTileUpdater(current));
                if (current == end)
                    break;
                foreach (var neighbor in grid.GetNeighbors(current))
                {
                    if (visited.Contains(neighbor))
                        continue;
                    surroundingArea.Add(neighbor);
                    visited.Add(neighbor);
                    neighbor.Previous = current;
                    if (neighbor != end)
                        outSteps.Add(new NeighborTileUpdater(neighbor, 0));
                }
            }
            var path = BacktrackToPath(end);
            foreach (var tile in path)
            {
                if (tile == start || tile == end)
                    continue;
                outSteps.Add(new MarkPathTileUpdater(tile));
            }
            return path;
        }

        private static float GetLocationCost(Tile current, Tile end)
        {
            return (current.GetLocation() - end.GetLocation()).magnitude;
        }
    
    private static List<Tile> BacktrackToPath(Tile end)
    {
        var current = end;
        var path = new List<Tile>();
        while (current != null)
        {
            path.Add(current);
            current = current.Previous;
        }
        path.Reverse();
        return path;
    }
}
