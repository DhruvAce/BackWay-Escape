using UnityEngine;

public class AppExitNotifier : MonoBehaviour
{
    private const string LAST_EXIT_TIME = "LastExitTime";

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            ScheduleSmartNotification();
            SaveExitTime();
        }
    }

    private void OnApplicationQuit()
    {
        ScheduleSmartNotification();
        SaveExitTime();
    }

    void SaveExitTime()
    {
        PlayerPrefs.SetString(LAST_EXIT_TIME, System.DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    void ScheduleSmartNotification()
    {
        // 🔥 PLATFORM SAFETY FIX
#if UNITY_ANDROID
        if (AndroidNotificationManager.Instance == null) return;
#else
        return; // Windows / Editor will skip safely
#endif

        if (LeaderboardManager.Instance == null) return;

        var lb = LeaderboardManager.Instance.entries;

        string title = "🎮 Come back!";
        string message;

        // ---------------- EMPTY LEADERBOARD ----------------
        if (lb == null || lb.Count == 0)
        {
            message = "Be the first one to make a highscore 🏆";
        }
        else
        {
            var best = lb[0];

            int myRank = LeaderboardManager.Instance.lastSavedRank;

            if (myRank < 0) myRank = lb.Count;

            int displayRank = myRank + 1;

            if (displayRank == 1)
            {
                message = "🔥 You are #1 right now!\nDefend your highscore!";
            }
            else if (displayRank <= 3)
            {
                message = "🏃 You are rank #" + displayRank +
                          "!\nClose to #1 — improve your time!";
            }
            else if (displayRank <= 5)
            {
                message = "⚡ You are in Top 5!\nPush harder to reach #1!";
            }
            else
            {
                message = best.playerName +
                          " is leading with " +
                          best.completionTime.ToString("F2") +
                          "s 🏆\nCan you beat this score?";
            }
        }

#if UNITY_ANDROID
        AndroidNotificationManager.Instance.SendNotification(
            title,
            message,
            10
        );
#endif
    }

    private void Start()
    {
        CheckReturnAfterTimeGap();
    }

    void CheckReturnAfterTimeGap()
    {
#if !UNITY_ANDROID
        return;
#endif

        if (!PlayerPrefs.HasKey(LAST_EXIT_TIME)) return;

        System.DateTime lastExit;

        if (!System.DateTime.TryParse(PlayerPrefs.GetString(LAST_EXIT_TIME), out lastExit))
            return;

        double hoursAway = (System.DateTime.Now - lastExit).TotalHours;

        if (hoursAway >= 6)
        {
            string msg = "Your ball is waiting ⚽\nCome back and beat the leaderboard!";

            if (LeaderboardManager.Instance != null &&
                LeaderboardManager.Instance.entries.Count == 0)
            {
                msg = "Be the first champion 🏆\nNo highscores yet!";
            }

#if UNITY_ANDROID
            AndroidNotificationManager.Instance.SendNotification(
                "We missed you 🎮",
                msg,
                5
            );
#endif
        }
    }
}