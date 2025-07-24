// PlayerRespawner.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerRespawner : MonoBehaviour
{
    [Tooltip("Y‑value below which the player should respawn")]
    [SerializeField] private float deathY = -10f;

    private CharacterController _cc;

    private void Start()
    {
        _cc = GetComponent<CharacterController>();
        // On start, register initial spawn point
        RespawnManager.Instance.SetSpawnPoint(transform.position, transform.rotation);
    }

    private void Update()
    {
        // Example: respawn if you fall off the level
        if (transform.position.y < deathY)
            RespawnManager.Instance.RespawnPlayer(_cc);
    }

    // Optional: you can also call RespawnPlayer on OnControllerColliderHit for "lethal" obstacles
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("KillZone"))
            RespawnManager.Instance.RespawnPlayer(_cc);
    }
}
