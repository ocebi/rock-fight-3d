using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Medkit : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        
        if(PhotonNetwork.IsMasterClient && 
            other.gameObject.tag == "PlayerBody")
        {
            MobileTPSGameManager.instance.SpawnMedkit();
        }
        
    }
}
