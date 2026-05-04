using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameplaySequence : MonoBehaviour
{
    [Header("ใส่ UI ที่นี่")]
    public GameObject infoPopup;
    public GameObject darkBackground;
    public GameObject startTextUI;
    public GameObject gameplayUI;

    void Start()
    {
        infoPopup.SetActive(true);
        darkBackground.SetActive(true);
        startTextUI.SetActive(false);
        gameplayUI.SetActive(false);
    }

    public void ClickClosePopup()
    {
        infoPopup.SetActive(false);
        StartCoroutine(RunStartSequence());
    }

    IEnumerator RunStartSequence()
    {
        startTextUI.SetActive(true);

        // ดึงคอมโพเนนต์ต่างๆ มาใช้งาน
        RectTransform textRect = startTextUI.GetComponent<RectTransform>();
        TextMeshProUGUI textMesh = startTextUI.GetComponent<TextMeshProUGUI>();
        Image bgImage = darkBackground.GetComponent<Image>();

        // จดจำตำแหน่งเริ่มต้น และค่าความโปร่งใสเดิมไว้
        Vector2 originalPos = textRect.anchoredPosition;
        float startBgAlpha = (bgImage != null) ? bgImage.color.a : 0.7f;

        // --- 1. เอฟเฟกต์ตอนโผล่: "Fade In & Slide Up" ---
        // เลื่อนข้อความลงไปด้านล่าง 50 พิกเซล และตั้งให้โปร่งใส (มองไม่เห็น)
        textRect.anchoredPosition = originalPos - new Vector2(0, 50f);
        if (textMesh != null) { Color c = textMesh.color; c.a = 0f; textMesh.color = c; }

        float time = 0;
        float duration = 0.5f; // เวลาตอนลอยขึ้นมา (นุ่มๆ ที่ครึ่งวินาที)
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float smoothStep = t * t * (3f - 2f * t); // สูตรคณิตศาสตร์ทำให้การเคลื่อนไหวสมูทหัว-ท้าย

            // ลอยขึ้นมาตำแหน่งกลางจอ
            textRect.anchoredPosition = Vector2.Lerp(originalPos - new Vector2(0, 50f), originalPos, smoothStep);

            // เฟดตัวหนังสือให้ค่อยๆ ชัดขึ้น
            if (textMesh != null)
            {
                Color c = textMesh.color;
                c.a = Mathf.Lerp(0f, 1f, smoothStep);
                textMesh.color = c;
            }
            yield return null;
        }

        // --- 2. โชว์ค้างไว้ให้ดูสวยงาม 1.5 วินาที ---
        yield return new WaitForSeconds(1.5f);

        // --- 3. เอฟเฟกต์ตอนหาย: "Fade Out & Slide Up" ---
        time = 0;
        duration = 0.5f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float smoothStep = t * t * (3f - 2f * t);

            // ลอยขึ้นไปด้านบนอีก 50 พิกเซล
            textRect.anchoredPosition = Vector2.Lerp(originalPos, originalPos + new Vector2(0, 50f), smoothStep);

            // เฟดตัวหนังสือให้จางหายไป
            if (textMesh != null)
            {
                Color c = textMesh.color;
                c.a = Mathf.Lerp(1f, 0f, smoothStep);
                textMesh.color = c;
            }

            // เฟดพื้นหลังดำให้จางหายไปพร้อมกันแบบนุ่มๆ
            if (bgImage != null)
            {
                Color c = bgImage.color;
                c.a = Mathf.Lerp(startBgAlpha, 0f, smoothStep);
                bgImage.color = c;
            }
            yield return null;
        }

        // --- 4. ปิด UI และเริ่มเกมเพลย์ ---
        startTextUI.SetActive(false);
        darkBackground.SetActive(false);

        // รีเซ็ตค่า UI กลับเป็นปกติ (สำคัญมาก เพื่อให้กลับมาเล่นด่านนี้ใหม่ได้โดยที่ตำแหน่งไม่เพี้ยน)
        textRect.anchoredPosition = originalPos;
        if (textMesh != null) { Color c = textMesh.color; c.a = 1f; textMesh.color = c; }
        if (bgImage != null) { Color c = bgImage.color; c.a = startBgAlpha; bgImage.color = c; }

        gameplayUI.SetActive(true);
    }
}