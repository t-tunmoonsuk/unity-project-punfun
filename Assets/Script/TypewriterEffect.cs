using System.Collections;
using UnityEngine;
using TMPro; // ใช้สำหรับ TextMeshPro

public class TypewriterEffect : MonoBehaviour
{
    public float typingSpeed = 0.05f; // ความเร็วในการพิมพ์
    private string fullText;
    private TMP_Text textComponent;

    public Animator characterAnimator; // ช่องใส่ตัวละครเพื่อให้ขยับปาก
    public GameObject continueButton;  // [เพิ่มใหม่] ช่องใส่ปุ่มแตะเพื่อไปต่อ

    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        fullText = textComponent.text;
        textComponent.text = "";

        // 1. ซ่อนปุ่ม "แตะเพื่อไปต่อ" ไว้ก่อนตอนเริ่ม
        if (continueButton != null)
        {
            continueButton.SetActive(false);
        }

        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        // สั่งเปิดสวิตช์ให้ตัวละครเริ่มขยับปาก
        if (characterAnimator != null) characterAnimator.SetBool("isTalking", true);

        // ค่อยๆ พิมพ์ตัวอักษรทีละตัว
        foreach (char c in fullText)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        // สั่งปิดสวิตช์ให้ตัวละครหยุดขยับปากเมื่อพิมพ์เสร็จ
        if (characterAnimator != null) characterAnimator.SetBool("isTalking", false);

        // 2. โชว์ปุ่ม "แตะเพื่อไปต่อ" ขึ้นมาเมื่อพิมพ์เสร็จหมดแล้ว
        if (continueButton != null)
        {
            continueButton.SetActive(true);
        }
    }
}