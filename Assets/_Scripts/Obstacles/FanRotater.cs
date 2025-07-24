// FanRotater.cs
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class FanRotater : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Local axis around which the fan will spin.")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [Tooltip("Degrees per second of rotation.")]
    [SerializeField] private float rotationSpeed = 180f;
    [Tooltip("If true, spins along rotationAxis; if false, spins opposite.")]
    [SerializeField] private bool clockwise = true;

    [Header("Fling Settings")]
    [Tooltip("Horizontal impulse strength.")]
    [SerializeField] private float horizontalForce = 6f;
    [Tooltip("Vertical impulse strength.")]
    [SerializeField] private float verticalForce = 12f;
    [SerializeField] private float launchDuration = 0.6f;  // how long we keep moving you
    [SerializeField] private float damping = 4f;
    [Tooltip("Tag of your player GameObject.")]
    [SerializeField] private string playerTag = "Player";

    private Vector3 _launchVelocity;
    private float _launchTimeLeft;
    private CharacterController cc;
    private void Reset()
    {
        // Make collider a trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        // Add/configure kinematic Rigidbody so triggers fire even when player is idle
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void Update()
    {
        float dir = clockwise ? 1f : -1f;
        transform.Rotate(
            rotationAxis.normalized,
            rotationSpeed * Time.deltaTime * dir,
            Space.Self
        );

        if (_launchTimeLeft > 0f)
        {
            // SimpleMove applies speed * deltaTime + gravity
            if(cc != null) cc.Move(_launchVelocity * Time.deltaTime);

            // decay the velocity
            _launchVelocity.y += Physics.gravity.y * Time.deltaTime;

            _launchVelocity = Vector3.Lerp(_launchVelocity, Vector3.zero, damping * Time.deltaTime);

            _launchTimeLeft -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        cc = other.GetComponent<CharacterController>();
        if (cc == null) return;

        // compute horizontal direction from fan center
        Vector3 launchDir = other.transform.position - transform.position;
        launchDir.y = 0f;
        if (launchDir.sqrMagnitude < 0.01f)
            launchDir = transform.forward;
        else
            launchDir.Normalize();

        // apply separate horizontal & vertical forces
        Vector3 horizontal = launchDir * horizontalForce;
        Vector3 vertical = Vector3.up * verticalForce;
        _launchVelocity = horizontal + vertical;
        // instantly move the character that way
        _launchTimeLeft = launchDuration;
        //cc.Move(push);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 worldAxis = transform.TransformDirection(rotationAxis.normalized) * 1.5f;
        Gizmos.DrawLine(transform.position, transform.position + worldAxis);
    }
}
