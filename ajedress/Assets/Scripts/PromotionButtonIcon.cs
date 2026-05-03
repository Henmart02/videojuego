using UnityEngine;
using UnityEngine.UI;

public class PromotionButtonIcon : MonoBehaviour
{
    public PieceType pieceType;
    public Image buttonImage;

    public void SetSprite(Sprite sprite)
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = sprite;
            buttonImage.color = Color.white;
            buttonImage.preserveAspect = true;
        }
    }
}