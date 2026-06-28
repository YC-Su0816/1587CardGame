using Photon.Pun;
using System;

public class PXXIX : PlayerBase
{
    private int casioRounds = 0;

    public override void Init()
    {
        // 【修正】：補上基礎的 1.0 倍率，確保基礎傷害不會被預設的 0.0 吃掉
        attackRatio = new double[] { 1.0, 1.0, 1.0 };
        defendRatio = new double[] { 1.0, 1.0, 1.0 };
        medRatio = new double[] { 1.0, 1.0, 1.0 };
        attackAdd = new int[3]; defendAdd = new int[3]; medAdd = new int[3];
    }

    public override void useSkill()
    {
        casioRounds = 2; // 接下來兩回合強化
        manager.photonView.RPC("Announcement", RpcTarget.All, handle.nickname + " 拿出了 Casio fx-991！", 1500);
    }

    public override void newRound() { }

    // 核心邏輯：強行篡改最終傷害
    public override void overrideFinalDamage(ref int w, ref int s, ref int r)
    {
        // 如果這是一張補血卡，或者根本沒有造成傷害 (數值 >= 0)，就不干涉
        if (w >= 0 && s >= 0 && r >= 0) return;

        int fixedDamage = 0;
        System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
        double rnd = rand.NextDouble();

        if (casioRounds > 0)
        {
            // 開了計算機：99% 會造成 15 點傷害，1% 造成 1 點傷害
            if (rnd <= 0.99) fixedDamage = -15;
            else fixedDamage = -1;
        }
        else
        {
            // 預設理論：80% 會造成 8 點傷害，20% 造成 1 點傷害
            if (rnd <= 0.80) fixedDamage = -8;
            else fixedDamage = -1;
        }

        // 只覆寫原本有「打算」造成傷害的對應屬性
        if (w < 0) w = fixedDamage;
        if (s < 0) s = fixedDamage;
        if (r < 0) r = fixedDamage;
    }

    // --- 其他必須實作的空函式 ---
    public override void updateAttack() { }
    public override void updateDefend() { }
    public override void updateMed() { }
    public override void endRound()
    {

        if (casioRounds > 0) casioRounds--;

    }
}