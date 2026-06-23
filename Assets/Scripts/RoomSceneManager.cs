using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using System.Text;
using Photon.Realtime;

public class RoomSceneManager : MonoBehaviourPunCallbacks
{
    public TMP_Text textRoomname;
    public TMP_Text playername;
    public TMP_Text playerlist;
    public Button button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [PunRPC]
    void upd(string list)
    {
        playerlist.text = list;
    }
    void Start()
    {
        if(PhotonNetwork.CurrentRoom == null)
        {
            SceneManager.LoadScene("LobbyScene");
        }
        else
        {
            textRoomname.text = "目前房間：" + PhotonNetwork.CurrentRoom.Name;
            if(PhotonNetwork.IsMasterClient)
            {
                playerlist.text = PhotonNetwork.LocalPlayer.NickName;
            }
        }
        button.interactable = PhotonNetwork.IsMasterClient;
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        button.interactable = PhotonNetwork.IsMasterClient;
    }
    public StringBuilder UpdatePlayerList()
    {
        StringBuilder sb = new StringBuilder();
        foreach(var kvp in PhotonNetwork.CurrentRoom.Players)
        {
            sb.AppendLine(kvp.Value.NickName);
        }
        return sb;
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StringBuilder sb = UpdatePlayerList();
            photonView.RPC("upd", RpcTarget.All, sb.ToString());
        }
        
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StringBuilder sb = UpdatePlayerList();
            photonView.RPC("upd", RpcTarget.All, sb.ToString());
        }
    }   
    public void OnClickStartGame()
    {
        SceneManager.LoadScene("PickingScene");
        Debug.Log("GameStart");

    }
    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
