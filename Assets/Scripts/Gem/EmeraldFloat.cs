using UnityEngine;

public class EmeraldFloat : MonoBehaviour
{
    public float height = 0.5f;
    public float speed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * speed) * height;
        transform.position = startPos + new Vector3(0, y, 0);
        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }
}