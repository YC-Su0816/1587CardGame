using Photon.Pun;
using System.Text;
using UnityEngine;

public class PIX : PlayerBase
{
    public override void Init()
    {

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
        manager.status = 0;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Dio lin lou mou");
        sb.Append(handle.nickname + " 使用了技能！");
        manager.photonView.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);
        if (manager.targetnum == -1)
        {
            int targetIdx = (manager.me + 1) % manager.total;
            while (!manager.isAlive[targetIdx] || 
                   manager.PlayerPanels[targetIdx].GetComponent<PlayerPanelController>().isExist("disappear") || 
                   manager.PlayerPanels[targetIdx].GetComponent<PlayerPanelController>().isExist("sleep"))
            {
                targetIdx = (targetIdx + 1) % manager.total;
                if (targetIdx == manager.me) break;
            }
            manager.targetnum = targetIdx;
        }
        manager.photonView.RPC("SetFromTo", RpcTarget.All, manager.me, manager.targetnum);
        manager.photonView.RPC("GetCard", RpcTarget.All, "attack", "balisong");
        manager.photonView.RPC("Played", RpcTarget.All, -10, 0, 0);
        PhotonNetwork.SendAllOutgoingCommands();
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

    }
    public override void endRound()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
