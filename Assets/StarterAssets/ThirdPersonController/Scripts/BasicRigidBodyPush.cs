using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BasicRigidBodyPush : MonoBehaviour
{
    [Header("Which layers to push rigidbodies")]
    public LayerMask pushLayers;

    [Header("Which layers can push you back (fan, spikes, etc)")]
    public LayerMask selfPushLayers;

    [Header("Push Other (Rigidbodies)")]
    public bool canPushOther = true;
    [Range(0.5f, 5f)] public float otherPushStrength = 1.1f;

    [Header("Be Pushed Back (CharacterController)")]
    public bool canBePushed = true;
    [Range(0.5f, 20f)] public float selfPushStrength = 5f;
    [Range(0f, 1f)] public float upwardModifier = 0.5f;

    private CharacterController _cc;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!GetComponent<PhotonView>().IsMine) return;
        // always push other rigidbodies if configured
        if (canPushOther)
            TryPushRigidBody(hit);

        // only push yourself when hitting “fan” or “spike” layers
        if (canBePushed && IsInLayerMask(hit.gameObject, selfPushLayers))
            TryBePushed(hit);
    }

    private void TryPushRigidBody(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic) return;

        if (!IsInLayerMask(body.gameObject, pushLayers)) return;
        if (hit.moveDirection.y < -0.3f) return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
        body.AddForce(pushDir.normalized * otherPushStrength,
                      ForceMode.Impulse);
    }

    private void TryBePushed(ControllerColliderHit hit)
    {
        Debug.Log(hit.gameObject.name);
        RespawnManager.Instance.RespawnPlayer(_cc);
        // make sure it’s actually a “lift” collision
        if (hit.moveDirection.y <= 0f) return;

        // compute a horizontal normal
        Vector3 horizontalNormal = new Vector3(hit.normal.x, 0f, hit.normal.z).normalized;
        Vector3 pushVec = (horizontalNormal + Vector3.up * upwardModifier).normalized
                          * selfPushStrength;

        _cc.Move(pushVec);
    }

    // helper that checks if go.layer is in the mask
    private bool IsInLayerMask(GameObject go, LayerMask mask)
    {
        return ((mask.value & (1 << go.layer)) != 0);
    }
}
