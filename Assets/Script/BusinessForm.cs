using UnityEngine;
using TMPro; // ใช้สำหรับจัดการ UI ของ TextMeshPro (InputField, Dropdown)
using System.IO; // ใช้สำหรับจัดการระบบไฟล์ (อ่าน/เขียนไฟล์ JSON)

// 1. สร้างโครงสร้างข้อมูลสำหรับ BusinessForm
// [System.Serializable] ทำให้ Unity สามารถแปลงคลาสนี้เป็น JSON หรือดูและตั้งค่าใน Inspector ได้
[System.Serializable]
public class BusinessData
{
    // ตัวแปรต่างๆ สำหรับเก็บค่าจากหน้า UI เพื่อเตรียมนำไปเซฟลงไฟล์
    public int rent;             // ค่าเช่า
    public int water;            // ค่าน้ำ
    public int electric;         // ค่าไฟ
    public int totalSalaryCost;  // รวมค่าจ้างพนักงานทั้งหมด
    public int employeeCount;    // จำนวนพนักงาน
    public int sellPrice;        // ราคาขาย
    public int meat;             // ต้นทุนเนื้อสัตว์
    public int fruit;            // ต้นทุนผลไม้
    public int carb;             // ต้นทุนแป้ง/คาร์โบไฮเดรต
    public int veg;              // ต้นทุนผัก
    public int seasoning;        // ต้นทุนเครื่องปรุง
    
    public int selectedMenuOption; // เก็บค่า Index ของ Dropdown ว่าผู้เล่นเลือกตัวเลือกไหนไว้ (0, 1, 2, ...)
    
    public string saveTimestamp;   // เก็บวันและเวลาที่กดเซฟข้อมูลเอาไว้ดูย้อนหลัง
}

public class BusinessForm : MonoBehaviour
{
    // [Header(...)] ช่วยจัดหมวดหมู่ตัวแปรในหน้าต่าง Inspector ของ Unity ให้เป็นระเบียบดูง่ายขึ้น
    [Header("UI Inputs")]
    // ตัวแปรสำหรับรับ Component ที่เป็นช่องกรอกข้อความ (InputField) ในหน้า UI ของเรา
    [SerializeField] private TMP_InputField rentInput;
    [SerializeField] private TMP_InputField waterInput;
    [SerializeField] private TMP_InputField electricInput;
    [SerializeField] private TMP_InputField salaryInput;
    [SerializeField] private TMP_InputField employeeCountInput;
    [SerializeField] private TMP_InputField sellPriceInput;

    [SerializeField] private TMP_InputField meatInput;
    [SerializeField] private TMP_InputField fruitInput;
    [SerializeField] private TMP_InputField carbInput;
    [SerializeField] private TMP_InputField vegInput;
    [SerializeField] private TMP_InputField seasoningInput;

    [Header("Dropdown Settings")]
    [SerializeField] private TMP_Dropdown mainMenuDropdown; // อ้างอิงถึงตัว Dropdown บน UI
    [SerializeField] private int defaultDropdownIndex = 1;  // กำหนดว่าจะให้ Dropdown เริ่มต้นที่ตัวเลือกไหน (0 = อันแรก, 1 = อันที่สอง)

    [Header("Default Costs Settings")]
    // ค่าเริ่มต้นพื้นฐานของธุรกิจ ที่เรากำหนดไว้ล่วงหน้า
    [SerializeField] private int baseRent = 30000;         // ค่าเช่าพื้นฐาน
    [SerializeField] private int baseWater = 2000;         // ค่าน้ำพื้นฐาน
    [SerializeField] private int baseElectric = 10000;     // ค่าไฟพื้นฐาน
    [SerializeField] private int salaryPerPerson = 12000;   // เงินเดือนพนักงาน 1 คน

    // ตัวแปรเช็คว่าโปรแกรมกำลังจัดฟอร์แมตตัวเลขอยู่หรือไม่ (ป้องกันบั๊กการจัดฟอร์แมตชนกันจนลูปค้าง)
    private bool isFormatting = false;

    private void Start()
    {
        // --- ส่วนจัดการ Dropdown ---
        if (mainMenuDropdown != null)
        {
            // ตั้งค่าให้ Dropdown เลือกตัวเลือกตาม defaultDropdownIndex ที่เราตั้งไว้ตอนเริ่มเกม
            mainMenuDropdown.value = defaultDropdownIndex;
            // สั่งให้อัปเดตข้อความบน UI ให้ตรงกับค่าที่เพิ่งเปลี่ยนไป
            mainMenuDropdown.RefreshShownValue(); 
        }

        // --- ส่วนจัดการ InputField แบบอ่านอย่างเดียว (ผู้เล่นกดพิมพ์แก้ไม่ได้) ---
        // เซ็ต readOnly เป็น true ให้แสดงผลได้อย่างเดียว
        if (rentInput != null) rentInput.readOnly = true;
        if (waterInput != null) waterInput.readOnly = true;
        if (electricInput != null) electricInput.readOnly = true;
        if (salaryInput != null) salaryInput.readOnly = true;

        // นำค่าเริ่มต้น (base) ที่ตั้งไว้มาแสดงในช่อง พร้อมจัดฟอร์แมตใส่ลูกน้ำ ("N0") 
        if (rentInput != null) rentInput.text = baseRent.ToString("N0");
        if (waterInput != null) waterInput.text = baseWater.ToString("N0");
        if (electricInput != null) electricInput.text = baseElectric.ToString("N0");

        // สร้าง Event: ถ้าช่อง "จำนวนพนักงาน" มีการเปลี่ยนแปลงตัวเลข ให้ไปเรียกฟังก์ชัน UpdateSalary ทันที
        if (employeeCountInput != null)
        {
            employeeCountInput.onValueChanged.AddListener(UpdateSalary);
        }

        // --- ส่วนตั้งค่าช่องกรอกตัวเลขแบบใส่ลูกน้ำอัตโนมัติ ---
        // เรียกฟังก์ชัน SetupAutoCommaInputField เพื่อตั้งค่าช่องต่างๆ ให้พิมพ์แล้วมีลูกน้ำคั่นหลักพัน
        SetupAutoCommaInputField(employeeCountInput, "1"); // พนักงานค่าเริ่มต้นที่ 1 คน
        SetupAutoCommaInputField(sellPriceInput, "60");
        SetupAutoCommaInputField(meatInput, "10");
        SetupAutoCommaInputField(fruitInput, "10");
        SetupAutoCommaInputField(carbInput, "10");
        SetupAutoCommaInputField(vegInput, "10");
        SetupAutoCommaInputField(seasoningInput, "10");

        // คำนวณเงินเดือนรวมครั้งแรกสุดตอนเปิดแอปฯ ขึ้นมา
        if (employeeCountInput != null)
        {
            UpdateSalary(employeeCountInput.text);
        }
    }

    // ฟังก์ชันสำหรับคำนวณ "เงินเดือนรวม" (จำนวนพนักงาน * เงินเดือนต่อคน)
    private void UpdateSalary(string countText)
    {
        int employeeCount = ParseToInt(countText); // ดึงข้อความในช่องมาแปลงเป็นตัวเลข
        int totalSalary = employeeCount * salaryPerPerson; // คำนวณยอดรวม

        // อัปเดตยอดรวมไปแสดงที่ช่อง salaryInput พร้อมใส่ลูกน้ำ
        if (salaryInput != null)
        {
            salaryInput.text = totalSalary.ToString("N0");
        }
    }

    // ฟังก์ชันตั้งค่าช่อง InputField เพื่อให้มันรองรับการใส่ลูกน้ำแบบอัตโนมัติ
    private void SetupAutoCommaInputField(TMP_InputField inputField, string defaultValue = "0")
    {
        if (inputField != null)
        {
            // ตั้งค่าประเภทการพิมพ์ให้เป็นแบบมาตรฐาน
            inputField.contentType = TMP_InputField.ContentType.Standard;
            
            // ใส่ค่าเริ่มต้นลงไป (ถ้าเป็น 0 ก็โชว์ "0", ถ้ามากกว่านั้นให้ใส่ลูกน้ำด้วย)
            int defVal = ParseToInt(defaultValue); 
            inputField.text = defVal == 0 ? "0" : defVal.ToString("N0");
            
            // เมื่อผู้เล่นพิมพ์ข้อความ ให้เรียกฟังก์ชัน FormatComma ทันที
            inputField.onValueChanged.AddListener((text) => FormatComma(inputField, text));
        }
    }

    // ฟังก์ชันหลักสำหรับจัดฟอร์แมตตัวเลข (ใส่เครื่องหมาย , หลักพัน/หมื่น/แสน) แบบเรียลไทม์เวลาผู้เล่นพิมพ์
    private void FormatComma(TMP_InputField inputField, string text)
    {
        if (isFormatting) return; // ถ้ากำลังจัดฟอร์แมตอยู่ให้เด้งออก เพื่อป้องกันมันเรียกซ้ำซ้อนจนพัง

        string onlyNumbers = "";
        // วนลูปอ่านตัวอักษรทีละตัว แล้วคัดเอาเฉพาะ "ตัวเลข" ออกมา (ตัดพวกลูกน้ำหรือตัวอักษรแปลกๆ ทิ้ง)
        foreach (char c in text) if (char.IsDigit(c)) onlyNumbers += c;

        // ลองแปลงข้อความตัวเลขที่คัดมาแล้ว ให้กลายเป็นตัวเลขจริงๆ (ใช้ long เพื่อรองรับตัวเลขเยอะๆ)
        if (long.TryParse(onlyNumbers, out long result))
        {
            // จัดฟอร์แมตใหม่ให้มีลูกน้ำ (เช่น 1000 กลายเป็น 1,000)
            string formattedText = result.ToString("N0");
            
            // ถ้าข้อความที่จัดฟอร์แมตแล้ว ไม่เหมือนกับที่พิมพ์อยู่ในช่อง ให้ทำการอัปเดตช่องใหม่
            if (inputField.text != formattedText)
            {
                isFormatting = true; // เปิดธงว่ากำลังทำฟอร์แมตอยู่นะ
                
                // --- โค้ดส่วนนี้คือการคำนวณตำแหน่งของตัวกะพริบ (Cursor / Caret) ---
                // เพื่อไม่ให้พิมพ์แล้วตัวกะพริบกระโดดหนีไปท้ายสุดเวลาลูกน้ำโผล่มา
                int cursorPosition = inputField.caretPosition;
                int digitsBeforeCursor = 0;
                if (cursorPosition == 0 && text.Length > 0) cursorPosition = 1;
                for (int i = 0; i < cursorPosition && i < text.Length; i++) if (char.IsDigit(text[i])) digitsBeforeCursor++;

                // เอาข้อความที่มีลูกน้ำใส่กลับเข้าไปในช่อง
                inputField.text = formattedText;

                // คำนวณจุดที่ควรจะวาง Cursor คืนกลับไป
                int newCursorPos = 0;
                int digitsCounted = 0;
                for (int i = 0; i < inputField.text.Length; i++)
                {
                    if (digitsCounted == digitsBeforeCursor) break;
                    if (char.IsDigit(inputField.text[i])) digitsCounted++;
                    newCursorPos++;
                }
                // ขยับ Cursor ไปตำแหน่งที่คำนวณได้
                inputField.caretPosition = newCursorPos;
                
                isFormatting = false; // ปิดธงทำฟอร์แมต
            }
        }
        else if (string.IsNullOrEmpty(onlyNumbers) && inputField.text != "")
        {
            // ถ้าผู้เล่นลบตัวเลขจนหมดช่องแล้ว ให้ช่องว่างเปล่าไปเลย
            isFormatting = true;
            inputField.text = "";
            isFormatting = false;
        }
    }

    // --- ส่วนการบันทึกข้อมูลเป็นไฟล์ JSON (ใช้ผูกกับปุ่ม Save บน UI ของคุณ) ---
    public void OnSubmitForm()
    {
        // 2. สร้างก้อนข้อมูลขึ้นมา 1 ชุด แล้วไล่ดึงค่าจากช่อง Input ต่างๆ มายัดใส่ 
        // (ข้อความจะต้องผ่าน ParseToInt เพื่อลบลูกน้ำออกก่อน ไม่งั้นจะเซฟเป็นตัวเลขไม่ได้)
        BusinessData data = new BusinessData();
        data.rent = ParseToInt(rentInput.text);
        data.water = ParseToInt(waterInput.text);
        data.electric = ParseToInt(electricInput.text);
        data.totalSalaryCost = ParseToInt(salaryInput.text);
        data.employeeCount = ParseToInt(employeeCountInput.text);
        data.sellPrice = ParseToInt(sellPriceInput.text);
        data.meat = ParseToInt(meatInput.text);
        data.fruit = ParseToInt(fruitInput.text);
        data.carb = ParseToInt(carbInput.text);
        data.veg = ParseToInt(vegInput.text);
        data.seasoning = ParseToInt(seasoningInput.text);
        
        // บันทึกค่า Dropdown ว่าผู้เล่นกำลังเลือก Option ไหนอยู่เป็นตัวเลข
        if (mainMenuDropdown != null)
        {
            data.selectedMenuOption = mainMenuDropdown.value;
        }
        
        // บันทึกเวลาปัจจุบันที่กดเซฟ
        data.saveTimestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // 3. แปลงก้อนข้อมูลให้กลายเป็นฟอร์แมต JSON (คำว่า true คือให้จัดเรียงบรรทัด JSON ให้อ่านง่ายๆ)
        string json = JsonUtility.ToJson(data, true);

        // 4. บันทึกลงไฟล์ (Application.persistentDataPath คือตำแหน่งโฟลเดอร์สำหรับเซฟเกมของเครื่องนั้นๆ)
        string savePath = Application.persistentDataPath + "/BusinessDataLog.json";
        File.WriteAllText(savePath, json);

        // แสดงผลในหน้าต่าง Console ของ Unity ว่าเซฟเสร็จแล้ว และปริ้นท์หน้าตาของ JSON ออกมาดู
        Debug.Log("บันทึกข้อมูลธุรกิจสำเร็จที่: " + savePath);
        Debug.Log("ข้อมูลที่บันทึก:\n" + json);
    }

    // ฟังก์ชันตัวช่วยสำหรับแปลงข้อความจากช่อง Input ให้กลายเป็นตัวเลขล้วน (Int) 
    // โดยมันจะทำหน้าที่เคลียร์ลูกน้ำทิ้งก่อนแปลงให้
    private int ParseToInt(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0; // ถ้าช่องว่างให้คืนค่าเป็น 0
        string cleanText = text.Replace(",", ""); // ลบเครื่องหมาย "," ออกให้หมด
        if (int.TryParse(cleanText, out int result)) return result; // พยายามแปลงข้อความให้เป็นตัวเลข
        return 0; // ถ้าแปลงไม่ได้ (เช่นเผลอพิมพ์ตัวอักษรปนมา) ให้คืนค่าเป็น 0
    }
}