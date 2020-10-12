using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviourPunCallbacks
{
    public string throwerTeam;
    public float throwSpeed;
    private Rigidbody rb;
    private float time;
    public bool hasHit = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        time = Time.time;
    }

    private void FixedUpdate()
    {
        if(MobileTPSGameManager.instance.activePlayer.GetComponent<PhotonView>().IsMine && GetComponent<PhotonView>().IsMine)
        {
            
            if ((Time.time - time) < 0.3f)
            {
                rb.AddForce(transform.forward * throwSpeed, ForceMode.Impulse);
            }
        }
    }

    [PunRPC]
    public void SetThrowerTeam(string team)
    {
        throwerTeam = team;
    }

    [PunRPC]
    public void SetHasHit()
    {
        hasHit = true;
    }
}
