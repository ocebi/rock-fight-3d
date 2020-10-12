using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float cameraZDistance;
    public GameObject player;

    private void Start()
    {
        cameraZDistance = transform.position.z - player.transform.position.z;
    }

    private void Update()
    {
        if(player != null) //throws error if player is disconnected
        {
            transform.position = new Vector3(player.transform.position.x,
            transform.position.y,
            player.transform.position.z + cameraZDistance);
        }
        else //delete the remainings
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject go in players)
            {
                if (go.transform.childCount < 2)
                {
                    Destroy(go);
                }
            }
        }
        
        
    }
}
