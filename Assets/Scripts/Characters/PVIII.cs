using Photon.Pun;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PVIII : PlayerBase
{
    GameObject skillPrefab;

    Transform transform;
    double chanceCounter;
    int incrementCounter;
    bool flag;
    public Photon.Realtime.Player guessPlayer;
    public string guessType;
    public override void Init()
    {
        skillPrefab = handle.VIIIskillPrefab;
        guessPlayer = null;
        guessType = null;
        transform = manager.UIparent;
        flag = false;
        attackRatio = new double[3];
        for (int i = 0; i < 3; ++i)
        {
            attackRatio[i] = 1.0f;
        }
        defendRatio = new double[3];
        for (int i = 0; i < 3; ++i)
        {
            defendRatio[i] = 1.0f;
        }
        medRatio = new double[3];
        for (int i = 0; i < 3; ++i)
        {
            medRatio[i] = 1.0f;
        }
        attackAdd = new int[3];
        defendAdd = new int[3];
        medAdd = new int[3];
    }
    public override void useSkill()
    {
        GameObject controlBoard = GameObject.Instantiate(skillPrefab, transform);
        PVIIISkill mono = controlBoard.GetComponent<PVIIISkill>();
        List<string> nameList = new List<string>();
        List<string> typeList = new List<string>();
        for(int i = 0; i < manager.total; ++i)
        {
            if (i != manager.me && manager.isAlive[i])
            {
                nameList.Add(manager.LocalPlayerList[i].NickName);
            }
        }
        typeList.Add("攻擊");
        typeList.Add("防禦");
        typeList.Add("其他");
        mono.init(this, nameList, typeList);
    }
    public void confirmChoice(string pChoice, string tChoice)
    {
        guessType = tChoice;
        foreach (Photon.Realtime.Player player in manager.LocalPlayerList)
        {
            if (player.NickName == pChoice)
            {
                guessPlayer = player;
                break;
            }
        }
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("En Passant and... Check");
        sb.Append(handle.nickname + " 使用了技能！");
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);
        PhotonNetwork.SendAllOutgoingCommands();
    }
    public void rightGuess()
    {
        manager.UpdatePlayerProperties(0, 5, 0);
        guessPlayer = null;
        guessType = null;
    }
    public void wrongGuess()
    {
        guessPlayer = null;
        guessType = null;
    }
    public override void updateAttack()
    {

    }
    public override void updateDefend()
    {

    }
    public override void updateMed()
    {

    }
    public override void newRound()
    {
        guessPlayer = null;
        guessType = null;
    }
    public override void endRound()
    {
        Debug.Log("我喊了");
        manager.UpdatePlayerProperties(0, -2, 0);
        Debug.Log("Update？");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
