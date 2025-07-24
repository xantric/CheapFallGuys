// SeesawSideTrigger.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SeesawSideTrigger : MonoBehaviour
{
    [Tooltip("Drag in the parent GameObject with the ScriptedSeesaw on it.")]
    public OscillatingSeesaw parent;
    [Tooltip("Which side this trigger represents.")]
    public OscillatingSeesaw.Side side;
    [Tooltip("Must match the playerTag in ScriptedSeesaw.")]
    public string playerTag = "Player";

    private void Reset()
    {
        // auto‑configure this collider as a trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(side);
        if (other.CompareTag(playerTag))
            parent.RegisterPlayer(other.gameObject, side);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            parent.UnregisterPlayer(other.gameObject, side);
    }
}
