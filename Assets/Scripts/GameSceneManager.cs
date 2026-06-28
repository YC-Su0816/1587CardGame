using ExitGames.Client.Photon;
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
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;
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
    public string toolcardtype;
    public Vector3[] position = new Vector3[20];
    public TMP_Text hintword, to, from, hint;
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
    public Button skillButton;
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
    void EndGame(string winner = "")
    {
        StaticData.winnerName = winner;
        SceneManager.LoadScene("GameScene");
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
        // 【關鍵修復】：只有在「第一次」反彈時，才記錄原始攻擊者與群攻下家！
        // 如果已經在反彈狀態中（連續反彈），絕對不覆寫這兩個記憶變數！
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
        HintPanel.SetActive(false);
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
        HintPanel.SetActive(false);
        displaytime = false;
    }
    [PunRPC]
    void Go()
    {
        if (over || !pickable) return;

        // 【新增攔截邏輯】：檢查自己當前有沒有被限制行動的效果
        PlayerPanelController myPanel = PlayerPanels[me].GetComponent<PlayerPanelController>();
        if (myPanel.isExist("disappear"))
        {
            System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());

            if (rand.NextDouble() <= 0.5f)
            {
                photonView.RPC("Announcement", RpcTarget.All, " 噢！"+ PhotonNetwork.LocalPlayer.NickName + "回來了！", 1500);
                photonView.RPC("RemoveEffect", RpcTarget.All, me, "disappear");
            }
            else
            {
                photonView.RPC("Announcement", RpcTarget.All, "不行，聯絡不上"+PhotonNetwork.LocalPlayer.NickName + "...", 1500);

                UpdatePlayerProperties(3, 3, 3);

                status = 0;
                player.endRound();

                Task.Delay(1500).ContinueWith(t => photonView.RPC("Go", LocalPlayerList[(me + 1) % total]));
                return;
            }
        }
        if (myPanel.isExist("sleep")) 
        {
            photonView.RPC("Announcement", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " 睡死了...", 1500);
            UpdatePlayerProperties(-1, 0, -1);
            photonView.RPC("UpdateEffect", RpcTarget.All, me);

            status = 0;
            player.endRound();
            photonView.RPC("Go", LocalPlayerList[(me + 1) % total]);
            return;
        }
        if (myPanel.isExist("dizzy")) // 假設有暈眩或結冰
        {
            photonView.RPC("Announcement", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " 斷片，遺忘了些什麼", 1500);
            DiscardACard();
            photonView.RPC("UpdateEffect", RpcTarget.All, me);
        }
        cd = Mathf.Max(0, cd - 1);
        displaytime = false;
        photonView.RPC("SetFromTo", RpcTarget.All, me, -1);
        PhotonNetwork.SendAllOutgoingCommands();
        status = 1;
        targetnum = -1;
        if (PlayerPanels != null) photonView.RPC("UpdateEffect", RpcTarget.All, me);
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
                canPlayDefense = true;
                canPlaySpecial = false;
            }
        }
        else if (DisplayType[0] == "unblockable")
        {
            // 【不可防禦路線】：直接顯示文字、強制扣血、結束回合！
            await Task.Delay(2000);
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine(FromAndTo[0].NickName + " 對 " + FromAndTo[1].NickName);
            //sb.AppendLine("發動了無法防禦的技能！");
            //HintPanel.SetActive(true);
            //await Task.Delay(2000);
            //HintPanel.SetActive(false);

            if (FromAndTo[1] == PhotonNetwork.LocalPlayer)
            {
                status = 2;
                canPlayDefense = true;
                canPlaySpecial = false;
            }
            //if (wisdomDamage < 0) sb.AppendLine("造成 " + (-wisdomDamage).ToString() + " 點智力損害");
            //if (strengthDamage < 0) sb.AppendLine("造成 " + (-strengthDamage).ToString() + " 點體力消耗");
            //if (reputationDamage < 0) sb.AppendLine("誹謗 " + (-reputationDamage).ToString() + " 點聲譽");

            //hint.text = sb.ToString();
            //HintPanel.SetActive(true);

            //// 受害者直接強行扣血
            //if (FromAndTo[1] == PhotonNetwork.LocalPlayer)
            //{
            //    UpdatePlayerProperties(wisdomDamage, strengthDamage, reputationDamage);
            //}

            //await Task.Delay(3000);

            // 發動者直接結束回合並交棒
            //if (FromAndTo[0] == PhotonNetwork.LocalPlayer)
            //{
            //    status = 0;
            //    player.endRound();
            //    photonView.RPC("Cleaning", RpcTarget.All);
            //    PhotonNetwork.SendAllOutgoingCommands();
            //    photonView.RPC("Go", LocalPlayerList[(me + 1) % total]);
            //    PhotonNetwork.SendAllOutgoingCommands();
            //}

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
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine(FromAndTo[0].NickName + " 對 " + FromAndTo[1].NickName);

            //if (wisdomDamage <= 0)
            //{
            //    sb.AppendLine("造成 " + (-wisdomDamage).ToString() + " 點智力損害");
            //}
            //else
            //{
            //    sb.AppendLine("回復 " + wisdomDamage.ToString() + " 點智力");
            //}

            //if (strengthDamage <= 0)
            //{
            //    sb.AppendLine("造成 " + (-strengthDamage).ToString() + " 點體力消耗");
            //}
            //else
            //{
            //    sb.AppendLine("回復 " + strengthDamage.ToString() + " 點體力");
            //}

            //if (reputationDamage <= 0)
            //{
            //    sb.Append("誹謗 " + (-reputationDamage).ToString() + " 點聲譽");
            //}
            //else
            //{
            //    sb.Append("挽回 " + reputationDamage.ToString() + " 點聲譽");
            //}
            //hint.text = sb.ToString();
            //HintPanel.SetActive(true);
            //if (FromAndTo[1] == PhotonNetwork.LocalPlayer)
            //{
            //    UpdatePlayerProperties(wisdomDamage, strengthDamage, reputationDamage);
            //}

            //if (FromAndTo[0] == PhotonNetwork.LocalPlayer)
            //{
            //    await Task.Delay(3000);
            //    status = 0;
            //    player.endRound();
            //    photonView.RPC("Cleaning", RpcTarget.All);
            //    PhotonNetwork.SendAllOutgoingCommands();
            //    photonView.RPC("Go", LocalPlayerList[(me + 1) % total]);
            //    PhotonNetwork.SendAllOutgoingCommands();
            //}
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
        daW = 0;
        daS = 0;
        daR = 0;
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
                photonView.RPC("Responded", RpcTarget.All, 0, 0, 0); // 自動安全下莊
                return;
            }

            // 【升級】：2. 被動閃避檢測 (取代原本寫死的 11 號)
            if (player.p.checkPassiveDodge())
            {
                photonView.RPC("Announcement", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " 躲過了攻擊！", 1500);
                PhotonNetwork.SendAllOutgoingCommands();
                photonView.RPC("Responded", RpcTarget.All, 0, 0, 0);
                return;
            }
            status = 2;
            canPlayDefense = true;
            canPlaySpecial = false;
            daW = wisdomDamage;
            daS = strengthDamage;
            daR = reputationDamage;
        }
    }

    [PunRPC]
    async Task Responded(int w, int s, int r)
    {
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
                tdc.init(w, s, r);
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

        // 2. 【確定性隨機】：利用雙方ID與傷害值當作種子，確保全網算出一樣的機率，解決平行時空！
        

        // ==========================================
        // 情況 B：正常傷害結算 (包含被 2號 反擊，或一般反彈命中)
        // ==========================================
        

        bool wasReflect = reflect; // 記下這次是否為反彈
        reflect = false; // 結算前關閉狀態，避免干擾後續群攻鏈
        if (k > temp && DisplayInRally[temp].GetComponent<ToolDisplayController>().face == "all_in_vain")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("算了，就這樣吧...");
            sb.AppendLine("卷不贏XXX...");
            sb.AppendLine("遁隱虛空，跳出三界");
            sb.Append(FromAndTo[1].NickName + "使所有失效");
            
            hint.text = sb.ToString();
            HintPanel.SetActive(true);
            await Task.Delay(3000);
            HintPanel.SetActive(false);

            
        }
        else
        {
            int toW, toS, toR;
            if (daW > 0) toW = daW + Mathf.Min(0, w);
            else toW = Mathf.Min(0, daW + w);
            if (daS > 0) toS = daS + +Mathf.Min(0, s);
            else toS = Mathf.Min(0, daS + s);
            if (daR > 0) toR = daR + +Mathf.Min(0, r);
            else toR = Mathf.Min(0, daR + r);
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

                hint.text = sb.ToString();
                HintPanel.SetActive(true);

                // 【真正的防禦者】扣血
                

                await Task.Delay(3000);
                if (PhotonNetwork.LocalPlayer == FromAndTo[1])
                {
                    UpdatePlayerProperties(toW, toS, toR);
                    if (toW < 0 || toS < 0 || toR < 0)
                    {
                        player.p.onTakeDamage(toW, toS, toR);
                    }
                }
                HintPanel.SetActive(false);
            }
            else
            {
                if (DisplayFace[0] == "king")
                {
                    sb.AppendLine("我進過四次NTUEE");
                    sb.AppendLine("而你...");
                    sb.AppendLine("又進過幾次台大");
                    sb.Append(FromAndTo[1].NickName + "展現王的風範");

                    hint.text = sb.ToString();
                    HintPanel.SetActive(true);
                    await Task.Delay(3000);
                    HintPanel.SetActive(false);
                }
                else
                {
                    sb.AppendLine("不知道要填什麼");
                    sb.AppendLine("再說吧");
                    sb.Append(FromAndTo[1].NickName + "Error! not...");

                    hint.text = sb.ToString();
                    HintPanel.SetActive(true);
                    await Task.Delay(3000);
                    HintPanel.SetActive(false);
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
                    photonView.RPC("GetCard", RpcTarget.All, "unblockable", "self_defense");
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
                        hint.text = "對上眼了！";
                        HintPanel.SetActive(true);
                        await Task.Delay(2000);
                        HintPanel.SetActive(false);
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
                                photonView.RPC("Go", LocalPlayerList[me]);
                            }
                            else
                            {
                                player.endRound();
                                photonView.RPC("Go", LocalPlayerList[(me + 1) % total]);
                            }
                        }
                        else
                        {
                            player.endRound();
                            photonView.RPC("Go", LocalPlayerList[(me + 1) % total]);
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
                            photonView.RPC("Go", LocalPlayerList[me]);
                        }
                        else
                        {
                            player.endRound();
                            photonView.RPC("Go", LocalPlayerList[(me + 1) % total]);
                        }
                    }
                    else
                    {
                        player.endRound();
                        photonView.RPC("Go", LocalPlayerList[(me + 1) % total]);
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
                        photonView.RPC("Go", LocalPlayerList[(me + 1) % total]);
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
    [PunRPC]
    async Task Announcement(string content, int time)
    {
        hint.text = content;
        HintPanel.SetActive(true);
        await Task.Delay(time);
        HintPanel.SetActive(false);   
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

        for(int x = 0; x < 2; ++x)
        {
            PickACard(0.67f, "test_attack");
            PickACard(1.67f, "test_multiattack");
            PickACard(2.67f, "test_defense");
            PickACard(3.67f, "test_medicine");
            PickACard(4.67f, "test_strengthen");
            PickACard(5.67f, "test_special");
        }
        PickACard(5.67f, "all_in_vain");
        for (int x = 0; x < 3; ++x)
        {
            float rnd = (float)(6*rand.NextDouble());
            PickACard(rnd);
        }
        //for (int xxxxx = 1; xxxxx <= 5; xxxxx++)
        //{
        //    PickACard(0.5f, xxxxx.ToString());
        //}
        //PickACard(2.5f, "2");
        //for (float r = 1f; r <= 5f; r++)
        //{
        //    for (int xxxxx = 0; xxxxx < 2; xxxxx++)
        //    {
        //        PickACard(0.5f + r, "1");
        //    }
        //}
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

        string playedType = "none"; 
        string playedFace = "none";
        int playedCount = 0;        

        if (displaycount > 0)
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
                PickACard(0.5f, "1");
                RefreshCards();
                displaycount = 0;
                toolcardtype = "none";
                cardpicking = -1;
                photonView.RPC("Go", LocalPlayerList[(me + 1) % total]);
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
                        if(playedCount == 0) playedType = obj.GetComponent<ToolDisplayController>().tooltype;
                        if (playedType == "special") playedFace = obj.GetComponent<ToolDisplayController>().face;
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


            if (playedType == "special")
            {
                photonView.RPC("Played", RpcTarget.All, 0, 0, 0);
                PhotonNetwork.SendAllOutgoingCommands();
                //For special effect
                if (playedFace == "magic_mirror")
                {
                    // 在這裡寫魔鏡的效果
                    Debug.Log("發動了魔鏡！");
                }
                else if (playedFace == "time_stop")
                {
                    // 在這裡寫時間停止的效果
                }
            }
            else if (playedType == "medicine")
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
                    float rnd = (float)(6 * rand.NextDouble());
                    PickACard(rnd);
                }
            }
            RefreshCards();
            toolcardtype = "none";
            cardpicking = -1;
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
                    float rnd = (float)(6 * rand.NextDouble());
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
            photonView.RPC("Go", LocalPlayerList[(me + 1) % total]);
        }
        displaycount = 0;
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
        float rnd = (float)(6 * rand.NextDouble());
        PickACard(rnd);
        RefreshCards();
    }
    
    public void RespondCard()
    {
        status = 0;
        int deW = 0, deS = 0, deR = 0, toW, toR, toS;
        int[] Defends = new int[3];
        bool isReflecting = false;
        List<string> playedSpecialFaces = new List<string>();
        if (displaycount > 0)
        {
            // 第一步：先掃描檢查展示區有沒有包含反彈卡 (face == "2")
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
        if (playedSpecialFaces.Count > 0)
        {
            foreach (string spFace in playedSpecialFaces)
            {
                if (spFace == "all_in_vain")
                {
                    photonView.RPC("Responded", RpcTarget.All, -daW, -daS, -daR);
                    PhotonNetwork.SendAllOutgoingCommands();
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

        RefreshCards();
        toolcardtype = "none";
        cardpicking = -1;
        displaycount = 0; // 確保重置計數
    }
    public void UpdatePlayerProperties(int w, int s, int r)
    {
        
        int W = (int)PhotonNetwork.LocalPlayer.CustomProperties["Wisdom"] + w;
        int S = (int)PhotonNetwork.LocalPlayer.CustomProperties["Strength"] + s;
        int R = (int)PhotonNetwork.LocalPlayer.CustomProperties["Reputation"] + r;

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
    public void PickACard(float rnd, string designatedFace = "")
    {
        string tooltype = "";
        int typeIndex = 0;

        // 1. 根據 rnd 決定卡牌「類別」與對應的「陣列 Index」
        if (rnd < 1) { tooltype = "attack"; typeIndex = 0; }
        else if (rnd < 2) { tooltype = "multiattack"; typeIndex = 1; }
        else if (rnd < 3) { tooltype = "defense"; typeIndex = 2; }
        else if (rnd < 4) { tooltype = "medicine"; typeIndex = 3; }
        else if (rnd < 5) { tooltype = "strengthen"; typeIndex = 4; }
        else { tooltype = "special"; typeIndex = 5; } // rnd < 6

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

    public void DiscardACard(string type = "not_specified")
    {
        int cardcount = 0;
        if(type != "not_specified")
        {
            for (int i = 0; i < typeNum; i++)
            {
                if (CardsInDisplay[i].Count > 0)
                {
                    foreach (GameObject obj in CardsInDisplay[i])
                    {
                        cardcount++;
                    }
                }
            }
            int pos = (int)(rand.NextDouble() * cardcount);
            cardcount = 0;
            for (int i = 0; i < typeNum; i++)
            {
                int j = 0;
                if (CardsInDisplay[i].Count > 0)
                {
                    foreach (GameObject obj in CardsInDisplay[i])
                    {
                        if (cardcount == pos)
                        {
                            break;
                        }
                        cardcount++;
                        j++;
                    }
                    CardsInType[i][j].GetComponent<ToolCardController>().kill();
                    CardsInType[i].Remove(CardsInType[i][j]);
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
        if(character != "2" || skillUseCounter >= 2)
            cd = cdLength;
        ++skillUseCounter;
        player.useSkill();
    }
    void Update()
    {
        switch (status)
        {
            case 0:
                hintword.text = "還不是你的回合";
                skillButton.enabled = false;
                break;
            case 1:
                hintword.text = "請出牌";
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
            //case 3: // 【新增】：動畫與網路結算專用的無敵鎖定狀態
            //    hintword.text = "動畫結算中...";
            //    skillButton.enabled = false;
            //    break;
        }

        }
    }
