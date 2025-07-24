// Checkpoint.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [Tooltip("Tag of the player to register checkpoints")]
    [SerializeField] private string playerTag = "Player";

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        // Register this position & rotation as the new spawn point
        RespawnManager.Instance.SetSpawnPoint(
            other.transform.position,
            other.transform.rotation
        );
        // Optional: play a checkpoint sound or visual here
        Debug.Log("Checkpoint reached at " + transform.position);
    }
}
