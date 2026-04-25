using UnityEngine;
using UnityEngine.UI;

public class CategorySelectionManager : MonoBehaviour
{
    [Header("รูปไอคอนร้านทั้ง 3")]
    public Image[] categoryImages;

    [Header("ปุ่มและหน้าจอ")]
    public Button nextButton;
    public GameObject categoryPanel; // หน้าเลือกร้าน
    public GameObject locationPanel; // หน้าแผนที่

    public Color selectedColor = Color.white;
    public Color unselectedColor = Color.gray;

    private int selectedIndex = -1;

    void Start()
    {
        nextButton.interactable = false; // เริ่มมาปุ่มถัดไปกดไม่ได้
    }

    // ฟังก์ชันตอนกดปุ่มร้าน
    public void SelectCategory(int index)
    {
        selectedIndex = index;
        for (int i = 0; i < categoryImages.Length; i++)
        {
            categoryImages[i].color = (i == index) ? selectedColor : unselectedColor;
        }
        nextButton.interactable = true; // พอเลือกแล้ว ปุ่มถัดไปกดได้
    }

    // ฟังก์ชันตอนกดปุ่ม "ถัดไป"
    public void GoToNextStep()
    {
        PlayerPrefs.SetInt("SelectedCategory", selectedIndex); // จำไว้ว่าเลือกร้านไหน
        PlayerPrefs.Save();

        // ปิดหน้าเลือกร้าน เปิดหน้าแผนที่
        categoryPanel.SetActive(false);
        locationPanel.SetActive(true);
    }
}