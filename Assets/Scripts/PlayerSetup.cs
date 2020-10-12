using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    private MobileTPSController mobileTPSController;
    public GameObject playerCamera;
    private void Start()
    {
        mobileTPSController = GetComponent<MobileTPSController>();

        if (!photonView.IsMine)
        {
            mobileTPSController.enabled = false;
            playerCamera.SetActive(false);
            
        }
        else
        {
            mobileTPSController.enabled = true;
            playerCamera.SetActive(true);
        }
        
    }
}
