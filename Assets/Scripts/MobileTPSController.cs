using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MobileTPSController : MonoBehaviour
{
    public float speed;
    [HideInInspector]
    public bool canRun;
    private FixedJoystick joystick;
    private Rigidbody rb;
    public Animator animator;

    private bool running = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<FixedJoystick>() as FixedJoystick;
        canRun = true;
    }

    public void FixedUpdate()
    {
        if(canRun)
        {
            Vector3 direction = (Vector3.forward * joystick.Vertical + Vector3.right * joystick.Horizontal) * -1;
            rb.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
            
            if (direction != Vector3.zero && rb.velocity != Vector3.zero)
            {
                rb.MoveRotation(Quaternion.LookRotation(direction));
                if (!running)
                {
                    running = true;
                    GetComponent<PhotonView>().RPC("SetRunAnimation", RpcTarget.AllBuffered, running);
                }
            }
            else
            {
                if (running)
                {
                    running = false;
                    GetComponent<PhotonView>().RPC("SetRunAnimation", RpcTarget.AllBuffered, running);
                }
            }
        }
    }
}
