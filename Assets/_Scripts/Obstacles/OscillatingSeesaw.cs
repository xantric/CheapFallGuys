// ScriptedSeesaw.cs
using UnityEngine;
using System.Collections.Generic;

public class OscillatingSeesaw : MonoBehaviour
{
    public enum Side { Left, Right }

    [Header("Tilt Settings")]
    [Tooltip("Local axis around which the seesaw pivots (e.g. Z‑axis for a X‑tilt).")]
    [SerializeField] private Vector3 pivotAxis = Vector3.forward;
    [Tooltip("Max tilt angle (degrees) from horizontal.")]
    [SerializeField] private float maxAngle = 30f;
    [Tooltip("Speed (deg/sec) at which it swings toward its target tilt.")]
    [SerializeField] private float tiltSpeed = 90f;
    [Tooltip("Tag used to identify player objects.")]
    [SerializeField] private string playerTag = "Player";

    private readonly HashSet<GameObject> _leftPlayers = new();
    private readonly HashSet<GameObject> _rightPlayers = new();
    private Quaternion _startRot;
    private float _targetAngle;

    private void Awake()
    {
        _startRot = transform.localRotation;
    }

    private void Update()
    {
        // 1) Decide which way to tilt
        int diff = _rightPlayers.Count - _leftPlayers.Count;
        if (diff > 0) _targetAngle = -maxAngle;
        else if (diff < 0) _targetAngle = maxAngle;
        else _targetAngle = 0f;

        // 2) Smoothly rotate toward it
        Quaternion desired = _startRot
            * Quaternion.AngleAxis(_targetAngle, pivotAxis.normalized);
        transform.localRotation = Quaternion.RotateTowards(
            transform.localRotation,
            desired,
            tiltSpeed * Time.deltaTime
        );
    }

    // Called by the child triggers:
    public void RegisterPlayer(GameObject player, Side side)
    {
        if (side == Side.Left) _leftPlayers.Add(player);
        else _rightPlayers.Add(player);
    }

    public void UnregisterPlayer(GameObject player, Side side)
    {
        if (side == Side.Left) _leftPlayers.Remove(player);
        else _rightPlayers.Remove(player);
    }
}
