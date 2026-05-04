using UnityEngine;
using UnityEngine.EventSystems; // Drag & Drop UI

public class PlayGame2DraggableCharacter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("สถานะตัวละคร")]
    public bool isSafe = false;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private Vector2 startPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        // หา Canvas หลักที่ครอบตัวละครอยู่ เพื่อเอาค่า Scale มาคำนวณตอนลากให้แม่นยำ
        canvas = GetComponentInParent<Canvas>(); 
        
        // จำตำแหน่งเริ่มต้นไว้
        startPosition = rectTransform.anchoredPosition; 
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isSafe) return; // ถ้าเข้าที่ปลอดภัยแล้ว จะไม่ให้ลากอีก

        canvasGroup.alpha = 0.8f; // ทำให้ตัวละครโปร่งแสงนิดนึงตอนกำลังลาก
        
        // ปิดการบังเมาส์ชั่วคราว เพื่อให้เมาส์คลิกทะลุไปโดน "ร่ม" ที่อยู่ด้านหลังได้
        canvasGroup.blocksRaycasts = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isSafe) return;

        // ทำให้ตัวละครขยับตามเมาส์
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isSafe) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (!isSafe)
        {
            rectTransform.anchoredPosition = startPosition;
        }
    }

    public void SetSafeStatus(Transform safeTransform)
    {
        isSafe = true;
        transform.position = safeTransform.position;
        canvasGroup.blocksRaycasts = false; 
    }
}