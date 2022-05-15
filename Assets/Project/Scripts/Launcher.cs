using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;


    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        MenuManger.Instance.OpenMenu("title");
        Debug.Log("joined Lobby");
    }
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;

        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManger.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManger.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        Debug.Log("joined room");

    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room creation failed: " + message;
        MenuManger.Instance.OpenMenu("error");
        Debug.Log("failed to join a room");
    }
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManger.Instance.OpenMenu("loading");
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManger.Instance.OpenMenu("loading");
    }
    public override void OnLeftRoom()
    {
        MenuManger.Instance.OpenMenu("title");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for(int i = 0; i <roomList.Count; i++)
        {
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().Setup(roomList[i] );
        }
    }
}
