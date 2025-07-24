// PlayerNetwork.cs
using Photon.Pun;
using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerNetwork : MonoBehaviourPun
{

    void Start()
    {
        if (photonView.IsMine)
        {
            // Local player → enable input & camera
            GetComponent<ThirdPersonController>().enabled = true;
            GetComponent<BasicRigidBodyPush>().enabled = true;
        }
        else
        {
            // Remote player → disable movement scripts
            GetComponent<ThirdPersonController>().enabled = false;
            GetComponent<BasicRigidBodyPush>().enabled = false;
        }
    }

    
}
