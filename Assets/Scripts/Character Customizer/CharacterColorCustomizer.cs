using UnityEngine;
using UnityEngine.UI;

public class CharacterColorCustomizer : MonoBehaviour
{
    public Renderer playerRenderer;
    public Slider colorSlider;

    Material playerMat;

    const string RKey = "PlayerColorR";
    const string GKey = "PlayerColorG";
    const string BKey = "PlayerColorB";

    void Start()
    {
        playerMat = playerRenderer.material;

        LoadSavedColor();

        colorSlider.onValueChanged.AddListener(UpdateColorFromSlider);
    }

    public void UpdateColorFromSlider(float value)
    {
        Color color = Color.HSVToRGB(value, 1f, 1f);

        playerMat.color = color;

        SaveColor(color);
    }

    void SaveColor(Color c)
    {
        PlayerPrefs.SetFloat(RKey, c.r);
        PlayerPrefs.SetFloat(GKey, c.g);
        PlayerPrefs.SetFloat(BKey, c.b);
        PlayerPrefs.Save();
    }

    void LoadSavedColor()
    {
        float r = PlayerPrefs.GetFloat(RKey,1);
        float g = PlayerPrefs.GetFloat(GKey,1);
        float b = PlayerPrefs.GetFloat(BKey,1);

        Color c = new Color(r,g,b);

        playerMat.color = c;

        Color.RGBToHSV(c, out float h, out _, out _);

        colorSlider.SetValueWithoutNotify(h);
    }
}