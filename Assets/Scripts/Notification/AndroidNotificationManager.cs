using System.Collections;
using UnityEngine;

#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;          // ← ADD 1: needed for Permission API
#endif

public class AndroidNotificationManager : MonoBehaviour
{
    public static AndroidNotificationManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

#if UNITY_ANDROID
            RegisterChannel();
            StartCoroutine(RequestNotificationPermission()); // ← ADD 2: trigger popup
#endif
        }
        else
        {
            Destroy(gameObject);
        }
    }

#if UNITY_ANDROID
    void RegisterChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "leaderboard_channel",
            Name = "Leaderboard Updates",
            Importance = Importance.High,
            Description = "Game Score Notifications"
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    // ← ADD 3: the actual permission popup logic
    IEnumerator RequestNotificationPermission()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            yield return new WaitForSecondsRealtime(0.5f); // let the popup appear
        }

        yield return null;
    }
#endif

    public void SendNotification(string title, string text, int seconds)
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Debug.LogWarning("[Notifications] Permission not granted. Notification skipped.");
            return;
        }

        var notification = new AndroidNotification
        {
            Title = title,
            Text = text,
            FireTime = System.DateTime.Now.AddSeconds(seconds)
        };

        AndroidNotificationCenter.SendNotification(notification, "leaderboard_channel");
#endif
    }
}