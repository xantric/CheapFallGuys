// LaunchReceiver.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class LaunchReceiver : MonoBehaviour
{
    private CharacterController _cc;
    private Vector3 _velocity;
    private float _timeLeft;
    private float _damping;

    /// <summary>
    /// Called by SpringTrap to start a launch.
    /// </summary>
    public void Initialize(Vector3 initialVelocity, float duration, float horizontalDamp)
    {
        if (_cc == null)
            _cc = GetComponent<CharacterController>();

        _velocity = initialVelocity;
        _timeLeft = duration;
        _damping = horizontalDamp;
    }

    private void Update()
    {
        if (_timeLeft <= 0f) return;

        // 1) Move by current velocity
        _cc.Move(_velocity * Time.deltaTime);

        // 2) Apply gravity
        _velocity.y += Physics.gravity.y * Time.deltaTime;

        // 3) Decay horizontal over time
        _velocity.x = Mathf.Lerp(_velocity.x, 0f, _damping * Time.deltaTime);
        _velocity.z = Mathf.Lerp(_velocity.z, 0f, _damping * Time.deltaTime);

        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0f)
        {
            Destroy(this);
        }
    }
}
