using Photon.Pun;
using System.Text;
using UnityEngine;

public class PX : PlayerBase
{
    public override void Init()
    {

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
        defendRatio[2] = 0.6f;
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
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("開始實施CPR");
        sb.Append(handle.nickname + " 使用了技能！");
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);
        handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.me);
        handle.View.RPC("GetCard", RpcTarget.All, "medicine", "redcorss");
        handle.View.RPC("Played", RpcTarget.All, 10, 10, 10);
        PhotonNetwork.SendAllOutgoingCommands();
    }
    public override void updateAttack()
    {

    }
    public override void updateDefend()
    {

    }
    public override void updateMed()
    {

    }
    public override void newRound()
    {

    }
    public override void endRound()
    {

    }
}
