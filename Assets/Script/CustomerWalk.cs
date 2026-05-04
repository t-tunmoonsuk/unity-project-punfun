using UnityEngine;
using UnityEngine.UI; // 🔴 เพิ่มอันนี้เข้ามาเพื่อให้โค้ดรู้จักและจัดการกับรูปภาพ (Image) ใน UI ได้
using System.Collections;

public class CustomerWalk : MonoBehaviour
{
    public GameObject customerObj;
    public GameObject dialogUI;
    public float startX;
    public float targetX;
    public float moveDuration = 1f;

    [Header("Sprite Settings")]
    public Sprite sideSprite;  // 🔴 ช่องสำหรับใส่รูป "ตอนเดินหันข้าง"
    public Sprite frontSprite; // 🔴 ช่องสำหรับใส่รูป "ตอนยืนคุยหันหน้า"

    private RectTransform rect;
    private Image customerImage; // ตัวแปรสำหรับคุมการเปลี่ยนรูป

    void Start()
    {
        rect = customerObj.GetComponent<RectTransform>();
        customerImage = customerObj.GetComponent<Image>(); // ดึงคอมโพเนนต์ Image จากตัวละครมาเตรียมไว้
    }

    // ขาเข้า
    public void WalkInAndShowDialog()
    {
        customerObj.SetActive(true);
        dialogUI.SetActive(false);

        // 🟢 เปลี่ยนเป็นรูปหันข้าง ตอนเริ่มเดินเข้า
        if (customerImage != null && sideSprite != null)
        {
            customerImage.sprite = sideSprite;
        }

        StartCoroutine(MoveCharacter(startX, targetX, true));
    }

    // ขาออก
    public void WalkOut()
    {
        dialogUI.SetActive(false);

        // 🟢 เปลี่ยนกลับเป็นรูปหันข้าง ตอนจะเดินออก
        if (customerImage != null && sideSprite != null)
        {
            customerImage.sprite = sideSprite;

            // ปล. ถ้าลูกค้าต้องเดินหันหลังกลับทางเดิม ลบเครื่องหมาย // บรรทัดล่างนี้ออกเพื่อกลับซ้ายขวาได้ครับ
            // customerObj.transform.localScale = new Vector3(-1, 1, 1); 
        }

        StartCoroutine(MoveCharacter(targetX, startX, false));
    }

    IEnumerator MoveCharacter(float fromX, float toX, bool showDialogAfter)
    {
        float elapsedTime = 0;
        Vector2 startPos = new Vector2(fromX, rect.anchoredPosition.y);
        Vector2 endPos = new Vector2(toX, rect.anchoredPosition.y);

        while (elapsedTime < moveDuration)
        {
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rect.anchoredPosition = endPos;

        if (showDialogAfter)
        {
            // 🟢 พอเดินถึงจุดยืนเป๊ะๆ ให้สลับเป็นรูป "หันหน้าตรง"
            if (customerImage != null && frontSprite != null)
            {
                customerImage.sprite = frontSprite;
            }

            if (dialogUI != null) dialogUI.SetActive(true);
        }
        else
        {
            customerObj.SetActive(false);

            // รีเซ็ตการกลับซ้ายขวาให้เป็นปกติ (เผื่อใช้ข้างบน)
            // customerObj.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}