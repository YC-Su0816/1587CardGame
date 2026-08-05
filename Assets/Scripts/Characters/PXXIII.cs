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
        manager.status = 0;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("白菜 分享了一則reel");
        sb.AppendLine("你有 99+ 則訊息");
        sb.Append(handle.nickname + " 使用了技能！");
        manager.photonView.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);

        // 使用幹片，提升 10% 失敗率
        fail = (fail <= 0.65f) ? fail + 0.1f : 0.75f;

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

        manager.photonView.RPC("SetFromTo", RpcTarget.All, manager.me, manager.targetnum);
        manager.photonView.RPC("GetCard", RpcTarget.All, "attack", "XXIIISkill");
        manager.photonView.RPC("Played", RpcTarget.All, -20, 0, 0);

        // 因為是 attack (可防禦)，必須將自己狀態鎖死，交給系統等待對手回應
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
        if (rand.NextDouble() < fail)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("我的主題......");
            sb.AppendLine("砰磅！(板溝凹陷)");
            sb.Append(handle.nickname + " 遭遇偶發性事故！");
            manager.photonView.RPC("Announcement", RpcTarget.All, sb.ToString(), 2000);
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
    public override string[] characterDetailHelper(string c)
    {
        string[] inf = new string[3];
        TextAsset det = Resources.Load<TextAsset>("text/CCard/" + c);
        inf = det.text.Split("\n");
        for (int i = 0; i < inf.Length; i++)
        {
            inf[i] = inf[i].Trim();
        }
        string[] p = {"智慧:", inf[0].ToString(), "體力:", inf[1].ToString(), "聲譽:", inf[2].ToString()};
        inf[0] = System.String.Join(" ", p);
        inf[1] = "被動 " + inf[3] + " (當前機率: " + (int)(100f*fail) + "%)\n" + inf[4];
        inf[2] = "主動 " + inf[5] + "\n" + inf[6];
        return inf;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
