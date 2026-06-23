using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    public TMP_Text[] rankTexts;

    private void Start()
    {
        RefreshLeaderboard();
    }

        void OnEnable()
    {
        RefreshLeaderboard();
    }

    public void RefreshLeaderboard()
    {
        if (LeaderboardManager.Instance == null)
        {
        Debug.LogWarning("LeaderboardManager not ready.");
        return;
        }

        for(int i=0;i<rankTexts.Length;i++)
        {
            rankTexts[i].text = "";
        }

        if (LeaderboardManager.Instance == null)
        {
            Debug.LogWarning("LeaderboardManager not loaded yet");
            return;
        }

        int count =
            Mathf.Min(
            LeaderboardManager.Instance.entries.Count,
            rankTexts.Length);

        for(int i=0;i<count;i++)
        {
            LeaderboardEntry entry =
                LeaderboardManager.Instance.entries[i];

            int minutes =
                Mathf.FloorToInt(entry.completionTime / 60);

            int seconds =
                Mathf.FloorToInt(entry.completionTime % 60);

            int milliseconds =
                Mathf.FloorToInt(
                (entry.completionTime * 100) % 100);

            string time =
                string.Format(
                "{0:00}:{1:00}.{2:00}",
                minutes,
                seconds,
                milliseconds);

            rankTexts[i].text =
                "#" + (i + 1)
                + " "
                + entry.playerName
                + " "
                + time;

        int savedRank = PlayerPrefs.GetInt("LastSavedRank", -1);

        if(i == savedRank)
        {
            rankTexts[i].color = Color.yellow;
        }
        else
        {
            rankTexts[i].color = Color.white;
        }
        }
        }
}