using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    private MobileTPSController mobileTPSController;
    private Camera playerCamera;
    private void Awake()
    {
        mobileTPSController = GetComponent<MobileTPSController>();
        playerCamera = GetComponent<Camera>();

        if (!photonView.IsMine)
        {
            mobileTPSController.enabled = false;
            playerCamera.enabled = false;
            return;
        }
        else
        {
            mobileTPSController.joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<FixedJoystick>() as FixedJoystick;
            mobileTPSController.rb = GetComponent<Rigidbody>();
        }
    }
}
