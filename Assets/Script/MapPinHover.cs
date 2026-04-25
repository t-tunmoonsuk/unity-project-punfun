using UnityEngine;
using UnityEngine.EventSystems;

public class MapPinHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject popupDialog;

    [Header("ตั้งค่าข้อมูลให้ตรงกับหมุด")]
    public int pinIndex; // ใส่เลข 0, 1 หรือ 2
    public LocationSelectionManager manager; // ลาก LocationPanel มาใส่

    void Start()
    {
        if (popupDialog != null) popupDialog.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // เอาเมาส์ชี้ เปิดป๊อปอัปเสมอ
        if (popupDialog != null) popupDialog.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ถ้าเอาเมาส์ออก จะปิดป๊อปอัปก็ต่อเมื่อ "หมุดนี้ไม่ได้ถูกคลิกเลือกอยู่" เท่านั้น
        if (manager != null && manager.GetSelectedIndex() != pinIndex)
        {
            if (popupDialog != null) popupDialog.SetActive(false);
        }
    }
}