using ExitGames.Client.Photon.StructWrapping;
using NUnit.Framework;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using HashTable = ExitGames.Client.Photon.Hashtable;

public class PickingSceneManager : MonoBehaviourPunCallbacks
{
    public bool sss = false;
    public GameObject[] obj = new GameObject[3];
    //public Button[] selecting = new Button[3];
    public Button button;
    public string picking;
    public TMP_Text wait;
    public GameObject panel;
    public Image image;
    public TMP_Text pro, bepp, ving;
    public int decided = 0;
    private bool selfpickingstatus = false;
    private bool everypickingstatus = false;
    private float delta = 0f;
    private float span = 0.5f;
    public string[] inf;
    Vector3 size = new Vector3(0.3f, 0.3f, 0.3f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [PunRPC]
    void ReceiveValue(string card)
    {
        Debug.Log("收" + card);
        string[] words = card.Split(' ');
        for(int j = 0; j<3; j++)
        {
            obj[j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("image/CCard/" + words[j]);
            obj[j].GetComponent<CharacterCard>().card = words[j];

            obj[j].GetComponent<CharacterCard>().SetupSize();
        }
    }
    [PunRPC]
    void StartGame(string start)
    {
        SceneManager.LoadScene("GameScene");
    }
    void Start()
    {
        picking = "nobody";
        panel.SetActive(false);
        Application.targetFrameRate = 60;
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        wait.gameObject.SetActive(false);
        if (PhotonNetwork.IsMasterClient)
        {
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            List<int> card = new List<int>();
            for (int j = 1; j <= 30; j++)
            {
                card.Add(j);
            }
            int times = 0;
            System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
            foreach (var kvp in PhotonNetwork.CurrentRoom.Players)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i <= 2; i++)
                {
                    float rnd = (float)rand.NextDouble();
                    //if(times == 0) rnd = 0.22f;
                    sb.Append(card[(int)(rnd * (30 - 3 * times - i))].ToString() + ' ');
                    card.RemoveAt((int)(rnd * (30 - 3 * times - i)));
                }
                photonView.RPC("ReceiveValue", kvp.Value, sb.ToString());
                times++;
            }
        }
    }
    public void OnDecidedPick()
    {
        sss = true;
        foreach(GameObject k in obj)
        {
            k.GetComponent<CharacterCard>().enabled = false;
        }
        /*foreach(Button b in selecting)
        {
            b.gameObject.SetActive(false);
        }*/
        GameObject pick = GameObject.Find(picking);

        pick.transform.localScale = pick.GetComponent<CharacterCard>().normalScale;

        //pick.GetComponent<Transform>().localScale = size;
        button.interactable = false;
        wait.gameObject.SetActive(true);
        TextAsset det = Resources.Load<TextAsset>("text/CCard/" + pick.GetComponent<CharacterCard>().card);
        inf = det.text.Split("\n");
        for (int i = 0; i < inf.Length; i++)
        {
            inf[i] = inf[i].Trim();
        }
        HashTable table = new HashTable();
        table.Add("Picking", pick.GetComponent<CharacterCard>().card);
        table.Add("Wisdom", int.Parse(inf[0]));
        table.Add("Strength", int.Parse(inf[1]));
        table.Add("Reputation", int.Parse(inf[2]));
        PhotonNetwork.LocalPlayer.SetCustomProperties(table);
        selfpickingstatus = true;
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, HashTable changedProps)
    {
        if (changedProps.ContainsKey("Picking"))
        {
            decided++;
            Debug.Log("選了");
        }
        if(decided == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("All Set");
            photonView.RPC("StartGame", RpcTarget.All, "go");
            everypickingstatus = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        delta += Time.deltaTime;
        int dots = ((int)(delta/span)) % 3;
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i <= dots; i++)
        {
            sb.Append(".");
        }
        if (everypickingstatus)
        {
            wait.text = "正在引導進入教室"+sb.ToString();
        }
        else if(selfpickingstatus)
        {
            wait.text = "等待其他玩家確認"+sb.ToString();
        }
        else
        {
            button.interactable = !(picking == "nobody");
        }
            
    }
    public void OnCloseClick()
    {
        panel.SetActive(false);
    }
    public void OnDetailClick()
    {
        panel.SetActive(true);
        GameObject go = GameObject.Find(picking);
        image.sprite = go.GetComponent<SpriteRenderer>().sprite;
        TextAsset det = Resources.Load<TextAsset>("text/CCard/" + go.GetComponent<CharacterCard>().card);
        inf = det.text.Split("\n");
        for (int i = 0; i < inf.Length; i++)
        {
            inf[i] = inf[i].Trim();
        }
        string[] p = {"智慧:", inf[0].ToString(), "體力:", inf[1].ToString(), "聲譽:", inf[2].ToString()};
        pro.text = String.Join(" ", p);
        bepp.text = "被動 " + inf[3] + "\n" + inf[4];
        ving.text = "主動 " + inf[5] + "\n" + inf[6];
    }
}
