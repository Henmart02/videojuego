using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool pocketSelectionActive = false;
private BoardManager selectedPocketBoard;
private bool selectedPocketColor;
private PieceType selectedPocketPieceType;

    private Piece selectedPiece;
    private Camera mainCamera;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        RefreshAllPocketButtons();
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        if (mainCamera == null)
            return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(screenPos);
        Vector2 mousePos2D = new Vector2(mouseWorld.x, mouseWorld.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider == null)
            return;

        Piece clickedPiece = hit.collider.GetComponent<Piece>();
        if (clickedPiece != null)
        {
            OnPieceClicked(clickedPiece);
            return;
        }

        Tile clickedTile = hit.collider.GetComponent<Tile>();
        if (clickedTile != null)
        {
            OnTileClicked(clickedTile);
        }
    }

    public void OnPieceClicked(Piece piece)
    {
        if (piece == null || piece.boardManager == null)
            return;

        BoardManager board = piece.boardManager;
        if (BughouseManager.Instance != null)
{
    Debug.Log("Equipo: " + BughouseManager.Instance.GetTeam(board, piece.isWhite));
    Debug.Log(BughouseManager.Instance.GetPartnerInfo(board, piece.isWhite));
}

        if (board.waitingForPromotion)
            return;

        if (selectedPiece == null)
        {
            if (piece.isWhite == board.whiteTurn)
            {
                selectedPiece = piece;
                Debug.Log("Seleccionada: " + piece.name + " en " + board.name);
            }
            return;
        }

        // Si ya había una pieza seleccionada pero es de otro tablero,
        // cambiamos la selección a la nueva pieza
        if (selectedPiece.boardManager != board)
        {
            if (piece.isWhite == board.whiteTurn)
            {
                selectedPiece = piece;
                Debug.Log("Nueva selección en otro tablero: " + piece.name + " en " + board.name);
            }
            return;
        }

        // Si haces clic en una pieza del mismo color, cambias selección
        if (selectedPiece.isWhite == piece.isWhite)
        {
            if (piece.isWhite == board.whiteTurn)
            {
                selectedPiece = piece;
                Debug.Log("Nueva selección: " + piece.name + " en " + board.name);
            }
            return;
        }

        // Si es una pieza enemiga del mismo tablero, intentar capturar
        TryMoveTo(piece.boardX, piece.boardY, board);
    }
/*
    public void OnTileClicked(Tile tile)
    {
        if (tile == null || tile.boardManager == null)
            return;

        TryMoveTo(tile.x, tile.y, tile.boardManager);
    }*/
    public void OnTileClicked(Tile tile)
{
    if (tile == null || tile.boardManager == null)
        return;

    // Si hay una pieza de pocket seleccionada, intentar drop
    if (pocketSelectionActive)
    {
        TryDropPiece(tile.x, tile.y, tile.boardManager);
        return;
    }

    TryMoveTo(tile.x, tile.y, tile.boardManager);
}

    public void SelectPocketPiece(BoardManager board, bool isWhitePocket, PieceType pieceType)
{
    if (board == null)
    {
        Debug.Log("Pocket rechazado: board es null");
        return;
    }

    Debug.Log("Click pocket -> tablero: " + board.name + ", color: " + (isWhitePocket ? "blancas" : "negras") + ", pieza: " + pieceType);
    Debug.Log("Turno actual en ese tablero: " + (board.whiteTurn ? "blancas" : "negras"));

    if (board.whiteTurn != isWhitePocket)
    {
        Debug.Log("Pocket rechazado: no es el turno de ese color");
        return;
    }

    pocketSelectionActive = true;
    selectedPocketBoard = board;
    selectedPocketColor = isWhitePocket;
    selectedPocketPieceType = pieceType;
    selectedPiece = null;

    Debug.Log("Pocket seleccionado correctamente");
}

    private void TryMoveTo(int x, int y, BoardManager board)
    {
        if (board == null)
            return;

        if (board.waitingForPromotion)
            return;

        if (selectedPiece == null)
            return;

        // No permitir mover una pieza en el otro tablero
        if (selectedPiece.boardManager != board)
            return;

        bool moved = board.TryMovePiece(selectedPiece, x, y);
        if (moved)
        {
            selectedPiece = null;
        }
    }
    private void TryDropPiece(int x, int y, BoardManager board)
{
    if (!pocketSelectionActive)
    {
        Debug.Log("TryDropPiece: no hay pocket seleccionado");
        return;
    }

    if (selectedPocketBoard == null)
    {
        Debug.Log("TryDropPiece: selectedPocketBoard es null");
        return;
    }

    if (board != selectedPocketBoard)
    {
        Debug.Log("TryDropPiece: tablero clickeado no coincide. Clickeado = " + board.name + ", seleccionado = " + selectedPocketBoard.name);
        return;
    }

    if (BughouseManager.Instance == null)
    {
        Debug.Log("TryDropPiece: BughouseManager es null");
        return;
    }

    Debug.Log("Intentando soltar " + selectedPocketPieceType + " en " + board.name + " " + x + "," + y);

    bool dropped = board.TryDropPocketPiece(selectedPocketColor, selectedPocketPieceType, x, y);
    if (dropped)
    {
        BughouseManager.Instance.RemovePieceFromPocket(board, selectedPocketColor, selectedPocketPieceType);

        pocketSelectionActive = false;
        selectedPocketBoard = null;

        RefreshAllPocketButtons();
    }
    else
    {
        Debug.Log("TryDropPiece: board rechazó el drop");
    }
}

public void RefreshAllPocketButtons()
{
    PocketButton[] pocketButtons = FindObjectsByType<PocketButton>(FindObjectsSortMode.None);
    for (int i = 0; i < pocketButtons.Length; i++)
    {
        pocketButtons[i].Refresh();
    }
}

}