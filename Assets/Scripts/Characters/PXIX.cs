using UnityEngine;
using Photon.Pun;
using System.Text;

public class PXIX : PlayerBase
{
    public override void Init()
    {
        attackRatio = new double[] { 1.0, 1.0, 1.0 };
        defendRatio = new double[] { 1.0, 1.0, 1.0 };
        medRatio = new double[] { 1.0, 1.0, 1.0 };
        attackAdd = new int[3]; defendAdd = new int[3]; medAdd = new int[3];
    }

    public override void newRound() { }

    // 主動技能：經濟實惠
    public override void useSkill()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("保證自己的最大利益！");
        sb.Append(handle.nickname + " 使用了經濟實惠！");
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);

        // 重新設定數值：三者的平均值的 0.5 + 25
        // 【修正】使用正確的 handle.getProperties() 語法
        int[] mine = handle.getProperties();
        int avg = (mine[0] + mine[1] + mine[2]) / 3;
        int finalVal = (int)(avg * 0.5f + 25);

        int[] change = new int[3];
        for(int i = 0; i < 3; ++i)
        {
            change[i] = finalVal - mine[i];
        }


        // 再呼叫卡牌展演，送 0 傷害進去過水
        handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.me);
        handle.View.RPC("GetCard", RpcTarget.All, "special", "XIXSkill");
        handle.View.RPC("Played", RpcTarget.All, change[0], change[1], change[2]);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    // 被動技能：ㄏㄧㄏ
    public override void overrideFinalDamage(ref int w, ref int s, ref int r)
    {
        // 傷害為負數才觸發 mod 10 (在 C# 裡，負數取餘數依然會保留負號，完美符合扣血運算)
        if (w < 0) w %= 10;
        if (s < 0) s %= 10;
        if (r < 0) r %= 10;
    }

    public override void updateAttack() { }
    public override void updateDefend() { }
    public override void updateMed() { }
    public override void endRound() { }
}