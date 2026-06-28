using UnityEngine;
using Photon.Pun;
using System.Text;

public class PXXVII : PlayerBase
{
    private bool isSkillActive = false;

    public override void Init()
    {
        attackRatio = new double[] { 1.0, 1.0, 1.0 };
        defendRatio = new double[] { 1.0, 1.0, 1.0 };
        medRatio = new double[] { 1.0, 1.0, 1.0 };
        attackAdd = new int[3]; defendAdd = new int[3]; medAdd = new int[3];
    }

    // 被動技能：愛情長跑
    public override void newRound()
    {
        // 每回合開始時恢復所有數值 1 點
        manager.UpdatePlayerProperties(1, 1, 1);

        // 播個小提示放閃
        manager.photonView.RPC("Announcement", RpcTarget.All, handle.nickname + " 受到愛情的滋潤，恢復了所有數值！", 1500);
    }

    // 主動技能：英文鬼才 (冷卻 6 回合由系統控管)
    public override void useSkill()
    {
        // 啟動篡改 Buff
        isSkillActive = true;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("如同中文翻譯成英文時的語法錯亂...");
        sb.Append(handle.nickname + " 發動了英文鬼才！");

        // 單純文字展演，不強制結束回合，玩家接下來可以馬上打出攻擊牌
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 2000);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    // 攔截器：實作發送出去前的數值篡改
    public override void overrideFinalDamage(ref int w, ref int s, ref int r)
    {
        // 確保技能已啟動，而且這張真的是「攻擊牌」(有造成負數傷害)
        if (isSkillActive && (w < 0 || s < 0 || r < 0))
        {
            // 注意：因為傷害是「負數」，所以在數學上扣最多的血反而是「最小值」
            int maxDamage = Mathf.Min(w, Mathf.Min(s, r));

            // 將三項屬性的傷害同時強制作為該最大傷害
            w = maxDamage;
            s = maxDamage;
            r = maxDamage;

            // 攻擊發射後，消耗掉這次的 Buff
            isSkillActive = false;
        }
    }

    // 如果他這回合開了技能卻沒攻擊，回合結束時自動把 Buff 取消掉
    public override void endRound()
    {
        isSkillActive = false;
    }

    public override void updateAttack() { }
    public override void updateDefend() { }
    public override void updateMed() { }
}