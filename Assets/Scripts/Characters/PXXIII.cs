using Photon.Pun;
using System.Text;
using System;
using UnityEngine;

public class PXXIII : PlayerBase
{
    PlayerPanelController myPanel;
    double fail;
    public override void Init()
    {
        fail = 0.05f;
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
        sb.AppendLine("白菜 分享了一則reel");
        sb.AppendLine("你有 99+ 則訊息");
        sb.Append(handle.nickname + " 使用了技能！");
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1000);

        // 使用幹片，提升 10% 失敗率
        fail = (fail <= 0.65f) ? fail + 0.1f : 0.75f;

        // 【新增】：原本漏掉的攻擊邏輯
        if (manager.targetnum == -1)
        {
            manager.targetnum = (manager.me + 1) % manager.total;
        }

        handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.targetnum);

        // 【修正】使用指定的技能卡片圖示
        handle.View.RPC("GetCard", RpcTarget.All, "attack", "XXIIISkill");
        handle.View.RPC("Played", RpcTarget.All, -20, 0, 0); // 對目標造成 20 點智力傷害

        // 因為是 attack (可防禦)，必須將自己狀態鎖死，交給系統等待對手回應
        manager.status = 0;
        PhotonNetwork.SendAllOutgoingCommands();
    }

    // 負責提供當前的疊加數值
    public override void updateAttack()
    {
        
    }

    // 負責擲骰子判定這次出牌是否被抓包
    public override bool checkActionFailure(System.Collections.Generic.List<string> attemptTypes)
    {
        // 只有打出攻擊或群攻時才會觸發抓包判定
        System.Random rand = new System.Random(System.Guid.NewGuid().GetHashCode());
        if (rand.Next(100) < fail)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("我的主題......");
            sb.AppendLine("砰磅！(板溝凹陷)");
            sb.Append(handle.nickname + " 遭遇偶發性事故！");
            handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 2000);
            return true; // 行動直接作廢
        }
        return false;
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
