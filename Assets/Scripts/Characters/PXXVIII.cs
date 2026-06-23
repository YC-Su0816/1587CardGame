using Photon.Pun;
using UnityEngine;

public class PXXVIII : PlayerBase
{
    private bool tarotActive = false;  // 技能是否發動
    private bool tarotDamaged = false; // 發動期間是否有受傷

    public override void Init() {
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
        tarotActive = true;
        tarotDamaged = false; // 發動時重置受傷判定
        manager.photonView.RPC("Announcement", RpcTarget.All, handle.nickname + " 發動了塔羅占卜！", 1500);
    }

    // 【解決問題 2】：只要有受傷，就失去回血資格
    public override void onTakeDamage(int w, int s, int r)
    {
        if (tarotActive)
        {
            tarotDamaged = true;
        }
    }

    public override void newRound()
    {
        if (tarotActive)
        {
            // 若到下次行動前「未受到任何傷害」
            if (!tarotDamaged)
            {
                manager.UpdatePlayerProperties(10, 10, 10);
                manager.photonView.RPC("Announcement", RpcTarget.All, handle.nickname + " 塔羅占卜生效，回復大量狀態！", 1500);
            }
            tarotActive = false; // 狀態解除
        }
    }

    // 【解決問題 1】：只復活「剛剛」才致死的數值
    public override bool checkRevive(ref int w, ref int s, ref int r)
    {
        if (tarotActive)
        {
            bool revived = false;

            // 利用 handle 取得「這發傷害扣下去之前」的原始狀態
            int[] oldProps = handle.getProperties();

            // 條件：原本還活著 (old > 0) 且 現在被打死了 (new <= 0)
            if (oldProps[0] > 0 && w <= 0) { w = 1; revived = true; }
            if (oldProps[1] > 0 && s <= 0) { s = 1; revived = true; }
            if (oldProps[2] > 0 && r <= 0) { r = 1; revived = true; }

            if (revived)
            {
                tarotActive = false; // 成功擋下致命傷，塔羅牌消耗掉
                return true;
            }
        }
        return false;
    }

    // 順便補上 28 號的被動：免疫群攻
    public override bool isImmuneToMultiAttack()
    {
        return true;
    }

    // --- 其他必須實作的空函式 ---
    public override void updateAttack() { }
    public override void updateDefend() { }
    public override void updateMed() { }
    public override void endRound() { }
}
