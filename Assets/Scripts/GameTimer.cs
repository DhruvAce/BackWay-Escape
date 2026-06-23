using TMPro;
using UnityEngine;
using System.Collections;


public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text finalTimeText;

    [Header("Gem Bonus")]
    public float secondsReducedPerGem = 1f;

    private float timer = 0f;

    private bool timerStarted = false;
    private bool timerStopped = false;

    [Header("Timer Effect")]
    public Color normalColor = Color.white;
    public Color bonusColor = Color.green;

    [Header("Level Duration (Lose Condition)")]
    public float levelDuration = 120f; // set in inspector

    public bool useLevelTimer = true;

    public System.Action OnTimeUp;
    private bool timeUpTriggered = false;

    public float effectDuration = 0.25f;
    public float effectScale = 1.25f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = timerText.transform.localScale;

        timerText.color = normalColor;
        timerText.transform.localScale = originalScale;
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (timerStarted && !timerStopped && !timeUpTriggered)
        {
            timer += Time.deltaTime;

            UpdateTimerUI();

            if (useLevelTimer && !timeUpTriggered && timer >= levelDuration)
            {
                timeUpTriggered = true;
                timerStopped = true;

                if (OnTimeUp != null)
                    OnTimeUp.Invoke();
            }
        }
    }

    public void StartTimer()
    {
        if (!timerStarted)
        {
            timerStarted = true;
        }
    }

    public void StopTimer()
    {
        timerStopped = true;

        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        int milliseconds = Mathf.FloorToInt((timer * 100) % 100);

        string finalTime =
            string.Format("{0:00}:{1:00}.{2:00}",
            minutes,
            seconds,
            milliseconds);

        timerText.text = finalTime;

        if (finalTimeText != null)
        {
            finalTimeText.text = "Time : " + finalTime;
        }
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        int milliseconds = Mathf.FloorToInt((timer * 100) % 100);

        timerText.text =
            string.Format("{0:00}:{1:00}.{2:00}",
            minutes,
            seconds,
            milliseconds);
    }

    public void ReduceTime(float seconds)
    {
        timer = Mathf.Max(0f, timer - seconds);

        UpdateTimerUI();

        StopCoroutine(nameof(TimerBonusEffect));
        StartCoroutine(TimerBonusEffect());

        Debug.Log("Timer Reduced. Current Time: " + timer);
    }

    IEnumerator TimerBonusEffect()
    {
        timerText.color = bonusColor;

        timerText.transform.localScale =
        originalScale * effectScale;

        yield return new WaitForSeconds(effectDuration);

        timerText.color = normalColor;

        timerText.transform.localScale =
        originalScale;
    }

    public float GetCurrentTime()
    {
        return timer;
    }

    public bool IsBetterTime(float timeToCheck)
    {
        return timeToCheck < timer;
    }

    public float GetFinalTime()
        {
            return timer;
        }

    public void RestoreTime(float time)
    {
        timer = time;
        timerStarted = true;
        timerStopped = false;

        UpdateTimerUI();
    }

    public void RestoreCheckpointTime(float time)
{
    timer = time;

    timerStarted = true;
    timerStopped = false;

    timeUpTriggered = false;

    UpdateTimerUI();
}

    public void ResetTimer()
    {
        timer = 0f;
        timerStarted = false;
        timerStopped = false;
        timeUpTriggered = false;

        UpdateTimerUI();
    }
}