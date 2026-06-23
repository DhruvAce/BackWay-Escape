using UnityEngine;
using Solo.MOST_IN_ONE;

public class HapticClick : MonoBehaviour
{
    [Header("Cooldown (optional)")]
    public float cooldown = 0.1f;

    public void PlayLightHaptic()
    {
        MOST_HapticFeedback.GenerateWithCooldown(
            MOST_HapticFeedback.HapticTypes.LightImpact,
            cooldown
        );
    }
}