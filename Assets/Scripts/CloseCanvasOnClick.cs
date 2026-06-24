using UnityEngine;
using UnityEngine.EventSystems;

public class CloseCanvasOnClick : MonoBehaviour
{
    [Header("Canvas to Close")]
    public GameObject targetCanvas;

    [Header("Optional: Prevent closing when clicking UI")]
    public bool ignoreUI = true;

    private bool isActive = false;

    void Start()
    {
        // Show panel only when game starts from Main Menu
        if (PlayerPrefs.GetInt("ShowIntroPanel", 0) == 1)
        {
            if (targetCanvas != null)
                targetCanvas.SetActive(true);

            isActive = true;

            // Pause game while panel is open
            Time.timeScale = 0f;

            // Remove flag so restart won't show it again
            PlayerPrefs.SetInt("ShowIntroPanel", 0);
            PlayerPrefs.Save();
        }
        else
        {
            if (targetCanvas != null)
                targetCanvas.SetActive(false);

            isActive = false;

            Time.timeScale = 1f;
        }
    }

    void Update()
    {
        if (!isActive) return;

        // Mouse click (PC)
        if (Input.GetMouseButtonDown(0))
        {
            HandleClose();
        }

        // Touch input (Mobile)
        if (Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleClose();
        }
    }

    void HandleClose()
    {
        if (ignoreUI && EventSystem.current != null)
        {
            if (Input.touchCount > 0)
            {
                if (EventSystem.current.IsPointerOverGameObject(
                    Input.GetTouch(0).fingerId))
                    return;
            }
            else
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
            }
        }

        CloseCanvas();
    }

    public void CloseCanvas()
    {
        if (targetCanvas != null)
        {
            targetCanvas.SetActive(false);
        }

        isActive = false;

        // Resume game
        Time.timeScale = 1f;
    }

    public void CloseFromButton()
    {
        CloseCanvas();
    }

    public void ResetCanvas()
    {
        isActive = true;

        if (targetCanvas != null)
            targetCanvas.SetActive(true);

        Time.timeScale = 0f;
    }
}