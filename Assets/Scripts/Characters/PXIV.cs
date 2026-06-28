using UnityEngine;
using Photon.Pun;
using System.Text;

public class PXIV : PlayerBase
{
    public override void Init()
    {
        attackRatio = new double[] { 1.0, 1.0, 1.0 };
        defendRatio = new double[] { 1.0, 1.0, 1.0 };
        medRatio = new double[] { 1.0, 1.0, 1.0 };
        attackAdd = new int[3]; defendAdd = new int[3]; medAdd = new int[3];
    }

    public override void newRound() { }

    // 主動技能：抽水機
    public override void useSkill()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("啟動抽水機！");
        sb.Append(handle.nickname + " 使用了技能！");
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);

        // 將目標設為自己，打出無法防禦的抽水機卡牌
        handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.me);
        handle.View.RPC("GetCard", RpcTarget.All, "medicine", "watermachine");

        // 挽回 15 點聲譽 (w=0, s=0, r=15)
        handle.View.RPC("Played", RpcTarget.All, 0, 0, 15);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    // 被動技能：作者看到的限動
    public override void overrideFinalDamage(ref int w, ref int s, ref int r)
    {
        // 確保這是一次有對象的單體攻擊
        if (manager.targetnum >= 0 && manager.targetnum < manager.total)
        {
            var targetProps = manager.LocalPlayerList[manager.targetnum].CustomProperties;
            var targetPanel = manager.PlayerPanels[manager.targetnum].GetComponent<PlayerPanelController>();

            int tw = (int)targetProps["Wisdom"];
            int ts = (int)targetProps["Strength"];
            int tr = (int)targetProps["Reputation"];
            StringBuilder sb = new StringBuilder();
            if (tw == targetPanel.maxWisdom && w < 0)
            {
                sb.AppendLine("造成三倍智慧傷害");
                w *= 3;
            }
            if (ts == targetPanel.maxStrength && s < 0)
            {
                sb.AppendLine("造成三倍體力傷害");
                s *= 3;
            }
            if (tr == targetPanel.maxReputation && r < 0)
            {
                sb.AppendLine("造成三倍聲譽傷害");
                r *= 3;
            }
            if (sb.ToString() != "")
            {
                manager.photonView.RPC("Announcement", RpcTarget.All, handle.nickname + " 被動生效\n" + sb.ToString(), 2000);
            }
            
            // 判斷目標是否為「滿數值」
            //if (tw == targetPanel.maxWisdom && ts == targetPanel.maxStrength && tr == targetPanel.maxReputation)
            //{
            //    manager.photonView.RPC("Announcement", RpcTarget.All, handle.nickname + " 的限動爆擊！傷害翻倍！", 1500);

            //    // 針對原本有扣血的屬性，造成雙倍傷害
            //    if (w < 0) w *= 3;
            //    if (s < 0) s *= 3;
            //    if (r < 0) r *= 3;
            //}
        }
    }

    public override void updateAttack() { }
    public override void updateDefend() { }
    public override void updateMed() { }
    public override void endRound() { }
}