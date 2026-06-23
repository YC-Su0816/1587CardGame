using Photon.Pun;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System;
using UnityEngine;

public class PXIII : PlayerBase
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
    //public override void useSkill()
    //{
    //    // 鎖定自己
    //    manager.status = 0;

    //    StringBuilder sb = new StringBuilder();
    //    sb.AppendLine("(精湛的舞姿)");
    //    sb.Append(handle.nickname + " 使用了技能！");

    //    int[] mine = manager.handle.getProperties();
    //    int lowest = 100;
    //    int index = 0;
    //    for (int i = 0; i < 3; ++i)
    //    {
    //        if (mine[i] < lowest)
    //        {
    //            lowest = mine[i];
    //            index = i;
    //        }
    //    }

    //    int[] heal = new int[3]; // 用來記錄要補多少血
    //    switch (index)
    //    {
    //        case 0: heal[0] = (int)((manager.maxW - mine[0]) * 0.8f); break;
    //        case 1: heal[1] = (int)((manager.maxS - mine[1]) * 0.8f); break;
    //        case 2: heal[2] = (int)((manager.maxR - mine[2]) * 0.8f); break; // 修正了這裡的 mine[0] 錯誤！
    //    }

    //    // 直接用 unblockable 來秀卡牌，並且強制加血給自己
    //    handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);
    //    handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.me);
    //    handle.View.RPC("GetCard", RpcTarget.All, "unblockable", "dance");

    //    // 注意：因為 Played() 裡面的 unblockable 是用來「扣血」的，所以我們傳入正數，在 UpdatePlayerProperties 裡就會變成補血！
    //    handle.View.RPC("Played", RpcTarget.All, heal[0], heal[1], heal[2]);
    //    PhotonNetwork.SendAllOutgoingCommands();
    //}
    public override void useSkill()
    {
        manager.status = 0;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("(精湛的舞姿)");
        sb.Append(handle.nickname + " 使用了技能！");
        int[] mine = new int[3];
        mine[0] = (int)manager.LocalPlayerList[manager.me].CustomProperties["Wisdom"];
        mine[1] = (int)manager.LocalPlayerList[manager.me].CustomProperties["Strength"];
        mine[2] = (int)manager.LocalPlayerList[manager.me].CustomProperties["Reputation"];
        int lowest = 100;
        int index = 0;
        for (int i = 0; i < 3; ++i)
        {
            if (mine[i] < lowest)
            {
                lowest = mine[i];
                index = i;
            }
        }
        switch (index)
        {
            case 0:
                mine[0] = (int)((manager.maxW - mine[0])*0.8f);
                mine[1] = 0;
                mine[2] = 0;
                break;
            case 1:
                mine[0] = 0;
                mine[1] = (int)((manager.maxS - mine[1]) * 0.8f);
                mine[2] = 0;
                break;
            case 2:
                mine[0] = 0;
                mine[1] = 0;
                mine[2] = (int)((manager.maxR - mine[2]) * 0.8f);
                break;
        }
        handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.me);
        handle.View.RPC("GetCard", RpcTarget.All, "unblockable", "dance");
        handle.View.RPC("Played", RpcTarget.All, mine[0], mine[1], mine[2]);
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
}
