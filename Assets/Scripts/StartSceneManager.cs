using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class StartSceneManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField InputPlayerName;
    public Button startButton;
    public TMP_Text hintWord;
    public bool canStart;
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
        canStart = false;
    }

    // Update is called once per frame
    void Update()
    {
        string playername = InputPlayerName.text;
        if(playername.Length <= 0)
        {
            hintWord.text = "名稱不得為空";
            canStart = false;
        }
        else if(playername.Length >= 11)
        {
            hintWord.text = "名稱請在十字以內";
            canStart = false;
        }
        else
        {
            hintWord.text = "真是個好名字";
            canStart = true;
        }
        startButton.enabled = canStart;
    }
    
}
