using System.Collections;
using UnityEngine;

public class SlidePanel : MonoBehaviour
{
    public RectTransform panel;

    public Vector2 openPosition = Vector2.zero;
    public Vector2 closedPosition = new Vector2(450, 0);

    public float animationTime = 0.25f;

    private Coroutine current;

    void Awake()
    {
        if(panel == null)
            panel = GetComponent<RectTransform>();
    }

    void Start()
    {
        panel.anchoredPosition = closedPosition;

        if(PlayerPrefs.GetInt("OpenLeaderboard", 0) == 1)
        {
            PlayerPrefs.SetInt("OpenLeaderboard", 0);
            OpenPanel();
        }
    }
    
    public void OpenPanel()
    {
        if(current != null) StopCoroutine(current);
        current = StartCoroutine(Move(openPosition));
    }

    public void ClosePanel()
    {
        if(current != null) StopCoroutine(current);
        current = StartCoroutine(Move(closedPosition));
    }

    IEnumerator Move(Vector2 target)
    {
        Vector2 start = panel.anchoredPosition;
        float t = 0;

        while(t < animationTime)
        {
            t += Time.unscaledDeltaTime;
            panel.anchoredPosition = Vector2.Lerp(start, target, t / animationTime);
            yield return null;
        }

        panel.anchoredPosition = target;
    }
    public void TogglePanel()
{
    StopAllCoroutines();

    if (panel.anchoredPosition == openPosition)
    {
        ClosePanel();
    }
    else
    {
        OpenPanel();
    }
}
}