using UnityEngine;
using System.Collections;
using Solo.MOST_IN_ONE; // ✅ ADD THIS

public class CheckpointZone : MonoBehaviour
{
    [Header("FX")]
    public ParticleSystem checkpointFX;

    [Tooltip("Extra shine effect prefab")]
    public GameObject shineFX;

    [Header("Color Settings")]
    public Color startColor = Color.blue;

    public Color savedColor = Color.green;

    [Range(1f, 100f)]
    public float colorTransitionSpeed = 30f;

    [Header("Player")]
    public Transform player;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip checkpointSFX;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.8f, 1.2f)] public float pitchMin = 0.95f;
    [Range(0.8f, 1.2f)] public float pitchMax = 1.05f;

    private bool activated = false;

    private ParticleSystem.MainModule mainModule;

    private void Start()
    {
        if (checkpointFX != null)
        {
            mainModule = checkpointFX.main;
            mainModule.startColor = startColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Ball"))
        {
            activated = true;

            if (CheckpointManager.Instance != null &&
                GameTimer.Instance != null)
            {
                CheckpointManager.Instance.SaveCheckpoint(
                    other.transform.position,
                    player.position,
                    GameTimer.Instance.GetCurrentTime()
                );

                // ✅ SUCCESS HAPTIC
                MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.Success);

                if (CheckpointPopup.Instance != null)
                {
                    CheckpointPopup.Instance.ShowPopup();
                }

                Debug.Log("Saved Player Position = " + player.position);
            }

            // 🔊 CHECKPOINT SFX
            PlayCheckpointSound();

            // Spawn shine effect
            if (shineFX != null)
            {
                Instantiate(
                    shineFX,
                    checkpointFX.transform.position,
                    checkpointFX.transform.rotation
                );
            }

            // Change checkpoint color
            if (checkpointFX != null)
            {
                StartCoroutine(ChangeColorSmooth());
            }
        }
    }

    // 🔊 SOUND FUNCTION
    void PlayCheckpointSound()
    {
        if (audioSource == null || checkpointSFX == null) return;

        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.PlayOneShot(checkpointSFX, volume);
    }

    IEnumerator ChangeColorSmooth()
    {
        Color current = mainModule.startColor.color;

        while (Vector4.Distance(current, savedColor) > 0.01f)
        {
            current = Color.Lerp(
                current,
                savedColor,
                colorTransitionSpeed * Time.deltaTime
            );

            mainModule.startColor = current;

            yield return null;
        }

        mainModule.startColor = savedColor;
    }
}