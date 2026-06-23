using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterCustomizationManager : MonoBehaviour
{
    [Header("Hats")]
    public GameObject[] hats;

    [Header("Face")]
    public Renderer faceRenderer;
    public Material[] faceMaterials;

    [Header("UI Navigation")]
    public GameObject firstButton;

    int currentHat;
    int currentFace;

    void Start()
    {
        Invoke(nameof(SetFirstSelection), 0.1f);
    }

    void Update()
    {
        // 🎮 Gamepad confirm
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            ActivateSelected();
        }

        // ⌨ Keyboard confirm
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

    void Awake()
    {
        LoadCustomization();
    }

    // ---------------- HATS ----------------

    public void NextHat()
    {
        currentHat++;

        if (currentHat >= hats.Length)
            currentHat = 0;

        ApplyHat();
        SaveCustomization();
    }

    public void PreviousHat()
    {
        currentHat--;

        if (currentHat < 0)
            currentHat = hats.Length - 1;

        ApplyHat();
        SaveCustomization();
    }

    void ApplyHat()
    {
        for (int i = 0; i < hats.Length; i++)
        {
            hats[i].SetActive(i == currentHat);
        }
    }

    // ---------------- FACE ----------------

    public void NextFace()
    {
        currentFace++;

        if (currentFace >= faceMaterials.Length)
            currentFace = 0;

        ApplyFace();
        SaveCustomization();
    }

    public void PreviousFace()
    {
        currentFace--;

        if (currentFace < 0)
            currentFace = faceMaterials.Length - 1;

        ApplyFace();
        SaveCustomization();
    }

    void ApplyFace()
    {
        Material[] mats = faceRenderer.materials;
        mats[1] = faceMaterials[currentFace];
        faceRenderer.materials = mats;
    }

    // ---------------- SAVE ----------------

    void SaveCustomization()
    {
        PlayerPrefs.SetInt("HatIndex", currentHat);
        PlayerPrefs.SetInt("FaceIndex", currentFace);
        PlayerPrefs.Save();
    }

    void LoadCustomization()
    {
        currentHat = PlayerPrefs.GetInt("HatIndex", 0);
        currentFace = PlayerPrefs.GetInt("FaceIndex", 0);

        ApplyHat();
        ApplyFace();
    }
}