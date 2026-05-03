using UnityEngine;
using UnityEngine.UI;

public class PocketButton : MonoBehaviour
{
    public BoardManager boardManager;
    public bool isWhitePocket;
    public PieceType pieceType;

    public Button button;
    public Image iconImage;
    public Text countText;

    public void Refresh()
    {
        if (BughouseManager.Instance == null || boardManager == null) return;

        var pocket = BughouseManager.Instance.GetPocket(boardManager, isWhitePocket);

        int count = 0;
        for (int i = 0; i < pocket.Count; i++)
        {
            if (pocket[i] == pieceType)
                count++;
        }

        Debug.Log(gameObject.name + " count = " + count);

        if (countText != null)
            countText.text = count.ToString();

        if (button != null)
            button.interactable = count > 0;

        if (iconImage != null)
        {
            Sprite sprite = boardManager.GetPromotionSprite(pieceType, isWhitePocket);
            iconImage.sprite = sprite;
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;
        }
    }

    public void OnClick()
{
    Debug.Log("Click en botón pocket: " + gameObject.name);

    if (GameManager.Instance == null)
    {
        Debug.Log("GameManager.Instance es null");
        return;
    }

    Debug.Log("BoardManager del botón: " + (boardManager != null ? boardManager.name : "null"));
    Debug.Log("Color del pocket: " + (isWhitePocket ? "blancas" : "negras"));
    Debug.Log("PieceType: " + pieceType);

    GameManager.Instance.SelectPocketPiece(boardManager, isWhitePocket, pieceType);
}
}