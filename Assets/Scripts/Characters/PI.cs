using Photon.Pun;
using System.Text;
using UnityEngine;

public class PI : PlayerBase
{
    public override void Init()
    {
        attackRatio = new double[] { 1.0, 1.0, 1.0 };
        defendRatio = new double[] { 1.0, 1.0, 1.0 };
        medRatio = new double[] { 1.0, 1.0, 1.0 };
        attackAdd = new int[3]; defendAdd = new int[3]; medAdd = new int[3];
    }

    public override void useSkill()
    {
        // 1. 防呆：如果展示區有被按上去的卡牌，先幫忙清空
        if (manager.displaycount > 0)
        {
            for (int i = 0; i < 7; ++i)
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

        // 2. 文字展演
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("(雜訊)!&$*#&$%)#!%...");
        sb.Append(handle.nickname + " 進入了神隱！");
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);

        // 3. 抽三張卡
        for (int i = 0; i < 3; i++)
        {
            double rnd = rand.NextDouble();
            PickACard(rnd);
        }
        manager.RefreshCards();

        // 4. 附加【神隱】效果 (-1 代表常駐)
        handle.View.RPC("PutEffect", RpcTarget.All, manager.me, -1, "disappear");

        // 5. 立即結束當前回合，交棒給下一位
        manager.status = 0;
        manager.player.endRound();
        handle.View.RPC("Go", manager.LocalPlayerList[(manager.me + 1) % manager.total]);

        PhotonNetwork.SendAllOutgoingCommands();
    }

    public override bool checkImmune(string effectName)
    {
        // 溫文儒雅：無法被附加任何效果 (排除自己施放的「神隱」)
        if (effectName != "disappear")
        {
            manager.photonView.RPC("Announcement", RpcTarget.All, handle.nickname + " 溫文儒雅，免疫了狀態！", 1500);
            return true; // 攔截成功
        }
        return false;
    }

    public override void updateAttack() { }
    public override void updateDefend() { }
    public override void updateMed() { }
    public override void newRound() { }
    public override void endRound() { }
}