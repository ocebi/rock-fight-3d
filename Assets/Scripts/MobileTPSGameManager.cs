using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MobileTPSGameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            if(playerPrefab != null)
            {
                int randomPoint = Random.RandomRange(0, 10);
                PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPoint, 0f, randomPoint), Quaternion.identity);
            }
            else
            {
                print("Player prefab is null");
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
