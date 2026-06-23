using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCustomizerUI : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
}