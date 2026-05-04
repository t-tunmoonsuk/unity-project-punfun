using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIAnimationHandler : MonoBehaviour
{
    [Header("Settings")]
    public float animationSpeed = 0.25f;
    public float targetBackgroundAlpha = 0.7f;
    public bool showOnStart = false;

    [Header("Summary Animation")]
    public GameObject[] cards;
    public float delayBetweenCards = 0.15f;

    [Header("References")]
    public GameObject darkBackground;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup.alpha = 0f;
        rectTransform.localScale = Vector3.one * 0.95f;
    }

    void Start()
    {
        if (showOnStart) ShowPopup();
    }

    public void ShowPopup()
    {
        StopAllCoroutines();
        gameObject.SetActive(true);
        StartCoroutine(AnimateInFull(false));
    }

    public void ShowPopupWithStaggeredCards()
    {
        StopAllCoroutines();
        gameObject.SetActive(true);

        // ซ่อนการ์ดทุกลูกก่อนเริ่ม
        foreach (var card in cards) if (card != null) card.transform.localScale = Vector3.zero;

        StartCoroutine(AnimateInFull(true));
    }

    // --- ฟังก์ชันใหม่: ปิดแค่หน้าต่าง แต่ทิ้งพื้นหลังดำไว้ให้หน้าถัดไป ---
    public void HidePopupOnly()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateOutPanelOnly());
    }

    public void HidePopup()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateOutFull());
    }

    IEnumerator AnimateInFull(bool useCards)
    {
        float timer = 0f;
        CanvasGroup bgGroup = null;

        if (darkBackground != null)
        {
            darkBackground.SetActive(true);
            bgGroup = darkBackground.GetComponent<CanvasGroup>();
            if (bgGroup == null) bgGroup = darkBackground.AddComponent<CanvasGroup>();
        }

        float startBGAlpha = (bgGroup != null) ? bgGroup.alpha : 0f;

        while (timer < animationSpeed)
        {
            timer += Time.deltaTime;
            float p = timer / animationSpeed;

            canvasGroup.alpha = p;
            rectTransform.localScale = Vector3.Lerp(Vector3.one * 0.95f, Vector3.one, p);

            if (bgGroup != null) bgGroup.alpha = Mathf.Lerp(startBGAlpha, targetBackgroundAlpha, p);

            yield return null;
        }

        canvasGroup.alpha = 1;
        rectTransform.localScale = Vector3.one;
        if (bgGroup != null) bgGroup.alpha = targetBackgroundAlpha;

        if (useCards)
        {
            foreach (var card in cards)
            {
                if (card != null)
                {
                    StartCoroutine(PopInCard(card));
                    yield return new WaitForSeconds(delayBetweenCards);
                }
            }
        }
    }

    IEnumerator PopInCard(GameObject card)
    {
        float t = 0;
        float dur = 0.2f;
        while (t < dur)
        {
            t += Time.deltaTime;
            card.transform.localScale = Vector3.one * Mathf.SmoothStep(0, 1, t / dur);
            yield return null;
        }
        card.transform.localScale = Vector3.one;
    }

    IEnumerator AnimateOutPanelOnly()
    {
        float timer = 0f;
        while (timer < animationSpeed)
        {
            timer += Time.deltaTime;
            float p = timer / animationSpeed;
            canvasGroup.alpha = 1 - p;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    IEnumerator AnimateOutFull()
    {
        float timer = 0f;
        CanvasGroup bgGroup = darkBackground != null ? darkBackground.GetComponent<CanvasGroup>() : null;
        float startBGAlpha = (bgGroup != null) ? bgGroup.alpha : 0f;

        while (timer < animationSpeed)
        {
            timer += Time.deltaTime;
            float p = timer / animationSpeed;
            canvasGroup.alpha = 1 - p;
            if (bgGroup != null) bgGroup.alpha = Mathf.Lerp(startBGAlpha, 0f, p);
            yield return null;
        }

        gameObject.SetActive(false);
        if (darkBackground != null) darkBackground.SetActive(false);
    }
}