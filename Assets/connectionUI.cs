using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class connectionUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.DevRegion))
        {
            gameObject.SetActive(false);
        }
    }
}
