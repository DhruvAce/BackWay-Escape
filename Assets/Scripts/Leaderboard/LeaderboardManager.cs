using UnityEngine;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    private const string SAVE_KEY = "Leaderboard";
    public int lastSavedRank = -1;

    public List<LeaderboardEntry> entries =
        new List<LeaderboardEntry>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            LoadLeaderboard();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool Qualifies(float newTime)
    {
        if (entries.Count < 5)
            return true;

        return newTime < entries[4].completionTime;
    }

    public void AddScore(string playerName, float time)
    {
        LeaderboardEntry entry = new LeaderboardEntry();

        entry.playerName = playerName;
        entry.completionTime = time;

        entries.Add(entry);
        Debug.Log("Saved: " + playerName + "  Time: " + time);

        entries.Sort(
            (a, b) => a.completionTime.CompareTo(b.completionTime));

        lastSavedRank = entries.IndexOf(entry);

        PlayerPrefs.SetInt("LastSavedRank", lastSavedRank);
        PlayerPrefs.Save();

        if (entries.Count > 5)
        {
            entries.RemoveAt(5);
        }

        SaveLeaderboard();
    }

    void SaveLeaderboard()
    {
        LeaderboardData data =
            new LeaderboardData();

        data.entries = entries;

        string json =
            JsonUtility.ToJson(data);

        PlayerPrefs.SetString(
            SAVE_KEY,
            json);

        PlayerPrefs.Save();
    }

    void LoadLeaderboard()
    {
        if(PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json =
                PlayerPrefs.GetString(SAVE_KEY);

            LeaderboardData data =
                JsonUtility.FromJson<LeaderboardData>(json);

        if (data != null && data.entries != null)
        {
            entries = data.entries;
        }
        else
        {
            entries = new List<LeaderboardEntry>();
        }
        }
    }
    
        [ContextMenu("Reset Leaderboard")]
    public void ResetLeaderboard()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();

        entries.Clear();
        lastSavedRank = -1;

        Debug.Log("Leaderboard Reset Done");
    }
}