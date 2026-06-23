using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveHighscore : MonoBehaviour
{
    public TMP_InputField nameInput;
    public GameObject highscorePopup;
    private bool saved = false;

    public void SaveScore()
{
    if(saved) return;

    saved = true;

    string playerName = nameInput.text.Trim();
    if(string.IsNullOrEmpty(playerName))
        playerName = "Player";

    float finalTime = 0f;

    if (GameTimer.Instance != null)
    {
        finalTime = GameTimer.Instance.GetFinalTime();
    }
    else
    {
        Debug.LogWarning("GameTimer missing, using 0 time");
    }

    if (LeaderboardManager.Instance != null)
    {
        LeaderboardManager.Instance.AddScore(playerName, finalTime);
    }
    else
    {
        Debug.LogError("LeaderboardManager missing in scene");
    }

    if (highscorePopup != null)
    {
        highscorePopup.SetActive(false);
    }

    Time.timeScale = 1f;

    // ONLY STORE SIGNAL FOR NEXT SCENE
    PlayerPrefs.SetInt("OpenLeaderboard", 1);

    SceneManager.LoadScene("MainMenu");
}

public void ResetSave()
{
    saved = false;

    if(nameInput != null)
        nameInput.text = "";
}
}