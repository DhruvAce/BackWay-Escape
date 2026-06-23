using UnityEngine;
using UnityEngine.EventSystems;

public class CloseCanvasOnClick : MonoBehaviour
{
    [Header("Canvas to Close")]
    public GameObject targetCanvas;

    [Header("Optional: Prevent closing when clicking UI")]
    public bool ignoreUI = true;

    private bool isActive = true;

    void Update()
    {
        if (!isActive) return;

        // Mouse click (PC)
        if (Input.GetMouseButtonDown(0))
        {
            HandleClose(Input.mousePosition);
        }

        // Touch input (Mobile)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleClose(Input.GetTouch(0).position);
        }
    }

    void HandleClose(Vector2 inputPosition)
    {
        if (ignoreUI && EventSystem.current != null)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.touchCount > 0 &&
                EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return;
        }

        CloseCanvas();
    }

    public void CloseCanvas()
    {
        if (targetCanvas != null)
        {
            targetCanvas.SetActive(false);
            isActive = false;
        }
    }

    // Optional: call this from a UI Button directly
    public void CloseFromButton()
    {
        CloseCanvas();
    }

    public void ResetCanvas()
    {
        isActive = true;

        if (targetCanvas != null)
            targetCanvas.SetActive(true);
    }
}