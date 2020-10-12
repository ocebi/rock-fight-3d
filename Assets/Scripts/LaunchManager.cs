using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LaunchManager : MonoBehaviourPunCallbacks
{

    public GameObject EnterGamePanel;
    public GameObject ConnectionStatusPanel;
    public GameObject LobbyPanel;
    public GameObject RoomPanel;
    public InputField PlayerNameInputField;
    private int redPlayers = 0;
    private int bluePlayers = 0;

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        EnterGamePanel.SetActive(true);
        ConnectionStatusPanel.SetActive(false);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #endregion

    #region Public Methods
    public void ConnectToPhotonServer()
    {
        if(string.IsNullOrEmpty(PlayerNameInputField.text))
        {
            PhotonNetwork.NickName = "User" + Random.Range(1, 1000);
        }

        if(!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();

            EnterGamePanel.SetActive(false);
            ConnectionStatusPanel.SetActive(true);
        }
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

    #endregion


    #region Photon Callbacks

    public override void OnConnected()
    {
        print("Connected to internet.");
    }

    public override void OnConnectedToMaster()
    {
        print(PhotonNetwork.NickName + " Connected to server.");
        ConnectionStatusPanel.SetActive(false);
        LobbyPanel.SetActive(true);

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected from server for reason " + cause.ToString());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print(otherPlayer.NickName + " disconnected from the room.");
        UpdatePlayerList();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateAndJoinRoom();
    }
    
    public override void OnJoinedRoom()
    {
        print(PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable hash = new Hashtable();
            hash.Add("team", 1); //1 means red, 2 means blue
            PhotonNetwork.SetPlayerCustomProperties(hash);
            ++redPlayers;
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "red_players", redPlayers } }); //# of players in the red team
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "blue_players", 0 } }); //# of players in the blue team
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "red_score", 0 } }); //total red team score
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "blue_score", 0 } }); //total blue team score
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "time", 180 } }); //time for match to be completed
        }
        
        EnterGamePanel.SetActive(false);
        ConnectionStatusPanel.SetActive(false);
        LobbyPanel.SetActive(false);
        UpdatePlayerList();
        RoomPanel.SetActive(true);
        if(!PhotonNetwork.IsMasterClient)
        {
            RoomPanel.transform.GetChild(0).GetComponentInChildren<Button>().gameObject.SetActive(false);
        }
    }

    public override void OnLeftRoom()
    {
        EnterGamePanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print(newPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name + " (" + PhotonNetwork.CurrentRoom.PlayerCount + "/20)");
        UpdatePlayerList();
        if (PhotonNetwork.IsMasterClient)
        {
            redPlayers = (int)PhotonNetwork.CurrentRoom.CustomProperties["red_players"];
            bluePlayers = (int)PhotonNetwork.CurrentRoom.CustomProperties["blue_players"];

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
        }
    }
    
    #endregion

    #region Private Methods

    private void CreateAndJoinRoom()
    {
        string roomName = "Room" + Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    private void UpdatePlayerList()
    {
        TextMeshProUGUI tmp = RoomPanel.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = "Players:\n";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            tmp.text += player.NickName + "\n";
        }
    }

    #endregion
}
