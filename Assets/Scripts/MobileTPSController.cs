using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MobileTPSController : MonoBehaviour
{
    public float speed;
    //public VariableJoystick variableJoystick;
    public FixedJoystick joystick;
    public Rigidbody rb;
    private Animator animator;

    private bool running = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void FixedUpdate()
    {
        Vector3 direction = Vector3.forward * joystick.Vertical + Vector3.right * joystick.Horizontal;
        rb.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        //Quaternion rotation = Quaternion.LookRotation(direction);
        if(direction != Vector3.zero && rb.velocity != Vector3.zero)
        {
            //rb.MoveRotation(Quaternion.LookRotation(direction));
            transform.LookAt();
            if(!running)
            {
                running = true;
                animator.SetBool("Running", running);
            }
        }
        else
        {
            if (running)
            {
                running = false;
                animator.SetBool("Running", running);
            }
        }
        
    }
}
