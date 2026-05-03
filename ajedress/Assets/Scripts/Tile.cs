using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;
    public BoardManager boardManager;

    private SpriteRenderer sr;
    private Color baseColor;

    public void SetCoordinates(int boardX, int boardY)
    {
        x = boardX;
        y = boardY;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetBaseColor(Color color)
    {
        baseColor = color;

        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        sr.color = baseColor;
    }

    public void ResetColor()
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        sr.color = baseColor;
    }

    public void HighlightRed()
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        sr.color = Color.red;
    }
}