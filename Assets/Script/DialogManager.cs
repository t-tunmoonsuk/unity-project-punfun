using System.Collections;
using UnityEngine;
using UnityEngine.Events; 
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class DialogManager : MonoBehaviour
{
    [Header("UI ข้อความ")]
    public TextMeshProUGUI dialogText;
    public GameObject continueIndicator;

    [Header("เหตุการณ์เมื่อคุยจบ (ระดับโปร)")]
    public UnityEvent onDialogComplete; 

    private CanvasGroup canvasGroup;

    [Header("ตั้งค่าความสมูท (ระดับโปร)")]
    public float putissTypingSpeed = 0.02f;
    public float maxCharacterFadeDuration = 0.2f;
    public float windowFadeSpeed = 0.3f;

    [Header("บทสนทนาทั้งหมด")]
    [TextArea(3, 5)]
    public string[] sentences;

    private int currentIndex;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        currentIndex = 0;
        dialogText.text = "";
        canvasGroup.alpha = 0f;
        if (continueIndicator != null) continueIndicator.SetActive(false);
        if (sentences.Length > 0) StartCoroutine(FadeInWindowAndStart());
    }

    IEnumerator FadeInWindowAndStart()
    {
        float t = 0;
        while (t < windowFadeSpeed)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / windowFadeSpeed);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        StartTyping(sentences[currentIndex]);
    }

    public void NextSentence()
    {
        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            ForceShowAllText();
            isTyping = false;
            return;
        }

        currentIndex++;
        if (currentIndex < sentences.Length)
        {
            StartTyping(sentences[currentIndex]);
        }
        else
        {
            if (continueIndicator != null) continueIndicator.SetActive(false);
            StartCoroutine(FadeOutWindowAndClose());
        }
    }

    IEnumerator FadeOutWindowAndClose()
    {
        float t = 0;
        while (t < windowFadeSpeed)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / windowFadeSpeed);
            yield return null;
        }
        canvasGroup.alpha = 0f;

        // --- ส่วนสำคัญ: เมื่อจางหายจนจบแล้ว ให้สั่งรันเหตุการณ์ถัดไปทันที ---
        if (onDialogComplete != null)
        {
            onDialogComplete.Invoke();
        }

        gameObject.SetActive(false);
    }

    void StartTyping(string sentence)
    {
        if (continueIndicator != null) continueIndicator.SetActive(false);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentenceSmooth(sentence));
    }

    IEnumerator TypeSentenceSmooth(string sentence)
    {
        isTyping = true;
        dialogText.text = sentence;
        dialogText.maxVisibleCharacters = sentence.Length;
        yield return new WaitForEndOfFrame();
        dialogText.ForceMeshUpdate();

        int totalCharacters = dialogText.textInfo.characterCount;
        float[] charAlphas = new float[totalCharacters];
        for (int i = 0; i < totalCharacters; i++) SetCharacterAlpha(i, 0);
        dialogText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        float time = 0;
        int currentChar = 0;
        while (currentChar < totalCharacters || charAlphas[totalCharacters - 1] < 1f)
        {
            time += Time.deltaTime;
            if (currentChar < totalCharacters && time >= putissTypingSpeed)
            {
                currentChar++;
                time = 0;
            }
            bool needsUpdate = false;
            for (int i = 0; i < currentChar; i++)
            {
                if (charAlphas[i] < 1f)
                {
                    charAlphas[i] += Time.deltaTime / maxCharacterFadeDuration;
                    charAlphas[i] = Mathf.Clamp01(charAlphas[i]);
                    SetCharacterAlpha(i, (byte)(charAlphas[i] * 255));
                    needsUpdate = true;
                }
            }
            if (needsUpdate) dialogText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            yield return null;
        }
        isTyping = false;
        if (continueIndicator != null) continueIndicator.SetActive(true);
    }

    void ForceShowAllText()
    {
        int totalChars = dialogText.textInfo.characterCount;
        for (int i = 0; i < totalChars; i++) SetCharacterAlpha(i, 255);
        dialogText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        if (continueIndicator != null) continueIndicator.SetActive(true);
    }

    void SetCharacterAlpha(int charIndex, byte alpha)
    {
        TMP_TextInfo textInfo = dialogText.textInfo;
        if (!textInfo.characterInfo[charIndex].isVisible) return;
        int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;
        Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;
        vertexColors[vertexIndex + 0].a = alpha;
        vertexColors[vertexIndex + 1].a = alpha;
        vertexColors[vertexIndex + 2].a = alpha;
        vertexColors[vertexIndex + 3].a = alpha;
    }
}