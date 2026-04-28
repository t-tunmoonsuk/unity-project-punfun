using UnityEngine;
using TMPro; 
using System.IO;

public class SummaryEnd : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI incomeText;  
    public TextMeshProUGUI costText;    
    public TextMeshProUGUI profitText;  
    public TextMeshProUGUI statusText; 

    [Header("Ingredient Unit Prices (ราคาต้นทุนต่อ 1 ชิ้น)")]
    public int pricePerMeat = 15;
    public int pricePerFruit = 10;
    public int pricePerCarb = 5;
    public int pricePerVeg = 5;
    public int pricePerSeasoning = 2;

    void Start()
    {
        LoadAndCalculateSummary();
    }

    public void LoadAndCalculateSummary()
    {
        string savePath = Application.persistentDataPath + "/BusinessDataLog.json";

        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            BusinessData data = JsonUtility.FromJson<BusinessData>(json);

            // 1. ค่าใช้จ่ายคงที่ 14 วัน (ค่าเช่า/น้ำ/ไฟ/เงินเดือน)
            int monthlyFixedCosts = data.rent + data.water + data.electric + data.totalSalaryCost;
            int twoWeeksFixedCosts = (monthlyFixedCosts / 30) * 14; 

            // 2. คำนวณจำนวนจานที่ทำได้จริง (หาว่าวัตถุดิบไหนมีน้อยสุด จานที่ทำได้จะไปตันที่ตรงนั้น)
            int maxPlates = Mathf.Min(data.meat, data.carb, data.veg, data.seasoning);

            // 3. รายได้ (จำนวนจาน x ราคาขาย)
            int totalIncome = maxPlates * data.sellPrice;

            // 4. ต้นทุนวัตถุดิบที่ผู้เล่นจ่ายเงินซื้อมาทั้งหมด (รวมพวกที่ซื้อมาเกินแล้วไม่ได้ใช้ด้วย)
            int ingredientCosts = (data.meat * pricePerMeat) + 
                                  (data.fruit * pricePerFruit) + 
                                  (data.carb * pricePerCarb) + 
                                  (data.veg * pricePerVeg) + 
                                  (data.seasoning * pricePerSeasoning);

            // 5. รวมต้นทุนทั้งหมด
            int totalCost = twoWeeksFixedCosts + ingredientCosts;

            // 6. กำไร
            int profitAmount = totalIncome - totalCost;

            // 7. แสดงผลบน UI
            if (incomeText != null) incomeText.text = totalIncome.ToString("N0");
            if (costText != null) costText.text = totalCost.ToString("N0");
            if (profitText != null) profitText.text = profitAmount.ToString("N0");

            // 8. เช็คสถานะ
            if (statusText != null)
            {
                if (maxPlates < 50) 
                {
                    statusText.text = "วัตถุดิบไม่พอขาย! ลูกค้าหนีหมด เจ๊งยับ!";
                    statusText.color = Color.red;
                }
                else if (profitAmount > 0)
                {
                    statusText.text = "บริหารร้านได้เยี่ยมมาก! คุณได้กำไร";
                    statusText.color = new Color32(70, 114, 184, 255); // สีฟ้า
                }
                else
                {
                    statusText.text = "รายได้ไม่พอจ่ายค่าเช่า... ร้านเจ๊งครับ!";
                    statusText.color = Color.red;
                }
            }
        }
        else
        {
            Debug.LogWarning("ไม่พบไฟล์ JSON");
        }
    }
}