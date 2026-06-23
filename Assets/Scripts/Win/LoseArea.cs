using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseArea : MonoBehaviour
{
    [Header("UI")]
    public GameObject losePanel;

    [Header("Delay")]
    public float loseDelay = 1f;

    [Header("Player")]
    public MonoBehaviour playerMovement;
    public GameObject player;
    public GameObject ball;

    private bool pauseWasDisabled = false;

    [Header("Preview Character")]
    public Animator previewAnimator;

    [Header("Lose Animation Trigger Name")]
    public string loseTriggerName = "PlayLose";

    [Header("UI Hide")]
    public GameObject canvasToHide;

    [Header("Pause")]
    public PauseManager pauseManager;

    private bool hasLost = false;

    void Start()
    {
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.OnTimeUp += TriggerLose;
        }
    }

    void OnDestroy()
    {
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.OnTimeUp -= TriggerLose;
        }
    }

    void TriggerLose()
    {
        if (hasLost) return;
        StartCoroutine(LoseSequence());
    }

    IEnumerator LoseSequence()
    {
        hasLost = true;

        // allow animation system to run
        Time.timeScale = 1f;

        yield return new WaitForSeconds(loseDelay);

        if (GameTimer.Instance != null)
            GameTimer.Instance.StopTimer();

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (canvasToHide != null)
            canvasToHide.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(true);


        if (previewAnimator != null)
        {
            previewAnimator.ResetTrigger(loseTriggerName);
            previewAnimator.SetTrigger(loseTriggerName);
        }

        if (pauseManager != null)
        {
            pauseWasDisabled = true;
            pauseManager.gameObject.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // freeze AFTER everything starts
        Time.timeScale = 0f;
    }

    // BUTTON 1 → retry from checkpoint
    public void RetryCheckpoint()
    {
        Time.timeScale = 1f;

        if (pauseManager != null && pauseWasDisabled)
        {
            pauseManager.gameObject.SetActive(true);
            pauseWasDisabled = false;
        }

        if (CheckpointManager.Instance != null &&
            CheckpointManager.Instance.hasCheckpoint)
        {
            ball.transform.position =
                CheckpointManager.Instance.savedBallPosition;

            CharacterController cc = player.GetComponent<CharacterController>();

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

            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (GameTimer.Instance != null)
                GameTimer.Instance.RestoreCheckpointTime(
                    CheckpointManager.Instance.savedTime
                );
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        losePanel.SetActive(false);
        canvasToHide.SetActive(true);

        hasLost = false;
        playerMovement.enabled = true;
    }

    // BUTTON 2 → restart full level
    public void RestartLevel()
    {
        if (pauseManager != null)
        {
            pauseManager.gameObject.SetActive(true);
            pauseWasDisabled = false;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // BUTTON 3 → main menu
    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}