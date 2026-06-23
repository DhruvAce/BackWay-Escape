using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public enum InputMode
    {
        Accelerometer,
        Joystick
    }

    [Header("Input Mode")]
    public InputMode inputMode = InputMode.Accelerometer;

    [Header("UI References")]
    public GameObject tiltSensitivityUI;
    public Slider tiltSlider;

    [Header("Joystick UI")]
    public GameObject joystickUI;

    [Header("Android Only UI")]
    public GameObject gyroIcon;
    public GameObject tiltIcon;

    [Header("Movement")]
    public float moveSpeed = 5f;

    public float rotationSpeed = 4f;
    public float deceleration = 12f;
    

    [Header("Mobile Tilt")]
    public float tiltSensitivity = 2f;
    public float gyroSensitivity = 1.2f;
    public float deadZone = 0.08f;

    [Header("Smoothing")]
    public float smoothness = 10f;
    public float inputCurve = 1.7f;

    [Header("Sticky Center (PUBG feel)")]
    public float stickyStrength = 3.5f;


    [Header("Jump")]
    public float jumpHeight = 2f;
    public float gravity = -20f;

    [Header("Ball")]
    public BallController ball;

    [Header("Dribbling")]
    public float dribbleForce = 0.35f;
    public float touchCooldown = 0.15f;

    [Header("Kick")]
    public float kickDistance = 1.5f;

    [Header("Camera Reference")]
    public Transform cameraTransform;

    private CharacterController controller;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private Animator animator;

    private Vector2 smoothedInput;
    private Vector2 rawTiltInput;
    private Vector2 gyroInput;

    private Vector3 velocity;

    private bool isGrounded;
    private float currentSpeed;
    private float lastTouchTime;

    private Vector3 neutralTilt;
    private bool timerStarted;

    private const string INPUT_MODE_KEY = "InputMode";
    private const string TILT_SENS_KEY = "TiltSensitivity";

    private const float DEFAULT_TILT = 3.5f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        animator = GetComponent<Animator>();
    }

    private void Start()
    {

    #if UNITY_ANDROID && !UNITY_EDITOR
        neutralTilt = Input.acceleration;

        if (SystemInfo.supportsGyroscope)
            Input.gyro.enabled = true;
    #endif

        // LOAD SAVED INPUT MODE
        inputMode = (InputMode)PlayerPrefs.GetInt(INPUT_MODE_KEY, 0);

        // LOAD SAVED TILT SENSITIVITY
        tiltSensitivity = PlayerPrefs.GetFloat(TILT_SENS_KEY, DEFAULT_TILT);

        if (tiltSlider != null)
            tiltSlider.value = tiltSensitivity;

        ApplyInputMode(inputMode);

    #if UNITY_ANDROID && !UNITY_EDITOR

        if (gyroIcon != null)
            gyroIcon.SetActive(true);

        if (tiltIcon != null)
            tiltIcon.SetActive(true);

        if (tiltSensitivityUI != null)
            tiltSensitivityUI.SetActive(true);

    #else

        if (gyroIcon != null)
            gyroIcon.SetActive(false);

        if (tiltIcon != null)
            tiltIcon.SetActive(false);

        if (tiltSensitivityUI != null)
            tiltSensitivityUI.SetActive(false);

    #endif
    }

    private void Update()
    {
        HandleInputModeUI();

        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;

            if (animator != null)
                animator.SetBool("IsJumping", false);
        }

        Vector2 moveInput = GetMoveInput();

        if (!timerStarted && moveInput.magnitude > 0.1f)
        {
            timerStarted = true;

            if (GameTimer.Instance != null)
                GameTimer.Instance.StartTimer();
        }

        if (animator != null)
            animator.SetFloat("Speed", moveInput.magnitude);

        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * input.z + camRight * input.x;



        if (moveDir.sqrMagnitude > 0.01f)
        {
            Vector3 direction = moveDir.normalized;

            Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );

            currentSpeed = moveSpeed;
            controller.Move(direction * currentSpeed * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, deceleration * Time.deltaTime);
            controller.Move(transform.forward * currentSpeed * Time.deltaTime);
        }

        // ---------------- JUMP ----------------
        bool jumpPressed = false;

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpPressed = true;

        if (Gamepad.current != null &&
            Gamepad.current.buttonSouth.wasPressedThisFrame)
            jumpPressed = true;

        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
                animator.SetBool("IsJumping", true);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // ---------------- KICK ----------------
        bool kickPressed = false;

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
            kickPressed = true;

        if (Gamepad.current != null &&
            Gamepad.current.buttonEast.wasPressedThisFrame)
            kickPressed = true;

        if (kickPressed)
        {
            Vector3 toBall = (ball.transform.position - transform.position).normalized;

            float distance = Vector3.Distance(transform.position, ball.transform.position);
            float dot = Vector3.Dot(transform.forward, toBall);

            if (distance <= kickDistance && dot > 0.3f)
                ball.Kick(transform.forward);
        }
    }

    // ---------------- INPUT SYSTEM FIXED ----------------
    private Vector2 GetMoveInput()
    {
        // JOYSTICK MODE (NEW INPUT SYSTEM - ON SCREEN STICK)
        if (inputMode == InputMode.Joystick)
        {
            if (moveAction != null)
                return moveAction.ReadValue<Vector2>();

            return Vector2.zero;
        }

        // ---------------- ACCELEROMETER MODE ----------------
#if UNITY_ANDROID && !UNITY_EDITOR

        Vector3 accel = Input.acceleration - neutralTilt;

        rawTiltInput = new Vector2(accel.x, -accel.z);
        rawTiltInput *= tiltSensitivity;

        rawTiltInput = Vector2.ClampMagnitude(rawTiltInput, 1f);

        if (SystemInfo.supportsGyroscope)
        {
            Vector3 gyro = Input.gyro.rotationRateUnbiased;

            gyroInput = new Vector2(gyro.y, -gyro.x) * gyroSensitivity * Time.deltaTime;
        }

        Vector2 combined = rawTiltInput + gyroInput;

        if (combined.magnitude < deadZone)
            combined = Vector2.zero;

        smoothedInput = Vector2.Lerp(smoothedInput, combined, smoothness * Time.deltaTime);

        smoothedInput = ApplyCurve(smoothedInput);

        return smoothedInput;

#else
        return moveAction.ReadValue<Vector2>();
#endif
    }

    public void SetAccelerometerMode()
    {
        inputMode = InputMode.Accelerometer;

        PlayerPrefs.SetInt(INPUT_MODE_KEY, (int)inputMode);
        PlayerPrefs.Save();

        ApplyInputMode(inputMode);
    }

    public void SetJoystickMode()
    {
        inputMode = InputMode.Joystick;

        PlayerPrefs.SetInt(INPUT_MODE_KEY, (int)inputMode);
        PlayerPrefs.Save();

        ApplyInputMode(inputMode);
    }

    private void ApplyInputMode(InputMode mode)
    {
        bool isAccelerometer = mode == InputMode.Accelerometer;

        // Tilt UI
        if (tiltSensitivityUI != null)
            tiltSensitivityUI.SetActive(isAccelerometer);

        // JOYSTICK UI (IMPORTANT FIX)
        if (joystickUI != null)
            joystickUI.SetActive(!isAccelerometer);
    }

    private void HandleInputModeUI()
    {
        if (tiltSlider != null)
        {
            tiltSensitivity = tiltSlider.value;

            PlayerPrefs.SetFloat(TILT_SENS_KEY, tiltSensitivity);
            PlayerPrefs.Save();
        }
    }
    
    private Vector2 ApplyCurve(Vector2 input)
    {
        input.x = Mathf.Sign(input.x) * Mathf.Pow(Mathf.Abs(input.x), inputCurve);
        input.y = Mathf.Sign(input.y) * Mathf.Pow(Mathf.Abs(input.y), inputCurve);
        return input;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.collider.CompareTag("Ball"))
            return;

        Rigidbody ballRb = hit.collider.attachedRigidbody;

        if (ballRb == null)
            return;

        if (Time.time - lastTouchTime < touchCooldown)
            return;

        lastTouchTime = Time.time;

        float pushAmount = dribbleForce * currentSpeed;

        ballRb.AddForce(transform.forward * pushAmount, ForceMode.Impulse);
    }
}