using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;

    private bool isPaused = false;

    public GameObject firstSelectedButton;

    [Header("UI Canvas Toggle")]
    public GameObject canvasToToggle;

    [Header("Android Tilt Settings")]
    public GameObject tiltSettingsPanel;
    public Slider sensitivitySlider;
    public PlayerMovement playerMovement;

    [Header("Checkpoint")]
    public GameObject ball;
    public GameObject player;

    void Awake()
    {

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    void Start()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

    #if UNITY_ANDROID && !UNITY_EDITOR
        if (canvasToToggle != null)
            canvasToToggle.SetActive(true);
    #else
        if (canvasToToggle != null)
            canvasToToggle.SetActive(false);
    #endif

    #if UNITY_ANDROID && !UNITY_EDITOR
        if (tiltSettingsPanel != null)
            tiltSettingsPanel.SetActive(true);
    #else
        if (tiltSettingsPanel != null)
            tiltSettingsPanel.SetActive(false);
    #endif

        sensitivitySlider.value = playerMovement.tiltSensitivity;
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);

        // 🔥 ANDROID SAFE DELAY FIX (NO SECOND START NEEDED)
    #if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(AndroidUIFix());
    #endif
    }

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }

        if (Gamepad.current != null &&
            Gamepad.current.startButton.wasPressedThisFrame)
        {
            // IGNORE FIRST FRAME SPAWN BUG
            if (Time.unscaledTime < 0.5f) return;

            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);

#if UNITY_ANDROID && !UNITY_EDITOR
        if (canvasToToggle != null)
        {
            canvasToToggle.SetActive(false);
        }
#endif

        if (firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);

#if UNITY_ANDROID && !UNITY_EDITOR
        if (canvasToToggle != null)
        {
            canvasToToggle.SetActive(true);
        }
#endif
    }

    public void ResetGameState()
    {
        Time.timeScale = 1f;

        isPaused = false;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

    #if UNITY_ANDROID && !UNITY_EDITOR
        if (canvasToToggle != null)
            canvasToToggle.SetActive(true);
    #else
        if (canvasToToggle != null)
            canvasToToggle.SetActive(false);
    #endif

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void SetSensitivity(float value)
    {
        playerMovement.tiltSensitivity = value;

        PlayerPrefs.SetFloat("TiltSensitivity", value);
        PlayerPrefs.Save();
    }

    public void ForceReset()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

    #if UNITY_ANDROID && !UNITY_EDITOR
        if (canvasToToggle != null)
            canvasToToggle.SetActive(true);
    #else
        if (canvasToToggle != null)
            canvasToToggle.SetActive(false);
    #endif
    }

    private IEnumerator AndroidUIFix()
    {
        yield return null; // wait 1 frame

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }

public void RespawnAtCheckpoint(GameObject ball)
{
    ResetGameState();

    if (CheckpointManager.Instance != null &&
        CheckpointManager.Instance.hasCheckpoint)
    {
        // PLAYER
        CharacterController cc =
            player.GetComponent<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;

            player.transform.position =
                CheckpointManager.Instance.savedPlayerPosition;

            cc.enabled = true;
        }
        else
        {
            player.transform.position =
                CheckpointManager.Instance.savedPlayerPosition;
        }

        // BALL
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            ball.transform.position =
                CheckpointManager.Instance.savedBallPosition;

            rb.position =
                CheckpointManager.Instance.savedBallPosition;

            rb.isKinematic = false;

            rb.WakeUp();
        }
        else
        {
            ball.transform.position =
                CheckpointManager.Instance.savedBallPosition;
        }

        // RESTORE TIMER
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.RestoreCheckpointTime(
                CheckpointManager.Instance.savedTime
            );
        }
    }
    else
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    if (canvasToToggle != null)
        canvasToToggle.SetActive(true);
#endif
}
public void RestartGame()
{
    Time.timeScale = 1f;

    ResetGameState();

    RespawnAtCheckpoint(ball);
}
}
