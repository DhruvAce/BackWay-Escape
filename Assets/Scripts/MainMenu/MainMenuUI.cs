using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public GameObject firstButton;

    void Start()
    {
        Invoke(nameof(SetFirstSelection), 0.1f);


    }

    void Update()
    {
        // 🎮 Gamepad confirm (A / Cross)
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            ActivateSelected();
        }

        // ⌨ Keyboard Enter / Space
        if (Keyboard.current != null &&
           (Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame))
        {
            ActivateSelected();
        }
    }

    void SetFirstSelection()
    {
        if (firstButton == null) return;
        if (EventSystem.current == null) return;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);

        Button btn = firstButton.GetComponent<Button>();
        if (btn != null)
        {
            btn.Select();
        }
    }

    void ActivateSelected()
    {
        if (EventSystem.current == null) return;

        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null) return;

        selected.GetComponent<Button>()?.onClick.Invoke();
    }

    public void PlayGame()
    {
        PlayerPrefs.SetInt("ShowIntroPanel", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Game");
    }

    public void OpenCharacterCustomizer()
    {
        SceneManager.LoadScene("CharacterCustomization");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }


}