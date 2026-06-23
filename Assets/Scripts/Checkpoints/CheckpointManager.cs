using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    public bool hasCheckpoint;

    public Vector3 savedBallPosition;
    public Vector3 savedPlayerPosition;

    public float savedTime;

    void Awake()
    {
        Instance = this;
    }

    public void SaveCheckpoint(
        Vector3 ballPos,
        Vector3 playerPos,
        float time)
    {
        hasCheckpoint = true;

        savedBallPosition = ballPos;
        savedPlayerPosition = playerPos;

        savedTime = time;

        Debug.Log("Checkpoint Saved ✔");
    }

    public void ClearCheckpoint()
    {
        hasCheckpoint = false;
    }
}