using UnityEngine;
using TMPro;

public class BusinessForm : MonoBehaviour
{
    [Header("ข้อมูลต้นทุน (Cost Data)")]
    [SerializeField] private TMP_InputField rentInput;       // ช่องกรอกค่าเช่าร้าน
    [SerializeField] private TMP_InputField waterInput;      // ช่องกรอกค่าน้ำ
    [SerializeField] private TMP_InputField electricInput;   // ช่องกรอกค่าไฟ
    [SerializeField] private TMP_InputField salaryInput;     // ช่องกรอกเงินเดือนพนักงาน (ต่อคน)

    [Header("ข้อมูลการดำเนินงาน (Operation Data)")]
    [SerializeField] private TMP_InputField employeeCountInput; // จำนวนพนักงาน
    [SerializeField] private TMP_InputField sellPriceInput;     // ราคาขาย

    [Header("ข้อมูลวัตถุดิบ (Raw Material Data)")]
    [SerializeField] private TMP_InputField meatInput;       // วัตถุดิบ_เนื้อสัตว์
    [SerializeField] private TMP_InputField fruitInput;      // วัตถุดิบ_ผลไม้
    [SerializeField] private TMP_InputField carbInput;       // วัตถุดิบ_คาร์โบไฮเดรต
    [SerializeField] private TMP_InputField vegInput;        // วัตถุดิบ_ผัก
    [SerializeField] private TMP_InputField seasoningInput;  // วัตถุดิบ_เครื่องปรุง

    private void Start()
    {
        // ตั้งค่าเริ่มต้นให้ช่องกรอกทั้งหมดเป็นเลข 0 และล็อกให้พิมพ์ได้แค่ตัวเลขเท่านั้น
        SetupInputField(rentInput);
        SetupInputField(waterInput);
        SetupInputField(electricInput);
        SetupInputField(salaryInput);
        SetupInputField(employeeCountInput);
        SetupInputField(sellPriceInput);

        SetupInputField(meatInput);
        SetupInputField(fruitInput);
        SetupInputField(carbInput);
        SetupInputField(vegInput);
        SetupInputField(seasoningInput);
    }

    // ฟังก์ชันตัวช่วยตั้งค่า InputField
    private void SetupInputField(TMP_InputField inputField)
    {
        if (inputField != null)
        {
            inputField.text = "0";
            // บังคับให้ผู้เล่นพิมพ์ได้เฉพาะตัวเลขจำนวนเต็ม (ป้องกัน Error ตอนนำไปคำนวณ)
            // หากเกมของคุณใช้เลขทศนิยม ให้เปลี่ยนเป็น TMP_InputField.ContentType.DecimalNumber
            inputField.contentType = TMP_InputField.ContentType.IntegerNumber; 
        }
    }

    public void OnSubmitForm()
    {
        // 1. ดึงข้อมูลจาก UI และแปลงเป็นตัวเลขอย่างปลอดภัย
        int rent = ParseToInt(rentInput.text);
        int water = ParseToInt(waterInput.text);
        int electric = ParseToInt(electricInput.text);
        int salary = ParseToInt(salaryInput.text);
        
        int employeeCount = ParseToInt(employeeCountInput.text);
        int sellPrice = ParseToInt(sellPriceInput.text);

        int meat = ParseToInt(meatInput.text);
        int fruit = ParseToInt(fruitInput.text);
        int carb = ParseToInt(carbInput.text);
        int veg = ParseToInt(vegInput.text);
        int seasoning = ParseToInt(seasoningInput.text);

        // 2. คำนวณสรุปผลเบื้องต้น
        // สมมติว่า "เงินเดือนพนักงาน" คือต่อคน เราต้องคูณกับ "จำนวนพนักงาน"
        int totalSalaryCost = salary * employeeCount; 
        int totalFixedCost = rent + water + electric + totalSalaryCost;
        int totalMaterialCost = meat + fruit + carb + veg + seasoning;
        int totalCost = totalFixedCost + totalMaterialCost;

        // 3. แสดงผลเพื่อเช็กความถูกต้อง
        Debug.Log("=== สรุปข้อมูลธุรกิจ ===");
        Debug.Log($"รายจ่ายประจำ: {totalFixedCost} (เช่า {rent}, น้ำ {water}, ไฟ {electric}, เงินเดือนพนักงานรวม {totalSalaryCost})");
        Debug.Log($"รายจ่ายวัตถุดิบ: {totalMaterialCost} (เนื้อ {meat}, ผลไม้ {fruit}, คาร์บ {carb}, ผัก {veg}, เครื่องปรุง {seasoning})");
        Debug.Log($"รวมต้นทุนทั้งหมด: {totalCost}");
        Debug.Log($"ราคาขายตั้งไว้ที่: {sellPrice} / จำนวนพนักงาน: {employeeCount} คน");
        Debug.Log("=========================");

        // TODO: คุณสามารถนำตัวแปรเหล่านี้ส่งต่อไปยัง Script อื่น เช่น GameManager หรือเซพลง Database ได้เลย
    }

    // ฟังก์ชันช่วยแปลง String เป็น Int (ถ้าช่องว่างเปล่า หรือมีตัวอักษรหลงมา จะคืนค่าเป็น 0 ทันที ไม่ทำให้เกมพัง)
    private int ParseToInt(string text)
    {
        if (int.TryParse(text, out int result))
        {
            return result;
        }
        return 0; 
    }
}