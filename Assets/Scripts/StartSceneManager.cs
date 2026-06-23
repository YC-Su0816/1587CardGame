using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class StartSceneManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField InputPlayerName;
    public void OnClickStart()
    {
        string playername = InputPlayerName.text;
        if(playername.Length > 0)
        {

            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.LocalPlayer.NickName = playername;

        }
        else
        {
            Debug.Log("Invalid UserName");
        }
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected");
        SceneManager.LoadScene("LobbyScene");
        //base.OnConnectedToMaster();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
