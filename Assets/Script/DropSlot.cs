using UnityEngine;
using UnityEngine.UI; // สำคัญ: ต้องมีเพื่อจัดการสี (Alpha)
using UnityEngine.EventSystems;
using System.Collections;

public class DropSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("การตั้งค่า UI แบบมืออาชีพ")]
    public float hoverScale = 1.05f;
    public float animationSpeed = 15f;

    [Header("ตั้งค่าการกระพริบ (Breathing Glow)")]
    public float pulseSpeed = 1.5f;
    public float minAlpha = 0.5f;

    private Vector3 originalScale;
    private Coroutine scaleCoroutine;
    private Coroutine pulseCoroutine;
    private Image slotImage;

    [Header("ส่วนประกอบหลัก")]
    public GameObject[] allPriceTags;
    public MiniGame1Manager manager;
    public GameObject tutorialHand;
    public float delayTime = 1.0f;

    void Awake()
    {
        originalScale = transform.localScale;
        slotImage = GetComponent<Image>();
    }

    void Start()
    {

        StartPulse();
    }


    private void StartPulse()
    {
        if (pulseCoroutine == null)
            pulseCoroutine = StartCoroutine(PulseAnimation());
    }

    private void StopPulse()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }

        // คืนค่าความสว่าง 100% ทันทีที่หยุดกระพริบ
        if (slotImage != null)
        {
            Color c = slotImage.color;
            c.a = 1f;
            slotImage.color = c;
        }
    }

    private IEnumerator PulseAnimation()
    {
        while (true)
        {
            if (slotImage != null)
            {
                Color c = slotImage.color;
                // ใช้ Mathf.PingPong ทำให้ค่าสวิงไปมาอย่างนุ่มนวล
                float range = 1f - minAlpha;
                c.a = minAlpha + Mathf.PingPong(Time.time * pulseSpeed, range);
                slotImage.color = c;
            }
            yield return null;
        }
    }
    // ---------------------------------------------

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            StopPulse(); // หยุดกระพริบตอนกำลังจะวาง
            if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
            scaleCoroutine = StartCoroutine(SmoothScale(originalScale * hoverScale));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
            scaleCoroutine = StartCoroutine(SmoothScale(originalScale));
            StartPulse(); // ถ้าเปลี่ยนใจดึงเมาส์ออก ให้กลับมากระพริบใหม่
        }
    }

    private IEnumerator SmoothScale(Vector3 targetScale)
    {
        while (Vector3.Distance(transform.localScale, targetScale) > 0.001f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
            yield return null;
        }
        transform.localScale = targetScale;
    }

    public void OnDrop(PointerEventData eventData)
    {
        StopPulse(); // วางเสร็จแล้ว หยุดกระพริบถาวร
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        transform.localScale = originalScale;

        if (eventData.pointerDrag != null)
        {
            DragItem draggedItem = eventData.pointerDrag.GetComponent<DragItem>();
            if (draggedItem != null)
            {
                if (draggedItem.isClone)
                {
                    draggedItem.isDroppedOnSlot = true;
                    draggedItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    return;
                }

                if (!draggedItem.isLocked)
                {
                    ClearSlot();
                    GameObject clone = Instantiate(eventData.pointerDrag, transform);
                    DragItem cloneDrag = clone.GetComponent<DragItem>();
                    cloneDrag.isClone = true;
                    cloneDrag.parentSlot = this;
                    cloneDrag.isLocked = false;

                    CanvasGroup cloneGroup = clone.GetComponent<CanvasGroup>();
                    if (cloneGroup != null)
                    {
                        cloneGroup.alpha = 1f;
                        cloneGroup.blocksRaycasts = true;
                    }

                    RectTransform rect = clone.GetComponent<RectTransform>();
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = Vector2.zero;
                    rect.localScale = Vector3.one;
                    rect.anchoredPosition = Vector2.zero;

                    UpdateTagsVisual(eventData.pointerDrag);

                    if (tutorialHand != null) tutorialHand.SetActive(false);

                    // ลูกเล่น: เด้ง Pop ตอนวาง
                    StartCoroutine(PopAnimation(clone.transform));

                    if (manager != null) StartCoroutine(WaitAndCheckAnswer());
                }
            }
        }
    }

    private IEnumerator PopAnimation(Transform target)
    {
        Vector3 baseScale = Vector3.one;
        Vector3 popScale = baseScale * 1.2f;

        float t = 0;
        while (t < 1f) { t += Time.deltaTime * 20f; target.localScale = Vector3.Lerp(baseScale, popScale, t); yield return null; }

        t = 0;
        while (t < 1f) { t += Time.deltaTime * 15f; target.localScale = Vector3.Lerp(popScale, baseScale, t); yield return null; }

        target.localScale = baseScale;
    }

    private IEnumerator WaitAndCheckAnswer()
    {
        yield return new WaitForSeconds(delayTime);
        manager.CheckAnswer();
    }

    public void ClearSlot()
    {
        foreach (Transform child in transform) Destroy(child.gameObject);
    }

    public void ClearSlotAndReset()
    {
        ClearSlot();
        UpdateTagsVisual(null);
    }

    private void UpdateTagsVisual(GameObject selectedTag)
    {
        foreach (GameObject tag in allPriceTags)
        {
            if (tag != null)
            {
                tag.SetActive(true);
                DragItem item = tag.GetComponent<DragItem>();
                if (item != null)
                {
                    if (selectedTag == null)
                    {
                        item.isLocked = false;
                        CanvasGroup cg = item.GetComponent<CanvasGroup>();
                        if (cg != null) { cg.alpha = 1f; cg.blocksRaycasts = true; }
                    }
                    else { item.SetLock(tag == selectedTag); }
                }
            }
        }
    }
}