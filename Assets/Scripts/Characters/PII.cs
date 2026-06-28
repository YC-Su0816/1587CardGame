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
    public override void Init()
    {
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
        if (flag) 
        {
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

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
