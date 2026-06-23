using Photon.Pun;
using System.Text;
using System;
using UnityEngine;

public class PIV : PlayerBase
{
    PlayerPanelController myPanel;
    double chanceCounter;
    int incrementCounter;
    bool flag;
    bool haveUsedSkill;
    public override void Init()
    {
        incrementCounter = 0;  // 從 0 開始比較直觀
        chanceCounter = 0.0f;
        flag = false;
        haveUsedSkill = false;
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
        sb.AppendLine("(剛從辦公室回來)");
        sb.AppendLine("嗯...一把三人南剛好");
        sb.Append(handle.nickname + " 使用了技能！");
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1000);
        manager.UpdatePlayerProperties(0, -8, -8);
        flag = true;
        haveUsedSkill = true;
        PhotonNetwork.SendAllOutgoingCommands();
    }

    // 負責提供當前的疊加數值
    public override void updateAttack()
    {
        int bonus = incrementCounter; // 注意：因為你 incrementCounter 適用減的 (負數)，所以翻倍會更負
        if (flag)
        {
            bonus *= 2; // 有悔過書時，增傷翻倍
        }

        for (int i = 0; i < 3; ++i)
        {
            attackAdd[i] = bonus; // 把算好的 bonus 真正塞給攻擊加成！
        }
    }

    // 負責擲骰子判定這次出牌是否被抓包
    public override bool checkActionFailure(System.Collections.Generic.List<string> attemptTypes)
    {
        // 只有打出攻擊或群攻時才會觸發抓包判定
        if (attemptTypes.Contains("attack") || attemptTypes.Contains("multiattack"))
        {
            System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
            double rnd = rand.NextDouble();

            if (rnd > chanceCounter || flag)
            {
                // 沒被抓包 (成功出牌)
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("雀魂登入中");
                sb.Append("嘻嘻，沒被抓到");
                handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1000);

                chanceCounter += 0.1f; // 失敗率增加 10%
                incrementCounter -= 1; // 傷害增加 (負數更負)

                flag = false; // 成功出牌後消耗掉悔過書的免死效果
                return false; // 回傳 false 代表「行動沒有失敗」
            }
            else
            {
                // 被抓包了！
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("雀魂登入中");
                sb.Append("昱霖，平板交上來");
                handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1000);

                manager.UpdatePlayerProperties(0, 0, -2); // 扣除信譽

                // 失敗後重置加成與機率
                chanceCounter = 0.0f;
                incrementCounter = 0;

                return true; // 回傳 true！通知 GameSceneManager 這張牌失效，直接結束回合！
            }
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
