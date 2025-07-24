using UnityEngine;

public class OscillatingPlatform : MonoBehaviour
{
    [Tooltip("How far (and in what direction) from the start position the platform will move.")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, 5f);
    [Tooltip("Speed of oscillation.")]
    [SerializeField] private float speed = 1f;

    private Vector3 _startPos;

    private void Start()
    {
        _startPos = transform.position;
    }

    private void Update()
    {
        // PingPong returns a value that smoothly goes from 0?1?0 over time.
        float t = Mathf.PingPong(Time.time * speed, 1f);
        transform.position = Vector3.Lerp(_startPos, _startPos + offset, t);
    }
}
