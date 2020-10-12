using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIMover : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        if (player == null)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position = player.position + transform.up * 3f;
        }
    }
}
