using UnityEngine;
using TMPro; 

public class BusinessForm : MonoBehaviour
{
    [Header("ข้อมูลการดำเนินงาน (Operation Data)")]
    [SerializeField] private TMP_InputField employeeCountInput; 
    [SerializeField] private TMP_InputField sellPriceInput;     

    private void Start()
    {
        // ตั้งค่าเริ่มต้นให้ช่องกรอกเป็นเลข 0
        if(employeeCountInput != null) employeeCountInput.text = "0";
        if(sellPriceInput != null) sellPriceInput.text = "0";
    }

    public void OnSubmitForm()
    {
        Debug.Log("กดปุ่มถัดไป และบันทึกข้อมูลเรียบร้อย!");
    }
}