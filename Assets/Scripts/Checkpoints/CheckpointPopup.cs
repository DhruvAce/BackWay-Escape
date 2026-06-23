using System.Collections;
using TMPro;
using UnityEngine;

public class CheckpointPopup : MonoBehaviour
{
    public static CheckpointPopup Instance;

    [Header("UI")]
    public TMP_Text popupText;

    [Header("Message")]
    public string message = "Checkpoint Saved!";

    [Header("Timing")]
    public float visibleTime = 1.5f;

    [Header("Animation")]
    public float popScale = 1.2f;
    public float popDuration = 0.15f;

    public float moveUpDistance = 40f;
    public float fadeDuration = 0.5f;

    private RectTransform rect;
    private CanvasGroup canvasGroup;

    Vector2 startPos;

    void Awake()
    {
        Instance = this;

        rect = popupText.GetComponent<RectTransform>();

        canvasGroup = popupText.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = popupText.gameObject.AddComponent<CanvasGroup>();

        startPos = rect.anchoredPosition;

        canvasGroup.alpha = 0f;
    }

    public void ShowPopup()
    {
        StopAllCoroutines();
        StartCoroutine(PopupRoutine());
    }

    IEnumerator PopupRoutine()
    {
        popupText.text = message;

        rect.anchoredPosition = startPos;

        canvasGroup.alpha = 1f;

        // POP
        float t = 0;

        while (t < popDuration)
        {
            t += Time.deltaTime;

            float p = t / popDuration;

            float scale = Mathf.Lerp(0.7f, popScale, p);

            rect.localScale = Vector3.one * scale;

            yield return null;
        }

        rect.localScale = Vector3.one;

        // STAY
        yield return new WaitForSeconds(visibleTime);

        // MOVE UP + FADE
        t = 0;

        Vector2 endPos = startPos + Vector2.up * moveUpDistance;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float p = t / fadeDuration;

            rect.anchoredPosition =
                Vector2.Lerp(startPos, endPos, p);

            canvasGroup.alpha =
                Mathf.Lerp(1f, 0f, p);

            yield return null;
        }

        canvasGroup.alpha = 0f;

        rect.anchoredPosition = startPos;
    }
}