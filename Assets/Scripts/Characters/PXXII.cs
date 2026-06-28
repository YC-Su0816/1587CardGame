using Photon.Pun;
using System.Text;
using UnityEngine;

public class PXXII : PlayerBase
{
    bool hasRevived;
    public override void Init()
    {
        hasRevived = false;
        attackRatio = new double[] { 1.0, 1.0, 1.0 };
        defendRatio = new double[] { 1.0, 1.0, 1.0 };
        medRatio = new double[] { 1.0, 1.0, 1.0 };
        attackAdd = new int[3]; defendAdd = new int[3]; medAdd = new int[3];
    }

    public override void useSkill()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("內捲之餘，看看課外讀物何嘗不是美事一樁？");
        sb.Append(handle.nickname + " 沉浸於輕小說之中...");
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);

        // 目標選自己，發射不可防禦的補血卡
        handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.me);
        handle.View.RPC("GetCard", RpcTarget.All, "unblockable", "XXIISkill");
        handle.View.RPC("Played", RpcTarget.All, 10, 10, 0); // 恢復10點智力與體力
        PhotonNetwork.SendAllOutgoingCommands();
    }

    public override bool checkRevive(ref int w, ref int s, ref int r)
    {
        if (!hasRevived)
        {
            w = 5; s = 5; r = 5; // 設定說復活給 5 點數值
            hasRevived = true;
            return true; // 閻王拒收，復活成功！
        }
        return false;
    }

    public override void updateAttack() { }
    public override void updateDefend() { }
    public override void updateMed() { }
    public override void newRound() { }
    public override void endRound() { }
}
