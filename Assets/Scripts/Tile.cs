using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer Renderer;
    [SerializeField]
    private Text Text;
    
    public int Row { get; private set; }
    public int Column { get; private set; }
    public int Weight;
    public int Cost;
    public Tile Previous;
    public Grid Grid { get; private set; }

    private Vector2 location = Vector2.zero;

    public void Init(Grid grid, int row, int column, int weight)
    {
        Grid = grid;
        Row = row;
        Column = column;
        Weight = weight;
        location = new Vector2(column, row);
        name = $"Tile ({Row},{Column})";
        transform.position = new Vector3(Column, Row, 0.0f);
    }

    public void SetColor(Color color)
    {
        Renderer.color = color;
    }

    public void SetText(string text)
    {
        this.Text.text = text;
    }

    public Vector2 GetLocation()
    {
        return location;
    }
}
