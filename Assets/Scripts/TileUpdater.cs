public interface ITileUpdater
{
    void Run();
}

public abstract class TileUpdater : ITileUpdater
{
    protected Tile tile;

    public TileUpdater(Tile tile)
    {
        this.tile = tile;
    }

    public abstract void Run();
}

public class StartTileUpdater : TileUpdater
{
    public StartTileUpdater(Tile tile) : base(tile) {}

    public override void Run()
    {
        tile.SetColor(tile.Grid.StartColor);
    }
}

public class EndTileUpdater : TileUpdater
{
    public EndTileUpdater(Tile tile) : base(tile) {}

    public override void Run()
    {
        tile.SetColor(tile.Grid.EndColor);
    }
}

public class MarkPathTileUpdater : TileUpdater
{
    public MarkPathTileUpdater(Tile tile) : base(tile) {}

    public override void Run()
    {
        tile.SetColor(tile.Grid.PathColor);
    }
}

public class NeighborTileUpdater : TileUpdater
{
    private int cost;

    public NeighborTileUpdater(Tile tile, int cost) : base(tile)
    {
        this.cost = cost;
    }

    public override void Run()
    {
        tile.SetColor(tile.Grid.NeighborColor);
        tile.SetText(cost != 0 ? $"{cost}" : string.Empty);
    }
}

public class VisitedTileUpdater : TileUpdater
{
    public VisitedTileUpdater(Tile tile) : base(tile) {}

    public override void Run()
    {
        tile.SetColor(tile.Grid.VisitedColor);
    }
}