using UnityEngine;
using Photon.Pun;
using System.Text;

public class PXXI : PlayerBase
{
    private bool isWater = false;  // 預設為 false，這樣第一回合 newRound 反轉後就會變成剛好是柔水
    private bool isInitUI = false;
    private int cooldown = 0;      // 主動技能冷卻計時器

    public override void Init()
    {
        cooldown = 0;
        attackRatio = new double[] { 1.0, 1.0, 1.0 };
        defendRatio = new double[] { 1.0, 1.0, 1.0 };
        medRatio = new double[] { 1.0, 1.0, 1.0 };
        attackAdd = new int[3];
        defendAdd = new int[3];
        medAdd = new int[3];
        
    }

    public override void newRound()
    {
        // 1. 扣除冷卻時間

        // 2. 切換型態
        isWater = !isWater;

        string effectToAdd = isWater ? "21water" : "21knife";
        string effectToRemove = isWater ? "21knife" : "21water";
        string formName = isWater ? "【柔水】" : "【鋼刀】";

        // 3. 更新 UI 狀態
        if (isInitUI)
        {
            manager.photonView.RPC("RemoveEffect", RpcTarget.All, manager.me, effectToRemove);
        }
        manager.photonView.RPC("PutEffect", RpcTarget.All, manager.me, 99, effectToAdd);
        isInitUI = true;

        // 4. 廣播切換通知
        manager.photonView.RPC("Announcement", RpcTarget.All, handle.nickname + " 切換為" + formName + "狀態！", 1500);
    }

    // 主動技能：抱一下嘛
    public override void useSkill()
    {
        // 消耗自身 5 點信譽
        manager.UpdatePlayerProperties(0, 0, -5);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("「宜蘭人天生帶山帶水，性格裡難免多一份巍峨的柔情」");
        sb.Append(handle.nickname + " 給了你一個擁抱！");
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 2500);

        // 如果沒有選定目標，預設抱下家
        if (manager.targetnum == -1)
        {
            manager.targetnum = (manager.me + 1) % manager.total;
        }

        // 廣播目標並發射攻擊 (造成智力 10、體力 10 的傷害)
        handle.View.RPC("SetFromTo", RpcTarget.All, manager.me, manager.targetnum);
        handle.View.RPC("GetCard", RpcTarget.All, "attack", "XXISkill"); // 這裡的 "hug" 可以換成你對應的擁抱卡面圖示
        handle.View.RPC("Played", RpcTarget.All, -10, -10, 0);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    public override void updateAttack()
    {
        if (isWater)
        {
            // 柔水：傷害值減少 30%
            attackRatio[0] = 0.7; attackRatio[1] = 0.7; attackRatio[2] = 0.7;
        }
        else
        {
            // 鋼刀：傷害值增加 40%
            attackRatio[0] = 1.4; attackRatio[1] = 1.4; attackRatio[2] = 1.4;
        }
    }

    public override void updateDefend()
    {
        if (isWater)
        {
            // 柔水：防禦值增加 40%
            defendRatio[0] = 1.4; defendRatio[1] = 1.4; defendRatio[2] = 1.4;
        }
        else
        {
            // 鋼刀：防禦值減少 30%
            defendRatio[0] = 0.7; defendRatio[1] = 0.7; defendRatio[2] = 0.7;
        }
    }

    public override void updateMed() { }
    public override void endRound() { }
}