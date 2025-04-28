using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("roomInfo")] [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListItemPrefab;
    private readonly List<RoomInfo> _cachedRoomList = new List<RoomInfo>();

    [Header("ErrorInfo")] [SerializeField] private TMP_Text errorText;


    [Header("PlayerInfo")] [SerializeField]
    private Transform playerListContent;

    [SerializeField] private TMP_InputField playerNameText;
    [SerializeField] private GameObject playerListItemPrefab;


    [SerializeField] private GameObject startGameButton;
    private static NetworkManager _instance;

    public static NetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(NetworkManager)) as NetworkManager;
            }

            return _instance;
        }
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        Debug.Log("Connecting to Photon server...");
        // Connect to Photon server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect to master server");
        // Connect to the lobby after connecting to the "master" server
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    public void CreateRoom()
    {
        string roomName = roomNameInputField.text.Trim();

        if (string.IsNullOrEmpty(roomName))
        {
            return;
        }
        bool roomAlreadyExists = _cachedRoomList.Any(room => room.Name == roomName);

        if (roomAlreadyExists)
        {
            MenuManager.Instance.OpenMenu(MenuType.ErrorMenu);
            errorText.text = "Room name already exists!";
            return;
        }
        SetName();
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 4, IsVisible = true, IsOpen = true });
        MenuManager.Instance.OpenMenu(MenuType.LoadingScreen);
    }


    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu(MenuType.RoomMenu);
        PhotonNetwork.NickName =
            string.IsNullOrEmpty(playerNameText.text) ? PhotonNetwork.NickName : playerNameText.text;
        playerNameText.text = PhotonNetwork.NickName;
        playerNameText.interactable = false;
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        UpdatePlayerNameRoom();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _cachedRoomList.Clear();

        foreach (var room in roomList)
        {
            if (!room.RemovedFromList)
            {
                _cachedRoomList.Add(room);
            }
        }

        foreach (Transform t in roomListContent)
        {
            Destroy(t.gameObject);
        }


        foreach (var room in _cachedRoomList)
        {
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(room);
        }
    }


    private void UpdatePlayerNameRoom()
    {
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Transform child in playerListContent)
        {
            Destroy(child);
        }

        foreach (Player p in players)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(p);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void JoinRoom(RoomInfo roomInfo)
    {
        SetName();
        PhotonNetwork.JoinRoom(roomInfo.Name);
        MenuManager.Instance.OpenMenu(MenuType.LoadingScreen);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        playerNameText.interactable = true;
        MenuManager.Instance.OpenMenu(MenuType.LoadingScreen);
        //errorMessage.gameObject.SetActive(false);
    }

    public override void OnLeftRoom()
    {
        if (playerListContent != null)
        {
            foreach (Transform child in playerListContent)
            {
                Destroy(child.gameObject);
            }
        }

        MenuManager.Instance.OpenMenu(MenuType.MainMenu);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        MenuManager.Instance.OpenMenu(MenuType.ErrorMenu);
        errorText.text = message;
        Debug.LogError("We try to join room but error : " + message);
    }

    private void SetName()
    {
        PhotonNetwork.NickName = string.IsNullOrEmpty(playerNameText.text)
            ? "Player" + Random.Range(0, 10000).ToString("00000")
            : playerNameText.text;
    }

    public void RefreshRoomList()
    {
        if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InLobby) return;
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinLobby();
    }

    public void StarGame()
    {
        PhotonNetwork.LoadLevel(1);
    }
}