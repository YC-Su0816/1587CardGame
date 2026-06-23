using UnityEngine;
using Photon.Pun;
using System;
using System.Text;

public class PXXVI : PlayerBase
{
    public override void Init()
    {
        attackRatio = new double[] { 1.0, 1.0, 1.0 };
        defendRatio = new double[] { 1.0, 1.0, 1.0 };
        medRatio = new double[] { 1.0, 1.0, 1.0 };
        attackAdd = new int[3]; defendAdd = new int[3]; medAdd = new int[3];
    }

    // 被動技能：摺疊機 (實用性待議，目前完全不影響遊戲邏輯)
    public override void newRound() { }

    // 主動技能：古文朗誦 (冷卻時間: 2回合由系統接管)
    public override void useSkill()
    {
        System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("測試《燭之武退劉姥姥》與《蹇叔逛大觀園》的讀音...");

        if (rand.NextDouble() <= 0.8f)
        {
            // ==========================================
            // 80% 機率成功：打出 AI 卡牌並恢復所有數值 5 點
            // ==========================================
            sb.Append("標註成功！" + handle.nickname + " 恢復了所有數值！");
            handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 2000);

            handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.me);
            handle.View.RPC("GetCard", RpcTarget.All, "unblockable", "AI");

            // 傳入正數 (5, 5, 5) 補血，系統會幫忙跑完動畫跟結束回合
            handle.View.RPC("Played", RpcTarget.All, 5, 5, 5);
        }
        else
        {
            // ==========================================
            // 20% 機率失敗：不打牌，直接扣聲譽並結束回合
            // ==========================================
            sb.Append("標註失敗！模型發生崩潰...");
            handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 2000);

            // 直接扣除 10 點聲譽 (w=0, s=0, r=-10)
            manager.UpdatePlayerProperties(0, 0, -10);

            // 既然沒出牌，我們必須手動結束回合並強行交棒給下家
            manager.status = 0;
            manager.player.endRound();
            handle.View.RPC("Go", manager.LocalPlayerList[(manager.me + 1) % manager.total]);
        }

        PhotonNetwork.SendAllOutgoingCommands();
    }

    public override void updateAttack() { }
    public override void updateDefend() { }
    public override void updateMed() { }
    public override void endRound() { }
}