using UnityEngine;
using UnityEngine.UI;

public class RainbowBar : MonoBehaviour
{
    public Image image;

    void Awake()
    {
        Texture2D tex = new Texture2D(256, 1);

        for (int i = 0; i < 256; i++)
        {
            float h = i / 255f;
            Color c = Color.HSVToRGB(h, 1f, 1f);
            tex.SetPixel(i, 0, c);
        }

        tex.Apply();

        image.sprite = Sprite.Create(
            tex,
            new Rect(0,0,256,1),
            new Vector2(0.5f,0.5f)
        );
    }
}