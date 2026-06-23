using UnityEngine;

public class GemPickup : MonoBehaviour
{
    [Header("Timer Reward")]
    public float timeReduction = 1f;

    [Header("Effects")]
    public GameObject pickupFX;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pickupSFX;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.8f, 1.2f)] public float pitchMin = 0.95f;
    [Range(0.8f, 1.2f)] public float pitchMax = 1.05f;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        if (!other.CompareTag("Ball"))
            return;

        collected = true;

        Debug.Log("Gem Collected");

        // ⏱ Timer reduction
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.ReduceTime(timeReduction);
        }

        // 💥 FX spawn
        if (pickupFX != null)
        {
            GameObject fx = Instantiate(
                pickupFX,
                transform.position,
                Quaternion.identity
            );

            fx.transform.localScale = Vector3.one * 0.2f;
        }

        // 🔊 SFX play
        PlayPickupSound();

        Destroy(gameObject);
    }

    private void PlayPickupSound()
    {
        if (audioSource == null || pickupSFX == null) return;

        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.PlayOneShot(pickupSFX, volume);
    }
}