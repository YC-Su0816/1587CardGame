using Photon.Pun;
using System.Text;
using TMPro;
using UnityEngine;

public class PII : PlayerBase 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int recordedStrength;
    public int recordedWisdom;
    public int recordedReputation;
    public bool flag;
    public bool flag2;
    public override void Init()
    {
        flag = false;
        flag2 = false;
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
        if (flag) 
        {
            flag2 = true;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("！擦喀");
            sb.Append(handle.nickname + " 使用了技能！");
            manager.setPlayerProperties(recordedWisdom, recordedStrength, recordedReputation);
            handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1000);
            PhotonNetwork.SendAllOutgoingCommands();
        }
        else
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("喀擦！");
            sb.Append(handle.nickname + " 使用了技能！");
            int[] properties = handle.getProperties();
            recordedWisdom = properties[0];
            recordedStrength = properties[1];
            recordedReputation = properties[2];
            handle.View.RPC("Announcement", RpcTarget.All, sb.ToString(), 1000);
            PhotonNetwork.SendAllOutgoingCommands();
            flag = true;
        }
            
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

    }public override string[] characterDetailHelper(string c)
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
        inf[1] = "被動 " + inf[3] + "\n" + inf[4];
        inf[2] = "主動 " + inf[5] + "\n" + inf[6] + "\n";
        if (flag2)
        {
            inf[2] += "(已使用技能)";
        }
        else if (flag)
        {
            inf[2] += "紀錄值: 智"+ recordedWisdom +" 體" + recordedStrength + " 譽" + recordedReputation;
        }
        else
        {
            inf[2] += "(尚未紀錄狀態)";
        }
        return inf;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
