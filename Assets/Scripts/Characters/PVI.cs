using Photon.Pun;
using System.Text;
using UnityEngine;

public class PVI : PlayerBase
{
    PlayerPanelController myPanel;
    double chanceCounter;
    int incrementCounter;
    bool flag;
    public override void Init()
    {
        incrementCounter = 1;
        chanceCounter = -1e-6;
        flag = false;
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
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("zzz...");
        sb.Append(handle.nickname + " 使用了技能！");
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 2000);
        handle.View.RPC("PutEffect", RpcTarget.All, manager.me, 1, "sleep");
        PhotonNetwork.SendAllOutgoingCommands();
        manager.status = 0;
        handle.View.RPC("Go", manager.LocalPlayerList[(manager.me + 1) % manager.total]);
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
