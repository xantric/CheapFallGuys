// OscillatingHammer.cs
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class OscillatingHammer : MonoBehaviour
{
    [Header("Swing Settings")]
    [Tooltip("Local axis around which the hammer will swing.")]
    [SerializeField] private Vector3 swingAxis = Vector3.forward;
    [Tooltip("Maximum swing angle in degrees.")]
    [SerializeField] private float swingAngle = 45f;
    [Tooltip("Speed of the pendulum (oscillations per second).")]
    [SerializeField] private float swingSpeed = 1f;

    [Header("Hit Settings")]
    [Tooltip("Horizontal force applied to the player when hit.")]
    [SerializeField] private float horizontalForce = 6f;
    [Tooltip("Vertical force applied to the player when hit.")]
    [SerializeField] private float verticalForce = 8f;
    [Tooltip("How long the launch effect lasts.")]
    [SerializeField] private float launchDuration = 0.5f;
    [Tooltip("How quickly horizontal velocity decays.")]
    [SerializeField] private float horizontalDamping = 2f;
    [Tooltip("Which layers the hammer can hit (e.g. your Player layer).")]
    [SerializeField] private LayerMask hitLayers;
    [Tooltip("Tag of your player GameObject.")]
    [SerializeField] private string playerTag = "Player";

    private Quaternion _initialLocalRot;

    private void Reset()
    {
        // Make this collider a trigger and ensure we have a kinematic Rigidbody
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void Awake()
    {
        // Cache the starting rotation
        _initialLocalRot = transform.localRotation;
    }

    private void Update()
    {
        // Oscillate like a pendulum: angle = A * sin(ωt)
        float angle = swingAngle * Mathf.Sin(Time.time * swingSpeed * Mathf.PI * 2f);
        transform.localRotation = _initialLocalRot * Quaternion.AngleAxis(angle, swingAxis.normalized);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only affect objects on the configured layer(s) and tag
        int layerBit = 1 << other.gameObject.layer;
        if (((layerBit & hitLayers.value) == 0) || !other.CompareTag(playerTag))
            return;

        // Get or add the LaunchReceiver to handle the fling arc
        var receiver = other.GetComponent<LaunchReceiver>();
        if (receiver == null)
            receiver = other.gameObject.AddComponent<LaunchReceiver>();

        // Compute a horizontal push direction away from the pivot
        Vector3 dir = other.transform.position - transform.position;
        dir.y = 0f;
        dir = (dir.sqrMagnitude < 0.01f) ? transform.right : dir.normalized;

        // Build the launch velocity
        Vector3 launchVel = dir * horizontalForce + Vector3.up * verticalForce;

        // Initialize the receiver (it will Move + apply gravity + damping over time)
        receiver.Initialize(launchVel, launchDuration, horizontalDamping);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the swing axis
        Gizmos.color = Color.yellow;
        Vector3 worldAxis = transform.TransformDirection(swingAxis.normalized);
        Gizmos.DrawLine(transform.position, transform.position + worldAxis);
    }
}
