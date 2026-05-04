using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ContinueIndicatorAnim : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private Vector3 startPos;
    private float timer;

    [Header("ความเร็วของลูกเล่น")]
    [Tooltip("ตัวเลขยิ่งเยอะ ยิ่งขยับ/กระพริบเร็ว (แนะนำ 3-5)")]
    public float speed = 4f;

    [Header("ลูกเล่นกระพริบนุ่มนวล (Breathing)")]
    [Tooltip("ความจางระดับต่ำสุด (0 = มองไม่เห็นเลย)")]
    public float minAlpha = 0.2f;
    [Tooltip("ความสว่างระดับสูงสุด (1 = สว่าง 100%)")]
    public float maxAlpha = 1f;

    [Header("ลูกเล่นลอยขึ้นลง (Floating)")]
    [Tooltip("ระยะการเด้งขึ้นลง (ใส่ 0 ถ้าอยากให้อยู่เฉยๆ แนะนำ 5-10)")]
    public float floatAmount = 5f;

    void Awake()
    {
        // ดึง TextMeshPro มาเตรียมไว้
        textMesh = GetComponent<TextMeshProUGUI>();
        // จำตำแหน่งเริ่มต้นไว้
        startPos = transform.localPosition;
    }

    void OnEnable()
    {
        // รีเซ็ตเวลาทุกครั้งที่โผล่ขึ้นมา เพื่อให้แอนิเมชันเริ่มใหม่เนียนๆ
        timer = 0f;
        transform.localPosition = startPos;
    }

    void Update()
    {
        // เพิ่มเวลาไปเรื่อยๆ คูณด้วยความเร็ว
        timer += Time.deltaTime * speed;

        // 1. ลูกเล่นเฟดสว่าง/จาง (ใช้สูตรคณิตศาสตร์ Sine Wave ให้มันสมูท)
        if (textMesh != null)
        {
            // แปลงค่า Sine (ที่เด้งไปมาระหว่าง -1 ถึง 1) ให้เป็นค่าระหว่าง minAlpha ถึง maxAlpha
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(timer) + 1f) / 2f);

            Color c = textMesh.color;
            c.a = alpha;
            textMesh.color = c;
        }

        // 2. ลูกเล่นลอยขึ้นลงสมูทๆ
        if (floatAmount > 0)
        {
            float newY = startPos.y + (Mathf.Sin(timer) * floatAmount);
            transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
        }
    }
}