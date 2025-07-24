// RespawnManager.cs
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }

    private Vector3 _spawnPosition;
    private Quaternion _spawnRotation;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Call this to set the player’s next respawn point.
    /// </summary>
    public void SetSpawnPoint(Vector3 position, Quaternion rotation)
    {
        _spawnPosition = position;
        _spawnRotation = rotation;
    }

    /// <summary>
    /// Instantly moves the player back to the last spawn point.
    /// </summary>
    public void RespawnPlayer(CharacterController cc)
    {
        // Temporarily disable the controller so Move/teleport is clean
        cc.enabled = false;
        cc.transform.position = _spawnPosition;
        cc.transform.rotation = _spawnRotation;
        cc.enabled = true;

        // If you have any velocity/state scripts, reset them here
        var lr = cc.GetComponent<LaunchReceiver>();
        if (lr != null) Destroy(lr);
    }
}
