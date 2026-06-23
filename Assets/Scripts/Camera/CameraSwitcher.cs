using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    public GameObject topDownCam;
    public GameObject followCam;

    private bool isTopDown = true;

    void Start()
    {
        ApplyCameraState();
    }

    void Update()
    {
        HandleInput();
    }

    public void SwitchCamera()
    {
        isTopDown = !isTopDown;
        ApplyCameraState();
    }

    void ApplyCameraState()
    {
        topDownCam.SetActive(isTopDown);
        followCam.SetActive(!isTopDown);
    }

    void HandleInput()
    {
        // ⌨ Keyboard input
        if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame)
        {
            SwitchCamera();
        }

        // 🎮 Gamepad input
        if (Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame)
        {
            SwitchCamera();
        }
    }
}