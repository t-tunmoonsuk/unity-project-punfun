using System.Collections; // จำเป็นต้องใส่เพื่อใช้งาน Coroutine (IEnumerator)
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayGame2UmbrellaDropZone : MonoBehaviour, IDropHandler
{
    [Header("จุดยืนใต้ร่ม")]
    public Transform[] safeSlots;
    private int currentOccupiedSlots = 0;

    [Header("UI Fade Settings")]
    public CanvasGroup playGameCanvasGroup;
    public CanvasGroup gamePassCanvasGroup;
    public float fadeDuration = 0.5f;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            PlayGame2DraggableCharacter character = eventData.pointerDrag.GetComponent<PlayGame2DraggableCharacter>();
            
            if (character != null && !character.isSafe && currentOccupiedSlots < safeSlots.Length)
            {
                character.SetSafeStatus(safeSlots[currentOccupiedSlots]);
                
                currentOccupiedSlots++; 
                
                Debug.Log("เข้ามาหลบฝนแล้ว! รวม: " + currentOccupiedSlots + " คน");
                
                if (currentOccupiedSlots >= 3)
                {
                    Debug.Log("currentOccupiedSlots is 3");
                    StartCoroutine(FadeOutAndShowPassScreen());
                }
            }
        }
    }

    private IEnumerator FadeOutAndShowPassScreen()
    {
        yield return new WaitForSeconds(0.5f);

        if (gamePassCanvasGroup != null)
        {
            gamePassCanvasGroup.gameObject.SetActive(true);
            gamePassCanvasGroup.alpha = 0f;
        }

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            if (playGameCanvasGroup != null)
            {
                playGameCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            }

            if (gamePassCanvasGroup != null)
            {
                gamePassCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            }

            yield return null;
        }

        if (playGameCanvasGroup != null)
        {
            playGameCanvasGroup.alpha = 0f;
            playGameCanvasGroup.interactable = false;
            playGameCanvasGroup.blocksRaycasts = false;
            playGameCanvasGroup.gameObject.SetActive(false);
        }

        if (gamePassCanvasGroup != null)
        {
            gamePassCanvasGroup.alpha = 1f;
            gamePassCanvasGroup.interactable = true;
            gamePassCanvasGroup.blocksRaycasts = true;
        }
    }
}