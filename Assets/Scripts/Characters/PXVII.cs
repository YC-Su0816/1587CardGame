using UnityEngine;
using Photon.Pun;
using System.Text;
using System.Threading.Tasks;

public class PXVII : PlayerBase
{
    private bool isInitUI = false;
    private bool isSkillActive = false;

    public override async void Init()
    {
        attackRatio = new double[] { 1.0, 1.0, 1.0 };
        defendRatio = new double[] { 1.0, 1.0, 1.0 };
        medRatio = new double[] { 1.0, 1.0, 1.0 };
        attackAdd = new int[3]; defendAdd = new int[3]; medAdd = new int[3];
        await Task.Delay(100);
        if (manager.PlayerPanels[manager.me] != null)
        {
            manager.PlayerPanels[manager.me].GetComponent<PlayerPanelController>().AddEffect("disappear", -1, true);
        }
        _ = BroadcastDisappearAsync();
    }

    private async Task BroadcastDisappearAsync()
    {
        await Task.Delay(1400);
        manager.photonView.RPC("PutEffect", RpcTarget.Others, manager.me, -1, "disappear");
    }

    public override void newRound()
    {
        // 預設起始狀態設為【神隱】
        // if (!isInitUI)
        // {
        //     manager.photonView.RPC("PutEffect", RpcTarget.All, manager.me, -1, "disappear");
        //     isInitUI = true;
        // }
    }

    // 主動技能：吊嘎男
    public override void useSkill()
    {
        isSkillActive = true;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("穿上吊嘎，解放自我！");
        sb.Append(handle.nickname + " 本回合攻擊力增加 100%！");

        // 單純文字展演，不呼叫 GetCard 與 Played，所以這回合還能繼續出牌！
        handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1500);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    // 被動技能：本回合攻擊力增加 30%
    public override void updateAttack()
    {
        if (isSkillActive)
        {
            for (int i = 0; i < 3; i++) attackRatio[i] = 2;
        }
        else
        {
            for (int i = 0; i < 3; i++) attackRatio[i] = 1.0;
        }
    }

    // 回合結束時自動解除吊嘎的增傷 Buff
    public override void endRound()
    {
        isSkillActive = false;
    }

    public override void updateDefend() { }
    public override void updateMed() { }
}