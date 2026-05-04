using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [Header("ลาก UI มาใส่ตรงนี้")]
    public Image timeBarFill;       // ช่องสำหรับหลอดสีน้ำเงิน
    public TextMeshProUGUI timeText;// ช่องสำหรับตัวหนังสือเวลา

    [Header("ตั้งค่าเวลา (วินาที)")]
    public float maxTime = 180f;    // 180 วินาที = 3 นาที
    private float currentTime;

    // ใช้ OnEnable แทน Start เพราะเราอยากให้นับเวลา "ทันทีที่หน้า GameplayUI โชว์ขึ้นมา"
    void OnEnable()
    {
        currentTime = maxTime;
    }

    void Update()
    {
        // ถ้ายเวลามากกว่า 0 ให้นับถอยหลังเรื่อยๆ
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            // ถ้าเวลาหมด ให้หยุดที่ 0
            if (currentTime <= 0)
            {
                currentTime = 0;
                Debug.Log("หมดเวลาเปิดร้านแล้ว!");
                // เดี๋ยวอนาคตเรามาใส่คำสั่งจบเกมตรงนี้ได้ครับ
            }

            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        // 1. อัปเดตความยาวหลอดสีน้ำเงิน (เอาเวลาปัจจุบัน หาร เวลาเต็ม จะได้ค่า 0.0 - 1.0)
        if (timeBarFill != null)
        {
            timeBarFill.fillAmount = currentTime / maxTime;
        }

        // 2. แปลงเวลาให้เป็นตัวหนังสือรูปแบบ นาที:วินาที (MM:SS)
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}