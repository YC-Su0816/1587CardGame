using UnityEngine;
using Photon.Pun;
using System;
using System.Text;

public class PXVIII : PlayerBase
{
    public override void Init()
    {
        attackRatio = new double[] { 1.0, 1.0, 1.0 };
        defendRatio = new double[] { 1.0, 1.0, 1.0 };
        medRatio = new double[] { 1.0, 1.0, 1.0 };
        attackAdd = new int[3]; defendAdd = new int[3]; medAdd = new int[3];
    }

    public override void newRound() { }

    // 主動技能：捧場
    public override void useSkill()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Pioneer與Farewell Neverland皆有出席。");
        sb.Append(handle.nickname + " 使用了捧場！");
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);

        handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.me);
        handle.View.RPC("GetCard", RpcTarget.All, "unblockable", "XVIIISkill");

        // 消耗體力2點，恢復10點聲譽 (w=0, s=-2, r=10)
        handle.View.RPC("Played", RpcTarget.All, 0, -2, 10);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    // 被動技能：特別容易成為玩笑的目標
    public override void updateDefend()
    {
        for (int i = 0; i < 3; i++)
        {
            defendAdd[i] = -2; // 防禦值常駐減少2
        }
    }

    // 被動技能：自我規劃 (回合結束時判定)
    public override void endRound()
    {
        PlayerPanelController myPanel = manager.PlayerPanels[manager.me].GetComponent<PlayerPanelController>();
        if (!myPanel.isExist("disappear"))
        {
            System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
            if (rand.NextDouble() <= 0.2f)
            {
                manager.photonView.RPC("Announcement", RpcTarget.All, handle.nickname + " 自我規劃，進入了神隱！", 1500);
                manager.photonView.RPC("PutEffect", RpcTarget.All, manager.me, -1, "disappear");
            }
        }
    }

    public override void updateAttack() { }
    public override void updateMed() { }
}