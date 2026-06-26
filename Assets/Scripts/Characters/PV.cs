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
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);
        if (manager.targetnum == -1)
        {
            manager.targetnum = (manager.me + 1) % manager.total;
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
        handle.View.RPC("GetCard", RpcTarget.All, "unblockable", "king");
        handle.View.RPC("Played", RpcTarget.All, Damages[0], Damages[1], Damages[2]);
        //manager.setPlayerProperties((mine[0] + yours[0] + 1) / 2, (mine[1] + yours[1] + 1) / 2, (mine[2] + yours[2] + 1) / 2);
        flag = true;
        PhotonNetwork.SendAllOutgoingCommands();

        //StringBuilder sb = new StringBuilder();
        //sb.AppendLine("見到神...為何不跪？");
        //sb.Append(handle.nickname + " 使用了技能！");
        //handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);

        //if (manager.targetnum == -1) manager.targetnum = (manager.me + 1) % manager.total;

        //int[] mine = manager.handle.getProperties(); // 取代原本的 CustomProperties 寫法
        //int[] yours = new int[3];
        //yours[0] = (int)manager.LocalPlayerList[manager.targetnum].CustomProperties["Wisdom"];
        //yours[1] = (int)manager.LocalPlayerList[manager.targetnum].CustomProperties["Strength"];
        //yours[2] = (int)manager.LocalPlayerList[manager.targetnum].CustomProperties["Reputation"];

        //int[] Damages = new int[3];
        //for (int i = 0; i < 3; ++i)
        //{
        //    // 算出對手要扣多少血才能達到平均值 (用負數表示傷害)
        //    Damages[i] = -Mathf.Max(0, yours[i] - (mine[i] + yours[i]) / 2);
        //}

        //handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.targetnum);

        //// 【關鍵改動】：使用 unblockable 類型！並移除錯誤的 Responded 呼叫
        //handle.View.RPC("GetCard", RpcTarget.All, "unblockable", "king");
        //handle.View.RPC("Played", RpcTarget.All, Damages[0], Damages[1], Damages[2]);

        //// 攻擊者本地端直接獲得平均後的血量
        //manager.UpdatePlayerProperties(Mathf.Max(0, (mine[0] + yours[0]) / 2 - mine[0]),
        //                               Mathf.Max(0, (mine[1] + yours[1]) / 2 - mine[1]),
        //                               Mathf.Max(0, (mine[2] + yours[2]) / 2 - mine[2]));

        //flag = true;
        //PhotonNetwork.SendAllOutgoingCommands();
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
