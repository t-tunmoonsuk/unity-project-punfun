using UnityEngine;
using TMPro;
using System.IO;
using System.Collections; // <-- เพิ่มบรรทัดนี้เข้ามาเพื่อให้ใช้ IEnumerator ได้

[System.Serializable]
public class FormData
{
    public string projectName;
    public string selectedMenu;
    public string selectedTime;
}

public class FromNewB : MonoBehaviour 
{
    [Header("Form Inputs")]
    public TMP_InputField nameProjectInput;
    public TMP_Dropdown mainMenuDropdown;
    public TMP_Dropdown timeDropdown;

    // เปลี่ยนรูปแบบ Start เพื่อให้มัน "รอ" ได้
    IEnumerator Start()
    {
        // คำสั่งนี้คือเวทมนตร์! สั่งให้โค้ดหยุดรอ 1 เฟรม ปล่อยให้ Unity จัดการ UI ของมันไปก่อน
        yield return null; 

        // หลังจาก Unity โหลดเสร็จ เราค่อยเข้าไปแทรกแซงและเขียนทับข้อความ
        mainMenuDropdown.SetValueWithoutNotify(-1);
        timeDropdown.SetValueWithoutNotify(-1);

        mainMenuDropdown.captionText.text = "--- กรุณาเลือกเมนู ---";
        timeDropdown.captionText.text = "--- กรุณาเลือกเวลา ---";
    }

    public void SubmitForm()
    {
        // ยังคงเช็คที่ -1 เหมือนเดิม
        if (mainMenuDropdown.value == -1 || timeDropdown.value == -1)
        {
            Debug.LogWarning("ตื๊ดๆ! กรุณาเลือกเมนูอาหารและเวลาให้ครบก่อนกดถัดไปครับ");
            return;
        }

        FormData myData = new FormData();
        myData.projectName = nameProjectInput.text;
        myData.selectedMenu = mainMenuDropdown.options[mainMenuDropdown.value].text;
        myData.selectedTime = timeDropdown.options[timeDropdown.value].text;

        string jsonContent = JsonUtility.ToJson(myData, true);
        string savePath = Application.persistentDataPath + "/FormDataLog.json";
        File.WriteAllText(savePath, jsonContent);

        Debug.Log("บันทึกข้อมูลแบบ JSON สำเร็จ! เปิดดูไฟล์ได้ที่: " + savePath);
        Debug.Log("หน้าตา JSON ที่เซฟ:\n" + jsonContent); 
    }
}