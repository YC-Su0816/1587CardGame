using UnityEngine;
using Photon.Pun;
using System.Text;

public class PXXX : PlayerBase
{

    public override void Init()
    {
        attackRatio = new double[] { 1.4, 1.4, 1.4 };
        defendRatio = new double[] { 1.3, 1.3, 1.3 };
        medRatio = new double[] { 1.0, 1.0, 1.0 };
        attackAdd = new int[3];
        defendAdd = new int[3];
        medAdd = new int[3];
    }

    public override void newRound()
    {
        
    }

    // 主動技能：抱一下嘛
    public override void useSkill()
    {
        
    }

    public override void updateAttack()
    {

    }

    public override void updateDefend()
    {
        
    }

    public override void updateMed() { }
    public override void endRound() { }
}