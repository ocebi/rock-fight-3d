using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public int playerStartHealth;
    private int currentHealth;
    public string playerTeam;
    public TextMeshPro tmp;
    public TextMeshProUGUI redScoreTmp;
    public TextMeshProUGUI blueScoreTmp;
    public TextMeshProUGUI remainingTimeTmp;
    private int remainingTime;
    private int redScore;
    private int blueScore;
    private bool endGame = false;

    #region Unity Methods

    private void Awake()
    {
        currentHealth = playerStartHealth;
    }
    void Start()
    {
        StartCoroutine(SetPlayerTeam());
        
        if(GetComponent<PhotonView>().IsMine)
        {
            remainingTime = (int)PhotonNetwork.CurrentRoom.CustomProperties["time"];
            StartCoroutine(SetPlayerUI());
            StartCoroutine(SetRemainingTime());
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(GetComponent<PhotonView>().IsMine && collision.gameObject.tag == "Projectile")
        {
            if (collision.gameObject.GetComponent<Projectile>().throwerTeam != playerTeam &&
                collision.gameObject.GetComponent<Projectile>().hasHit == false &&
                collision.gameObject.GetComponent<Rigidbody>().velocity != Vector3.zero)
            {
                int damage = Random.Range(10, 30);
                GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, damage);
                collision.gameObject.GetComponent<Projectile>().hasHit = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(GetComponent<PhotonView>().IsMine && other.gameObject.tag == "Medkit")
        {
            GetComponent<PhotonView>().RPC("PickMedkit", RpcTarget.AllBuffered);
        }
    }

    #endregion

    #region RPC's

    [PunRPC]
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            //respawn, increment opposite team score
            if(PhotonNetwork.IsMasterClient)
            {
                if (playerTeam == "blue")
                {
                    int redScore = (int)PhotonNetwork.CurrentRoom.CustomProperties["red_score"];
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "red_score", (redScore + 1) } });
                }
                else if(playerTeam == "red")
                {
                    int blueScore = (int)PhotonNetwork.CurrentRoom.CustomProperties["blue_score"];
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "blue_score", (blueScore + 1) } });
                }
            }
            StartCoroutine(SetPlayerUI());
            currentHealth = 100;
        }
        SetTMPText();
    }

    [PunRPC]
    public void PickMedkit()
    {
        currentHealth = 100;
        SetTMPText();
    }

    [PunRPC]
    public void SetRunAnimation(bool value)
    {
        GetComponent<MobileTPSController>().animator.SetBool("Running", value);
    }

    [PunRPC]
    public void TriggerThrowAnimation()
    {
        GetComponent<MobileTPSController>().animator.SetTrigger("Throw");
    }

    #endregion

    #region Public Methods

    public void SetTMPText()
    {
        tmp.text = photonView.Owner.NickName + "\n(" + currentHealth + ")";
        if(playerTeam == "red")
        {
            tmp.color = Color.red;
        }
        else if(playerTeam == "blue")
        {
            tmp.color = Color.blue;
        }
    }

    #endregion

    #region Photon Callbacks

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(PhotonNetwork.IsMasterClient && photonView.IsMine) //to run it once only
        {
            int redPlayers = (int)PhotonNetwork.CurrentRoom.CustomProperties["red_players"];
            int bluePlayers = (int)PhotonNetwork.CurrentRoom.CustomProperties["blue_players"];

            if (redPlayers > bluePlayers)
            {
                Hashtable hash = new Hashtable();
                hash.Add("team", 2);
                newPlayer.SetCustomProperties(hash);
                ++bluePlayers;
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "blue_players", bluePlayers } });
            }
            else
            {
                Hashtable hash = new Hashtable();
                hash.Add("team", 1);
                newPlayer.SetCustomProperties(hash);
                ++redPlayers;
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "red_players", redPlayers } });
            }
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "time", remainingTime } });
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //update team players after a player leaves
    {
        if (PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            int otherPlayerTeam = (int)otherPlayer.CustomProperties["team"];
            if(otherPlayerTeam == 1)
            {
                int redPlayers = (int)PhotonNetwork.CurrentRoom.CustomProperties["red_players"];
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "red_players", redPlayers - 1 } });
            }
            else if(otherPlayerTeam == 2)
            {
                int bluePlayers = (int)PhotonNetwork.CurrentRoom.CustomProperties["blue_players"];
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "blue_players", bluePlayers - 1 } });
            }
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        StartCoroutine(SetPlayerUI());
        if(photonView.IsMine)
        {
            remainingTime = (int)PhotonNetwork.CurrentRoom.CustomProperties["time"];
        }
    }

    #endregion

    #region Coroutines

    public IEnumerator SetPlayerTeam()
    {
        while (GetComponent<PhotonView>().Owner.CustomProperties.ContainsKey("team") == false) //wait for team to be set by master client
        {
            yield return new WaitForSeconds(0.2f);
        }
        int playerTeamInt = (int)GetComponent<PhotonView>().Owner.CustomProperties["team"];

        if (playerTeamInt == 2)
        {
            playerTeam = "blue";
        }
        else if (playerTeamInt == 1)
        {
            playerTeam = "red";
        }
        SetTMPText();
    }

    public IEnumerator SetPlayerUI() //sets the score texts
    {
        yield return new WaitForSeconds(0.5f); //time for master to update the score
        redScore = (int)PhotonNetwork.CurrentRoom.CustomProperties["red_score"];
        blueScore = (int)PhotonNetwork.CurrentRoom.CustomProperties["blue_score"];
        redScoreTmp.SetText("Red: " + redScore);
        blueScoreTmp.SetText("Blue: " + blueScore);
        if(endGame)
        {
            if (redScore > blueScore)
            {
                StartCoroutine(EndGame("Red"));
            }
            else if (blueScore > redScore)
            {
                StartCoroutine(EndGame("Blue"));
            }
        }
    }

    public IEnumerator SetRemainingTime()
    {
        while(remainingTime > 0)
        {
            yield return new WaitForSeconds(1);
            int min = Mathf.FloorToInt(remainingTime / 60);
            int sec = Mathf.FloorToInt(remainingTime % 60);
            remainingTimeTmp.SetText(min.ToString("00") + ":" + sec.ToString("00"));
            if(PhotonNetwork.IsMasterClient && photonView.IsMine)
            {
                --remainingTime;
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "time", remainingTime } });
            }
        }

        
        if (redScore > blueScore)
        {
            StartCoroutine(EndGame("Red"));
        }
        else if(blueScore > redScore)
        {
            StartCoroutine(EndGame("Blue"));
        }
        else
        {
            endGame = true;
        }
        
    }

    public IEnumerator EndGame(string winner)
    {
        GetComponent<MobileTPSController>().canRun = false;
        remainingTimeTmp.SetText(winner + " has won.");
        yield return new WaitForSeconds(5);
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

    #endregion
}
