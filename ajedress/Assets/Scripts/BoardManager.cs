using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject piecePrefab;

    [Header("White Pieces")]
    public Sprite whitePawn;
    public Sprite whiteRook;
    public Sprite whiteKnight;
    public Sprite whiteBishop;
    public Sprite whiteQueen;
    public Sprite whiteKing;

    [Header("Black Pieces")]
    public Sprite blackPawn;
    public Sprite blackRook;
    public Sprite blackKnight;
    public Sprite blackBishop;
    public Sprite blackQueen;
    public Sprite blackKing;

    public int width = 8;
    public int height = 8;
    public bool whiteTurn = true;

    [Header("Promotion UI")]
public GameObject promotionPanel;

[HideInInspector] public Piece promotionPawn;
[HideInInspector] public bool waitingForPromotion = false;

    private Piece[,] pieceGrid = new Piece[8, 8];
    private Tile[,] tileGrid = new Tile[8, 8];

    // En passant
    private Vector2Int? enPassantTarget = null;
    private Piece enPassantPawn = null;

    private void Start()
    {
        GenerateBoard();
        SpawnPieces();
    }

    void GenerateBoard()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = BoardToWorldPosition(x, y, 0);

                GameObject tileObj = Instantiate(tilePrefab, position, Quaternion.identity);
                tileObj.name = "Tile_" + x + "_" + y;
                tileObj.transform.SetParent(transform);
/*
                SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    if ((x + y) % 2 == 0)
                        sr.color = new Color(0.95f, 0.85f, 0.7f);
                    else
                        sr.color = new Color(0.45f, 0.3f, 0.2f);
                }*/

                Tile tile = tileObj.GetComponent<Tile>();
if (tile != null)
{
    tile.SetCoordinates(x, y);
    tile.boardManager = this;

    Color tileColor;
    if ((x + y) % 2 == 0)
        tileColor = new Color(0.95f, 0.85f, 0.7f);
    else
        tileColor = new Color(0.45f, 0.3f, 0.2f);

    tile.SetBaseColor(tileColor);
    tileGrid[x, y] = tile;
}
            }
        }
    }
/*
    public Vector3 BoardToWorldPosition(int x, int y, float z = -1f)
    {
        float offsetX = -(width / 2f) + 0.5f;
        float offsetY = -(height / 2f) + 0.5f;
        return new Vector3(x + offsetX, y + offsetY, z);
    }*/
    public Vector3 BoardToWorldPosition(int x, int y, float z = -1f)
{
    float offsetX = -(width / 2f) + 0.5f;
    float offsetY = -(height / 2f) + 0.5f;

    return transform.position + new Vector3(x + offsetX, y + offsetY, z);
}

    void SpawnPieces()
    {
        // Blancas
        CreatePiece(PieceType.Rook, true, 0, 0);
        CreatePiece(PieceType.Knight, true, 1, 0);
        CreatePiece(PieceType.Bishop, true, 2, 0);
        CreatePiece(PieceType.Queen, true, 3, 0);
        CreatePiece(PieceType.King, true, 4, 0);
        CreatePiece(PieceType.Bishop, true, 5, 0);
        CreatePiece(PieceType.Knight, true, 6, 0);
        CreatePiece(PieceType.Rook, true, 7, 0);

        for (int x = 0; x < 8; x++)
            CreatePiece(PieceType.Pawn, true, x, 1);

        // Negras
        CreatePiece(PieceType.Rook, false, 0, 7);
        CreatePiece(PieceType.Knight, false, 1, 7);
        CreatePiece(PieceType.Bishop, false, 2, 7);
        CreatePiece(PieceType.Queen, false, 3, 7);
        CreatePiece(PieceType.King, false, 4, 7);
        CreatePiece(PieceType.Bishop, false, 5, 7);
        CreatePiece(PieceType.Knight, false, 6, 7);
        CreatePiece(PieceType.Rook, false, 7, 7);

        for (int x = 0; x < 8; x++)
            CreatePiece(PieceType.Pawn, false, x, 6);
    }

    void CreatePiece(PieceType type, bool isWhite, int x, int y)
    {
        Vector3 position = BoardToWorldPosition(x, y, -1f);

        GameObject pieceObj = Instantiate(piecePrefab, position, Quaternion.identity);
        pieceObj.transform.SetParent(transform);
        pieceObj.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
/*
        Piece piece = pieceObj.GetComponent<Piece>();
        if (piece != null)
        {
            piece.SetData(type, isWhite, x, y, GetPromotionSprite(type, isWhite));
            pieceGrid[x, y] = piece;
        }*/
        Piece piece = pieceObj.GetComponent<Piece>();
if (piece != null)
{
    piece.boardManager = this;
    piece.SetData(type, isWhite, x, y, GetPromotionSprite(type, isWhite));
    pieceGrid[x, y] = piece;
}
    }

    public Sprite GetPromotionSprite(PieceType type, bool isWhite)
{
    if (isWhite)
    {
        switch (type)
        {
            case PieceType.Pawn: return whitePawn;
            case PieceType.Rook: return whiteRook;
            case PieceType.Knight: return whiteKnight;
            case PieceType.Bishop: return whiteBishop;
            case PieceType.Queen: return whiteQueen;
            case PieceType.King: return whiteKing;
        }
    }
    else
    {
        switch (type)
        {
            case PieceType.Pawn: return blackPawn;
            case PieceType.Rook: return blackRook;
            case PieceType.Knight: return blackKnight;
            case PieceType.Bishop: return blackBishop;
            case PieceType.Queen: return blackQueen;
            case PieceType.King: return blackKing;
        }
    }
    return null;
}

    public bool IsInsideBoard(int x, int y)
    {
        return x >= 0 && x < 8 && y >= 0 && y < 8;
    }

    public Piece GetPieceAt(int x, int y)
    {
        if (!IsInsideBoard(x, y)) return null;
        return pieceGrid[x, y];
    }

    public bool TryMovePiece(Piece piece, int newX, int newY)
{
    if (piece == null)
    {
        Debug.Log("TryMovePiece: pieza nula");
        return false;
    }

    if (!IsInsideBoard(newX, newY))
    {
        Debug.Log("TryMovePiece: fuera del tablero");
        return false;
    }

    if (piece.isWhite != whiteTurn)
    {
        Debug.Log("TryMovePiece: no es el turno de esa pieza");
        return false;
    }

    Debug.Log("Intentando mover " + piece.name + " a " + newX + "," + newY);

    List<Vector2Int> legalMoves = GetLegalMoves(piece);

    Debug.Log("Movimientos legales encontrados: " + legalMoves.Count);
    foreach (Vector2Int move in legalMoves)
    {
        Debug.Log("Legal: " + move.x + "," + move.y);
    }

    bool found = false;

    foreach (Vector2Int move in legalMoves)
    {
        if (move.x == newX && move.y == newY)
        {
            found = true;
            break;
        }
    }

    if (!found)
    {
        Debug.Log("Ese movimiento no está en la lista legal.");
        return false;
    }

    ExecuteMove(piece, newX, newY, true);

UpdateCheckHighlight();

if (!waitingForPromotion)
    whiteTurn = !whiteTurn;

Debug.Log("Ahora es turno de: " + (whiteTurn ? "Blancas" : "Negras"));
return true;
}

    public List<Vector2Int> GetLegalMoves(Piece piece)
    {
        List<Vector2Int> rawMoves = GetPseudoLegalMoves(piece);
        List<Vector2Int> legalMoves = new List<Vector2Int>();

        foreach (Vector2Int move in rawMoves)
        {
            if (!WouldLeaveKingInCheck(piece, move.x, move.y))
                legalMoves.Add(move);
        }

        return legalMoves;
    }

    List<Vector2Int> GetPseudoLegalMoves(Piece piece)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        int x = piece.boardX;
        int y = piece.boardY;

        switch (piece.pieceType)
        {
            case PieceType.Pawn:
                AddPawnMoves(piece, moves);
                break;

            case PieceType.Rook:
                AddLineMoves(piece, moves, new Vector2Int(1, 0));
                AddLineMoves(piece, moves, new Vector2Int(-1, 0));
                AddLineMoves(piece, moves, new Vector2Int(0, 1));
                AddLineMoves(piece, moves, new Vector2Int(0, -1));
                break;

            case PieceType.Bishop:
                AddLineMoves(piece, moves, new Vector2Int(1, 1));
                AddLineMoves(piece, moves, new Vector2Int(1, -1));
                AddLineMoves(piece, moves, new Vector2Int(-1, 1));
                AddLineMoves(piece, moves, new Vector2Int(-1, -1));
                break;

            case PieceType.Queen:
                AddLineMoves(piece, moves, new Vector2Int(1, 0));
                AddLineMoves(piece, moves, new Vector2Int(-1, 0));
                AddLineMoves(piece, moves, new Vector2Int(0, 1));
                AddLineMoves(piece, moves, new Vector2Int(0, -1));
                AddLineMoves(piece, moves, new Vector2Int(1, 1));
                AddLineMoves(piece, moves, new Vector2Int(1, -1));
                AddLineMoves(piece, moves, new Vector2Int(-1, 1));
                AddLineMoves(piece, moves, new Vector2Int(-1, -1));
                break;

            case PieceType.Knight:
                AddKnightMoves(piece, moves);
                break;

            case PieceType.King:
                AddKingMoves(piece, moves);
                AddCastlingMoves(piece, moves);
                break;
        }

        return moves;
    }

    void AddPawnMoves(Piece piece, List<Vector2Int> moves)
    {
        int dir = piece.isWhite ? 1 : -1;
        int startRow = piece.isWhite ? 1 : 6;

        int x = piece.boardX;
        int y = piece.boardY;

        // avance 1
        if (IsInsideBoard(x, y + dir) && GetPieceAt(x, y + dir) == null)
        {
            moves.Add(new Vector2Int(x, y + dir));

            // avance 2
            if (y == startRow && GetPieceAt(x, y + 2 * dir) == null)
                moves.Add(new Vector2Int(x, y + 2 * dir));
        }

        // capturas diagonales
        int[] dx = { -1, 1 };
        foreach (int d in dx)
        {
            int nx = x + d;
            int ny = y + dir;

            if (!IsInsideBoard(nx, ny)) continue;

            Piece target = GetPieceAt(nx, ny);
            if (target != null && target.isWhite != piece.isWhite)
                moves.Add(new Vector2Int(nx, ny));
        }

        // en passant
        if (enPassantTarget.HasValue)
        {
            Vector2Int ep = enPassantTarget.Value;
            if (ep.y == y + dir && Mathf.Abs(ep.x - x) == 1)
                moves.Add(ep);
        }
    }

    void AddKnightMoves(Piece piece, List<Vector2Int> moves)
    {
        int[,] offsets = {
            { 1, 2 }, { 2, 1 }, { -1, 2 }, { -2, 1 },
            { 1, -2 }, { 2, -1 }, { -1, -2 }, { -2, -1 }
        };

        for (int i = 0; i < 8; i++)
        {
            int nx = piece.boardX + offsets[i, 0];
            int ny = piece.boardY + offsets[i, 1];

            if (!IsInsideBoard(nx, ny)) continue;

            Piece target = GetPieceAt(nx, ny);
            if (target == null || target.isWhite != piece.isWhite)
                moves.Add(new Vector2Int(nx, ny));
        }
    }

    void AddKingMoves(Piece piece, List<Vector2Int> moves)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int nx = piece.boardX + dx;
                int ny = piece.boardY + dy;

                if (!IsInsideBoard(nx, ny)) continue;

                Piece target = GetPieceAt(nx, ny);
                if (target == null || target.isWhite != piece.isWhite)
                    moves.Add(new Vector2Int(nx, ny));
            }
        }
    }

    void AddLineMoves(Piece piece, List<Vector2Int> moves, Vector2Int dir)
    {
        int x = piece.boardX + dir.x;
        int y = piece.boardY + dir.y;

        while (IsInsideBoard(x, y))
        {
            Piece target = GetPieceAt(x, y);

            if (target == null)
            {
                moves.Add(new Vector2Int(x, y));
            }
            else
            {
                if (target.isWhite != piece.isWhite)
                    moves.Add(new Vector2Int(x, y));
                break;
            }

            x += dir.x;
            y += dir.y;
        }
    }

    void AddCastlingMoves(Piece king, List<Vector2Int> moves)
    {
        if (king.hasMoved) return;
        if (IsKingInCheck(king.isWhite)) return;

        int row = king.isWhite ? 0 : 7;

        // enroque corto
        Piece rookShort = GetPieceAt(7, row);
        if (rookShort != null &&
            rookShort.pieceType == PieceType.Rook &&
            rookShort.isWhite == king.isWhite &&
            !rookShort.hasMoved &&
            GetPieceAt(5, row) == null &&
            GetPieceAt(6, row) == null &&
            !IsSquareUnderAttack(5, row, !king.isWhite) &&
            !IsSquareUnderAttack(6, row, !king.isWhite))
        {
            moves.Add(new Vector2Int(6, row));
        }

        // enroque largo
        Piece rookLong = GetPieceAt(0, row);
        if (rookLong != null &&
            rookLong.pieceType == PieceType.Rook &&
            rookLong.isWhite == king.isWhite &&
            !rookLong.hasMoved &&
            GetPieceAt(1, row) == null &&
            GetPieceAt(2, row) == null &&
            GetPieceAt(3, row) == null &&
            !IsSquareUnderAttack(2, row, !king.isWhite) &&
            !IsSquareUnderAttack(3, row, !king.isWhite))
        {
            moves.Add(new Vector2Int(2, row));
        }
    }

    bool WouldLeaveKingInCheck(Piece piece, int newX, int newY)
    {
        int oldX = piece.boardX;
        int oldY = piece.boardY;

        Piece captured = pieceGrid[newX, newY];
        Piece epCaptured = null;

        bool isEnPassant = piece.pieceType == PieceType.Pawn &&
                           enPassantTarget.HasValue &&
                           enPassantTarget.Value.x == newX &&
                           enPassantTarget.Value.y == newY &&
                           captured == null;

        // simular
        pieceGrid[oldX, oldY] = null;

        if (isEnPassant && enPassantPawn != null)
        {
            epCaptured = enPassantPawn;
            pieceGrid[epCaptured.boardX, epCaptured.boardY] = null;
        }

        pieceGrid[newX, newY] = piece;
        piece.SetBoardPosition(newX, newY);

        bool inCheck = IsKingInCheck(piece.isWhite);

        // deshacer
        pieceGrid[oldX, oldY] = piece;
        pieceGrid[newX, newY] = captured;
        piece.SetBoardPosition(oldX, oldY);

        if (epCaptured != null)
        {
            pieceGrid[epCaptured.boardX, epCaptured.boardY] = epCaptured;
        }

        return inCheck;
    }

    public bool IsKingInCheck(bool whiteKing)
    {
        Piece king = FindKing(whiteKing);
        if (king == null) return false;

        return IsSquareUnderAttack(king.boardX, king.boardY, !whiteKing);
    }

    Piece FindKing(bool isWhite)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Piece p = pieceGrid[x, y];
                if (p != null && p.isWhite == isWhite && p.pieceType == PieceType.King)
                    return p;
            }
        }
        return null;
    }

    public bool IsSquareUnderAttack(int x, int y, bool byWhite)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece p = pieceGrid[i, j];
                if (p == null || p.isWhite != byWhite) continue;

                if (AttacksSquare(p, x, y))
                    return true;
            }
        }
        return false;
    }

    bool AttacksSquare(Piece piece, int targetX, int targetY)
    {
        int x = piece.boardX;
        int y = piece.boardY;

        switch (piece.pieceType)
        {
            case PieceType.Pawn:
                int dir = piece.isWhite ? 1 : -1;
                return (targetY == y + dir) && (targetX == x + 1 || targetX == x - 1);

            case PieceType.Knight:
                int dx = Mathf.Abs(targetX - x);
                int dy = Mathf.Abs(targetY - y);
                return (dx == 1 && dy == 2) || (dx == 2 && dy == 1);

            case PieceType.Bishop:
                if (Mathf.Abs(targetX - x) != Mathf.Abs(targetY - y)) return false;
                return IsPathClear(x, y, targetX, targetY);

            case PieceType.Rook:
                if (targetX != x && targetY != y) return false;
                return IsPathClear(x, y, targetX, targetY);

            case PieceType.Queen:
                bool straight = (targetX == x || targetY == y);
                bool diagonal = Mathf.Abs(targetX - x) == Mathf.Abs(targetY - y);
                if (!straight && !diagonal) return false;
                return IsPathClear(x, y, targetX, targetY);

            case PieceType.King:
                return Mathf.Abs(targetX - x) <= 1 && Mathf.Abs(targetY - y) <= 1;
        }

        return false;
    }

    bool IsPathClear(int startX, int startY, int endX, int endY)
    {
        int dx = System.Math.Sign(endX - startX);
        int dy = System.Math.Sign(endY - startY);

        int x = startX + dx;
        int y = startY + dy;

        while (x != endX || y != endY)
        {
            if (pieceGrid[x, y] != null)
                return false;

            x += dx;
            y += dy;
        }

        return true;
    }

    void ExecuteMove(Piece piece, int newX, int newY, bool realMove)
    {
        int oldX = piece.boardX;
        int oldY = piece.boardY;

        bool isEnPassantMove = piece.pieceType == PieceType.Pawn &&
                               enPassantTarget.HasValue &&
                               enPassantTarget.Value.x == newX &&
                               enPassantTarget.Value.y == newY &&
                               pieceGrid[newX, newY] == null;

        // captura normal
if (pieceGrid[newX, newY] != null)
{
    Piece capturedPiece = pieceGrid[newX, newY];

    if (BughouseManager.Instance != null)
    {
        BughouseManager.Instance.AddCapturedPieceForPartner(this, piece.isWhite, capturedPiece.pieceType);
    }

    Destroy(capturedPiece.gameObject);
}

        // captura en passant
if (isEnPassantMove && enPassantPawn != null)
{
    if (BughouseManager.Instance != null)
    {
        BughouseManager.Instance.AddCapturedPieceForPartner(this, piece.isWhite, enPassantPawn.pieceType);
    }

    pieceGrid[enPassantPawn.boardX, enPassantPawn.boardY] = null;
    Destroy(enPassantPawn.gameObject);
}

        pieceGrid[oldX, oldY] = null;
        pieceGrid[newX, newY] = piece;

        piece.SetBoardPosition(newX, newY);
        piece.transform.position = BoardToWorldPosition(newX, newY, -1f);

        // enroque
        if (piece.pieceType == PieceType.King && Mathf.Abs(newX - oldX) == 2)
        {
            int row = piece.isWhite ? 0 : 7;

            // corto
            if (newX == 6)
            {
                Piece rook = pieceGrid[7, row];
                if (rook != null)
                {
                    pieceGrid[7, row] = null;
                    pieceGrid[5, row] = rook;
                    rook.SetBoardPosition(5, row);
                    rook.transform.position = BoardToWorldPosition(5, row, -1f);
                    rook.hasMoved = true;
                }
            }
            // largo
            else if (newX == 2)
            {
                Piece rook = pieceGrid[0, row];
                if (rook != null)
                {
                    pieceGrid[0, row] = null;
                    pieceGrid[3, row] = rook;
                    rook.SetBoardPosition(3, row);
                    rook.transform.position = BoardToWorldPosition(3, row, -1f);
                    rook.hasMoved = true;
                }
            }
        }

        // actualizar en passant
        enPassantTarget = null;
        enPassantPawn = null;

        if (piece.pieceType == PieceType.Pawn && Mathf.Abs(newY - oldY) == 2)
        {
            int middleY = (newY + oldY) / 2;
            enPassantTarget = new Vector2Int(newX, middleY);
            enPassantPawn = piece;
        }

        // promoción de peón
if (piece.pieceType == PieceType.Pawn)
{
    if ((piece.isWhite && newY == 7) || (!piece.isWhite && newY == 0))
    {
        promotionPawn = piece;
        waitingForPromotion = true;

    if (promotionPanel != null)
{
    PromotionUI promotionUI = promotionPanel.GetComponent<PromotionUI>();
    if (promotionUI != null)
    {
        promotionUI.boardManager = this;
        promotionUI.UpdateButtonIcons();
        promotionUI.MovePanelToBoard();
    }

    promotionPanel.SetActive(true);
}
    }
}

        piece.hasMoved = true;
    }

    public bool IsCheckmate(bool sideToMoveIsWhite)
    {
        if (!IsKingInCheck(sideToMoveIsWhite))
            return false;

        return !HasAnyLegalMove(sideToMoveIsWhite);
    }

    public bool IsStalemate(bool sideToMoveIsWhite)
    {
        if (IsKingInCheck(sideToMoveIsWhite))
            return false;

        return !HasAnyLegalMove(sideToMoveIsWhite);
    }

    bool HasAnyLegalMove(bool isWhite)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Piece p = pieceGrid[x, y];
                if (p == null || p.isWhite != isWhite) continue;

                if (GetLegalMoves(p).Count > 0)
                    return true;
            }
        }
        return false;
    }
    public void PromotePawn(PieceType newType)
{
    Debug.Log("PromotePawn llamado con: " + newType);

    if (promotionPawn == null) return;

    promotionPawn.pieceType = newType;

    SpriteRenderer sr = promotionPawn.GetComponent<SpriteRenderer>();
    if (sr != null)
        sr.sprite = GetPromotionSprite(newType, promotionPawn.isWhite);

    promotionPawn.gameObject.name =
        (promotionPawn.isWhite ? "White_" : "Black_") +
        promotionPawn.pieceType + "_" +
        promotionPawn.boardX + "_" +
        promotionPawn.boardY;

    promotionPawn = null;
    waitingForPromotion = false;

    if (promotionPanel != null)
        promotionPanel.SetActive(false);

    UpdateCheckHighlight();

    whiteTurn = !whiteTurn;
}
public void UpdateCheckHighlight()
{
    // Restaurar colores normales de todas las casillas
    for (int x = 0; x < 8; x++)
    {
        for (int y = 0; y < 8; y++)
        {
            if (tileGrid[x, y] != null)
                tileGrid[x, y].ResetColor();
        }
    }

    HighlightKingIfInCheck(true);   // rey blanco
    HighlightKingIfInCheck(false);  // rey negro
}

private void HighlightKingIfInCheck(bool isWhiteKing)
{
    if (!IsKingInCheck(isWhiteKing))
        return;

    Piece king = FindKing(isWhiteKing);
    if (king != null && tileGrid[king.boardX, king.boardY] != null)
    {
        tileGrid[king.boardX, king.boardY].HighlightRed();
    }
}

public bool TryDropPocketPiece(bool isWhite, PieceType pieceType, int x, int y)
{
    Debug.Log("Intentando DROP en " + name + " color " + (isWhite ? "blancas" : "negras") + " pieza " + pieceType + " en " + x + "," + y);

    if (!IsInsideBoard(x, y))
    {
        Debug.Log("DROP rechazado: fuera del tablero");
        return false;
    }

    if (pieceGrid[x, y] != null)
    {
        Debug.Log("DROP rechazado: la casilla está ocupada");
        return false;
    }

    if (whiteTurn != isWhite)
    {
        Debug.Log("DROP rechazado: no coincide con el turno. Turno actual = " + (whiteTurn ? "blancas" : "negras"));
        return false;
    }

    if (pieceType == PieceType.Pawn && (y == 0 || y == 7))
    {
        Debug.Log("DROP rechazado: un peón no puede ponerse en fila 1 ni 8");
        return false;
    }

    Vector3 position = BoardToWorldPosition(x, y, -1f);

    GameObject pieceObj = Instantiate(piecePrefab, position, Quaternion.identity);
    pieceObj.transform.SetParent(transform);
    pieceObj.transform.localScale = new Vector3(0.3f, 0.3f, 1f);

    Piece piece = pieceObj.GetComponent<Piece>();
    if (piece == null)
    {
        Debug.Log("DROP rechazado: pieceObj no tiene componente Piece");
        return false;
    }

    piece.boardManager = this;
    piece.SetData(pieceType, isWhite, x, y, GetPromotionSprite(pieceType, isWhite));
    piece.hasMoved = true;

    pieceGrid[x, y] = piece;

    UpdateCheckHighlight();
    whiteTurn = !whiteTurn;

    Debug.Log("DROP realizado correctamente");
    return true;
}


}