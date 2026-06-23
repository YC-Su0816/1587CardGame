using Photon.Pun;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PXV : PlayerBase
{
    // 【被動技能】：財大氣粗，每回合基礎抽牌數為 2
    public override int getDrawCardCount()
    {
        return 2;
    }

    GameObject skillPrefab;
    Transform uiParent;
    public Photon.Realtime.Player pickedPlayer;

    public override void Init()
    {
        skillPrefab = handle.XVskillPrefab;
        pickedPlayer = null;
        uiParent = manager.UIparent;

        attackRatio = new double[] { 1.0, 1.0, 1.0 };
        defendRatio = new double[] { 1.0, 1.0, 1.0 };
        medRatio = new double[] { 1.0, 1.0, 1.0 };
        attackAdd = new int[3]; defendAdd = new int[3]; medAdd = new int[3];
    }

    public override void useSkill()
    {
        // 1. 防呆：如果是群攻，不能使用此技能
        if (manager.multi)
        {
            manager.hintword.text = "無法轉移群體攻擊！";
            return;
        }

        GameObject controlBoard = GameObject.Instantiate(skillPrefab, uiParent);
        PXVSkill mono = controlBoard.GetComponent<PXVSkill>();
        List<string> nameList = new List<string>();

        for (int i = 0; i < manager.total; ++i)
        {
            // 2. 防錯與防呆：確保玩家存在，且把「自己」從替死鬼名單中排除
            if (manager.isPickable[i] && manager.LocalPlayerList[i] != null && manager.LocalPlayerList[i] != PhotonNetwork.LocalPlayer)
            {
                nameList.Add(manager.LocalPlayerList[i].NickName);
            }
        }

        if (mono != null)
        {
            mono.init(this, nameList);
        }
    }

    public void confirmChoice(string pChoice)
    {
        foreach (Photon.Realtime.Player player in manager.LocalPlayerList)
        {
            // 3. 防錯：確保 player 不是 null
            if (player != null && player.NickName == pChoice)
            {
                pickedPlayer = player;
                break;
            }
        }

        if (manager.status != 2) return;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("這點事交給其他人辦就好了。");
        sb.Append(handle.nickname + " 發動了社交蝴蝶，轉移了目標！");
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 2000);

        // --- 核心轉移邏輯 ---

        // 自己脫離回應階段，安全下莊
        manager.status = 0;

        // 尋找原攻擊者
        int attackerIndex = 0;
        for (int i = 0; i < manager.total; i++)
        {
            if (manager.LocalPlayerList[i] == manager.FromAndTo[0])
            {
                attackerIndex = i;
                break;
            }
        }

        // 尋找替死鬼
        int newTargetIndex = 0;
        for (int i = 0; i < manager.total; i++)
        {
            if (manager.LocalPlayerList[i] == pickedPlayer)
            {
                newTargetIndex = i;
                break;
            }
        }

        // 4. 修復 UI 重疊 Bug：清除畫面上舊的攻擊卡牌，避免稍後廣播 Played 時重複生成
        for (int i = 0; i < manager.DisplayInRally.Count; i++)
        {
            if (manager.DisplayInRally[i] != null)
            {
                var controller = manager.DisplayInRally[i].GetComponent<ToolDisplayController>();
                if (controller != null) controller.kill();
                else GameObject.Destroy(manager.DisplayInRally[i]);
            }
        }
        manager.DisplayInRally.Clear();

        // 廣播更新目標 (原攻擊者 -> 替死鬼)
        manager.photonView.RPC("SetFromTo", RpcTarget.All, attackerIndex, newTargetIndex);

        // 再次發射 Played，讓系統完美生成卡牌並把狀態移交給新目標
        manager.photonView.RPC("Played", RpcTarget.All, manager.daW, manager.daS, manager.daR);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    public override void updateAttack() { }
    public override void updateDefend() { }
    public override void updateMed() { }
    public override void newRound() { }
    public override void endRound() { }
}