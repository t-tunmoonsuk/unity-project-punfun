using UnityEngine;

public class MiniGame1Manager : MonoBehaviour
{
    [Header("ตั้งค่าเกม (ระบบหยอดเงิน)")]
    public DropSlot[] dropSlots;   // ลากช่องรับเงินมาใส่ให้ครบ

    [Header("หน้าต่าง UI หลัก")]
    public GameObject receiptUI;       // ลาก Game1_Street_UI มาใส่ (แผ่นบิล)
    public GameObject nextPopupPanel;  // ลากหน้าต่างรวม nextPopup2 มาใส่
    public GameObject wrongPopup;      // ลากหน้าต่างตอนตอบผิด (เช่น ใส่มา 0 บาท)

    [Header("Popup เนื้อหาตามราคาที่ใส่")]
    public GameObject popup40; // ลาก popup40 มาใส่
    public GameObject popup30; // ลาก popup30 มาใส่
    public GameObject popup20; // ลาก popup20 มาใส่

    void Start()
    {
        // เริ่มเกมมา ซ่อนหน้าสรุปผลและหน้าตอบผิดไว้ก่อน
        if (nextPopupPanel != null) nextPopupPanel.SetActive(false);
        if (wrongPopup != null) wrongPopup.SetActive(false);
    }

    public void CheckAnswer()
    {
        int currentTotal = 0;

        // เช็คทุกช่องรับเงินว่ามีเงินอยู่ข้างในไหม (โค้ดเดิมของคุณ)
        foreach (DropSlot slot in dropSlots)
        {
            if (slot.transform.childCount > 0)
            {
                DragItem moneyInSlot = slot.transform.GetChild(0).GetComponent<DragItem>();
                if (moneyInSlot != null)
                {
                    currentTotal += moneyInSlot.moneyValue;
                }
            }
        }

        Debug.Log("ยอดเงินที่ผู้เล่นเปย์มาคือ: " + currentTotal + " บาท");

        // ถ้ายอดเงินมากกว่า 0 ให้ไปเช็คต่อว่าเป็น 40, 30 หรือ 20
        if (currentTotal > 0)
        {
            // ถ้าตรงกับเงื่อนไข 40, 30, หรือ 20 ให้โชว์หน้าสรุปผล
            if (currentTotal == 40 || currentTotal == 30 || currentTotal == 20)
            {
                // 1. ปิดหน้าบิล และหน้าตอบผิด
                if (receiptUI != null) receiptUI.SetActive(false);
                if (wrongPopup != null) wrongPopup.SetActive(false);

                // 2. เปิดหน้า nextPopup2 ทับฉากหลัง
                if (nextPopupPanel != null) nextPopupPanel.SetActive(true);

                // 3. ปิดเนื้อหาย่อยทั้งหมดก่อนกันพลาด
                if (popup40 != null) popup40.SetActive(false);
                if (popup30 != null) popup30.SetActive(false);
                if (popup20 != null) popup20.SetActive(false);

                // 4. เปิดเนื้อหาให้ตรงกับเงินเป๊ะๆ
                if (currentTotal == 40 && popup40 != null) popup40.SetActive(true);
                if (currentTotal == 30 && popup30 != null) popup30.SetActive(true);
                if (currentTotal == 20 && popup20 != null) popup20.SetActive(true);
            }
            else
            {
                // ดักไว้เผื่อผู้เล่นใส่เงินมาเป็นเลขแปลกๆ (เช่น 10, 50) จะให้ขึ้นหน้า wrongPopup
                if (wrongPopup != null) wrongPopup.SetActive(true);
            }
        }
        else
        {
            // ถ้าเป็น 0 บาท (ไม่ได้ลากอะไรมาใส่เลย) ให้ขึ้นหน้าต่างผิด
            if (wrongPopup != null) wrongPopup.SetActive(true);
        }
    }
}