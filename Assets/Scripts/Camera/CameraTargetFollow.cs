using UnityEngine;

public class CameraTargetFollow : MonoBehaviour
{
    public Transform player;

    public float followSpeed = 8f;
    public float rotationSpeed = 1.5f;

    void LateUpdate()
    {
        if (player == null) return;

        transform.position = Vector3.Lerp(
            transform.position,
            player.position,
            followSpeed * Time.deltaTime);

        Quaternion targetRotation =
            Quaternion.Euler(0f, player.eulerAngles.y, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime);
    }
}