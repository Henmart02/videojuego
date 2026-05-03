using UnityEngine;

public enum PieceType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}

public class Piece : MonoBehaviour
{
    public PieceType pieceType;
    public bool isWhite;
    public int boardX;
    public int boardY;
    public bool hasMoved = false;
    public BoardManager boardManager;

    public void SetData(PieceType type, bool white, int x, int y, Sprite sprite)
    {
        pieceType = type;
        isWhite = white;
        boardX = x;
        boardY = y;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = sprite;
            sr.color = Color.white;
        }

        gameObject.name = (isWhite ? "White_" : "Black_") + pieceType + "_" + x + "_" + y;
    }

    public void SetBoardPosition(int x, int y)
    {
        boardX = x;
        boardY = y;
    }
}