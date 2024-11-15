using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropLabel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Text nameText;

    public ItemDropInfo DropInfo { get; private set; }
    public float pickupRadiusPixels = 300f; // Радиус подбора предмета в пикселях

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Fill(ItemDropInfo dropInfo)
    {
        this.DropInfo = dropInfo;
        this.nameText.text = dropInfo.item.name;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (DropInfo != null)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();

            if (playerController != null)
            {
                // Определяем центр экрана
                Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

                Vector2 dropLabelScreenPosition;
                if (rectTransform.parent is RectTransform parentRect)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        parentRect,
                        rectTransform.position,
                        null,
                        out dropLabelScreenPosition
                    );
                }
                else
                {
                    dropLabelScreenPosition = rectTransform.position;
                }

                // Проверяем расстояние
                float distanceToCenter = Vector2.Distance(screenCenter, dropLabelScreenPosition);
                Debug.Log($"Расстояние до центра: {distanceToCenter} (радиус подбора: {pickupRadiusPixels})");

                if (distanceToCenter <= pickupRadiusPixels)
                {
                    Debug.Log("DropLabel в радиусе подбора, выполняем подбор...");
                    playerController.PickUpDrop(this);
                }
                else
                {
                    Debug.Log("DropLabel слишком далеко от центра экрана для подбора.");
                }
            }
            else
            {
                Debug.LogError("PlayerController не найден!");
            }
        }
        else
        {
            Debug.LogError("DropInfo не найден для DropLabel.");
        }
    }
}
