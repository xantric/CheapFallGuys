using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(PhotonView))]
public class WinZone : MonoBehaviourPun
{
    [Tooltip("Tag used to identify the player")]
    [SerializeField] private string playerTag = "Player";

    // prevent multiple triggers
    private bool _finished = false;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_finished) return;
        if (!other.CompareTag(playerTag)) return;

        // mark so no one else can trigger twice
        _finished = true;

        // find which actorNumber is our winner
        var pv = other.GetComponent<PhotonView>();
        if (pv == null) return;
        int winnerActor = pv.Owner.ActorNumber;

        // tell everyone who won (buffered so late‑joiners see it too)
        photonView.RPC(
          nameof(RPC_AnnounceResult),
          RpcTarget.AllBuffered,
          winnerActor
        );
    }

    [PunRPC]
    private void RPC_AnnounceResult(int winnerActor)
    {
        // if I'm the winner...
        if (PhotonNetwork.LocalPlayer.ActorNumber == winnerActor)
            GameManager.Instance.WinGame("YOU WIN!");
        else
            GameManager.Instance.LoseGame("YOU LOSE!");
    }
}
