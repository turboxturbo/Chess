using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoveredSprite;
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private GameResultView gameResultView;

    private void Start()
    {
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        SetSprite(normalSprite);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetSprite(hoveredSprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetSprite(normalSprite);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetSprite(pressedSprite);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetSprite(hoveredSprite);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        gameResultView?.Restart();
    }

    private void SetSprite(Sprite sprite)
    {
        if (buttonImage != null && sprite != null)
            buttonImage.sprite = sprite;
    }
}
