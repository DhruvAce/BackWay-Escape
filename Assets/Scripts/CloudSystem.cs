using UnityEngine;

public class CloudSystem : MonoBehaviour
{
    [Header("Cloud Movement")]
    public float speed = 2f;
    public float endX = 50f;
    public float startX = -50f;

    [Header("Fade")]
    public float fadeSpeed = 1.5f;

    private Renderer[] clouds;
    private Material[] mats;
    private Color[] baseColors;

    private float alpha = 1f;
    private bool fadingOut = false;

    void Start()
    {
        clouds = GetComponentsInChildren<Renderer>();

        mats = new Material[clouds.Length];
        baseColors = new Color[clouds.Length];

        for (int i = 0; i < clouds.Length; i++)
        {
            mats[i] = clouds[i].material; // instance per cloud
            baseColors[i] = mats[i].color;
        }
    }

    void Update()
    {
        MoveClouds();
        HandleFade();
    }

    void MoveClouds()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        if (transform.position.x > endX)
        {
            fadingOut = true;
        }
    }

    void HandleFade()
    {
        if (fadingOut)
        {
            alpha -= Time.deltaTime * fadeSpeed;

            if (alpha <= 0f)
            {
                alpha = 0f;
                ResetClouds();
            }
        }
        else
        {
            alpha += Time.deltaTime * fadeSpeed;
            if (alpha >= 1f)
                alpha = 1f;
        }

        for (int i = 0; i < mats.Length; i++)
        {
            Color c = baseColors[i];
            c.a = alpha;
            mats[i].color = c;
        }
    }

    void ResetClouds()
    {
        transform.position = new Vector3(startX, transform.position.y, transform.position.z);
        fadingOut = false;
    }
}