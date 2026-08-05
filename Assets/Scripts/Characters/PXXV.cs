using UnityEngine;
using Photon.Pun;
using System;
using System.Text;

public class PXXV : PlayerBase
{
    public override void Init()
    {
        attackRatio = new double[] { 1.0, 1.0, 1.0 };
        defendRatio = new double[] { 1.0, 1.0, 1.0 };
        medRatio = new double[] { 1.0, 1.0, 1.0 };
        attackAdd = new int[3]; defendAdd = new int[3]; medAdd = new int[3];
    }

    // 被動技能：日本人 (每回合都會恢復 3 點「聲望」)
    public override void newRound()
    {
        
    }

    // 主動技能：托球與扣球
    public override void useSkill()
    {
        manager.status = 0;
        System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
        StringBuilder sb = new StringBuilder();

        // 50% 機率判定
        if (rand.NextDouble() <= 0.5f)
        {
            // ==========================================
            // 【托球】(Set) - 恢復所有屬性 5 點
            // ==========================================
            sb.AppendLine("精準的托球！");
            sb.Append(handle.nickname + " 恢復了所有屬性！");
            manager.photonView.RPC("Announcement", RpcTarget.All, sb.ToString(), 2000);
            manager.photonView.RPC("SetFromTo", RpcTarget.All, manager.me, manager.me);
            manager.photonView.RPC("GetCard", RpcTarget.All, "medicine", "set");
            manager.photonView.RPC("Played", RpcTarget.All, 5, 5, 5);
        }
        else
        {
            // ==========================================
            // 【扣球】(Spike) - 造成 10 點智力與 10 點體力傷害
            // ==========================================
            sb.AppendLine("威力十足的扣球！");
            sb.Append(handle.nickname + " 擊中了對手的頭部！");
            handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 2000);

            // 過濾無效目標
            if (manager.targetnum == -1)
            {
                int targetIdx = (manager.me + 1) % manager.total;
                while (!manager.isAlive[targetIdx] || 
                       manager.PlayerPanels[targetIdx].GetComponent<PlayerPanelController>().isExist("disappear") || 
                       manager.PlayerPanels[targetIdx].GetComponent<PlayerPanelController>().isExist("sleep"))
                {
                    targetIdx = (targetIdx + 1) % manager.total;
                    if (targetIdx == manager.me) break;
                }
                manager.targetnum = targetIdx;
            }

            manager.photonView.RPC("SetFromTo", RpcTarget.All, manager.me, manager.targetnum);
            manager.photonView.RPC("GetCard", RpcTarget.All, "attack", "spike");
            manager.photonView.RPC("Played", RpcTarget.All, -10, -10, 0);
        }

        PhotonNetwork.SendAllOutgoingCommands();
    }

    public override void updateAttack() { }
    public override void updateDefend() { }
    public override void updateMed() { }
    public override void endRound() 
    {
        // 恢復 3 點聲譽 (w=0, s=0, r=3)
        manager.UpdatePlayerProperties(0, 0, 3);

        // 播個小提示讓大家知道他展現了彬彬有禮的態度
        manager.photonView.RPC("Announcement", RpcTarget.All, handle.nickname + " 展現了日本人的彬彬有禮，恢復聲譽！", 1500);
    }
}