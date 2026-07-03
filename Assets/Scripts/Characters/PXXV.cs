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
            handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 2000);

            handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.me);
            handle.View.RPC("GetCard", RpcTarget.All, "medicine", "set");

            // 傳入正數 (5, 5, 5) 代表補血
            handle.View.RPC("Played", RpcTarget.All, 5, 5, 5);
        }
        else
        {
            // ==========================================
            // 【扣球】(Spike) - 造成 10 點智力與 10 點體力傷害
            // ==========================================
            sb.AppendLine("威力十足的扣球！");
            sb.Append(handle.nickname + " 擊中了對手的頭部！");
            handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 2000);

            // 防呆：如果沒有選定目標，預設打下家
            if (manager.targetnum == -1)
            {
                manager.targetnum = (manager.me + 1) % manager.total;
            }

            // 對目標發動攻擊，走 attack 路線 (對方可防禦)
            handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.targetnum);
            handle.View.RPC("GetCard", RpcTarget.All, "attack", "spike");

            // 傳入負數 (-10, -10, 0) 代表傷害
            handle.View.RPC("Played", RpcTarget.All, -10, -10, 0);

            // 【關鍵鎖定】：因為是 attack，必須把自己鎖死，讓系統進入等待對方防禦的狀態
            manager.status = 0;
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