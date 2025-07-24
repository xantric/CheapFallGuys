// SpringTrap.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class SpringTrap : MonoBehaviour
{
    [Header("Launch Settings")]
    [Tooltip("Initial launch speed along the spring's up axis.")]
    [SerializeField] private float launchSpeed = 15f;
    [Tooltip("How long (seconds) the launch impulse is applied.")]
    [SerializeField] private float launchDuration = 0.5f;
    [Tooltip("How quickly horizontal velocity decays (higher = faster).")]
    [SerializeField] private float horizontalDamping = 2f;
    [Tooltip("Which layers get launched (e.g. your Player layer).")]
    [SerializeField] private LayerMask launchLayers;
    [Tooltip("Tag of your player GameObject.")]
    [SerializeField] private string playerTag = "Player";

    [Header("Pop‑Out Visual")]
    [Tooltip("How high (in local units) the trap itself jumps when activated.")]
    [SerializeField] private float popHeight = 0.2f;
    [Tooltip("Total time (sec) for the pop‑out animation.")]
    [SerializeField] private float popDuration = 0.1f;

    private Vector3 _initialLocalPos;
    private bool _isPopping;

    private void Reset()
    {
        // auto‑configure as trigger + kinematic so it fires triggers
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void Awake()
    {
        // remember where we start
        _initialLocalPos = transform.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        // only launch your player on the right layer
        if (!other.CompareTag(playerTag)) return;
        if (((1 << other.gameObject.layer) & launchLayers.value) == 0) return;

        // pop the spring visually
        if (!_isPopping)
            StartCoroutine(PopOut());

        // find (or add) the LaunchReceiver on the player
        var receiver = other.GetComponent<LaunchReceiver>();
        if (receiver == null)
            receiver = other.gameObject.AddComponent<LaunchReceiver>();

        // build the launch velocity along the spring’s up axis
        //Debug.Log(transform.right);
        Vector3 launchVelocity = transform.up * launchSpeed + -1 * transform.right * launchSpeed;
        receiver.Initialize(launchVelocity, launchDuration, horizontalDamping);
    }

    private IEnumerator PopOut()
    {
        _isPopping = true;
        float halfTime = popDuration * 0.5f;
        Vector3 upPos = _initialLocalPos + Vector3.up * popHeight;

        // pop up
        float t = 0f;
        while (t < halfTime)
        {
            transform.localPosition = Vector3.Lerp(_initialLocalPos, upPos, t / halfTime);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = upPos;

        // pop back down
        t = 0f;
        while (t < halfTime)
        {
            transform.localPosition = Vector3.Lerp(upPos, _initialLocalPos, t / halfTime);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = _initialLocalPos;

        _isPopping = false;
    }
}
