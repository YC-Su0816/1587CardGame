using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using System;
using UnityEngine.UIElements;



public class PlayerPanelController : MonoBehaviourPunCallbacks
{
    public class Eff
    {
        public GameObject obj;       // 畫面上的小圖示物件
        public string id;            // 效果代號 (ex: "disappear", "poison")
        public string displayName;   // 顯示的中文名稱 (ex: "隱身", "中毒")
        public string description;   // 點擊顯示的詳細資訊
        public int lastRound;        // 剩餘回合數 (-1 代表常駐)
        public bool isPermanent;     // 是否為常駐效果
        public bool canCleanByMed;   // 能否被特定卡牌(medicine)消除
    }

    // 效果靜態資料庫 (查表法)：把所有效果的屬性寫在這裡，方便未來隨時新增
    private readonly Dictionary<string, (string name, string desc, bool isPerm, bool canClean)> effectDatabase
        = new Dictionary<string, (string, string, bool, bool)>()
    {
        // 格式: { "效果代號", ("顯示名稱", "詳細敘述文字", 是否常駐, 能否被藥物消除) }
        { "disappear", ("神隱", "大家肯定是再熟悉不過了...\n不可選中，每回合回復所有數值三點。\n回合開始50%維持、50%解除本效果", true, false) }, // 常駐，不可除
        { "sleep", ("沉睡", "不可選中，每回合扣除智慧、聲譽各一點。\n指定回合後解除", false, true) },  // 非常駐，可被藥消除
        { "dizzy", ("失神", "每回合隨機失去一張卡片。\n指定回合後解除", false, true) },
        { "malice", ("怨念", "時刻給予關注，否則...後果自負。\nCD歸0時引爆，扣除所有數值各15點。\n單體攻擊自己並造成傷害可重置回合", false, true) },
        { "21water", ("柔水", "降攻、增防。", true, false) },
        { "21knife", ("鋼刀", "增攻、破防。", true, false) },
        { "chenchienting", ("陳建廷", "建章常繞勒沙理，廷殿時聞酸鹼音。", true, false) },
        { "wuminglin", ("吳明麟", "明師化雨澤天地，麟閣流芳縱往今。", true, false) },
        { "linminching", ("林敏靜", "敏心能辨他邦字，靜格易通百國言。", true, false) },
        { "chenpenghsu", ("陳鵬旭", "鵬摶千里展新翅，旭照萬疆破險題。", true, false) },
        { "chenchihsheng", ("陳智勝", "智術略窺量子貌，勝才能頌電磁詩。", true, false) },
        { "loyinting", ("羅尹廷", "尹昭萬物存佳譽，廷滿百昌博藇名。", true, false) },
        { "wangchinghua", ("王靖華", "靖志自懷千古事，華才能授四時情。", true, false) }
    };

    public List<Eff> effectlist;
    public GameObject ef;
    public string nick, character;
    public int Wisdom, Strength, Reputation;
    public int maxWisdom, maxStrength, maxReputation, y, NumInList;
    
    public GameObject manager;
    public int effEdgex, effEdgey;
    public bool pickable;
    Transform bar;
    TMP_Text plname, wis, stren, rep;
    UnityEngine.UI.Image chara;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Application.targetFrameRate = 60;
        effectlist = new List<Eff>();
        effEdgex = 520; effEdgey = -20;
        manager = GameObject.Find("GameSceneManager");
        bar = this.transform;
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
        UnityEngine.UI.Image[] images = GetComponentsInChildren<UnityEngine.UI.Image>();
        foreach( TMP_Text text in texts)
        {
            if (text.name == "Name") plname = text;
            else if (text.name == "Wisdom") wis = text;
            else if (text.name == "Strength") stren = text;
            else rep = text;
        }
        foreach(UnityEngine.UI.Image image in images)
        {
            if (image.name == "Image")
            {
                chara = image;
            }
        }
        
        ef = manager.GetComponent<GameSceneManager>().effectprfeb;
        Debug.Log(chara.name);
        Debug.Log(character);
        plname.text = nick;
        chara.sprite = Resources.Load<Sprite>("image/CCard/" + character);
        chara.GetComponent<CharacterDescController>().init(character);
        Wisdom = maxWisdom;
        Strength = maxStrength;
        Reputation = maxReputation;
        UpdateProperties();
    }
    void UpdateProperties()
    {
        wis.text = "智慧：" + Wisdom.ToString() + "/" + maxWisdom.ToString();
        stren.text = "體力：" + Strength.ToString() + "/" + maxStrength.ToString();
        rep.text = "聲譽：" + Reputation.ToString() + "/" + maxReputation.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public bool isExist(string effName)
    {
        if(effectlist != null)
        {
            for (int i = 0; i < effectlist.Count; ++i)
            {
                if (effectlist[i].id != effName) continue;
                return true;
            }
        }
        
        return false;
    }
    // 修改後的 AddEffect：現在只要傳效果代號與持續回合即可
    public void AddEffect(string effId, int round, bool can) // 保留原本 PUN 呼叫的參數介面
    {
        // 如果資料庫裡沒有定義這個效果，給個預設值防止崩潰
        string dName = effId;
        string dDesc = "未知效果";
        bool isPerm = (round == -1); // 如果傳入 -1 則自動視為常駐
        bool canClean = can;

        // 從資料庫撈取詳細設定
        if (effectDatabase.TryGetValue(effId, out var data))
        {
            dName = data.name;
            dDesc = data.desc;
            isPerm = data.isPerm;
            canClean = data.canClean;
        }

        // 生成 UI 小圖示
        GameObject effectObj = Instantiate(ef, gameObject.GetComponent<Transform>());
        // 填寫資料結構
        Eff t = new Eff
        {
            obj = effectObj,
            id = effId,
            displayName = dName,
            description = dDesc,
            lastRound = isPerm ? -1 : round,
            isPermanent = isPerm,
            canCleanByMed = canClean
        };
        effectlist.Add(t);

        // --- 解決需求 3：將資料塞給小圖示的 Controller ---
        EffectController controller = effectObj.GetComponent<EffectController>();
        controller.init(dName, (t.isPermanent ? "常駐" : t.lastRound.ToString()), dDesc, effId);

        // 設定小圖示圖片
        effectObj.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("image/Tool/effect/" + effId);

        updateEffectPosition();
    }

    // 修改後的 UpdateEffect：每回合呼叫時，常駐效果不扣回合
    public void UpdateEffect()
    {
        if (effectlist == null) return;
        int i = 0;

        while (i < effectlist.Count)
        {
            if (effectlist[i].isPermanent)
            {
                // 常駐效果：不扣減回合，直接看下一個
                i++;
                continue;
            }

            // 非常駐效果：回合數 - 1
            effectlist[i].lastRound--;

            if (effectlist[i].lastRound > 0)
            {
                // 更新小圖示上顯示的數字
                effectlist[i].obj.GetComponent<EffectController>().setRound(effectlist[i].lastRound.ToString());
                i++;
            }
            else
            {
                // 回合結束，銷毀效果
                Destroy(effectlist[i].obj);
                effectlist.RemoveAt(i);
                updateEffectPosition();
            }
        }
    }

    // 【新功能】：當玩家使用 Medicine 卡片時，呼叫此函式清除可被消除的效果
    public void RemoveMedicineEffects()
    {
        int i = 0;
        while (i < effectlist.Count)
        {
            if (effectlist[i].canCleanByMed)
            {
                Destroy(effectlist[i].obj);
                effectlist.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
        updateEffectPosition();
    }
    public void RemoveEffect(string effId)
    {
        // 從陣列尾巴往前遍歷，這樣刪除元素時才不會影響還沒檢查到的 Index
        for (int i = effectlist.Count - 1; i >= 0; i--)
        {
            if (effectlist[i].id == effId)
            {
                // 1. 摧毀畫面上的 UI 小圖示
                Destroy(effectlist[i].obj);

                // 2. 從資料陣列中移除
                effectlist.RemoveAt(i);
            }
        }

        // 3. 刪除完畢後，呼叫你原本寫好的排列函式，讓剩下的圖示往前靠攏
        updateEffectPosition();
    }
    public void updateEffectPosition()
    {
        int counts = effectlist.Count;
        if(counts > 0)
        {
            for(int i = 0; i < counts; ++i)
            {
                effectlist[i].obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(effEdgex + 50 * i, effEdgey, 0);
            }
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer.NickName == nick)
        {
            if (changedProps.ContainsKey("Wisdom"))
            {
                Wisdom = (int)targetPlayer.CustomProperties["Wisdom"];
            }
            if (changedProps.ContainsKey("Strength"))
            {
                Strength = (int)targetPlayer.CustomProperties["Strength"];
            }
            if (changedProps.ContainsKey("Reputation"))
            {
                Reputation = (int)targetPlayer.CustomProperties["Reputation"];
            }
            
         }
        UpdateProperties();
    }
}
