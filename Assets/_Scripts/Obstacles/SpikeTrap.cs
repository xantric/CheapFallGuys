// SpikeMover.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class SpikeTrap : MonoBehaviour
{
    [Header("Spike Motion Settings")]
    [Tooltip("If left empty, first child of this GameObject will be used")]
    [SerializeField] private Transform spikesTransform;
    [Tooltip("How far up (in local units) the spikes should rise")]
    [SerializeField] private float riseHeight = 1f;
    [Tooltip("Time it takes for the spikes to rise")]
    [SerializeField] private float riseDuration = 0.5f;
    [Tooltip("How long the spikes stay fully up")]
    [SerializeField] private float activeTime = 1f;
    [Tooltip("Time it takes for the spikes to fall")]
    [SerializeField] private float fallDuration = 0.5f;
    [Tooltip("How long the spikes stay fully down")]
    [SerializeField] private float inactiveTime = 1f;

    [Header("Fling Settings")]
    [Tooltip("Horizontal impulse strength")]
    [SerializeField] private float horizontalForce = 6f;
    [Tooltip("Vertical impulse strength")]
    [SerializeField] private float verticalForce = 12f;
    [Tooltip("Tag of your player GameObject")]
    [SerializeField] private string playerTag = "Player";

    private Vector3 _downPos, _upPos;
    private bool _spikesUp;

    private void Reset()
    {
        // Make sure this collider is a trigger...
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        // ...and that we have a kinematic Rigidbody so Trigger events fire
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void Awake()
    {
        // Auto‑assign child spikes if none provided
        if (spikesTransform == null && transform.childCount > 0)
            spikesTransform = transform.GetChild(0);

        // Cache start/end positions in local space
        _downPos = spikesTransform.localPosition;
        _upPos = _downPos + Vector3.up * riseHeight;
    }

    private void Start()
    {
        StartCoroutine(SpikeCycle());
    }

    private IEnumerator SpikeCycle()
    {
        while (true)
        {
            // 1) Rise up
            yield return MoveSpikes(_downPos, _upPos, riseDuration);

            // 2) Spikes are up now—enable fling
            _spikesUp = true;
            yield return new WaitForSeconds(activeTime);

            // 3) Spikes go down—disable fling
            _spikesUp = false;
            yield return MoveSpikes(_upPos, _downPos, fallDuration);

            // 4) Rest before next cycle
            yield return new WaitForSeconds(inactiveTime);
        }
    }

    private IEnumerator MoveSpikes(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            spikesTransform.localPosition = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        spikesTransform.localPosition = to;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only fling when spikes are fully up
        if (!_spikesUp) return;

        // Only affect the player
        if (!other.CompareTag(playerTag)) return;
        var cc = other.GetComponent<CharacterController>();
        if (cc == null) return;

        // Compute horizontal direction away from the spike center
        Vector3 dir = other.transform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.01f)
            dir = transform.forward;
        else
            dir.Normalize();

        // Combine horizontal + vertical forces
        Vector3 push = dir * horizontalForce + Vector3.up * verticalForce;

        // Instantly move the character
        cc.Move(push);
        RespawnManager.Instance.RespawnPlayer(cc);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the “up” offset in the editor
        Gizmos.color = Color.red;
        if (spikesTransform != null)
        {
            Vector3 worldUp = spikesTransform.TransformPoint(Vector3.up * riseHeight);
            Gizmos.DrawLine(spikesTransform.position, worldUp);
        }
    }
}
