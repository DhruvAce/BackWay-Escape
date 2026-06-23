using UnityEngine;



#if UNITY_ANDROID

using Unity.Notifications.Android;

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

#endif



    public void SendNotification(string title, string text, int seconds)

    {

#if UNITY_ANDROID

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