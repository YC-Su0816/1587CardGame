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
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("1..23，跳！");
        System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
        float rnd = (float)rand.NextDouble();
        if(rnd <= 0.7f)
        {
            sb.AppendLine("...Touch OUT！");
            sb.Append(handle.nickname + " 使用了技能！");
            handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);
            if (manager.targetnum == -1)
            {
                manager.targetnum = (manager.me + 1) % manager.total;
            }
            handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.targetnum);
            handle.View.RPC("GetCard", RpcTarget.All, "attack", "volleyball");
            handle.View.RPC("Played", RpcTarget.All, -10, -10, 0);
            PhotonNetwork.SendAllOutgoingCommands();
        }
        else
        {
            sb.AppendLine("...觸網+OUTSIDE！");
            sb.Append(handle.nickname + " 使用了技能！");
            handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);
            if (manager.targetnum == -1)
            {
                manager.targetnum = (manager.me + 1) % manager.total;
            }
            handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.targetnum);
            handle.View.RPC("GetCard", RpcTarget.All, "attack", "volleyball");
            handle.View.RPC("Played", RpcTarget.All, 0, 0, 0);
            PhotonNetwork.SendAllOutgoingCommands();
        }        
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
}
