using UnityEngine;

public class PromotionUI : MonoBehaviour
{
    public BoardManager boardManager;

    public PromotionButtonIcon queenButton;
    public PromotionButtonIcon rookButton;
    public PromotionButtonIcon bishopButton;
    public PromotionButtonIcon knightButton;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        UpdateButtonIcons();
        MovePanelToBoard();
    }

    public void UpdateButtonIcons()
    {
        if (boardManager == null || boardManager.promotionPawn == null)
            return;

        bool isWhite = boardManager.promotionPawn.isWhite;

        if (queenButton != null)
            queenButton.SetSprite(boardManager.GetPromotionSprite(PieceType.Queen, isWhite));

        if (rookButton != null)
            rookButton.SetSprite(boardManager.GetPromotionSprite(PieceType.Rook, isWhite));

        if (bishopButton != null)
            bishopButton.SetSprite(boardManager.GetPromotionSprite(PieceType.Bishop, isWhite));

        if (knightButton != null)
            knightButton.SetSprite(boardManager.GetPromotionSprite(PieceType.Knight, isWhite));
    }

    public void MovePanelToBoard()
    {
        if (boardManager == null || rectTransform == null)
            return;

        Camera cam = Camera.main;
        if (cam == null)
            return;

        Vector3 boardCenterWorld = boardManager.transform.position;
        Vector3 screenPoint = cam.WorldToScreenPoint(boardCenterWorld);

        rectTransform.position = screenPoint;
    }

    public void PromoteToQueen()
    {
        if (boardManager != null)
            boardManager.PromotePawn(PieceType.Queen);
    }

    public void PromoteToRook()
    {
        if (boardManager != null)
            boardManager.PromotePawn(PieceType.Rook);
    }

    public void PromoteToBishop()
    {
        if (boardManager != null)
            boardManager.PromotePawn(PieceType.Bishop);
    }

    public void PromoteToKnight()
    {
        if (boardManager != null)
            boardManager.PromotePawn(PieceType.Knight);
    }
}