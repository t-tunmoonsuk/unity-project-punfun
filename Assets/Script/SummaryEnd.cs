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
 
    [Header("Ingredient Unit Prices")]
    public int pricePerMeat = 15;
    public int pricePerFruit = 10;
    public int pricePerCarb = 5;
    public int pricePerVeg = 5;
    public int pricePerSeasoning = 2;
 
    [Header("Staff Settings")]
    public int platesPerEmployeePerDay = 50;
 
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
 

            int staffCapacity14Days = data.employeeCount * platesPerEmployeePerDay * 14;
 

            int platesFromIngredients = 0;
            if (data.selectedMenuOption == 0) 
            {
                platesFromIngredients = Mathf.Min(data.meat, data.carb, data.veg, data.seasoning);
            }
            else // ของหวาน
            {
                platesFromIngredients = Mathf.Min(data.fruit, data.carb);
            }
 
            int actualPlatesSold = Mathf.Min(platesFromIngredients, staffCapacity14Days);
            int totalIncome = actualPlatesSold * data.sellPrice;
 
            int monthlyFixedCosts = data.rent + data.water + data.electric + (data.employeeCount * 8000); 
            int twoWeeksFixedCosts = (monthlyFixedCosts * 14) / 30;
 
            int totalIngredientInvestment = (data.meat * pricePerMeat) + 
                                            (data.fruit * pricePerFruit) + 
                                            (data.carb * pricePerCarb) + 
                                            (data.veg * pricePerVeg) + 
                                            (data.seasoning * pricePerSeasoning);

            int totalCost = twoWeeksFixedCosts + totalIngredientInvestment;
            int profitAmount = totalIncome - totalCost;
 
            if (incomeText != null) incomeText.text = totalIncome.ToString("N0");
            if (costText != null) costText.text = totalCost.ToString("N0");
            if (profitText != null) profitText.text = profitAmount.ToString("N0");
 
            UpdateStatus(actualPlatesSold, staffCapacity14Days, platesFromIngredients, profitAmount);
        }
        else
        {
            Debug.LogWarning("ไม่พบไฟล์ JSON");
        }
    }
 
    private void UpdateStatus(int actualSold, int capacity, int fromIngredients, int profit)
    {
        if (statusText == null) return;
 
        if (fromIngredients <= 0)
        {
            statusText.text = "ร้านเปิดไม่ได้! วัตถุดิบไม่ครบสูตร";
            statusText.color = Color.red;
        }
        else if (fromIngredients > capacity)
        {
            statusText.text = "ของเน่าเสียเยอะมาก! พนักงานทำขายไม่ทันคนซื้อ";
            statusText.color = new Color32(255, 165, 0, 255); 
        }
        else if (profit > 0)
        {
            statusText.text = "บริหารร้านได้ยอดเยี่ยม! กำไรเน้นๆ ดำเนินกิจการต่อได้";
            statusText.color = Color.cyan;
        }
        else
        {
            statusText.text = "ขาดทุนยับเยิน! รายได้ไม่พอจ่ายค่าเช่าและค่าแรง";
            statusText.color = Color.red;
        }
    }
}