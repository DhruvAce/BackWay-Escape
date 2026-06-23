using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinArea : MonoBehaviour
{
    [Header("UI")]
    public GameObject winPanel;

    [Header("FX")]
    public GameObject winFXPrefab;

    [Header("Delay")]
    public float winDelay = 1.5f;

    [Header("Player")]
    public MonoBehaviour playerMovement;

    [Header("Preview Character")]
    public Animator previewAnimator;

    [Header("Hide On Win")]
    public GameObject canvasToHide;

    [Header("Pause")]
    public PauseManager pauseManager;

    public GameObject highscorePopup;

    [Header("Win Buttons")]
    public GameObject retryButton;
    public GameObject menuButton;

    public GameObject ball;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip winSFX;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.8f, 1.2f)] public float pitchMin = 0.95f;
    [Range(0.8f, 1.2f)] public float pitchMax = 1.05f;

    private bool hasWon = false;

    void Awake()
    {
        if (pauseManager != null)
            pauseManager.ForceReset();
    }

    void Start()
    {
        if (pauseManager != null)
            pauseManager.ForceReset();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasWon) return;

        if (other.CompareTag("Ball"))
        {
            hasWon = true;
            StartCoroutine(WinSequence());
        }
    }

    IEnumerator WinSequence()
    {
        if (GameTimer.Instance == null)
        {
            Debug.LogError("GameTimer is missing in scene!");
            yield break;
        }

        // STOP TIMER IMMEDIATELY
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.StopTimer();
        }

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (winFXPrefab != null)
        {
            Instantiate(
                winFXPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        // 🔊 WIN SOUND PLAY
        PlayWinSound();

        yield return new WaitForSeconds(winDelay);

        if (canvasToHide != null)
        {
            canvasToHide.SetActive(false);
        }

        if (pauseManager != null)
        {
            pauseManager.ForceReset();
            pauseManager.enabled = false;
        }

        winPanel.SetActive(true);

        if (retryButton != null)
            retryButton.SetActive(true);

        if (menuButton != null)
            menuButton.SetActive(true);

        if (GameTimer.Instance != null &&
            LeaderboardManager.Instance != null)
        {
            float finalTime = GameTimer.Instance.GetFinalTime();

            if (LeaderboardManager.Instance.Qualifies(finalTime))
            {
                if (highscorePopup != null)
                    highscorePopup.SetActive(true);
            }
        }

        if (previewAnimator != null)
        {
            previewAnimator.SetTrigger("PlayWin");
        }

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 🔊 WIN SOUND FUNCTION
    void PlayWinSound()
    {
        if (audioSource == null || winSFX == null) return;

        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.PlayOneShot(winSFX, volume);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void RespawnAtCheckpoint(GameObject ball)
    {
        if (CheckpointManager.Instance != null &&
            CheckpointManager.Instance.hasCheckpoint)
        {
            ball.transform.position =
                CheckpointManager.Instance.savedBallPosition;

            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (GameTimer.Instance != null)
            {
                GameTimer.Instance.StartTimer();
            }
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.ResetTimer();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}