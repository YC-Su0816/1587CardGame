using Photon.Pun;
using System.Text;
using UnityEngine;

public class PV : PlayerBase
{
    PlayerPanelController myPanel;
    double chanceCounter;
    int incrementCounter;
    bool flag;
    bool hasRevived;
    public override void Init()
    {
        hasRevived = false;
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
        sb.AppendLine("見到神...為何不跪？");
        sb.Append(handle.nickname + " 使用了技能！");
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 2000);
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
        int[] mine = new int[3];
        int[] yours = new int[3];
        int[] Damages = new int[3];
        mine[0] = (int)manager.LocalPlayerList[manager.me].CustomProperties["Wisdom"];
        mine[1] = (int)manager.LocalPlayerList[manager.me].CustomProperties["Strength"];
        mine[2] = (int)manager.LocalPlayerList[manager.me].CustomProperties["Reputation"];

        yours[0] = (int)manager.LocalPlayerList[manager.targetnum].CustomProperties["Wisdom"];
        yours[1] = (int)manager.LocalPlayerList[manager.targetnum].CustomProperties["Strength"];
        yours[2] = (int)manager.LocalPlayerList[manager.targetnum].CustomProperties["Reputation"];
        for(int i = 0; i < 3; ++i)
        {
            Damages[i] = (mine[i] - yours[i] + 1) / 2;
        }
        handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.targetnum);
        handle.View.RPC("GetCard", RpcTarget.All, "special", "king");
        handle.View.RPC("Played", RpcTarget.All, Damages[0], Damages[1], Damages[2]);
        manager.setPlayerProperties((mine[0] + yours[0] + 1) / 2, (mine[1] + yours[1] + 1) / 2, (mine[2] + yours[2] + 1) / 2);
        flag = true;
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
    public override bool checkRevive(ref int w, ref int s, ref int r)
    {
        if (!hasRevived)
        {
            w = 10;
            s = 10;
            r = 10;
            hasRevived = true;
            return true; // 攔截成功，復活！
        }
        return false; // 已經復活過了，乖乖死亡
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
