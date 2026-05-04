using UnityEngine;
using TMPro;
using System.IO;
using System.Collections;

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

    IEnumerator Start()
    {
        yield return null; 

        mainMenuDropdown.SetValueWithoutNotify(-1);
        timeDropdown.SetValueWithoutNotify(-1);

        mainMenuDropdown.captionText.text = "--- กรุณาเลือกเมนู ---";
        timeDropdown.captionText.text = "--- กรุณาเลือกเวลา ---";
    }

    public void SubmitForm()
    {
        if (mainMenuDropdown.value == -1 || timeDropdown.value == -1)
        {
            Debug.LogWarning("กรุณาเลือกเมนูอาหารและเวลาให้ครบ");
            return;
        }

        FormData myData = new FormData();
        myData.projectName = nameProjectInput.text;
        myData.selectedMenu = mainMenuDropdown.options[mainMenuDropdown.value].text;
        myData.selectedTime = timeDropdown.options[timeDropdown.value].text;

        string jsonContent = JsonUtility.ToJson(myData, true);
        string savePath = Application.persistentDataPath + "/FormDataLog.json";
        File.WriteAllText(savePath, jsonContent);

        Debug.Log("บันทึกข้อมูลสำเร็จ! path: " + savePath);
        Debug.Log(jsonContent); 
    }
}