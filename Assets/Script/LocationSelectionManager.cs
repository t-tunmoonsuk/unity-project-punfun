using UnityEngine;
using UnityEngine.UI;

public class LocationSelectionManager : MonoBehaviour
{
    [Header("ใส่รูปหมุดแผนที่ทั้ง 3 (Image)")]
    public Image[] pinImages;

    [Header("ใส่ป๊อปอัปทั้ง 3 (GameObject)")]
    public GameObject[] popups;

    [Header("ปุ่มถัดไป")]
    public Button nextButton;

    public Color selectedColor = Color.white; // สีปกติ
    public Color unselectedColor = new Color(0.6f, 0.6f, 0.6f, 1f); // สีเทาเมื่อไม่ได้เลือก

    private int selectedIndex = -1; // -1 คือยังไม่ได้เลือก

    void Start()
    {
        if (nextButton != null) nextButton.interactable = false;
    }

    // ฟังก์ชันนี้ไว้ให้สคริปต์อื่นมาดึงข้อมูลว่ากำลังเลือกหมุดไหนอยู่
    public int GetSelectedIndex()
    {
        return selectedIndex;
    }

    // ฟังก์ชันตอนกดคลิกหมุด
    public void SelectLocation(int index)
    {
        selectedIndex = index;

        // วนลูปเช็กหมุดทุกอัน
        for (int i = 0; i < pinImages.Length; i++)
        {
            if (i == index)
            {
                // อันที่เลือก -> สีสว่าง และ เปิดป๊อปอัปค้างไว้
                pinImages[i].color = selectedColor;
                if (popups[i] != null) popups[i].SetActive(true);
            }
            else
            {
                // อันที่ไม่ได้เลือก -> สีเทา และ ปิดป๊อปอัป
                pinImages[i].color = unselectedColor;
                if (popups[i] != null) popups[i].SetActive(false);
            }
        }

        if (nextButton != null) nextButton.interactable = true;
    }
}