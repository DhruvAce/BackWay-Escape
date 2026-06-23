using UnityEngine;

public class KeepScreenAwake : MonoBehaviour
{
    public static KeepScreenAwake instance;

    [Header("Screen Sleep Settings")]
    public bool keepAwakeOnAndroid = true;

    private void Awake()
    {
        // Singleton so only one exists
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

#if UNITY_ANDROID && !UNITY_EDITOR
        EnableAwake();
#else
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
#endif
    }

    private void EnableAwake()
    {
        if (keepAwakeOnAndroid)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Debug.Log("Screen Sleep Disabled (Android)");
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (focus && keepAwakeOnAndroid)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
#endif
    }

    private void OnApplicationPause(bool pause)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!pause && keepAwakeOnAndroid)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
#endif
    }
}