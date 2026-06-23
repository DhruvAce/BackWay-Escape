using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;

    public AudioSource bgmSource;

    void Awake()
    {
        // Singleton (prevents duplicates)
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Keep across scenes
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Start music only once
        if (bgmSource != null && !bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }
}