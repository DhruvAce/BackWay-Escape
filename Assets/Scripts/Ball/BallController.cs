using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Physics")]
    public Rigidbody rb;

    [Header("Kick Settings")]
    public float kickForce = 6f;
    public float torqueForce = 1.5f;

    [Header("Limits")]
    public float maxBallSpeed = 12f;
    public float maxSpin = 15f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip kickSFX;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.8f, 1.2f)] public float pitchMin = 0.95f;
    [Range(0.8f, 1.2f)] public float pitchMax = 1.05f;

    [Header("Kick Feel (Tuning)")]
    public float velocityReductionBeforeKick = 0.7f;
    public bool randomizeTorqueDirection = false;
    public float torqueMultiplier = 1f;

    private void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > maxBallSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxBallSpeed;
        }

        if (rb.angularVelocity.magnitude > maxSpin)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * maxSpin;
        }
    }

    public void Kick(Vector3 direction)
    {
        // reduce current velocity slightly before kick
        rb.linearVelocity *= velocityReductionBeforeKick;

        // apply kick force
        rb.AddForce(direction.normalized * kickForce, ForceMode.Impulse);

        // apply torque (spin)
        Vector3 torqueDir = randomizeTorqueDirection
            ? Random.onUnitSphere
            : transform.right;

        rb.AddTorque(torqueDir * torqueForce * torqueMultiplier, ForceMode.Impulse);

        // 🔊 Play SFX
        PlayKickSound();
    }

    private void PlayKickSound()
    {
        if (audioSource == null || kickSFX == null) return;

        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.PlayOneShot(kickSFX, volume);
    }
}