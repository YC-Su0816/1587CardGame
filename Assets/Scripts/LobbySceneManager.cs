using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Photon.Realtime;
using System.Text;

public class LobbySceneManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField inputroomname;
    public Button joinButton;
    public Button creatButton;
    public TMP_Text hintWord;
    public TMP_Text roomlist;
    public bool canStart;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(PhotonNetwork.IsConnected == false)
        {
            SceneManager.LoadScene("StartScene");
        }
        else
        {
            if(PhotonNetwork.CurrentLobby == null)
            {
                PhotonNetwork.JoinLobby();
            }
        }
            
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("JoinedLobby");
    }

    public string GetRoomName()
    {
        string roomName = inputroomname.text; 
        return roomName.Trim();
    }

    public void OnClickCreatRoom()
    {
        string roomName = GetRoomName();

        if (roomName.Length > 0)
        {
            PhotonNetwork.CreateRoom(roomName);
        }
        else
        {
            Debug.Log("Invalid Room Name");
        }
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined");
        SceneManager.LoadScene("ReadyScene");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        StringBuilder sb = new StringBuilder();
        foreach(RoomInfo roomInfo in roomList)
        {
            if (roomInfo.PlayerCount >= 1)
            {
                sb.AppendLine("→" + roomInfo.Name + "　　　目前人數："+roomInfo.PlayerCount);
            }
            
        }
        if (sb.Length <= 0)
        {
            sb.AppendLine("Empty");
        }
        roomlist.text = sb.ToString();
    }
    // Update is called once per frame
    public void OnClickJoinRoom()
    {
        string roomName = GetRoomName();
        if (roomName.Length > 0)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            Debug.Log("Invalid RoomName");
        }

    }

    void Update()
    {
        string playername = inputroomname.text;
        if(playername.Length <= 0)
        {
            hintWord.text = "房號不得為空";
            canStart = false;
        }
        else if(playername.Length >= 6)
        {
            hintWord.text = "房號請在五字以內";
            canStart = false;
        }
        else
        {
            hintWord.text = "你開心就好";
            canStart = true;
        }
        joinButton.enabled = canStart;
        creatButton.enabled = canStart;
    }
}
