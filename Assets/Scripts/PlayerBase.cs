using Photon.Pun;
using UnityEngine;

public abstract class PlayerBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected PlayerHandle handle;
    protected GameSceneManager manager;
    public double[] attackRatio = new double[3];
    public double[] defendRatio = new double[3];
    public double[] medRatio = new double[3];

    public int[] attackAdd = new int[3];
    public int[] defendAdd = new int[3];
    public int[] medAdd = new int[3];
    void Start()
    {
        
    }
    public void getHandleManager(PlayerHandle handle, GameSceneManager m)
    {
        this.handle = handle;
        this.manager = m;
    }
    public abstract void Init();
    public abstract void useSkill();
    public abstract void updateAttack();
    public abstract void updateDefend();
    public abstract void updateMed();
    public abstract void newRound();
    public abstract void endRound();

    // 1. 【最終傷害覆寫】(解決 29號 睿均的固定傷害問題)
    // 傳入結算完的 Damages 陣列，讓子類別可以直接強行修改最終扣除的數值
    public virtual void overrideFinalDamage(ref int w, ref int s, ref int r)
    {
        // 預設什麼都不做，維持原傷害
    }

    // 2. 【被動閃避檢測】(解決 6號 育田、12號 喻翔)
    // 每次遭受攻擊時呼叫，回傳 true 代表成功閃躲，傷害歸零
    public virtual bool checkPassiveDodge()
    {
        return false;
    }

    // 3. 【被動反彈檢測】(解決 2號 騰勳、14號 楷杰)
    // 遭受攻擊時呼叫，回傳要反彈的比例 (0.0 代表不反彈，0.27 代表彈 27%，1.0 代表全彈)
    public virtual float getPassiveReflectRatio()
    {
        return 0f;
    }

    // 4. 【行動失敗與走火入魔檢測】(解決 4號 昱霖、23號 岳霖)
    // 出牌前呼叫，回傳 true 代表行動失敗 (例如被抓包、看幹片)。
    // 可以在覆寫這個函式時，順便在裡面扣除自己的聲譽或體力。
    public virtual bool checkActionFailure(System.Collections.Generic.List<string> attemptTypes)
    {
        return false;
    }

    // 5. 【免死金牌與復活】(解決 5號 睿宸、22號 恩圻、28號 政瑋)
    // 當系統偵測到數值歸零準備呼叫 imDead 時，先呼叫這個。
    // 使用 ref 關鍵字，讓子類別可以直接把 0 拉回 10 或 5。回傳 true 代表閻王拒收，繼續活著。
    public virtual bool checkRevive(ref int currentW, ref int currentS, ref int currentR)
    {
        return false;
    }

    // 6. 【狀態免疫檢測】(解決 1號 雅盈)
    // 被掛上負面效果前呼叫，回傳 true 代表免疫該效果。
    public virtual bool checkImmune(string effectName)
    {
        return false;
    }

    // 7. 【群攻免疫檢測】(解決 28號 政瑋)
    public virtual bool isImmuneToMultiAttack()
    {
        return false;
    }

    // 8. 【每回合抽牌數量】(解決 16號 劭宇)
    // 讓財大氣粗的玩家可以一回合抽兩張。
    public virtual int getDrawCardCount()
    {
        return 1;
    }
    public virtual void onTakeDamage(int w, int s, int r)
    {
        // 預設什麼都不做
    }

    public virtual string[] characterDetailHelper(string c)
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
        inf[2] = "主動 " + inf[5] + "\n" + inf[6];
        return inf;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
