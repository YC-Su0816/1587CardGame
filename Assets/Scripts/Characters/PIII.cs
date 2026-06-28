using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System.Text;
using UnityEngine;

public class PIII : PlayerBase
{
    PlayerPanelController myPanel;
    int normal = 1, inSkill = 3;
    public override void Init()
    {
        myPanel = null;
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
        for (int i = 0; i < 3; ++i)
        {
            defendAdd[i] = 1;
        }
    }
    public override void useSkill()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("藍牙已連線");
        sb.Append(handle.nickname + " 使用了技能！");
        handle.View.RPC("PutEffect", RpcTarget.All, manager.me, 3, "headphone");
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1000);
        PhotonNetwork.SendAllOutgoingCommands();
    }
    public override void updateDefend()
    {
        if (!myPanel)
        {
            myPanel = manager.PlayerPanels[manager.me].GetComponent<PlayerPanelController>();
        }
        if (myPanel.isExist("headphone"))
        {
            for (int i = 0; i < 3; ++i)
            {
                defendAdd[i] = inSkill;
            }
        }
        else
        {
            for (int i = 0; i < 3; ++i)
            {
                defendAdd[i] = normal;
            }
        }
    }
    public override void updateAttack()
    {

    }
    public override void updateMed()
    {

    }
    public override void newRound()
    {

    }
    public override void endRound()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
