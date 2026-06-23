using UnityEngine;

public class MobileUIManager : MonoBehaviour
{
    public GameObject mobileCanvas;

    void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        // 📱 Always show on mobile
        mobileCanvas.SetActive(true);
#else
        // 💻 Always hide on PC
        mobileCanvas.SetActive(false);
#endif
    }
}