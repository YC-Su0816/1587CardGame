using Photon.Pun;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System;
using UnityEngine;

public class PXI : PlayerBase
{
    public override void Init()
    {
        attackRatio = new double[3];
        for (int i = 0; i < 3; ++i) attackRatio[i] = 1.0f;
        defendRatio = new double[3];
        for (int i = 0; i < 3; ++i) defendRatio[i] = 1.0f;
        medRatio = new double[3];
        for (int i = 0; i < 3; ++i) medRatio[i] = 1.0f;
        
        attackAdd = new int[3];
        defendAdd = new int[3];
        medAdd = new int[3];
    }
    
    public override bool checkPassiveDodge()
    {
        System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
        if (rand.NextDouble() <= 0.2f)
        {
            manager.photonView.RPC("Announcement", RpcTarget.All, "呼...躲過了", 1500);
            return true; 
        }
        return false;
    }

    public override void useSkill()
    {
        manager.status = 0; 

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("1..23，跳！");
        System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
        
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

        if (rand.NextDouble() <= 0.7f)
        {
            sb.AppendLine("...Touch OUT！");
            sb.Append(handle.nickname + " 使用了技能！");
            
            manager.photonView.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);
            manager.photonView.RPC("SetFromTo", RpcTarget.All, manager.me, manager.targetnum);
            manager.photonView.RPC("GetCard", RpcTarget.All, "attack", "volleyball");
            manager.photonView.RPC("Played", RpcTarget.All, -10, -10, 0);
        }
        else
        {
            sb.AppendLine("...觸網+OUTSIDE！");
            sb.Append(handle.nickname + " 使用了技能！");
            
            manager.photonView.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);
            manager.photonView.RPC("SetFromTo", RpcTarget.All, manager.me, manager.targetnum);
            manager.photonView.RPC("GetCard", RpcTarget.All, "attack", "volleyball");
            manager.photonView.RPC("Played", RpcTarget.All, 0, 0, 0);
        }        
        
        PhotonNetwork.SendAllOutgoingCommands();
    }

    public override void updateAttack() { }
    public override void updateDefend() { }
    public override void updateMed() { }
    public override void newRound() { }
    public override void endRound() { }
}