using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using Solo.MOST_IN_ONE;

public class BushStuck : MonoBehaviour
{
    [Header("Player")]
    public PlayerMovement player;

    [Header("Renderer")]
    public Renderer playerRenderer;

    [Header("Bush")]
    public float stuckSpeed = 0f;

    [Header("Mobile UI")]
    public GameObject jumpMobileButton;
    public UnityEngine.UI.Image jumpButtonImage;

    [Header("Escape")]
    public int tapsToEscape = 20;
    public float escapeDistance = 4f;
    public float forwardBoost = 6f;

    [Header("Flash")]
    public Color flashColor = Color.red;
    public float flashSpeed = 8f;

    private bool ignoreTrigger;
    private bool inTrapSession;

    [Header("UI")]
    public GameObject escapeText;
    public TMP_Text escapeTextTMP;
    
    [Header("Animation")]
    public Animator playerAnimator;

    private bool flashButton;
    private Color originalButtonColor;
    private bool hapticTriggered;
    
    private int currentTaps;
    private bool trapped;
    private bool escaping;

    private float originalSpeed;
    private Color originalColor;

    private Collider bushCollider;

    private bool jumpConsumed;

    void Start()
    {
        bushCollider = GetComponent<Collider>();
        originalSpeed = player.moveSpeed;

        if (jumpButtonImage != null)
            originalButtonColor = jumpButtonImage.color;

        if (playerRenderer != null)
            originalColor = playerRenderer.material.color;

        if (escapeText != null)
            escapeText.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        flashButton = true;
        if (escaping || ignoreTrigger) return;
        if (other.gameObject != player.gameObject) return;

        if (inTrapSession) return;

        inTrapSession = true;
        trapped = true;

        currentTaps = 0;

        player.moveSpeed = stuckSpeed;

        if (playerAnimator != null)
            playerAnimator.enabled = false;

        if (escapeText != null)
            escapeText.SetActive(true);

        UpdateText();

        SnapToCenter();

    }

    void Update()
    {
        if (!trapped || escaping) return;

        SnapToCenter();

        if (playerRenderer != null)
        {
            float t = Mathf.PingPong(Time.time * flashSpeed, 1f);
            playerRenderer.material.color = Color.Lerp(originalColor, flashColor, t);

            // ================= HAPTIC CONTROL =================
            if (t > 0.85f && !hapticTriggered)
            {
                MOST_HapticFeedback.Generate(
                    MOST_HapticFeedback.HapticTypes.LightImpact
                );

                hapticTriggered = true;
            }
            else if (t < 0.2f)
            {
                hapticTriggered = false;
            }
            // ==================================================
        }

        bool jumpPressed =
            (Keyboard.current != null && Keyboard.current.spaceKey.isPressed) ||
            (Gamepad.current != null && Gamepad.current.buttonSouth.isPressed);

        if (jumpPressed && !jumpConsumed)
        {
            jumpConsumed = true;

            currentTaps++;

            UpdateText();

            if (currentTaps >= tapsToEscape)
            {
                StartCoroutine(EscapeRoutine());
            }
        }

        if (!jumpPressed)
        {
            jumpConsumed = false;
        }

        #if UNITY_ANDROID || UNITY_IOS
        if (flashButton && jumpButtonImage != null)
        {
            float t = Mathf.PingPong(Time.time * 6f, 1f);
            jumpButtonImage.color = Color.Lerp(originalButtonColor, Color.yellow, t);
        }
        #endif
    }

    void UpdateText()
    {
        if (escapeTextTMP != null)
            escapeTextTMP.text = "Press Jump to Escape (" + currentTaps + "/" + tapsToEscape + ")";
    }

    void SnapToCenter()
    {
        Vector3 center = transform.position;
        center.y = player.transform.position.y;
        player.transform.position = center;
    }

    IEnumerator EscapeRoutine()
    {
        escaping = true;
        trapped = false;
        inTrapSession = false;
        ignoreTrigger = true;

        flashButton = false;


        if (jumpButtonImage != null)
            jumpButtonImage.color = originalButtonColor;

        currentTaps = 0;

        player.moveSpeed = originalSpeed;

        if (playerAnimator != null)
            playerAnimator.enabled = true;

        if (playerRenderer != null)
            playerRenderer.material.color = originalColor;

        if (escapeText != null)
            escapeText.SetActive(false);

        bushCollider.enabled = false;

        yield return null;

        Vector3 dir = player.transform.forward;
        dir.y = 0f;
        dir.Normalize();

        Vector3 escapePos =
            transform.position +
            dir * (escapeDistance + 2f) +
            Vector3.up * 0.5f;

        player.transform.position = escapePos;
        player.transform.position += dir * forwardBoost;

        yield return new WaitForSeconds(0.8f);

        bushCollider.enabled = true;

        yield return new WaitForSeconds(0.5f);

        ignoreTrigger = false;
        escaping = false;
    }



}