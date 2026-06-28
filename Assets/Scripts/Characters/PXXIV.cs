using UnityEngine;
using Photon.Pun;
using System;
using System.Text;

public class PXXIV : PlayerBase
{
    
    private float strengthDefBoost = 1.3f; // 初始體力防禦提升 30%

    public override void Init()
    {
        attackRatio = new double[] { 1.0, 1.0, 1.0 };
        defendRatio = new double[] { 1.0, 1.0, 1.0 };
        medRatio = new double[] { 1.0, 1.0, 1.0 };
        attackAdd = new int[3]; defendAdd = new int[3]; medAdd = new int[3];
    }

    public override void newRound()
    {

    }

    // 主動技能：一代傳奇 (冷卻時間: 3回合)
    public override void useSkill()
    {
        // 【防呆】：只能在被攻擊 (請回應) 的階段使用
        if (manager.status != 2)
        {
            manager.hintword.text = "此技能只能在遭受攻擊時使用！";
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("保證自己有大學的手段！");
        sb.Append(handle.nickname + " 發動了【國際瑞士人才論壇】！");
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 2000);

        // 如果玩家手殘已經先拉了防禦牌上去才按技能，幫他把展示區清空 (牌會被消耗掉當作防呆代價)
        if (manager.displaycount > 0)
        {
            for (int i = 0; i < 6; i++)
            {
                if (manager.CardsInDisplay[i].Count > 0)
                {
                    foreach (GameObject obj in manager.CardsInDisplay[i])
                    {
                        obj.GetComponent<ToolDisplayController>().kill();
                    }
                    manager.CardsInDisplay[i].Clear();
                }
            }
            manager.displaycount = 0;
        }

        // 1. 廣播秀出防禦卡 "ISTF"
        handle.View.RPC("GetCard", RpcTarget.All, "defense", "ISTF");

        // 2. 直接強制回傳 0 傷害，完美免疫！
        handle.View.RPC("Responded", RpcTarget.All, -manager.daW, -manager.daS, -manager.daR);

        // 3. 本地端安全下莊
        manager.status = 0;
        manager.RefreshCards();
        PhotonNetwork.SendAllOutgoingCommands();
    }

    // 被動技能 A：防禦倍率更新
    public override void updateDefend()
    {
        defendRatio[0] = 1.3f; // 智力防禦提升 30%
        defendRatio[1] = strengthDefBoost; // 體力防禦 (會隨受傷遞減)
        defendRatio[2] = 1.3f; // 聲譽防禦提升 30%
    }

    // 被動技能 B：監聽受傷事件 (扣除體力防禦加成)
    public override void onTakeDamage(int w, int s, int r)
    {
        // s < 0 代表受到體力傷害，且體力防禦加成還沒扣完
        if (s < 0 && strengthDefBoost > 0f)
        {
            // 每次下降 10% (0.1)，最低歸 0
            strengthDefBoost = Mathf.Max(0f, strengthDefBoost - 0.1f);

            // 廣播通知大家總召的身體越來越差了
            manager.photonView.RPC("Announcement", RpcTarget.All, handle.nickname + " 體力不支，體力防禦效果下降了！", 1500);
        }
    }

    public override void updateAttack() { }
    public override void updateMed() { }
    public override void endRound() { }
}