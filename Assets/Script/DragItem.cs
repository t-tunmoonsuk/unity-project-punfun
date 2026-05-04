using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public int moneyValue = 0;

    // --- ส่วนที่เพิ่มใหม่สำหรับสอนเล่น ---
    [Header("สอนเล่น: ลาก TutorialHand จาก Hierarchy มาใส่ช่องนี้")]
    public GameObject tutorialHand;
    // ------------------------------------

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    [HideInInspector] public bool isLocked = false;

    // สำหรับแยกว่าตัวนี้คือร่างโคลนที่อยู่ในช่องหรือไม่
    [HideInInspector] public bool isClone = false;
    [HideInInspector] public bool isDroppedOnSlot = false;
    [HideInInspector] public DropSlot parentSlot;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // ถ้าเป็นป้ายเมนูที่โดนล็อคอยู่ ห้ามลาก
        if (isLocked && !isClone)
        {
            eventData.pointerDrag = null;
            return;
        }

        // --- ส่วนที่เพิ่มใหม่สำหรับสอนเล่น ---
        // สั่งปิดมือสอนทันทีที่ผู้เล่นเริ่มกดลากป้าย
        if (tutorialHand != null)
        {
            tutorialHand.SetActive(false);
        }
        // ------------------------------------

        isDroppedOnSlot = false; // รีเซ็ตค่าเสมอเมื่อเริ่มลาก
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked && !isClone) return;
        rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isClone)
        {
            // สำหรับร่างโคลน
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            if (isDroppedOnSlot)
            {
                // ถ้าลากกลับมาใส่ช่องเดิม หรือช่องอื่นที่รับ มันจะจัดการจัดตำแหน่งตัวเอง
            }
            else
            {
                // ถ้าปล่อยเมาส์กลางอากาศ (ที่ว่างๆ) -> สั่งลบตัวเอง และปลดล็อคป้ายเมนูขวา
                if (parentSlot != null)
                {
                    parentSlot.ClearSlotAndReset();
                }
                Destroy(gameObject);
            }
        }
        else
        {
            // สำหรับป้ายเมนูขวา เด้งกลับที่เดิมเสมอ
            rectTransform.anchoredPosition = originalPosition;

            if (!isLocked)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }
        }
    }

    public void SetLock(bool isSelected)
    {
        isLocked = true;
        canvasGroup.alpha = isSelected ? 1f : 0.5f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // ถ้าร่างโคลนโดนป้ายใหม่ลากมาทับ ให้ส่งการกระทำลงไปที่ช่อง DropSlot ด้านล่างแทน
        if (isClone && parentSlot != null)
        {
            parentSlot.OnDrop(eventData);
        }
    }
}