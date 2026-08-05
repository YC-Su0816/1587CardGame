using ExitGames.Client.Photon;
using NUnit.Framework;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
//using Unity.Android.Gradle;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;
using static UnityEngine.Rendering.DebugUI.Table;
using HashTable = ExitGames.Client.Photon.Hashtable;


public class GameSceneManager : MonoBehaviourPunCallbacks
{
    public List<GameObject>[] CardsInType;
    public List<GameObject>[] CardsInDisplay;
    public List<GameObject> DisplayInRally;
    public List<string> DisplayType = new List<string>();
    public List<string> DisplayFace = new List<string>();
    public bool pickable;
    public bool displaytime = false, playing, responding; 
    public int cardpicking, targetnum, displaycount;
    public int status; //0: 沒事 1: 出牌 2: 回牌

    public bool PD; //false: 棄牌 true: 出牌
    public string toolcardtype;
    public Vector3[] position = new Vector3[20];
    public TMP_Text hintword, to, from, hint, PDtext, cdText;
    public PlayerHandle player;
    public GameObject playerprfeb;
    public GameObject effectprfeb;
    public GameObject toolcardprfeb;
    public GameObject carddisplayprfeb;
    public GameObject[] PlayerPanels;
    public GameObject HintPanel;
    public Transform UIparent;
    public Transform cardpanel;
    public Transform displayfrom;
    public Transform displayto;
    public Button skillButton, PDSwitch;
    public Photon.Realtime.Player[] LocalPlayerList;
    public Photon.Realtime.Player[] FromAndTo = new Photon.Realtime.Player[2];
    public bool[] isAlive;
    public bool[] isPickable;
    public int me;
    public int cd, cdLength;
    public string character;
    public string reflectMemoType;
    public int reflectMemoNext;
    public Photon.Realtime.Player reflectMemoPlayer;

    System.Random rand;
    int typeNum = 6;
    int count = 0;
    int skillUseCounter;
    int temp;
    public int total;
    float SW, SH, cs;
    public int daW, daS, daR;
    public int maxW, maxS, maxR;
    public bool Wdead, Sdead, Rdead, multi, reflect, over, canPlayDefense, canPlaySpecial;
    int multicount;
    public bool isResolvingVirtualCard = false;

    double[] cardRatio = {3.0, 1.0, 2.0, 2.0, 1.0, 1.0};
    
    double[] cardCumulativeProbability;
    // Data structure for "Special"
    public class SpecialCardData
    {
        public string face;
        public bool canPlayOnTurn;
        public bool canPlayOnAttacked;
        public bool canRespond;
    }


    public Dictionary<string, List<string>> cardPools = new Dictionary<string, List<string>>();
    public Dictionary<string, SpecialCardData> specialCardDict = new Dictionary<string, SpecialCardData>();

    [PunRPC]
    public void RPC_LiteratureReview_Ask(int targetIdx, int attackerIdx)
    {
        if (PhotonNetwork.LocalPlayer == LocalPlayerList[targetIdx])
        {
            int defCount = CardsInType[2].Count; // defense
            photonView.RPC("RPC_LiteratureReview_PrivateAnswer", LocalPlayerList[attackerIdx], targetIdx, defCount);
            photonView.RPC("RPC_LiteratureReview_PublicAnswer", RpcTarget.All, targetIdx, attackerIdx);
        }
    }
    [PunRPC]
    public void RPC_LiteratureReview_PrivateAnswer(int targetIdx, int defCount)
    {
        string tName = LocalPlayerList[targetIdx].NickName;
        StringBuilder sb = new StringBuilder();
        
        sb.AppendLine("Literature review...");
        sb.AppendLine("林敏靜 告訴你：");
        sb.Append(tName + " 手中有 " + defCount + " 張防禦卡");
        
        EnqueueLocalAnnouncement(sb.ToString(), 3000);
    }
    [PunRPC]
    public void RPC_LiteratureReview_PublicAnswer(int targetIdx, int attackerIdx)
    {
        if (PhotonNetwork.LocalPlayer == LocalPlayerList[attackerIdx]) return;

        string tName = LocalPlayerList[targetIdx].NickName;
        string aName = LocalPlayerList[attackerIdx].NickName;
        StringBuilder sb = new StringBuilder();
        
        sb.AppendLine("Literature review...");
        sb.AppendLine("在 林敏靜 的指導下");
        sb.Append(aName + " 看穿了 " + tName + " 的防禦底細");
        
        EnqueueLocalAnnouncement(sb.ToString(), 3000);
    }
    [PunRPC]
    public void RPC_ChenChihSheng_NonZero()
    {
        if (!isAlive[me]) return;

        int currentW = (int)PhotonNetwork.LocalPlayer.CustomProperties["Wisdom"];
        int currentS = (int)PhotonNetwork.LocalPlayer.CustomProperties["Strength"];
        int currentR = (int)PhotonNetwork.LocalPlayer.CustomProperties["Reputation"];

        int addW = 0, addS = 0, addR = 0;

        if (currentW == 0) { addW = 1; Wdead = false; }
        if (currentS == 0) { addS = 1; Sdead = false; }
        if (currentR == 0) { addR = 1; Rdead = false; }

        if (addW > 0 || addS > 0 || addR > 0)
        {
            UpdatePlayerProperties(addW, addS, addR);
        }
    }
    [PunRPC]
    void EndGame(string winner = "")
    {
        StaticData.winnerName = winner;
        SceneManager.LoadScene("EndGameScene");
    }
    [PunRPC]
    void PlayerList(Photon.Realtime.Player k)
    {
        GameObject playerpanel = Instantiate(playerprfeb, UIparent);
        playerpanel.GetComponent<PlayerPanelController>().NumInList = count;
        playerpanel.GetComponent<PlayerPanelController>().nick = k.NickName;
        playerpanel.GetComponent<PlayerPanelController>().character = (string)k.CustomProperties["Picking"];
        playerpanel.GetComponent<PlayerPanelController>().maxWisdom = (int)k.CustomProperties["Wisdom"];
        playerpanel.GetComponent<PlayerPanelController>().maxStrength = (int)k.CustomProperties["Strength"];
        playerpanel.GetComponent<PlayerPanelController>().maxReputation = (int)k.CustomProperties["Reputation"];
        isAlive[count] = true;
        isPickable[count] = true;
        if (k == PhotonNetwork.LocalPlayer)
        {
            me = count;
            character = (string)k.CustomProperties["Picking"];
            player.Init(character);
            player.nickname = k.NickName;
            TextAsset det = Resources.Load<TextAsset>("text/CCard/" + character);
            string[] inf = det.text.Split("\n");
            for (int i = 0; i < inf.Length; i++)
            {
                inf[i] = inf[i].Trim();
            }
            if(character == "2" || character == "30")
            {
                cdLength = 9999;
            }
            else
            {
                int.TryParse(inf[7], out cdLength);
            }
            Debug.Log(character == "2");
            if (character == "2")
            {
                cd = 2;
            }
            else
            {
                cd = 1;
            }
            Debug.Log(cd);
            maxW = (int)k.CustomProperties["Wisdom"];
            maxS = (int)k.CustomProperties["Strength"];
            maxR = (int)k.CustomProperties["Reputation"];
        }

        playerpanel.transform.position = new Vector3(SW-30, SH - 30 - count*100, 0);
        PlayerPanels[count] = playerpanel;
        LocalPlayerList[count] = k;
        count++;
        
            //playerpanel.GetComponent<>

    }
    [PunRPC]
    async Task StartReflection(int reflectorIndex, int targetIndex, int w, int s, int r, bool isMultiAttack, bool allowDefense, bool allowSpecial)
    {
        if (!reflect)
        {
            reflectMemoPlayer = LocalPlayerList[targetIndex];
            if (isMultiAttack)
            {
                reflectMemoNext = (reflectorIndex + 1) % total;
            }
        }

        reflect = true; // 設為反彈狀態
        int k = DisplayType.Count;
        float x = displayfrom.GetComponent<RectTransform>().sizeDelta.x / 2;
        float y = displayfrom.GetComponent<RectTransform>().sizeDelta.y;
        float inter = y / (k + 1);
        temp = k;
        displaytime = true;
        for (int i = 0; i < k; i++)
        {
            GameObject detail = Instantiate(carddisplayprfeb, displayfrom);
            DisplayInRally.Add(detail);
            ToolDisplayController tdc = detail.GetComponent<ToolDisplayController>();
            detail.transform.localPosition = new Vector3(x, y - inter * (i + 1), 1);

            tdc.tooltype = DisplayType[i];
            tdc.face = DisplayFace[i];
            tdc.forDisplay = true;
            tdc.init(w, s, r);
        }
        // 攻守方逆轉
        FromAndTo[0] = LocalPlayerList[reflectorIndex];
        FromAndTo[1] = LocalPlayerList[targetIndex];
        from.text = FromAndTo[0].NickName;
        to.text = FromAndTo[1].NickName;
        canPlayDefense = allowDefense;
        canPlaySpecial = allowSpecial;
        if (PhotonNetwork.LocalPlayer == LocalPlayerList[targetIndex])
        {
            status = 2; // 被反彈的原攻擊者本地端進入回應階段
            daW = w;
            daS = s;
            daR = r;
        }

        await Task.Delay(1500);
    }
    [PunRPC]
    void imDead(int n)
    {
        isAlive[n] = false;
        isPickable[n] = false;
    }
    [PunRPC]
    void PutEffect(int n, int last, string f)
    {
        PlayerPanels[n].GetComponent<PlayerPanelController>().AddEffect(f, last, true);
    }
    [PunRPC]
    void UpdateEffect(int n)
    {
        PlayerPanels[n].GetComponent<PlayerPanelController>().UpdateEffect();
    }
    [PunRPC]
    void RemoveEffect(int playerIndex, string effectName)
    {
        PlayerPanels[playerIndex].GetComponent<PlayerPanelController>().RemoveEffect(effectName);
    }
    [PunRPC]
    void SetFromTo(int F, int T)
    {
        if(F != -1)
        {
            FromAndTo[0] = LocalPlayerList[F];
            from.text = FromAndTo[0].NickName;
        }
        if (T != -1)
        {
            FromAndTo[1] = LocalPlayerList[T];
            to.text = FromAndTo[1].NickName;
        }
        else
        {
            to.text = "";
        }
    }
    [PunRPC]
    void Cleaning()
    {
        for (int j = 0; j < DisplayInRally.Count; j++)
        {
            DisplayInRally[j].GetComponent<ToolDisplayController>().kill();
        }
        DisplayInRally.Clear();
        DisplayType.Clear();
        DisplayFace.Clear();
        displaytime = false;
    }
    [PunRPC]
    async Task Go()
    {
        if (over || !pickable) return;

        // 【新增攔截邏輯】：檢查自己當前有沒有被限制行動的效果
        PlayerPanelController myPanel = PlayerPanels[me].GetComponent<PlayerPanelController>();
        if (myPanel.isExist("disappear"))
        {
            System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());

            if (rand.NextDouble() <= 0.5f)
            {
                photonView.RPC("Announcement", RpcTarget.All, " 噢！"+ PhotonNetwork.LocalPlayer.NickName + "回來了！", 2000);
                photonView.RPC("RemoveEffect", RpcTarget.All, me, "disappear");
                await Task.Delay(2000);
            }
            else
            {
                photonView.RPC("Announcement", RpcTarget.All, "不行，聯絡不上"+PhotonNetwork.LocalPlayer.NickName + "...", 2000);
                UpdatePlayerProperties(3, 3, 3);
                status = 0;
                player.endRound();
                await Task.Delay(2000);
                isGameEnded((me + 1) % total);
                if (PlayerPanels != null) photonView.RPC("UpdateEffect", RpcTarget.All, me);
                return;
            }
        }
        if (myPanel.isExist("sleep")) 
        {
            photonView.RPC("Announcement", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " 睡死了...", 2000);
            UpdatePlayerProperties(-1, 0, -1);
            status = 0;
            player.endRound();
            await Task.Delay(2000);
            isGameEnded((me + 1) % total);
            if (PlayerPanels != null) photonView.RPC("UpdateEffect", RpcTarget.All, me);
            return;
        }
        
        cd = Mathf.Max(0, cd - 1);
        displaytime = false;
        photonView.RPC("SetFromTo", RpcTarget.All, me, -1);
        PhotonNetwork.SendAllOutgoingCommands();
        status = 1;
        PD = true;
        targetnum = -1;
        player.newRound();
        Debug.Log("It's " + PhotonNetwork.LocalPlayer.NickName);
    }
    [PunRPC]
    void GetCard(string tooltype, string face)
    {
        DisplayType.Add(tooltype);
        DisplayFace.Add(face);
    }
    [PunRPC]
    void CallResponse(bool allowDefense, bool allowSpecial)
    {
        status = 2;
        canPlayDefense = allowDefense;
        canPlaySpecial = allowSpecial;
    }
    [PunRPC]
    async Task Played(int wisdomDamage, int strengthDamage, int reputationDamage)//need modify
    {
        if(!reflect)
            multi = false;
        int k = DisplayType.Count;
        float x = displayfrom.GetComponent<RectTransform>().sizeDelta.x / 2;
        float y = displayfrom.GetComponent<RectTransform>().sizeDelta.y;
        float inter = y / (k + 1);
        temp = k;
        displaytime = true;
        daW = wisdomDamage;
        daS = strengthDamage;
        daR = reputationDamage;
        if (character == "8")
        {
            PVIII plr = (PVIII)player.p;
            if (FromAndTo[0] == plr.guessPlayer)
            {
                switch (plr.guessType)
                {
                    case "攻擊":
                        if (DisplayType[0] == "attack" || DisplayType[0] == "multiattack")
                        {
                            plr.rightGuess();
                        }
                        else
                        {
                            plr.wrongGuess();
                        }
                        break;
                    case "防禦":
                        if (DisplayType[0] == "defense")
                        {
                            plr.rightGuess();
                        }
                        else
                        {
                            plr.wrongGuess();
                        }
                        break;
                    case "其他":
                        if (DisplayType[0] == "medicine" || DisplayType[0] == "special")
                        {
                            plr.rightGuess();
                        }
                        else
                        {
                            plr.wrongGuess();
                        }
                        break;
                }
            }
        }
        for (int i = 0; i < k; i++)
        {
            GameObject detail = Instantiate(carddisplayprfeb, displayfrom);
            DisplayInRally.Add(detail);
            ToolDisplayController tdc = detail.GetComponent<ToolDisplayController>();
            detail.transform.localPosition = new Vector3(x, y - inter * (i + 1), 1);
            
            tdc.tooltype = DisplayType[i];
            tdc.face = DisplayFace[i];
            tdc.forDisplay = true;
            tdc.init(wisdomDamage, strengthDamage, reputationDamage);
        }
        
        if (DisplayType[0] == "attack")
        {
            if (FromAndTo[1] == PhotonNetwork.LocalPlayer)
            {
                if (character == "11")
                {
                    System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
                    float rnd = (float)rand.NextDouble();
                    if (rnd <= 0.2f)
                    {
                        photonView.RPC("Announcement", RpcTarget.All, "呼...躲過了", 1500);
                        PhotonNetwork.SendAllOutgoingCommands();
                        photonView.RPC("Responded", RpcTarget.All, -daW, -daS, -daR);
                        PhotonNetwork.SendAllOutgoingCommands();
                        return;
                    }
                }
                status = 2;
                PD = true;
                canPlayDefense = true;
                canPlaySpecial = false;
            }
        }
        else if (DisplayType[0] == "medicine")
        {
            await Task.Delay(2000);

            if (FromAndTo[1] == PhotonNetwork.LocalPlayer)
            {
                status = 2;
                canPlayDefense = false;
                canPlaySpecial = false;
            }
        }
        else if(DisplayType[0] == "special")
        {
            if (FromAndTo[1] == PhotonNetwork.LocalPlayer)
            {
                status = 2;
                if (DisplayFace[0] == "test_special")
                {
                    canPlayDefense = false;
                    canPlaySpecial = false;
                }
                else if (DisplayFace[0] == "mate")
                {
                    canPlayDefense = false;
                    canPlaySpecial = true;
                }
                else if (DisplayFace[0] == "sleep_special")
                {
                    canPlayDefense = false;
                    canPlaySpecial = true;
                }
                else if (DisplayFace[0] == "femboy1" || DisplayFace[0] == "femboy2")
                {
                    canPlayDefense = true;
                    canPlaySpecial = true;
                }
                else if (DisplayFace[0] == "carbon")
                {
                    canPlayDefense = false;
                    canPlaySpecial = true;
                }
                else if (DisplayFace[0] == "confiscate_smartphone_ipad")
                {
                    canPlayDefense = false;
                    canPlaySpecial = true;
                }
                else if (DisplayFace[0] == "diplomacy")
                {
                    canPlayDefense = false;
                    canPlaySpecial = true;
                }
                else if (DisplayFace[0] == "zhenverse_broom")
                {
                    canPlayDefense = false;
                    canPlaySpecial = true;
                }
                else if (DisplayFace[0] == "science_train")
                {
                    canPlayDefense = false;
                    canPlaySpecial = true;
                }
                else
                {
                    canPlayDefense = false;
                    canPlaySpecial = true;
                }
            }
        }
            
    }
    [PunRPC]
    void Multi(int wisdomDamage, int strengthDamage, int reputationDamage)
    {
        Debug.Log("收到群攻");
        multi = true;
        int k = DisplayType.Count;
        float x = displayfrom.GetComponent<RectTransform>().sizeDelta.x / 2;
        float y = displayfrom.GetComponent<RectTransform>().sizeDelta.y;
        float inter = y / (k + 1);
        daW = wisdomDamage;
        daS = strengthDamage;
        daR = reputationDamage;
        temp = k;
        displaytime = true;
        for (int i = 0; i < k; i++)
        {
            GameObject detail = Instantiate(carddisplayprfeb, displayfrom);
            DisplayInRally.Add(detail);
            detail.transform.localPosition = new Vector3(x, y - inter * (i + 1), 1);

            ToolDisplayController tdc = detail.GetComponent<ToolDisplayController>();
            // 1. 先賦值
            tdc.tooltype = DisplayType[i];
            tdc.face = DisplayFace[i];
            tdc.forDisplay = true;

            // 2. 後呼叫 init (傳入群攻原始傷害)
            tdc.init(wisdomDamage, strengthDamage, reputationDamage);
        }
        if (FromAndTo[1] == PhotonNetwork.LocalPlayer)
        {
            if (player.p.isImmuneToMultiAttack())
            {
                photonView.RPC("Announcement", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " 不受群體攻擊影響！", 1500);
                PhotonNetwork.SendAllOutgoingCommands();
                photonView.RPC("Responded", RpcTarget.All, -daW, -daS, -daR); // 自動安全下莊
                return;
            }

            // 【升級】：2. 被動閃避檢測 (取代原本寫死的 11 號)
            if (player.p.checkPassiveDodge())
            {
                photonView.RPC("Announcement", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " 躲過了攻擊！", 1500);
                PhotonNetwork.SendAllOutgoingCommands();
                photonView.RPC("Responded", RpcTarget.All, -daW, -daS, -daR);
                return;
            }
            status = 2;
            PD = true;
            canPlayDefense = true;
            canPlaySpecial = false;

        }
    }

    [PunRPC]
    async Task Responded(int w, int s, int r)
    {
        Debug.Log("w是" + w +" / s是"+ s +" / r是" + r);
        int deW = w, deS = s, deR = r;
        int k = DisplayType.Count;
        if (character == "8")
        {
            PVIII plr = (PVIII)player.p;
            if (FromAndTo[1] == plr.guessPlayer && k > temp)
            {
                switch (plr.guessType)
                {
                    case "攻擊":
                        if (DisplayType[temp] == "attack" || DisplayType[temp] == "multiattack")
                        {
                            plr.rightGuess();
                        }
                        else
                        {
                            plr.wrongGuess();
                        }
                        break;
                    case "防禦":
                        if (DisplayType[temp] == "defense")
                        {
                            plr.rightGuess();
                        }
                        else
                        {
                            plr.wrongGuess();
                        }
                        break;
                    case "其他":
                        if (DisplayType[temp] == "medicine" || DisplayType[temp] == "special")
                        {
                            plr.rightGuess();
                        }
                        else
                        {
                            plr.wrongGuess();
                        }
                        break;
                }
            }
        }
        if (k - temp > 0)
        {
            float x = displayto.GetComponent<RectTransform>().sizeDelta.x / 2;
            float y = displayto.GetComponent<RectTransform>().sizeDelta.y;
            float inter = y / (k - temp + 1);
            for (int i = temp; i < k; i++)
            {
                GameObject detail2 = Instantiate(carddisplayprfeb, displayto);
                DisplayInRally.Add(detail2);
                detail2.transform.localPosition = new Vector3(x, y - inter * (i - temp + 1), 1);

                ToolDisplayController tdc = detail2.GetComponent<ToolDisplayController>();
                // 1. 先賦值
                tdc.forDisplay = true;
                if (i == temp)
                    tdc.num = 0;
                else
                    tdc.num = 1;
                tdc.tooltype = DisplayType[i];
                tdc.face = DisplayFace[i];
                tdc.init(deW, deS, deR);
            }
        }
        await Task.Delay(2000);

        // 1. 找出真正的攻擊者與防禦者 Index
        int attIdx = 0, defIdx = 0;
        for (int i = 0; i < total; ++i)
        {
            if (LocalPlayerList[i] == FromAndTo[0]) attIdx = i;
            if (LocalPlayerList[i] == FromAndTo[1]) defIdx = i;
        }
        string defCharacter = PlayerPanels[defIdx].GetComponent<PlayerPanelController>().character;

        bool wasReflect = reflect; // 記下這次是否為反彈
        reflect = false; // 結算前關閉狀態，避免干擾後續群攻鏈
        if (k > temp && DisplayInRally[temp].GetComponent<ToolDisplayController>().face == "all_in_vain" && DisplayInRally[0].GetComponent<ToolDisplayController>().face != "femboy1" && DisplayInRally[0].GetComponent<ToolDisplayController>().face != "femboy2")
        {
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("算了，就這樣吧...");
            sb.AppendLine("卷不贏XXX...");
            sb.AppendLine("遁隱虛空，跳出三界");
            sb.Append(FromAndTo[1].NickName + "使所有失效");
            
            EnqueueLocalAnnouncement(sb.ToString(), 3000);
            await Task.Delay(3000);
        }
        else if (k > temp && DisplayInRally[temp].GetComponent<ToolDisplayController>().face == "nah_bro" && DisplayType[0] == "special")
        {
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("在尹X把這坨線裡好之前");
            sb.AppendLine("你這牌先別出了吧");
            sb.AppendLine("(把線直接藏在投影機上方)");
            sb.AppendLine("哎呀？還有這招");
            sb.Append(FromAndTo[1].NickName + "使特殊牌失效");
            
            EnqueueLocalAnnouncement(sb.ToString(), 3000);
            await Task.Delay(3000);
        }
        else
        {
            int toW, toS, toR;
            if (daW > 0) toW = daW + Mathf.Min(0, deW);
            else toW = Mathf.Min(0, daW + deW);
            if (daS > 0) toS = daS + +Mathf.Min(0, deS);
            else toS = Mathf.Min(0, daS + deS);
            if (daR > 0) toR = daR + +Mathf.Min(0, deR);
            else toR = Mathf.Min(0, daR + deR);

            player.p.overrideFinalReceiveDamage(ref toW, ref toS, ref toR);

            if(FromAndTo[0] == FromAndTo[1] && (DisplayType[0] == "attack" || DisplayType[0] == "multiattack") && (toW < 0 || toS < 0 || toR < 0))
            {
                if(FromAndTo[1].NickName == PhotonNetwork.LocalPlayer.NickName)
                {
                    PlayerPanelController ppc = PlayerPanels[me].GetComponent<PlayerPanelController>();
                    if (ppc.isExist("malice"))
                    {
                        photonView.RPC("RemoveEffect", RpcTarget.All, me, "malice");
                        PhotonNetwork.SendAllOutgoingCommands();
                        await Task.Delay(100);
                        photonView.RPC("PutEffect", RpcTarget.All, me, 5, "malice");
                        PhotonNetwork.SendAllOutgoingCommands();
                    }
                }
            }
            else
            {
                await Task.Delay(100);
            }
            // 統一顯示受傷文字
            StringBuilder sb = new StringBuilder();
            if (DisplayType[0] == "attack" || DisplayType[0] == "multiattack" || DisplayType[0] == "medicine")
            {
                if (wasReflect) sb.AppendLine(FromAndTo[0].NickName + " 反彈給 " + FromAndTo[1].NickName);
                else sb.AppendLine(FromAndTo[0].NickName + " 對 " + FromAndTo[1].NickName);

                if (toW <= 0) sb.AppendLine("造成 " + (-toW).ToString() + " 點智力損害");
                else sb.AppendLine("回復 " + toW.ToString() + " 點智力");

                if (toS <= 0) sb.AppendLine("造成 " + (-toS).ToString() + " 點體力消耗");
                else sb.AppendLine("回復 " + toS.ToString() + " 點體力");

                if (toR <= 0) sb.Append("誹謗 " + (-toR).ToString() + " 點聲譽");
                else sb.Append("挽回 " + toR.ToString() + " 點聲譽");

                EnqueueLocalAnnouncement(sb.ToString(), 3000);
            }

            if (PhotonNetwork.LocalPlayer == FromAndTo[1])
            {
                UpdatePlayerProperties(toW, toS, toR);

                if (toW < 0 || toS < 0 || toR < 0)
                {
                    player.p.onTakeDamage(toW, toS, toR);
                }
            }

            if (DisplayType[0] == "attack" || DisplayType[0] == "multiattack" || DisplayType[0] == "medicine")
            {
                await Task.Delay(3000);
            }
            else
            {
                if (DisplayFace[0] == "mate")
                {
                    sb = new StringBuilder();
                    sb.AppendLine("抗瓊珶以和予兮...");
                    sb.AppendLine("指潛淵而為期");
                    sb.Append(FromAndTo[0].NickName + " 與 " + FromAndTo[1].NickName + " 締結了契約");

                    EnqueueLocalAnnouncement(sb.ToString(), 3000);

                    if (PhotonNetwork.LocalPlayer == FromAndTo[0])
                    {
                        Player targetPlayer = FromAndTo[1];

                        int targetW = (int)targetPlayer.CustomProperties["Wisdom"];
                        int targetS = (int)targetPlayer.CustomProperties["Strength"];
                        int targetR = (int)targetPlayer.CustomProperties["Reputation"];

                        int myW = (int)PhotonNetwork.LocalPlayer.CustomProperties["Wisdom"];
                        int myS = (int)PhotonNetwork.LocalPlayer.CustomProperties["Strength"];
                        int myR = (int)PhotonNetwork.LocalPlayer.CustomProperties["Reputation"];

                        int diffW = Mathf.Min(targetW, maxW) - myW;
                        int diffS = Mathf.Min(targetS, maxS) - myS;
                        int diffR = Mathf.Min(targetR, maxR) - myR;

                        UpdatePlayerProperties(diffW, diffS, diffR);
                    }

                    await Task.Delay(3000);
                }
                else if (DisplayFace[0] == "sleep_special")
                {
                    sb = new StringBuilder();
                    sb.AppendLine("歌筵畔，先安簟枕...");
                    sb.AppendLine("容我醉時眠...");
                    sb.Append(FromAndTo[0].NickName + " 讓 " + FromAndTo[1].NickName + " 陷入了沉睡");

                    EnqueueLocalAnnouncement(sb.ToString(), 3000);

                    if (PhotonNetwork.LocalPlayer == FromAndTo[0])
                    {
                        photonView.RPC("PutEffect", RpcTarget.All, defIdx, 2, "sleep");
                    }

                    await Task.Delay(3000);
                }
                else if (DisplayFace[0] == "carbon")
                {
                    if (PhotonNetwork.LocalPlayer == FromAndTo[0])
                    {
                        float roll = UnityEngine.Random.value;
                        string teacherId;
                        string teacherName;

                        if (roll <= 1f / 7f) { teacherId = "chenchienting"; teacherName = "陳建廷"; }
                        else if (roll <= 2f / 7f) { teacherId = "wuminglin"; teacherName = "吳明麟"; }
                        else if (roll <= 3f / 7f) { teacherId = "linminching"; teacherName = "林敏靜"; }
                        else if (roll <= 4f / 7f) { teacherId = "chenpenghsu"; teacherName = "陳鵬旭"; }
                        else if (roll <= 5f / 7f) { teacherId = "chenchihsheng"; teacherName = "陳智勝"; }
                        else if (roll <= 6f / 7f) { teacherId = "loyinting"; teacherName = "羅尹廷"; }
                        else { teacherId = "wangchinghua"; teacherName = "王靖華"; }

                        photonView.RPC("PutEffect", RpcTarget.All, defIdx, -1, teacherId);

                        string msg = "古之學者必有師...\n師者，所以傳道、受業、解惑也...\n" + FromAndTo[0].NickName + " 召喚了 " + teacherName;
                        photonView.RPC("Announcement", RpcTarget.All, msg, 3000);
                    }
                    await Task.Delay(3000);
                }
                else if (DisplayFace[0] == "confiscate_smartphone_ipad")
                {
                    if (PhotonNetwork.LocalPlayer == FromAndTo[1])
                    {
                        List<int> availableCategories = new List<int>();
                        for (int i = 0; i < typeNum; i++)
                        {
                            if (CardsInType[i].Count > 0) availableCategories.Add(i);
                        }

                        if (availableCategories.Count == 0)
                        {
                            string emptyMsg = "陳建廷 試圖沒收" + FromAndTo[1].NickName + " 的卡牌\n但 " + FromAndTo[1].NickName + " 已經沒有卡牌了，笑死！";
                            photonView.RPC("Announcement", RpcTarget.All, emptyMsg, 3000);
                        }
                        else
                        {
                            int rndCatIndex = UnityEngine.Random.Range(0, availableCategories.Count);
                            int chosenCat = availableCategories[rndCatIndex];
                            
                            string[] typeNames = { "攻擊", "群攻", "防禦", "治療", "增傷", "特殊" };
                            string chosenTypeName = typeNames[chosenCat];
                            int discardCount = CardsInType[chosenCat].Count;

                            foreach (GameObject card in CardsInType[chosenCat])
                            {
                                card.GetComponent<ToolCardController>().kill();
                            }
                            CardsInType[chosenCat].Clear();
                            RefreshCards();

                            string announceMsg = "陳建廷 沒收了 " + FromAndTo[1].NickName + " 所有的" + chosenTypeName + "卡，共 " + discardCount + " 張";
                            photonView.RPC("Announcement", RpcTarget.All, announceMsg, 3000);
                        }
                    }
                    await Task.Delay(3000);
                }
                else if (DisplayFace[0] == "diplomacy")
                {
                    sb = new StringBuilder();
                    sb.AppendLine("越國以鄙遠，君知其難也...");
                    sb.AppendLine("焉用亡鄭以陪鄰...");
                    sb.Append("林敏靜 協助 " + FromAndTo[0].NickName + " 與 " + FromAndTo[1].NickName + " 建交");

                    EnqueueLocalAnnouncement(sb.ToString(), 3000);

                    if (PhotonNetwork.LocalPlayer == FromAndTo[0])
                    {
                        Player targetPlayer = FromAndTo[1];

                        int targetW = (int)targetPlayer.CustomProperties["Wisdom"];
                        int targetS = (int)targetPlayer.CustomProperties["Strength"];
                        int targetR = (int)targetPlayer.CustomProperties["Reputation"];

                        int myW = (int)PhotonNetwork.LocalPlayer.CustomProperties["Wisdom"];
                        int myS = (int)PhotonNetwork.LocalPlayer.CustomProperties["Strength"];
                        int myR = (int)PhotonNetwork.LocalPlayer.CustomProperties["Reputation"];

                        int diffW = (targetW > myW) ? Mathf.Min(targetW, maxW) - myW : 0;
                        int diffS = (targetS > myS) ? Mathf.Min(targetS, maxS) - myS : 0;
                        int diffR = (targetR > myR) ? Mathf.Min(targetR, maxR) - myR : 0;

                        UpdatePlayerProperties(diffW, diffS, diffR);
                    }
                    await Task.Delay(3000);
                }
                else if (DisplayFace[0] == "zhenverse_broom")
                {
                    sb = new StringBuilder();
                    sb.AppendLine("有朋自遠方來...");
                    sb.AppendLine("花徑不曾緣客掃，蓬門今始為君開...");
                    sb.Append(FromAndTo[0].NickName + " 掃掉了 " + FromAndTo[1].NickName + " 的三張卡牌");

                    EnqueueLocalAnnouncement(sb.ToString(), 3000);

                    if (PhotonNetwork.LocalPlayer == FromAndTo[1])
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            DiscardACard();
                        }
                    }

                    await Task.Delay(3000);
                }
                else if (DisplayFace[0] == "nameless_doll")
                {
                    sb = new StringBuilder();
                    sb.AppendLine("人家好孤單喔...");
                    sb.AppendLine("可以抱抱我嗎...");
                    sb.AppendLine(FromAndTo[0].NickName + " 讓 " + FromAndTo[1].NickName);     
                    sb.AppendLine("遭怨念注視");
                    sb.Append("(第五人格沒有授權)");

                    EnqueueLocalAnnouncement(sb.ToString(), 3000);

                    if (PhotonNetwork.LocalPlayer == FromAndTo[1])
                    {
                        photonView.RPC("PutEffect", RpcTarget.All, defIdx, 5, "malice");
                    }
                    await Task.Delay(3000);
                }
                else if (DisplayFace[0] == "science_train")
                {
                    sb = new StringBuilder();
                    sb.AppendLine("急速列車出發~");
                    sb.AppendLine("就叫你不要打翻嘛");
                    EnqueueLocalAnnouncement(sb.ToString(), 2000);
                    sb = new StringBuilder();
                    sb.AppendLine("說到精準，我們就...");
                    sb.AppendLine("下一個是三視圖和光反射");
                    EnqueueLocalAnnouncement(sb.ToString(), 2000);
                    sb = new StringBuilder();
                    sb.AppendLine("好想回到那些日子...");
                    sb.AppendLine("再多相處一點...");
                    sb.AppendLine("希望永遠如此...");
                    sb.AppendLine("清除了 " + FromAndTo[1].NickName + " 的附加效果");     
                    
                    EnqueueLocalAnnouncement(sb.ToString(), 2000);

                    if (PhotonNetwork.LocalPlayer == FromAndTo[1])
                    {
                        PlayerPanelController ppc = PlayerPanels[me].GetComponent<PlayerPanelController>();
                        if(ppc.effectlist.Count > 0)
                        {
                            List<string> effe = new List<string>();
                            foreach(var ef in ppc.effectlist)
                            {
                                effe.Add(ef.id);
                            }
                            foreach(string id in effe)
                            {
                                photonView.RPC("RemoveEffect", RpcTarget.All, me, id);
                            }
                        }
                    }
                    await Task.Delay(6000);
                }
                else if (DisplayFace[0] == "king")
                {
                    sb.AppendLine("我進過四次NTUEE");
                    sb.AppendLine("而你...");
                    sb.AppendLine("又進過幾次台大");
                    sb.Append(FromAndTo[1].NickName + "展現王的風範");

                    EnqueueLocalAnnouncement(sb.ToString(), 3000);
                    await Task.Delay(3000);
                }
                else if (DisplayFace[0] == "femboy1" || DisplayFace[0] == "femboy2")
                {
                    if(daW < 0)
                    {
                        if(toW == 0 && toS == 0 && toR == 0)
                        {
                            sb.AppendLine("我們如此國色天香");
                            sb.AppendLine("你居然不動如山！");
                            sb.Append(FromAndTo[1].NickName + " 並不喜歡男娘");
                        }
                        else
                        {
                            sb.AppendLine(FromAndTo[1].NickName + "看到兩位男娘");
                            sb.AppendLine("高興壞了！大腦過載");
                            sb.AppendLine("忽不悟其所舍");
                            sb.Append("傷害自己算吧");
                            if (PhotonNetwork.LocalPlayer == FromAndTo[1])
                            {
                                UpdatePlayerProperties(toW, toS, toR);
                                if (toW < 0 || toS < 0 || toR < 0)
                                {
                                    player.p.onTakeDamage(toW, toS, toR);
                                }
                            }
                            if (PhotonNetwork.LocalPlayer == FromAndTo[0])
                            {
                                photonView.RPC("PutEffect", RpcTarget.All, defIdx, 2, "dizzy");
                            }
                        }
                    }
                    else
                    {
                        sb.AppendLine("似乎還有未湊齊的碎片");
                        sb.AppendLine(FromAndTo[0].NickName + " 再想想吧");
                        sb.Append("沒事的，" + FromAndTo[0].NickName);
                    }
                    EnqueueLocalAnnouncement(sb.ToString(), 3000);
                    await Task.Delay(3000);
                }
                else
                {
                    sb.AppendLine("不知道要填什麼");
                    sb.AppendLine("再說吧");
                    sb.Append(FromAndTo[1].NickName + "發現bug了");

                    EnqueueLocalAnnouncement(sb.ToString(), 3000);
                    await Task.Delay(3000);
                }
            }

                bool is13Reflecting = false;
            // 2號 防身術被動結算：讓【真正的攻擊者】扣血
            if ((DisplayType[0] == "attack" || DisplayType[0] == "multiattack") && (toW < 0 || toS < 0 || toR < 0))
            {
                if (defCharacter == "2" && PhotonNetwork.LocalPlayer == FromAndTo[0])
                {
                    photonView.RPC("Announcement", RpcTarget.All, "防身術！", 1500);
                    photonView.RPC("Cleaning", RpcTarget.All);
                    await Task.Delay(1600);
                    PhotonNetwork.SendAllOutgoingCommands();
                    photonView.RPC("GetCard", RpcTarget.All, "special", "self_defense");
                    photonView.RPC("StartReflection", RpcTarget.All, targetnum, me, Calculator(daW, 0.27f), Calculator(daS, 0.27f), Calculator(daR, 0.27f), multi, false, false);
                    PhotonNetwork.SendAllOutgoingCommands();
                    return;
                    //UpdatePlayerProperties(Calculator(toW, 0.27f), Calculator(toS, 0.27f), Calculator(toR, 0.27f));
                }
                else if(defCharacter == "13")
                {
                    System.Random syncRand = new System.Random(FromAndTo[0].ActorNumber + FromAndTo[1].ActorNumber + w + s + r);
                    if (syncRand.NextDouble() <= 0.25f) is13Reflecting = true;
                    if (is13Reflecting)
                    {
                        EnqueueLocalAnnouncement("對上眼了！", 2000);
                        await Task.Delay(2000);
                        if (PhotonNetwork.LocalPlayer == FromAndTo[1])
                        {
                            photonView.RPC("Cleaning", RpcTarget.All);
                            photonView.RPC("GetCard", RpcTarget.All, "attack", "boxing");
                            photonView.RPC("StartReflection", RpcTarget.All, targetnum, me, Calculator(daW, 0.27f), Calculator(daS, 0.27f), Calculator(daR, 0.27f), multi, true, false);

                            PhotonNetwork.SendAllOutgoingCommands();
                        }
                        return; // 成功打出新攻擊卡，直接中斷本次結算，讓狀態機進入下一輪
                    }
                }
                
            }
        }

        // === 殘局處理與回合交棒 ===
        if (PhotonNetwork.LocalPlayer == FromAndTo[0]) // 原則上由發起攻擊的人負責清空與交棒
        {
            if (isResolvingVirtualCard)
            {
                photonView.RPC("Cleaning", RpcTarget.All);
                status = 0;
                isResolvingVirtualCard = false;
                return; // don't 交棒！！！
            }
            if (wasReflect)
            {
                // 反彈殘局：交棒邏輯由反彈起點 (reflectMemoPlayer) 接管
                if (PhotonNetwork.LocalPlayer == reflectMemoPlayer)
                {
                    if (!multi || reflectMemoNext == me)
                    {
                        // 單體反彈結束，或群攻反彈繞完一圈回到自己：清空場地，交棒下家
                        photonView.RPC("Cleaning", RpcTarget.All);
                        status = 0;
                        
                        if(character == "7")
                        {
                            double p = rand.NextDouble();
                            if(p < 0.2)
                            {
                                StringBuilder sb = new StringBuilder(); 
                                sb.AppendLine("不特別針對誰");
                                sb.AppendLine("但你們的專題");
                                sb.AppendLine("配不上稱為研究");
                                sb.Append(LocalPlayerList[me].NickName + "再次行動");
                                photonView.RPC("Announcement", RpcTarget.All, sb.ToString(), 2500);
                                await Task.Delay(2500);
                                isGameEnded(me);
                            }
                            else
                            {
                                player.endRound();
                                await endRoundEffectHandler(PlayerPanels, me, 2000);
                                if (PlayerPanels != null) photonView.RPC("UpdateEffect", RpcTarget.All, me);
                                isGameEnded((me + 1) % total);
                            }
                        }
                        else
                        {
                            player.endRound();
                            await endRoundEffectHandler(PlayerPanels, me, 2000);
                            if (PlayerPanels != null) photonView.RPC("UpdateEffect", RpcTarget.All, me);
                            isGameEnded((me + 1) % total);
                        }
                    }
                    else
                    {
                        // 群攻尚未結束，繼續傳給下一個排隊的人
                        photonView.RPC("Cleaning", RpcTarget.All);
                        status = 0;
                        photonView.RPC("SetFromTo", RpcTarget.All, me, reflectMemoNext);
                        PhotonNetwork.SendAllOutgoingCommands();
                        photonView.RPC("Multi", RpcTarget.All, daW, daS, daR);
                    }
                }
            }
            else
            {
                // 一般攻擊殘局
                if (!multi)
                {
                    photonView.RPC("Cleaning", RpcTarget.All);
                    status = 0;
                    if (character == "7")
                    {
                        double p = rand.NextDouble();
                        if (p < 0.2)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("不特別針對誰");
                            sb.AppendLine("但你們的專題");
                            sb.AppendLine("配不上稱為研究");
                            sb.Append(LocalPlayerList[me].NickName + "再次行動");
                            photonView.RPC("Announcement", RpcTarget.All, sb.ToString(), 2500);
                            await endRoundEffectHandler(PlayerPanels, me, 2000);
                            isGameEnded(me);
                        }
                        else
                        {
                            player.endRound();
                            await endRoundEffectHandler(PlayerPanels, me, 2000);
                            if (PlayerPanels != null) photonView.RPC("UpdateEffect", RpcTarget.All, me);
                            isGameEnded((me + 1) % total);
                        }
                    }
                    else
                    {
                        player.endRound();
                        await endRoundEffectHandler(PlayerPanels, me, 2000);
                        if (PlayerPanels != null) photonView.RPC("UpdateEffect", RpcTarget.All, me);
                        isGameEnded((me + 1) % total);
                    }
                }
                else
                {
                    // 繼續傳遞群攻
                    photonView.RPC("Cleaning", RpcTarget.All);
                    int nextVictim = (defIdx + 1) % total;
                    if (nextVictim == attIdx)
                    {
                        // 繞完一圈回到自己，結束
                        status = 0;
                        player.endRound();
                        await endRoundEffectHandler(PlayerPanels, me, 2000);
                        if (PlayerPanels != null) photonView.RPC("UpdateEffect", RpcTarget.All, me);
                        isGameEnded((me + 1) % total);
                    }
                    else
                    {
                        status = 0;
                        photonView.RPC("SetFromTo", RpcTarget.All, me, nextVictim);
                        PhotonNetwork.SendAllOutgoingCommands();
                        photonView.RPC("Multi", RpcTarget.All, daW, daS, daR);
                    }
                }
            }
        }
        // 清空本地陣列防呆
        DisplayFace.Clear();
        DisplayType.Clear();
        multi = false;
    }
    private class AnnounceMsg 
    {
        public string content;
        public int time;
    }
    private Queue<AnnounceMsg> announceQueue = new Queue<AnnounceMsg>();
    private bool isAnnouncing = false;

    public void EnqueueLocalAnnouncement(string content, int time)
    {
        announceQueue.Enqueue(new AnnounceMsg { content = content, time = time });
        if (!isAnnouncing)
        { 
            _ = ProcessAnnouncementQueue(); // _: do not wait for it
        }
    }

    private async Task ProcessAnnouncementQueue()
    {
        isAnnouncing = true;
        HintPanel.SetActive(true);
        while (announceQueue.Count > 0)
        {
            var msg = announceQueue.Dequeue();
            hint.text = msg.content;
            await Task.Delay(msg.time);
        }
        HintPanel.SetActive(false);
        isAnnouncing = false;
    }

    [PunRPC]
    void Announcement(string content, int time)
    {
        EnqueueLocalAnnouncement(content, time);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PhotonNetwork.IsConnected == false)
        {
            SceneManager.LoadScene("StartScene");
            return;
        }
        typeNum = 6;
        cardCumulativeProbability = new double[typeNum];
        for(int i = 0; i < typeNum; ++i)
        {
            if(i == 0) 
                cardCumulativeProbability[i] = cardRatio[i];
            else
                cardCumulativeProbability[i] = cardCumulativeProbability[i - 1] + cardRatio[i];
        }
        for(int i = 0; i < typeNum; ++i)
        {
            cardCumulativeProbability[i] /= cardCumulativeProbability[typeNum - 1];
        }
        rand = new System.Random(Guid.NewGuid().GetHashCode());
        LoadCardPools();
        player = gameObject.GetComponent<PlayerHandle>();
        total = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log(total);
        LocalPlayerList = new Photon.Realtime.Player[total];
        PlayerPanels = new GameObject[total];
        CardsInType = new List<GameObject>[6];
        CardsInDisplay = new List<GameObject>[6];
        isAlive = new bool[total];
        isPickable = new bool[total];
        for (int j = 0; j < 6; j++)
        {
            List<GameObject> a = new List<GameObject>();
            List<GameObject> b = new List<GameObject>();
            CardsInType[j] = a;
            CardsInDisplay[j] = b;
        }
        Wdead = false;
        Sdead = false;
        Rdead = false;
        pickable = true;
        over = false;
        reflect = false;
        SW = Screen.width;
        SH = Screen.height;
        cs = (0.7f * SW - 220) / 10;
        multicount = 0;
        toolcardprfeb.GetComponent<RectTransform>().sizeDelta = new Vector3(cs, cs, 1);
        cardpanel.GetComponent<RectTransform>().sizeDelta = new Vector3(0.7f * SW, 2 * cs + 60, 1);
        count = 0;
        
        skillUseCounter = 0;
        status = 0;
        PD = true;
        displaycount = 0;
        targetnum = -1;
        toolcardtype = "none";
        Application.targetFrameRate = 60;
        cardpicking = -1;
        HintPanel.SetActive(false);
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                position[i * 10 + j] = new Vector3(20 + (cs + 20) * j, 2 * cs + 40 - (cs + 20) * i, 0);
            }
        }
        if(PhotonNetwork.LocalPlayer.NickName == "我愛蘇昱全" || PhotonNetwork.LocalPlayer.NickName == "ILoveAndy")
        {
            for(int x = 0; x < 15; ++x)
            {
                PickACard(cardCumulativeProbability[4] - 0.01, "Su_sticker");
            }
        }
        else
        {
            // for(int x = 0; x < 1; ++x)
            // {
            //     PickACard(0.67f, "test_attack");
            //     PickACard(1.67f, "test_multiattack");
            //     PickACard(2.67f, "test_defense");
            //     PickACard(3.67f, "test_medicine");
            //     PickACard(4.67f, "test_strengthen");
            //     PickACard(5.67f, "test_special");    
            // }
            // PickACard(5.67f, "sleep_special");
            // PickACard(5.67f, "all_in_vain");
            // PickACard(5.67f, "femboy1");
            // PickACard(5.67f, "femboy2");
            // PickACard(5.67f, "femboy1");
            // PickACard(5.67f, "nameless_doll");
            // PickACard(5.67f, "carbon");
            // for (int i = 0; i < 10; i++)
            // {
            //     PickACard(5.67f, "carbon");
            // }
            // for (int x = 0; x < 4; ++x)
            // {
            //     double rnd = rand.NextDouble();
            //     PickACard(rnd);
            // }
            // PickACard(5.67f, "nameless_doll");
            // PickACard(5.67f, "all_in_vain");
            // PickACard(5.67f, "nah_bro");
            for(int x = 0; x < 15; ++x)
            {
                PickACard(rand.NextDouble());
            }
        }
        

        RefreshCards();
        if (PhotonNetwork.IsMasterClient)
        {
            Photon.Realtime.Player firstPlayer = null;
            foreach (var kvp in PhotonNetwork.CurrentRoom.Players)
            {
                // 記下房間裡的第一位玩家
                if (firstPlayer == null)
                {
                    firstPlayer = kvp.Value;
                }
                photonView.RPC("PlayerList", RpcTarget.All, kvp.Value);
            }

            // 【修正】：不要用 LocalPlayerList[0]，直接傳入剛剛抓到的 firstPlayer！
            photonView.RPC("Go", firstPlayer);
        }

        
    }
    public void LoadCardPools()
    {
        string[] normalTypes = { "attack", "multiattack", "defense", "medicine", "strengthen" };

        foreach (string type in normalTypes)
        {
            cardPools[type] = new List<string>();
            TextAsset txtFile = Resources.Load<TextAsset>("pool/" + type);
            if (txtFile != null)
            {
                string[] lines = txtFile.text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    if (!string.IsNullOrEmpty(line.Trim())) cardPools[type].Add(line.Trim());
                }
            }
        }

        cardPools["special"] = new List<string>();
        TextAsset specialTxt = Resources.Load<TextAsset>("pool/special");
        if (specialTxt != null)
        {
            string[] lines = specialTxt.text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] data = line.Split(',');
                if (data.Length >= 3)
                {
                    string cName = data[0].Trim();
                    bool onTurn = (data[1].Trim().ToLower() == "true");
                    bool onAttacked = (data[2].Trim().ToLower() == "true");
                    bool respond = (data[3].Trim().ToLower() == "true");
                    cardPools["special"].Add(cName);
                    specialCardDict[cName] = new SpecialCardData
                    {
                        face = cName,
                        canPlayOnTurn = onTurn,
                        canPlayOnAttacked = onAttacked,
                        canRespond = respond
                    };
                }
            }
            Debug.Log($"成功載入 special 卡池，共 {cardPools["special"].Count} 張卡牌。");
        }
    }
    
    public async Task PlayCard()
    {
        status = 0; 
        int[] Damages = new int[3];
        bool m = false;
        List<string> playedType = new List<string>();
        List<string> playedFace = new List<string>();
        // string playedType = "none"; 
        // string playedFace = "none";
        int playedCount = 0;        

        if (displaycount > 0)
        {
            if (PD)
            {
                List<string> attemptTypes = new List<string>();
                for (int i = 0; i < typeNum; i++)
                {
                    if (CardsInDisplay[i].Count > 0)
                    {
                        if (i == 1) m = true;
                        foreach (GameObject obj in CardsInDisplay[i])
                        {
                            attemptTypes.Add(obj.GetComponent<ToolDisplayController>().tooltype);
                        }
                    }
                }

                if (player.p.checkActionFailure(attemptTypes))
                {
                    for (int i = 0; i < typeNum; i++)
                    {
                        if (CardsInDisplay[i].Count > 0)
                        {
                            foreach (GameObject obj in CardsInDisplay[i])
                            {
                                int j = 0;
                                foreach (GameObject o in CardsInType[i])
                                {
                                    if (o.GetComponent<ToolCardController>().num == obj.GetComponent<ToolDisplayController>().num) break;
                                    else j++;
                                }
                                CardsInType[i][j].GetComponent<ToolCardController>().kill();
                                CardsInType[i].Remove(CardsInType[i][j]);
                                obj.GetComponent<ToolDisplayController>().kill();
                            }
                            CardsInDisplay[i].Clear();
                        }
                    }
                    int amount = player.p.getDrawCardCount();
                    for (int i = 0; i < amount; i++)
                    {
                        int count = 0;
                        foreach (var Cards in CardsInType)
                        {
                            if (Cards == null) continue;
                            count += Cards.Count;
                        }
                        if (count < 20)
                        {
                            double rnd = rand.NextDouble();
                            PickACard(rnd);
                        }
                    }
                    RefreshCards();
                    displaycount = 0;
                    toolcardtype = "none";
                    cardpicking = -1;
                    isGameEnded((me + 1) % total);
                    return;
                }

                for (int i = 0; i < typeNum; i++)
                {
                    if (CardsInDisplay[i].Count > 0)
                    {
                        if (i == 1) m = true;
                        if (targetnum == -1)
                        {
                            if (i == 0 || i == typeNum - 1)
                            {
                                int r = 1;
                                for (r = 1; r <= total; ++r)
                                {
                                    PlayerPanelController ppc = PlayerPanels[(me + r) % total].GetComponent<PlayerPanelController>();
                                    if (ppc.isExist("disappear") || ppc.isExist("hide")) continue;
                                    else break;
                                }
                                targetnum = (me + r) % total;
                            }
                            else targetnum = me;
                        }
                        foreach (GameObject obj in CardsInDisplay[i])
                        {
                            // if(playedCount == 0) playedType = obj.GetComponent<ToolDisplayController>().tooltype;
                            string getType = obj.GetComponent<ToolDisplayController>().tooltype;
                            playedType.Add(getType);
                            if (getType == "special") playedFace.Add(obj.GetComponent<ToolDisplayController>().face);
                            else
                            {
                                int[] readInValue = getValue(obj.GetComponent<ToolDisplayController>().tooltype, obj.GetComponent<ToolDisplayController>().face);
                                for (int dummy = 0; dummy < 3; dummy++)
                                    Damages[dummy] += readInValue[dummy];
                            }

                            playedCount++;

                            photonView.RPC("GetCard", RpcTarget.All, obj.GetComponent<ToolDisplayController>().tooltype, obj.GetComponent<ToolDisplayController>().face);
                            int j = 0;
                            foreach (GameObject o in CardsInType[i])
                            {
                                if (o.GetComponent<ToolCardController>().num == obj.GetComponent<ToolDisplayController>().num) break;
                                else j++;
                            }
                            CardsInType[i][j].GetComponent<ToolCardController>().kill();
                            CardsInType[i].Remove(CardsInType[i][j]);
                            obj.GetComponent<ToolDisplayController>().kill();
                        }
                        CardsInDisplay[i].Clear();
                    }
                }

                bool isStandardAction = true;

                if (playedType[0] == "special")
                {
                    isStandardAction = false;
                    photonView.RPC("SetFromTo", RpcTarget.All, me, targetnum);
                    PhotonNetwork.SendAllOutgoingCommands();
                    if(playedFace.Count >= 2 && playedFace.Contains("femboy1") && playedFace.Contains("femboy2"))
                    {
                        player.p.updateAttack();
                    
                        for (int i = 0; i < 3; ++i)
                        {
                            Damages[i] = -20;
                            Damages[i] = Calculator(Damages[i], player.p.attackRatio[i]);
                            if (Damages[i] != 0) Damages[i] += player.p.attackAdd[i];
                        }
                        photonView.RPC("Played", RpcTarget.All, -20, -20, -20);
                    }
                    else
                    {
                        photonView.RPC("Played", RpcTarget.All, 0, 0, 0);
                    }
                    PhotonNetwork.SendAllOutgoingCommands();
                }
                
                if (isStandardAction)
                {
                    if (playedType[0] == "medicine")
                    {
                        player.p.updateMed();

                        for (int i = 0; i < 3; ++i)
                        {
                            Damages[i] = Calculator(Damages[i], player.p.medRatio[i]);
                            if (Damages[i] != 0) Damages[i] += player.p.medAdd[i];
                        }
                    }
                    else
                    {
                        player.p.updateAttack();
                    
                        for (int i = 0; i < 3; ++i)
                        {
                            Damages[i] = Calculator(Damages[i], player.p.attackRatio[i]);
                            if (Damages[i] != 0) Damages[i] += player.p.attackAdd[i];
                        }
                    }

                    daW = Damages[0]; daS = Damages[1]; daR = Damages[2];
                    player.p.overrideFinalDamage(ref daW, ref daS, ref daR);

                    if (m)
                    {
                        photonView.RPC("SetFromTo", RpcTarget.All, me, (me + 1) % total);
                        PhotonNetwork.SendAllOutgoingCommands();
                        photonView.RPC("Multi", RpcTarget.All, daW, daS, daR);
                        PhotonNetwork.SendAllOutgoingCommands();
                    }
                    else
                    {
                        photonView.RPC("SetFromTo", RpcTarget.All, me, targetnum);
                        PhotonNetwork.SendAllOutgoingCommands();
                        photonView.RPC("Played", RpcTarget.All, daW, daS, daR);
                        PhotonNetwork.SendAllOutgoingCommands();
                    }
                }

                int drawAmount = player.p.getDrawCardCount();
                for (int i = 0; i < drawAmount; i++)
                {
                    int count = 0;
                    foreach (var Cards in CardsInType)
                    {
                        if (Cards == null) continue;
                        count += Cards.Count;
                    }
                    if (count < 20)
                    {
                        double rnd = rand.NextDouble();
                        PickACard(rnd);
                    }
                }
                RefreshCards();
                toolcardtype = "none";
                cardpicking = -1;
            }
            else
            {
                List<string> attemptTypes = new List<string>();
                attemptTypes.Add("Discarding");
                StringBuilder sb = new StringBuilder();
                if (player.p.checkActionFailure(attemptTypes))
                {
                    for (int i = 0; i < typeNum; i++)
                    {
                        if (CardsInDisplay[i].Count > 0)
                        {
                            foreach (GameObject obj in CardsInDisplay[i])
                            {
                                obj.GetComponent<ToolDisplayController>().kill();
                            }
                            CardsInDisplay[i].Clear();
                        }
                    }
                    
                    sb.AppendLine(FromAndTo[0].NickName + " 連牌都棄不掉");
                    sb.Append("可憐阿");
                    photonView.RPC("Announcement", RpcTarget.All, sb.ToString(), 2000);
                    await Task.Delay(2000);
                    RefreshCards();
                    displaycount = 0;
                    toolcardtype = "none";
                    cardpicking = -1;
                    isGameEnded((me + 1) % total);
                    return;
                }
                int discardCount = 0;
                for (int i = 0; i < typeNum; i++)
                {
                    if (CardsInDisplay[i].Count > 0)
                    {
                        foreach (GameObject obj in CardsInDisplay[i])
                        {
                            int j = 0;
                            foreach (GameObject o in CardsInType[i])
                            {
                                if (o.GetComponent<ToolCardController>().num == obj.GetComponent<ToolDisplayController>().num) break;
                                else j++;
                            }
                            CardsInType[i][j].GetComponent<ToolCardController>().kill();
                            CardsInType[i].Remove(CardsInType[i][j]);
                            obj.GetComponent<ToolDisplayController>().kill();
                            discardCount++;
                        }
                        
                        CardsInDisplay[i].Clear();
                    }
                }

                sb.AppendLine(FromAndTo[0].NickName + " 選擇棄牌");
                sb.AppendLine("如老師當掉他般");
                sb.AppendLine("棄掉了 " + discardCount + " 張牌");
                photonView.RPC("Announcement", RpcTarget.All, sb.ToString(), 2000);
                await Task.Delay(2000);
                RefreshCards();
                displaycount = 0;
                toolcardtype = "none";
                cardpicking = -1;
                isGameEnded((me + 1) % total);
                return;
            }
            
        }
        else // 跳過不出牌
        {
            int drawAmount = player.p.getDrawCardCount();
            for (int i = 0; i < drawAmount; i++)
            {
                int count = 0;
                foreach (var Cards in CardsInType)
                {
                    if (Cards == null) continue;
                    count += Cards.Count;
                }
                if (count < 20)
                {
                    double rnd = rand.NextDouble();
                    PickACard(rnd);
                }
            }
            RefreshCards();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(LocalPlayerList[me].NickName + " 這個孬種");
            sb.AppendLine("什麼也不敢做");
            sb.AppendLine("難怪會失去她");
            sb.Append(LocalPlayerList[me].NickName + " 抽取卡牌");
            photonView.RPC("Announcement", RpcTarget.All, sb.ToString(), 2500);
            PhotonNetwork.SendAllOutgoingCommands();
            await Task.Delay(2600);
            player.endRound();
            await endRoundEffectHandler(PlayerPanels, me, 2000);
            if (PlayerPanels != null) photonView.RPC("UpdateEffect", RpcTarget.All, me);
            isGameEnded((me + 1) % total);
        }
        displaycount = 0;
    }
    public void PlayVirtualCard(string virtualType, string virtualFace, string teacherName)
    {
        int[] cardValues = getValue(virtualType, virtualFace);
        daW = cardValues[0];
        daS = cardValues[1];
        daR = cardValues[2];

        int targetIdx = me;
        
        if (virtualType == "attack" || virtualType == "special")
        {
            targetIdx = (me + 1) % total;
            while (PlayerPanels[targetIdx].GetComponent<PlayerPanelController>().isExist("disappear") || PlayerPanels[targetIdx].GetComponent<PlayerPanelController>().isExist("sleep") || !isAlive[targetIdx])
            {
                targetIdx = (targetIdx + 1) % total;
                if (targetIdx == me) break;
            }
        }
        else if (virtualType == "multiattack")
        {
            targetIdx = (me + 1) % total;
        }

        if (virtualFace == "confiscate_smartphone_ipad" || virtualFace == "diplomacy")
        {
            List<int> validTargets = new List<int>();
            for (int i = 0; i < total; i++)
            {
                if (i == me) continue;
                if (!isAlive[i]) continue;
                PlayerPanelController ppc = PlayerPanels[i].GetComponent<PlayerPanelController>();
                if (ppc.isExist("disappear") || ppc.isExist("sleep")) continue;
                validTargets.Add(i);
            }

            if (validTargets.Count > 0)
            {
                targetIdx = validTargets[UnityEngine.Random.Range(0, validTargets.Count)];
            }
            else
            {
                targetIdx = me;
            }
        }

        if (teacherName == "chenchienting")
        {
            photonView.RPC("Announcement", RpcTarget.All, "陳建廷 使用技能！", 1500);
        }
        else if (teacherName == "wuminglin")
        {
            photonView.RPC("Announcement", RpcTarget.All, "吳明麟 使用技能！", 1500);
        }
        else if (teacherName == "linminching")
        {
            photonView.RPC("Announcement", RpcTarget.All, "林敏靜 使用技能！", 1500);
        }
        else if (teacherName == "chenpenghsu")
        {
            photonView.RPC("Announcement", RpcTarget.All, "陳鵬旭 使用技能！", 1500);
        }
        else if (teacherName == "chenchihsheng")
        {
            photonView.RPC("Announcement", RpcTarget.All, "陳智勝 使用技能！", 1500);
        }
        else if (teacherName == "loyinting")
        {
            photonView.RPC("Announcement", RpcTarget.All, "羅尹廷 使用技能！", 1500);
        }
        else if (teacherName == "wangchinghua")
        {
            photonView.RPC("Announcement", RpcTarget.All, "王靖華 使用技能！", 1500);
        }
        photonView.RPC("SetFromTo", RpcTarget.All, me, targetIdx);
        photonView.RPC("GetCard", RpcTarget.All, virtualType, virtualFace);

        if (virtualType == "multiattack")
        {
            photonView.RPC("Multi", RpcTarget.All, daW, daS, daR);
            PhotonNetwork.SendAllOutgoingCommands();
        }
        else
        {
            photonView.RPC("Played", RpcTarget.All, daW, daS, daR);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }
    int Calculator(int originNum, double ratio)
    {
        if(ratio <= 1.0f)
        {
            return (int)((double)originNum * ratio);
        }
        else
        {
            int fix = (int)((double)originNum * ratio * 10.0f) % 10;
            if(fix == 0)
            {
                return (int)((double)originNum * ratio);
            }
            else
            {
                return (int)((double)originNum * ratio + 1.0f);
            }
        }
    }
    public int[] getValue(string type, string face)
    {
        int[] result = new int[3];
        TextAsset det = Resources.Load<TextAsset>("text/Tool/" + type + "/" + face);
        if (det != null)
        {
            string[] inf = det.text.Split("\n");
            for (int i = 0; i < inf.Length; i++)
            {
                inf[i] = inf[i].Trim();
            }
            int.TryParse(inf[0], out result[0]);
            int.TryParse(inf[1], out result[1]);
            int.TryParse(inf[2], out result[2]);
        }

        return result;
    }
    public void Skip()
    {
        double rnd = rand.NextDouble();
        PickACard(rnd);
        RefreshCards();
    }
    
    public void RespondCard()
    {
        status = 0;
        int[] Defends = new int[3];
        for(int i = 0; i < 3; ++i)
        {
            Defends[i] = 0;
        }
        bool isReflecting = false;
        List<string> playedSpecialFaces = new List<string>();
        if (displaycount > 0)
        {
            for (int i = 0; i < typeNum; i++)
            {
                foreach (GameObject obj in CardsInDisplay[i])
                {
                    if (obj.GetComponent<ToolDisplayController>().tooltype == "special")
                    {
                        playedSpecialFaces.Add(obj.GetComponent<ToolDisplayController>().face);
                        if (playedSpecialFaces[0] == "test_reflect")
                            isReflecting = true;
                    }
                }
            }

            // 第二步：回收卡牌，並且【只有非反彈卡】才呼叫 GetCard 廣播
            for (int i = 0; i < typeNum; i++)
            {
                if (CardsInDisplay[i].Count > 0)
                {
                    foreach (GameObject obj in CardsInDisplay[i])
                    {
                        var controller = obj.GetComponent<ToolDisplayController>();

                        if (controller.face == "test_reflect")
                        {
                            // 反彈卡不需要 GetCard 廣播，它的效果由 StartReflection 獨立處理
                        }
                        else
                        {
                            // 普通防禦卡才需要同步給所有人看
                            photonView.RPC("GetCard", RpcTarget.All, controller.tooltype, controller.face);
                            if(controller.tooltype == "defense")
                            {
                                int[] readInValue = getValue(controller.tooltype, controller.face);
                                for(int dummy = 0; dummy < 3; dummy++)
                                {
                                    Defends[dummy] += readInValue[dummy];
                                }
                            }
                            
                        }

                        // 回收本地的手牌與展示牌
                        int j = 0;
                        foreach (GameObject o in CardsInType[i])
                        {
                            if (o.GetComponent<ToolCardController>().num == controller.num) break;
                            else j++;
                        }
                        CardsInType[i][j].GetComponent<ToolCardController>().kill();
                        CardsInType[i].Remove(CardsInType[i][j]);
                        obj.GetComponent<ToolDisplayController>().kill();
                    }
                    CardsInDisplay[i].Clear();
                }
            }
        }
        
        // 第三步：分流處理狀態機
        if (isReflecting)
        {
            int originalAttackerIndex = 0;
            for (int i = 0; i < total; i++)
            {
                if (LocalPlayerList[i] == FromAndTo[0]) { originalAttackerIndex = i; break; }
            }
            photonView.RPC("StartReflection", RpcTarget.All, me, originalAttackerIndex, daW, daS, daR, multi);
            PhotonNetwork.SendAllOutgoingCommands();
        }
        else
        {
            if (playedSpecialFaces.Count > 0)
            {
                foreach (string spFace in playedSpecialFaces)
                {
                    if (spFace == "all_in_vain")
                    {
                        Debug.Log("w是" + -daW +" / s是"+ -daS +" / r是" + -daR);
                        Debug.Log("送出之後");
                        photonView.RPC("Responded", RpcTarget.All, (int)-daW, (int)-daS, (int)-daR);
                        PhotonNetwork.SendAllOutgoingCommands();
                    }
                    else if(spFace == "nah_bro")
                    {
                        photonView.RPC("Responded", RpcTarget.All, (int)-daW, (int)-daS, (int)-daR);
                        PhotonNetwork.SendAllOutgoingCommands();
                    }
                }
            }
            else
            {
                player.p.currentBaseDefends = Defends;
                player.p.updateDefend();
                for (int i = 0; i < 3; ++i)
                {
                    if (Defends[i] < 0) continue;
                    Defends[i] = Calculator(Defends[i], player.p.defendRatio[i]);
                    Defends[i] += player.p.defendAdd[i];
                }
                photonView.RPC("Responded", RpcTarget.All, Defends[0], Defends[1], Defends[2]);
                PhotonNetwork.SendAllOutgoingCommands();
            }
        }

        RefreshCards();
        toolcardtype = "none";
        cardpicking = -1;
        displaycount = 0; // 確保重置計數
    }
    public void UpdatePlayerProperties(int w, int s, int r)
    {
        
        if (r < 0) // teacher leaving
        {
            PlayerPanelController myPanel = PlayerPanels[me].GetComponent<PlayerPanelController>();
            if (myPanel.isExist("chenchienting")) 
            {
                if (UnityEngine.Random.value <= 0.25f)
                {
                    photonView.RPC("RemoveEffect", RpcTarget.All, me, "chenchienting");
                    photonView.RPC("Announcement", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " 聲譽嚴重受損，朽木不可雕也！\n陳建廷失望地離開了...", 2000);
                }
            }
            if (myPanel.isExist("wuminglin")) 
            {
                if (UnityEngine.Random.value <= 0.25f)
                {
                    photonView.RPC("RemoveEffect", RpcTarget.All, me, "wuminglin");
                    photonView.RPC("Announcement", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " 聲譽嚴重受損，朽木不可雕也！\n吳明麟失望地離開了...", 2000);
                }
            }
            if (myPanel.isExist("linminching")) 
            {
                if (UnityEngine.Random.value <= 0.25f)
                {
                    photonView.RPC("RemoveEffect", RpcTarget.All, me, "linminching");
                    photonView.RPC("Announcement", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " 聲譽嚴重受損，朽木不可雕也！\n林敏靜失望地離開了...", 2000);
                }
            }
            if (myPanel.isExist("chenpenghsu")) 
            {
                if (UnityEngine.Random.value <= 0.25f)
                {
                    photonView.RPC("RemoveEffect", RpcTarget.All, me, "chenpenghsu");
                    photonView.RPC("Announcement", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " 聲譽嚴重受損，朽木不可雕也！\n陳鵬旭失望地離開了...", 2000);
                }
            }
            if (myPanel.isExist("chenchihsheng")) 
            {
                if (UnityEngine.Random.value <= 0.25f)
                {
                    photonView.RPC("RemoveEffect", RpcTarget.All, me, "chenchihsheng");
                    photonView.RPC("Announcement", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " 聲譽嚴重受損，朽木不可雕也！\n陳智勝失望地離開了...", 2000);
                }
            }
            if (myPanel.isExist("loyinting")) 
            {
                if (UnityEngine.Random.value <= 0.25f)
                {
                    photonView.RPC("RemoveEffect", RpcTarget.All, me, "loyinting");
                    photonView.RPC("Announcement", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " 聲譽嚴重受損，朽木不可雕也！\n羅尹廷失望地離開了...", 2000);
                }
            }
            if (myPanel.isExist("wangchinghua")) 
            {
                if (UnityEngine.Random.value <= 0.25f)
                {
                    photonView.RPC("RemoveEffect", RpcTarget.All, me, "wangchinghua");
                    photonView.RPC("Announcement", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " 聲譽嚴重受損，朽木不可雕也！\n王靖華失望地離開了...", 2000);
                }
            }
        }

        int W = 0, S = 0, R = 0;
        if(!Wdead)
            W = (int)PhotonNetwork.LocalPlayer.CustomProperties["Wisdom"] + w;
        if(!Sdead)
            S = (int)PhotonNetwork.LocalPlayer.CustomProperties["Strength"] + s;
        if(!Rdead)
            R = (int)PhotonNetwork.LocalPlayer.CustomProperties["Reputation"] + r;

        // 限制血量不要超過上限或低於 0
        W = Mathf.Clamp(W, 0, maxW);
        S = Mathf.Clamp(S, 0, maxS);
        R = Mathf.Clamp(R, 0, maxR);

        // 暫時判定是否歸零
        bool tempWdead = (W == 0);
        bool tempSdead = (S == 0);
        bool tempRdead = (R == 0);

        // 如果滿足死亡條件 (包含30號的單數值歸零特判)
        if (isOver(tempWdead, tempSdead, tempRdead))
        {
            // 呼叫死亡攔截器！看閻羅王收不收！
            if (player.p.checkRevive(ref W, ref S, ref R))
            {
                // 閻王拒收，數值被復活技能修改了，重新更新死亡標記
                tempWdead = (W == 0);
                tempSdead = (S == 0);
                tempRdead = (R == 0);

                // 廣播給所有人看他復活了
                photonView.RPC("Announcement", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " 觸發了免死金牌！", 2500);
            }
            else
            {
                // 真的死了，發送死亡廣播
                over = true;
                photonView.RPC("imDead", RpcTarget.All, me);
            }
        }

        // 更新全域死亡標記
        Wdead = tempWdead;
        Sdead = tempSdead;
        Rdead = tempRdead;

        // 將最終正確的數值寫回網路屬性
        HashTable table = new HashTable();
        table.Add("Wisdom", W);
        table.Add("Strength", S);
        table.Add("Reputation", R);
        PhotonNetwork.LocalPlayer.SetCustomProperties(table);
    }
    public void setPlayerProperties(int w, int s, int r)
    {
        HashTable table = new HashTable();
        if (!Wdead)
        {
            table.Add("Wisdom", w);
        }
        if (!Sdead)
        {
            table.Add("Strength", s);
        }
        if (!Rdead)
        {
            table.Add("Reputation", r);
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(table);
    }
    // Update is called once per frame
    public void TargetSet(int k)
    {
        targetnum = k;
        PlayerPanelController targetPanel = PlayerPanels[k].GetComponent<PlayerPanelController>();
        while (targetPanel.isExist("disappear"))
        {
            ++targetnum;
            targetPanel = PlayerPanels[targetnum].GetComponent<PlayerPanelController>();
        }
        Photon.Realtime.Player target;
        target = LocalPlayerList[k];
        TMP_Text[] texts =  UIparent.GetComponent<Transform>().Find("Round").GetComponentsInChildren<TMP_Text>();
        foreach(TMP_Text t in texts)
        {
            if (t.name == "To")
            {
                t.text = target.NickName;
                break;
            }
        }
    }
    // 將 rndface 設為預設空字串 ("")。
    // 如果不傳第二個參數，就會「自動隨機抽」；如果傳了，就會「強制生出那張牌」！
    public void PickACard(double rnd, string designatedFace = "")
    {
        string tooltype = "";
        int typeIndex = 0;

        // 1. 根據 rnd 決定卡牌「類別」與對應的「陣列 Index」
        if (rnd < cardCumulativeProbability[0]) { tooltype = "attack"; typeIndex = 0; }
        else if (rnd < cardCumulativeProbability[1]) { tooltype = "multiattack"; typeIndex = 1; }
        else if (rnd < cardCumulativeProbability[2]) { tooltype = "defense"; typeIndex = 2; }
        else if (rnd < cardCumulativeProbability[3]) { tooltype = "medicine"; typeIndex = 3; }
        else if (rnd < cardCumulativeProbability[4]) { tooltype = "strengthen"; typeIndex = 4; }
        else { tooltype = "special"; typeIndex = 5; }

        // 2. 決定卡牌的 Face (檔名)
        string finalFace = designatedFace;

        // 如果沒有指定卡牌，就從我們剛剛讀好的字典裡「隨機抽一張」
        if (string.IsNullOrEmpty(finalFace))
        {
            if (cardPools.ContainsKey(tooltype) && cardPools[tooltype].Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, cardPools[tooltype].Count);
                finalFace = cardPools[tooltype][randomIndex];
            }
            else
            {
                Debug.LogWarning(tooltype + " 卡池是空的！請檢查 txt 檔。");
                finalFace = "ErrorCard"; // 防呆機制
            }
        }

        // 3. 生成實體卡牌並塞入對應的陣列中
        GameObject card = Instantiate(toolcardprfeb, cardpanel);
        CardsInType[typeIndex].Add(card);

        card.GetComponent<ToolCardController>().tooltype = tooltype;
        card.GetComponent<ToolCardController>().face = finalFace;
    }

    // public void DiscardACard(string type = "not_specified")
    // {
    //     int cardcount = 0;
    //     if(type == "not_specified")
    //     {
    //         for (int i = 0; i < typeNum; i++)
    //         {
    //             if (CardsInType[i].Count > 0)
    //             {
    //                 foreach (GameObject obj in CardsInType[i])
    //                 {
    //                     cardcount++;
    //                 }
    //             }
    //         }
    //         int pos = (int)(rand.NextDouble() * cardcount);
    //         cardcount = 0;
    //         for (int i = 0; i < typeNum; i++)
    //         {
    //             int j = 0;
    //             if (CardsInType[i].Count > 0)
    //             {
    //                 foreach (GameObject obj in CardsInType[i])
    //                 {
    //                     if (cardcount == pos)
    //                     {
    //                         break;
    //                     }
    //                     cardcount++;
    //                     j++;
    //                 }
    //                 CardsInType[i][j].GetComponent<ToolCardController>().kill();
    //                 CardsInType[i].RemoveAt(j);
    //                 RefreshCards();
    //                 return;
    //             }
    //         }
            
    //     }
    // }
    public void DiscardACard(string type = "not_specified", int n = -1)
    {
        if (type == "not_specified")
        {
            // 1. 先計算出總共有幾張手牌
            int totalCards = 0;
            for (int i = 0; i < typeNum; i++)
            {
                totalCards += CardsInType[i].Count;
            }

            // 如果已經沒有手牌了，就直接結束，防止隨機數報錯
            if (totalCards == 0) return; 

            // 2. 隨機選一個要丟掉的位置 (rand.Next 產生的範圍包含下限，不包含上限)
            int pos = rand.Next(0, totalCards); 
            
            // 3. 找出那張牌並徹底移除
            int currentCount = 0;
            for (int i = 0; i < typeNum; i++)
            {
                for (int j = 0; j < CardsInType[i].Count; j++)
                {
                    if (currentCount == pos)
                    {
                        // 找到了！先觸發卡牌本身的刪除特效/摧毀實體
                        CardsInType[i][j].GetComponent<ToolCardController>().kill();
                        
                        // 再從陣列名單中移除
                        CardsInType[i].RemoveAt(j);
                        
                        // 重新排列剩餘的手牌
                        RefreshCards();
                        
                        // 【關鍵】事情做完了，立刻結束整個函式，不要再往下找了！
                        return; 
                    }
                    currentCount++;
                }
            }
        }
    }
    public void RefreshCards()
    {
        int L = 0;
        int t = 0;
        foreach(var Cards in CardsInType)
        {
            int num = 0;
            if (Cards.Count > 0)
            {
                foreach (GameObject Card in Cards) 
                {
                    Card.GetComponent<ToolCardController>().num = 100*t + num;
                    Card.transform.localPosition = position[L];
                    L++;
                    num++;
                }
            }
            t++;
        }
    }
    public void RefreshDisplay()
    {
        if(status == 1)
        {
            int n = 1;

            float x = displayfrom.GetComponent<RectTransform>().sizeDelta.x / 2;
            float y = displayfrom.GetComponent<RectTransform>().sizeDelta.y;
            float inter = y / (displaycount + 1);
            Debug.Log(CardsInDisplay[0].Count);
            foreach (var Cards in CardsInDisplay)
            {
                if (Cards.Count > 0)
                {
                    foreach (GameObject Card in Cards)
                    {
                        Card.transform.localPosition = new Vector3(x, y - inter * n, 1);
                        n++;
                    }
                }
            }
        }
        else if(status == 2)
        {
            int n = 1;

            float x = displayto.GetComponent<RectTransform>().sizeDelta.x / 2;
            float y = displayto.GetComponent<RectTransform>().sizeDelta.y;
            float inter = y / (displaycount + 1);
            foreach (var Cards in CardsInDisplay)
            {
                if (Cards.Count > 0)
                {
                    foreach (GameObject Card in Cards)
                    {
                        Card.transform.localPosition = new Vector3(x, y - inter * n, 1);
                        n++;
                    }
                }
            }
        }
        
    }
    public bool isOver(bool w, bool s, bool r)
    {
        if (character == "30")
        {
            return (w || s || r);
        }
        int rst = 0;
        if (w) ++rst;
        if (s) ++rst;
        if (r) ++rst;
        if(rst >= 2) 
            return true;
        else
            return false;
    }
    public void UseSkill()
    {
        status = 0;
        if (targetnum == -1)
        {
            targetnum = (me + 1) % total;
        }
        if (displaycount > 0)
        {
            for (int y = 0; y < typeNum; y++)
            {
                if (CardsInDisplay[y].Count > 0)
                {
                    foreach (GameObject o in CardsInDisplay[y])
                    {
                        o.GetComponent<ToolDisplayController>().kill();
                    }
                    CardsInDisplay[y].Clear();
                }
            }
            displaycount = 0;
            RefreshCards();
        }
        if(character != "2" || skillUseCounter >= 2)
            cd = cdLength;
        ++skillUseCounter;
        player.useSkill();
    }

    public async Task endRoundEffectHandler(GameObject[] panelList, int playeridx, int dt)
    {
        PlayerPanelController myPanel = panelList[playeridx].GetComponent<PlayerPanelController>();
        if (myPanel.isExist("dizzy"))
        {
            photonView.RPC("Announcement", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " 斷片，遺忘了些什麼", dt);
            await Task.Delay(dt + 100);
            DiscardACard();
        }
        
        if (myPanel.isExist("malice"))
        {
            foreach(var eff in myPanel.effectlist)
            {
                if(eff.id == "malice" && eff.lastRound == 1)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("無名布偶怒了，引爆怨念");
                    sb.AppendLine(LocalPlayerList[me].NickName + "被惡意纏身");
                    sb.AppendLine("受到15點智慧傷害");
                    sb.AppendLine("受到15點體力傷害");
                    sb.Append("受到15點聲譽傷害");
                    photonView.RPC("Announcement", RpcTarget.All, sb.ToString(), dt);
                    UpdatePlayerProperties(-15, -15, -15);
                    await Task.Delay(dt + 100);
                    break;
                }
            }
        }
        if (myPanel.isExist("chenchienting"))
        {   
            float roll = UnityEngine.Random.value;
            if (roll <= 0.5f && playeridx == me)
            {
                isResolvingVirtualCard = true;
                roll = UnityEngine.Random.value;
                if (roll <= 0.4f)
                {
                    PlayVirtualCard("medicine", "lunch_break_chat", "chenchienting");
                }
                else if (roll <= 0.8f)
                {
                    PlayVirtualCard("special", "confiscate_smartphone_ipad", "chenchienting");
                }
                else
                {
                    // PlayVirtualCard("special", "go_biking_together", "chenchienting");
                    StringBuilder sb1 = new StringBuilder("今天天氣真好...\n明麟，我們去騎車吧！\n");
                    StringBuilder sb2 = new StringBuilder("任你選一個方向\n任你上一條通道\n");
                    StringBuilder sb3 = new StringBuilder("順著這帶草味的和風\n放輪遠去\n");
                    StringBuilder sb4 = new StringBuilder("保管你這半天的逍遙\n是你性靈的補劑\n");
                    StringBuilder sb5 = new StringBuilder("陳建廷 與 吳明麟 離開了戰場");

                    photonView.RPC("Announcement", RpcTarget.All, sb1.ToString(), 2500);
                    photonView.RPC("Announcement", RpcTarget.All, sb2.ToString(), 2500);
                    photonView.RPC("Announcement", RpcTarget.All, sb3.ToString(), 2500);
                    photonView.RPC("Announcement", RpcTarget.All, sb4.ToString(), 2500);
                    photonView.RPC("Announcement", RpcTarget.All, sb5.ToString(), 2500);

                    await Task.Delay(10000);

                    for (int i = 0; i < total; i++)
                    {
                        if (!isAlive[i]) continue;

                        PlayerPanelController ppc = PlayerPanels[i].GetComponent<PlayerPanelController>();
                        if (ppc.isExist("chenchienting"))
                        {
                            photonView.RPC("RemoveEffect", RpcTarget.All, i, "chenchienting");
                        }
                        if (ppc.isExist("wuminglin"))
                        {
                            photonView.RPC("RemoveEffect", RpcTarget.All, i, "wuminglin");
                        }
                    }

                    await Task.Delay(2500);
                    isResolvingVirtualCard = false;
                }
                while (isResolvingVirtualCard)
                {
                    await Task.Delay(100);
                }
            }
        }
        if (myPanel.isExist("wuminglin"))
        {
            float roll = UnityEngine.Random.value;
            if (roll <= 0.5f && playeridx == me)
            {
                isResolvingVirtualCard = true;
                roll = UnityEngine.Random.value;
                if (roll <= 0.8f)
                {
                    PlayVirtualCard("multiattack", "nietzsche", "wuminglin");
                }
                else
                {
                    // PlayVirtualCard("special", "go_biking_together", "wuminglin");
                    StringBuilder sb1 = new StringBuilder("今天天氣真好...\n建廷，我們去騎車吧！\n");
                    StringBuilder sb2 = new StringBuilder("任你選一個方向\n任你上一條通道\n");
                    StringBuilder sb3 = new StringBuilder("順著這帶草味的和風\n放輪遠去\n");
                    StringBuilder sb4 = new StringBuilder("保管你這半天的逍遙\n是你性靈的補劑\n");
                    StringBuilder sb5 = new StringBuilder("吳明麟 與 陳建廷 離開了戰場");

                    photonView.RPC("Announcement", RpcTarget.All, sb1.ToString(), 2500);
                    photonView.RPC("Announcement", RpcTarget.All, sb2.ToString(), 2500);
                    photonView.RPC("Announcement", RpcTarget.All, sb3.ToString(), 2500);
                    photonView.RPC("Announcement", RpcTarget.All, sb4.ToString(), 2500);
                    photonView.RPC("Announcement", RpcTarget.All, sb5.ToString(), 2500);

                    await Task.Delay(10000);

                    for (int i = 0; i < total; i++)
                    {
                        if (!isAlive[i]) continue;

                        PlayerPanelController ppc = PlayerPanels[i].GetComponent<PlayerPanelController>();
                        if (ppc.isExist("chenchienting"))
                        {
                            photonView.RPC("RemoveEffect", RpcTarget.All, i, "chenchienting");
                        }
                        if (ppc.isExist("wuminglin"))
                        {
                            photonView.RPC("RemoveEffect", RpcTarget.All, i, "wuminglin");
                        }
                    }

                    await Task.Delay(2500);
                    isResolvingVirtualCard = false;
                }
                while (isResolvingVirtualCard) 
                {
                    await Task.Delay(100);
                }
            }
        }
        if (myPanel.isExist("linminching"))
        {
            float roll = UnityEngine.Random.value;
            if (roll <= 0.5f && playeridx == me)
            {
                isResolvingVirtualCard = true;
                roll = UnityEngine.Random.value;
                if (roll <= 0.8f)
                {
                    List<int> validTargets = new List<int>();
                    for (int i = 0; i < total; i++)
                    {
                        if (i == me) continue;
                        if (!isAlive[i]) continue;
                        PlayerPanelController ppc = PlayerPanels[i].GetComponent<PlayerPanelController>();
                        if (ppc.isExist("disappear") || ppc.isExist("sleep")) continue;
                        validTargets.Add(i);
                    }
                    int targetIdx = me;
                    if (validTargets.Count > 0)
                    {
                        targetIdx = validTargets[UnityEngine.Random.Range(0, validTargets.Count)];
                    }
                    if (targetIdx != me)
                    {
                        photonView.RPC("RPC_LiteratureReview_Ask", RpcTarget.All, targetIdx, me);
                        await Task.Delay(3500);
                    }
                    else
                    {
                        photonView.RPC("Announcement", RpcTarget.All, "缺乏參考文獻...\n林敏靜 找不到目標進行 Literature Review", 2500);
                        await Task.Delay(2500);
                    }
                    isResolvingVirtualCard = false;
                }
                else
                {
                    PlayVirtualCard("special", "diplomacy", "linminching");
                }
                while (isResolvingVirtualCard) 
                {
                    await Task.Delay(100);
                }
            }
        }
        if (myPanel.isExist("chenpenghsu"))
        {
            float roll = UnityEngine.Random.value;
            if (roll <= 0.5f && playeridx == me)
            {
                isResolvingVirtualCard = true;
                roll = UnityEngine.Random.value;
                if (roll <= 0.5f)
                {
                    PlayVirtualCard("multiattack", "stand_at_the_back", "chenpenghsu");
                }
                else if (roll <= 0.8f)
                {
                    PlayVirtualCard("multiattack", "cosine_law", "chenpenghsu");
                }
                else
                {
                    // 同學上課不要閉眼睛喔
                    StringBuilder sb1 = new StringBuilder("同學上課不要閉眼睛喔\n");
                    StringBuilder sb2 = new StringBuilder("陳鵬旭 喚醒了所有沉睡的同學\n");

                    photonView.RPC("Announcement", RpcTarget.All, sb1.ToString(), 2500);
                    photonView.RPC("Announcement", RpcTarget.All, sb2.ToString(), 2500);

                    await Task.Delay(2500);

                    for (int i = 0; i < total; i++)
                    {
                        if (!isAlive[i]) continue;

                        PlayerPanelController ppc = PlayerPanels[i].GetComponent<PlayerPanelController>();
                        if (ppc.isExist("sleep"))
                        {
                            photonView.RPC("RemoveEffect", RpcTarget.All, i, "sleep");
                        }
                    }

                    await Task.Delay(2500);
                    isResolvingVirtualCard = false;
                }
                while (isResolvingVirtualCard) 
                {
                    await Task.Delay(100);
                }
            }
        }
        if (myPanel.isExist("chenchihsheng"))
        {
            float roll = UnityEngine.Random.value;
            if (roll <= 0.5f && playeridx == me)
            {
                isResolvingVirtualCard = true;
                roll = UnityEngine.Random.value;
                if (roll <= 0.5f)
                {
                    PlayVirtualCard("multiattack", "chalk_dust_hand", "chenchihsheng");
                }
                else if (roll <= 0.8f)
                {
                    PlayVirtualCard("multiattack", "circular_motion", "chenchihsheng");
                }
                else
                {
                    StringBuilder sb1 = new StringBuilder("大家的分數...\n");
                    StringBuilder sb2 = new StringBuilder("都是非零整數！\n");
                    StringBuilder sb3 = new StringBuilder("陳智勝 讓全場玩家\n為 0 的數值強行 +1\n");

                    photonView.RPC("Announcement", RpcTarget.All, sb1.ToString(), 2000);
                    photonView.RPC("Announcement", RpcTarget.All, sb2.ToString(), 2000);
                    photonView.RPC("Announcement", RpcTarget.All, sb3.ToString(), 2500);

                    await Task.Delay(4000);

                    photonView.RPC("RPC_ChenChihSheng_NonZero", RpcTarget.All);

                    await Task.Delay(2500);
                    isResolvingVirtualCard = false;
                }
                while (isResolvingVirtualCard) 
                {
                    await Task.Delay(100);
                }
            }
        }
        if (myPanel.isExist("loyinting"))
        {
            float roll = UnityEngine.Random.value;
            if (roll <= 0.5f && playeridx == me)
            {
                isResolvingVirtualCard = true;
                roll = UnityEngine.Random.value;
                if (roll <= 0.9f)
                {
                    PlayVirtualCard("medicine", "heyheyhey", "loyinting");
                }
                else
                {
                    PlayVirtualCard("multiattack", "dissection", "loyinting");
                }
                while (isResolvingVirtualCard) 
                {
                    await Task.Delay(100);
                }
            }
        }
        if (myPanel.isExist("wangchinghua"))
        {
            float roll = UnityEngine.Random.value;
            if (roll <= 0.5f && playeridx == me)
            {
                isResolvingVirtualCard = true;
                roll = UnityEngine.Random.value;
                if (roll <= 0.8f)
                {
                    PlayVirtualCard("medicine", "geological_age", "wangchinghua");
                }
                else
                {
                    // 庭光今天很開心哦
                    StringBuilder sb1 = new StringBuilder("庭光今天很開心哦\n");
                    StringBuilder sb2 = new StringBuilder("王靖華 使所有神隱同學現身\n");

                    photonView.RPC("Announcement", RpcTarget.All, sb1.ToString(), 2500);
                    photonView.RPC("Announcement", RpcTarget.All, sb2.ToString(), 2500);

                    await Task.Delay(2500);

                    for (int i = 0; i < total; i++)
                    {
                        if (!isAlive[i]) continue;

                        PlayerPanelController ppc = PlayerPanels[i].GetComponent<PlayerPanelController>();
                        if (ppc.isExist("disappear"))
                        {
                            photonView.RPC("RemoveEffect", RpcTarget.All, i, "disappear");
                        }
                    }

                    await Task.Delay(2500);
                    isResolvingVirtualCard = false;
                }
                while (isResolvingVirtualCard) 
                {
                    await Task.Delay(100);
                }
            }
        }
    }
    void isGameEnded(int nextIdx)
    {
        string win = "";
        if(isAlive.Length > 1)
        {
            for(int i = 0; i < isAlive.Length; ++i)
            {
                if (isAlive[i])
                {
                    if(win != "")
                    {
                        photonView.RPC("Go", LocalPlayerList[nextIdx]);
                        return;
                    }
                    else
                    {
                        win = LocalPlayerList[i].NickName;
                    }
                }
            }
            photonView.RPC("EndGame", RpcTarget.All, win);
        }
        else
        {
            if (isAlive[0])
            {
                photonView.RPC("Go", LocalPlayerList[nextIdx]);
            }
            else
            {
                photonView.RPC("EndGame", RpcTarget.All, win);
            }
        }
    }
    public void switchPlayDiscard()
    {
        PD = !PD;
        for (int y = 0; y < typeNum; y++)
        {
            if (CardsInDisplay[y].Count > 0)
            {
                foreach (GameObject o in CardsInDisplay[y])
                {
                    o.GetComponent<ToolDisplayController>().kill();
                }
                CardsInDisplay[y].Clear();
            }
        }
        displaycount = 0;
        RefreshCards();
    }
    void Update()
    {
        if (isAlive[me])
        {
            switch (status)
            {
                case 0:
                    hintword.text = "還不是你的回合";
                    skillButton.enabled = false;
                    PDSwitch.enabled = false;
                    break;
                case 1:
                    hintword.text = "請出牌";
                    PDSwitch.enabled = true;
                    switch (character)
                    {
                        case "2":
                            if (skillUseCounter < 2 && cd == 0)
                            {
                                skillButton.enabled = true;
                            }
                            else
                            {
                                skillButton.enabled = false;
                            }
                            break;
                        case "15": // 劭宇：只能在被攻擊時使用
                        case "24": // 宥璿：只能在被攻擊時使用
                        case "30": // 昱全：沒有主動技能
                            skillButton.enabled = false;
                            break;
                        default:
                            if (cd == 0)
                            {
                                skillButton.enabled = true;
                            }
                            else
                            {
                                skillButton.enabled = false;
                            }
                            break;
                    }
                    break;
                case 2:
                    hintword.text = "請回應";
                    PDSwitch.enabled = false;
                    switch (character)
                    {
                        case "15": // 劭宇的社交蝴蝶
                            if (cd == 0 && !multi)
                            {
                                skillButton.enabled = true;
                            }
                            else
                            {
                                skillButton.enabled = false;
                            }
                            break;
                        case "24": // 宥璿的一代傳奇
                            if (cd == 0 && canPlayDefense)
                            {
                                skillButton.enabled = true;
                            }
                            else
                            {
                                skillButton.enabled = false;
                            }
                            break;
                        default: // 其他所有人在這階段都不能按技能
                            skillButton.enabled = false;
                            break;
                    }
                    break;
            }
            if (PD)
            {
                PDtext.text = "出牌";
            }
            else
            {
                PDtext.text = "棄牌";
            }
        }
        else
        {
            hintword.text = "您(社)死了";
            skillButton.enabled = false;
            PDSwitch.enabled = false;
        }
        cdText.text = "CD: " + cd;
    }
}
