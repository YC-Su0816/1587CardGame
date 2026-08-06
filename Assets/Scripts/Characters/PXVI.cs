using UnityEngine;
using Photon.Pun;
using System;
using System.Text;

public class PXVI : PlayerBase
{
    public override void Init()
    {
        attackRatio = new double[] { 1.0, 1.0, 1.0 };
        defendRatio = new double[] { 1.0, 1.0, 1.0 };
        medRatio = new double[] { 1.0, 1.0, 1.0 };
        attackAdd = new int[3]; defendAdd = new int[3]; medAdd = new int[3];
    }

    public override void newRound() { }

    // 主動技能：滷味面
    public override void useSkill()
    {
        double mult = (manager.rand.NextDouble() <= 0.35) ? 3.0 : 0.2;

        // 計算最終補血量 (智慧基礎 5，體力基礎 10)，聲譽固定扣 5 不受影響
        int finalW = (mult > 1.0) ? 15 : 1;
        int finalS = (mult > 1.0) ? 30 : 2;
        int finalR = -5;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("沒有什麼比得上一碗\n熱騰騰的滷味面，OK？");

        string resultText = (mult > 1.0) ? "【大起】超大碗！" : "【大落】只有一口...";
        sb.Append(handle.nickname + " 吃麵觸發了 " + resultText);

        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 2000);

        // 將目標設為自己，打出無法防禦的滷味面卡牌
        handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.me);
        handle.View.RPC("GetCard", RpcTarget.All, "medicine", "mian");

        // 傳入經過大起大落計算後的數值
        handle.View.RPC("Played", RpcTarget.All, finalW, finalS, finalR);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    // 封裝被動擲骰子邏輯，套用給所有出牌行動
    private void ApplyDaQiDaLuo(double[] ratioArray, string actionType)
    {

        double mult = (manager.rand.NextDouble() <= 0.35) ? 3.0 : 0.2;

        for (int i = 0; i < 3; i++)
        {
            ratioArray[i] = mult;
        }

        string resultText = (mult > 1.0) ? "【大起】數值爆發！(x3)" : "【大落】軟弱無力...(x0.2)";
        manager.photonView.RPC("Announcement", RpcTarget.All, handle.nickname + actionType + resultText, 1500);
    }

    public override void updateAttack()
    {
        ApplyDaQiDaLuo(attackRatio, " 的攻擊觸發了 ");
    }

    public override void updateDefend()
    {
        ApplyDaQiDaLuo(defendRatio, " 的防守觸發了 ");
    }

    public override void updateMed()
    {
        ApplyDaQiDaLuo(medRatio, " 的治療觸發了 ");
    }

    public override void endRound() { }
}
