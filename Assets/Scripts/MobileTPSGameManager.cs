using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MobileTPSGameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject rockPrefab;
    [SerializeField]
    private GameObject medkitPrefab;
    [HideInInspector]
    public GameObject activePlayer;
    public string activePlayerTeam;
    private Animator animator;
    private MobileTPSController tpsController;
    public float throwTime;
    [HideInInspector]
    public GameObject activeRock;
    [HideInInspector]
    public GameObject activeMedkit;

    //public PhotonView activeRockPhotonView;

    public static MobileTPSGameManager instance = null;

    // Start is called before the first frame update
    void Start()
    {
        instance = GetComponent<MobileTPSGameManager>();
        
        if(PhotonNetwork.IsConnectedAndReady)
        {
            if(playerPrefab != null)
            {
                int randomPoint = Random.Range(-10, 10);
                GameObject go = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPoint, 0f, randomPoint), Quaternion.identity);
                activePlayer = go.transform.GetChild(0).gameObject;
                animator = activePlayer.GetComponent<Animator>();
                tpsController = activePlayer.GetComponent<MobileTPSController>();
            }
            else
            {
                print("Player prefab is null");
            }
            if(PhotonNetwork.IsMasterClient)
            {
                SpawnMedkit();
            }
        }
    }

    public void OnClickedThrow()
    {
        if(tpsController.canRun)
        {
            if (rockPrefab != null)
            {
                
                if(activeRock != null)
                {
                    if(activeRock.GetComponent<PhotonView>().IsMine)
                    {
                        PhotonNetwork.Destroy(activeRock);
                    }
                }

                tpsController.canRun = false;
                activePlayer.GetComponent<PhotonView>().RPC("SetRunningAnimation", RpcTarget.AllBuffered, false);
                activePlayer.GetComponent<PhotonView>().RPC("TriggerThrowAnimation", RpcTarget.AllBuffered);
                activePlayerTeam = activePlayer.GetComponent<PlayerManager>().playerTeam;
                activeRock = PhotonNetwork.Instantiate(rockPrefab.name, activePlayer.transform.position + activePlayer.transform.forward * 1f, activePlayer.transform.rotation);
                activeRock.GetComponent<Projectile>().gameObject.GetComponent<PhotonView>().RPC("SetThrowerTeam", RpcTarget.AllBuffered, activePlayerTeam);
                StartCoroutine(waiter());
            }
            else
            {
                print("Rock prefab is null");
            }
        }
    }

    public void SpawnMedkit()
    {
        if(medkitPrefab != null)
        {
            if(activeMedkit != null)
            {
                if (activeMedkit.GetComponent<PhotonView>().IsMine)
                {
                    PhotonNetwork.Destroy(activeMedkit);
                }
            }
            //random pos
            int randomPoint = Random.Range(-10, 10);
            activeMedkit = PhotonNetwork.Instantiate(medkitPrefab.name, new Vector3(randomPoint, medkitPrefab.transform.position.y, randomPoint), medkitPrefab.transform.rotation);
        }
        else
        {
            print("Medkit prefab is null");
        }
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(throwTime);
        tpsController.canRun = true;
        activeRock.GetComponent<PhotonView>().RPC("SetHasHit", RpcTarget.AllBuffered);
    }
}
