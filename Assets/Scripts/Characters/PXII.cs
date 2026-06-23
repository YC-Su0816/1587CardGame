using Photon.Pun;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System;
using UnityEngine;

public class PXII : PlayerBase
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
        sb.AppendLine("注意到，然後...");
        sb.AppendLine("得證。Q.E.D.");
        sb.Append(handle.nickname + " 使用了技能！");
        int[] mine = new int[3];
        mine[0] = (int)manager.LocalPlayerList[manager.me].CustomProperties["Wisdom"];
        mine[1] = (int)manager.LocalPlayerList[manager.me].CustomProperties["Strength"];
        mine[2] = (int)manager.LocalPlayerList[manager.me].CustomProperties["Reputation"];
        int highest = -1;
        for(int i = 0; i < 3; ++i)
        {
            if (mine[i] > highest) highest = mine[i];
        }
        manager.setPlayerProperties(highest, highest, highest);
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);
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
        System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
        float rnd1 = (float)rand.NextDouble(), rnd2 = (float)rand.NextDouble();
        Debug.Log(rnd1);
        Debug.Log(rnd2);
        if(rnd1 <= 0.3f)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("有靈感了！");
            if (rnd2 <= 0.33f)
            {
                manager.UpdatePlayerProperties(5, 0, 0);
                sb.Append("智慧恢復5點");
            }
            else if(rnd2 <= 0.67f)
            {
                manager.UpdatePlayerProperties(0, 5, 0);
                sb.Append("體力恢復5點");
            }
            else
            {
                manager.UpdatePlayerProperties(0, 0, 5);
                sb.Append("聲譽恢復5點");
            }
            handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);
            PhotonNetwork.SendAllOutgoingCommands();
        }

    }
    public override void endRound()
    {

    }
}
