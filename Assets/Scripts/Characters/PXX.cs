using UnityEngine;
using Photon.Pun;
using System;
using System.Text;

public class PXX : PlayerBase
{
    private int cooldown = 0;

    public override void Init()
    {
        attackRatio = new double[] { 1.0, 1.0, 1.0 };
        defendRatio = new double[] { 1.0, 1.0, 1.0 };
        medRatio = new double[] { 1.0, 1.0, 1.0 };
        attackAdd = new int[3]; defendAdd = new int[3]; medAdd = new int[3];
    }

    public override void newRound()
    {
        if (cooldown > 0) cooldown--;

        // 被動技能：自律 (每回合回復 2 點智力與體力)
        manager.UpdatePlayerProperties(2, 2, 0);

        // 播個小提示讓大家知道他乖乖睡覺補血了
        manager.photonView.RPC("Announcement", RpcTarget.All, handle.nickname + " 透過自律維持了良好作息！", 1500);
    }

    // 主動技能：中醫 (冷卻時間: 5回合)
    public override void useSkill()
    {
        if (cooldown > 0) return;
        cooldown = 5;

        System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());

        // 使用正確的 getProperties 獲取當前屬性
        int[] mine = handle.getProperties();

        // 獲取最大值上限
        int[] maxes = new int[] { manager.maxW, manager.maxS, manager.maxR };

        int lowestVal = int.MaxValue;
        int lowestIdx = -1;
        int highestVal = int.MinValue;
        int highestIdx = -1;

        // 掃描找出最高與最低的數值及其索引
        for (int i = 0; i < 3; i++)
        {
            if (mine[i] < lowestVal)
            {
                lowestVal = mine[i];
                lowestIdx = i;
            }
            if (mine[i] > highestVal)
            {
                highestVal = mine[i];
                highestIdx = i;
            }
        }

        int[] diffs = new int[3] { 0, 0, 0 };
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("望聞問切...");

        if (rand.NextDouble() <= 0.8f)
        {
            // 80% 機率：成功，最低數值恢復至最大值的 0.8
            sb.Append("診斷成功！最低數值大幅恢復！");
            int targetVal = (int)(maxes[lowestIdx] * 0.8f);

            // 使用 Mathf.Max 確保如果原本數值已經高於 0.8 不會反向扣血
            diffs[lowestIdx] = Mathf.Max(0, targetVal - mine[lowestIdx]);
        }
        else
        {
            // 20% 機率：失敗，最高數值變為當前最低數值 - 1
            sb.Append("診斷失誤...最高數值暴跌！");
            int targetVal = lowestVal - 1;

            // 這裡算出來的 diffs 必定為負數（扣血）
            diffs[highestIdx] = targetVal - mine[highestIdx];
        }

        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 2500);

        handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.me);
        handle.View.RPC("GetCard", RpcTarget.All, "medicine", "XXSkill");

        // 將計算好的屬性變化量（diffs）送進 Played 中結算
        handle.View.RPC("Played", RpcTarget.All, diffs[0], diffs[1], diffs[2]);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    public override void updateAttack() { }
    public override void updateDefend() { }
    public override void updateMed() { }
    public override void endRound() { }
}