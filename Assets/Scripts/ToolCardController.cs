using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ToolCardController : MonoBehaviour, IPointerClickHandler
{
    public int num;
    public string face, tooltype;
    public GameObject discription;
    public Transform canva;
    public GameObject previewPrefeb;

    // 將原本散落的 Find 和 GetComponent 集中快取，減少效能浪費
    private GameSceneManager gameManager;
    private Transform displayfrom, displayto;

    // 建立卡牌類型與陣列 Index 的對應表 (類似 Hash Map / Lookup Table)
    private readonly Dictionary<string, int> typeToIndex = new Dictionary<string, int>
    {
        { "attack", 0 },
        { "multiattack", 1 },
        { "defense", 2 },
        { "medicine", 3 },
        { "strengthen", 4 },
        { "special", 5 }
    };

    void Start()
    {
        // 只在初始化時找一次並存成參考 (Reference)
        canva = GameObject.Find("Canvas").GetComponent<Transform>();;
        gameManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();

        // 善用 transform.Find 可以直接找子物件，不需動用全域尋找
        Transform roundTransform = canva.transform.Find("Round");
        displayfrom = roundTransform.Find("FromBoard");
        displayto = roundTransform.Find("ToBoard");

        GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("image/Tool/" + tooltype + "/" + face);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int sta = gameManager.status;

        if (sta == 1) // 1: 出牌階段
        {
            if (tooltype == "strengthen")
                HandleStrengthen(displayfrom);
            else if (tooltype == "attack" || tooltype == "multiattack" || tooltype == "medicine")
                HandleStandardCard(typeToIndex[tooltype], displayfrom);
            else if (tooltype == "special")
            {
                // 【權限判定】：確認這張特別卡允許在「自己的回合」出牌
                if (gameManager.specialCardDict.ContainsKey(face) && gameManager.specialCardDict[face].canPlayOnTurn)
                {
                    if(face == "femboy1" || face == "femboy2")
                    {
                        HandleFemboyCard(face, displayfrom);
                    }
                    else
                    {
                        HandleStandardCard(typeToIndex[tooltype], displayfrom);
                    }
                    
                }
                else
                {
                    Debug.Log("這張特殊牌不能在主動回合打出！");
                }
            }
            gameManager.RefreshDisplay();
        }
        else if (sta == 2) // 2: 回應階段
        {
            if (gameManager.canPlayDefense && tooltype == "defense")
                HandleDefenseCard(displayto);
            else if (face == "all_in_vain" || (gameManager.canPlaySpecial && tooltype == "special"))
            {
                // 【權限判定】：確認這張特別卡允許在「被攻擊/作用時」出牌
                if (gameManager.specialCardDict.ContainsKey(face) && gameManager.specialCardDict[face].canPlayOnAttacked)
                {
                    HandleStandardCard(typeToIndex[tooltype], displayto);
                }
                else
                {
                    Debug.Log("這張特殊牌不能當作防禦/回應打出！");
                }
            }
            gameManager.RefreshDisplay();
        }
        else
        {
            GameObject detail = Instantiate(previewPrefeb, canva);
            ToolPreviewHandler TPH = detail.GetComponent<ToolPreviewHandler>();
            TPH.img.sprite = Resources.Load<Sprite>("image/Tool/" + tooltype + "/" + face);
            TextAsset det = Resources.Load<TextAsset>("text/Tool/" + tooltype + "/" + face);
            string toolname = "", discription = "", tooleff = "", toolType = "";
            int w, s, r;
            string[] inf;
            if (det != null)
            {
                inf = det.text.Split("\n");
                for (int i = 0; i < inf.Length; i++)
                {
                    inf[i] = inf[i].Trim();
                }
                if(tooltype == "special")
                {
                    toolname = inf[0];
                    discription = inf[1];
                    tooleff = inf[2];
                    toolType = "特殊";
                }
                else
                {
                    int.TryParse(inf[0], out w);
                    int.TryParse(inf[1], out s);
                    int.TryParse(inf[2], out r);
                    toolname = inf[4];
                    discription = inf[5];
                    switch (tooltype)
                    {
                        case "attack":
                            if (w < 0) tooleff += "傷智" + (-w) + " ";
                            else if (w > 0) tooleff += "治智" + w + " ";

                            if (s < 0) tooleff += "傷體" + (-s) + " ";
                            else if (s > 0) tooleff += "治體" + s + " ";

                            if (r < 0) tooleff += "傷譽" + (-r) + " ";
                            else if (r > 0) tooleff += "治譽" + r + " ";

                            tooleff = tooleff.Trim();
                            if (string.IsNullOrEmpty(tooleff)) tooleff = "無效果";
                            toolType = "攻擊";
                            break;
                        case "medicine":
                            if (w < 0) tooleff += "傷智" + (-w) + " ";
                            else if (w > 0) tooleff += "治智" + w + " ";

                            if (s < 0) tooleff += "傷體" + (-s) + " ";
                            else if (s > 0) tooleff += "治體" + s + " ";

                            if (r < 0) tooleff += "傷譽" + (-r) + " ";
                            else if (r > 0) tooleff += "治譽" + r + " ";

                            tooleff = tooleff.Trim();
                            if (string.IsNullOrEmpty(tooleff)) tooleff = "無效果";
                            toolType = "治療";
                            break;
                        case "multiattack":
                            if (w < 0) tooleff += "傷智" + (-w) + " ";
                            else if (w > 0) tooleff += "治智" + w + " ";

                            if (s < 0) tooleff += "傷體" + (-s) + " ";
                            else if (s > 0) tooleff += "治體" + s + " ";

                            if (r < 0) tooleff += "傷譽" + (-r) + " ";
                            else if (r > 0) tooleff += "治譽" + r + " ";

                            tooleff = tooleff.Trim();
                            if (string.IsNullOrEmpty(tooleff)) tooleff = "無效果";
                            toolType = "群攻";
                            break;
                        case "defense":
                            
                            if (w < 0) tooleff += "耗智" + (-w) + " ";
                            else if (w > 0) tooleff += "智防" + w + " ";

                            if (s < 0) tooleff += "耗體" + (-s) + " ";
                            else if (s > 0) tooleff += "體防" + s + " ";

                            if (r < 0) tooleff += "耗譽" + (-r) + " ";
                            else if (r > 0) tooleff += "譽防" + r + " ";
                            tooleff = tooleff.Trim();
                            if (string.IsNullOrEmpty(tooleff)) tooleff = "無效果";
                            toolType = "防禦";
                            break;
                        case "strengthen":

                            if (w < 0) tooleff += "增智傷" + (-w) + " ";
                            else if (w > 0) tooleff += "減智傷" + w + " ";

                            if (s < 0) tooleff += "增體傷" + (-s) + " ";
                            else if (s > 0) tooleff += "減體傷" + s + " ";

                            if (r < 0) tooleff += "增譽傷" + (-r) + " ";
                            else if (r > 0) tooleff += "減譽傷" + r + " ";

                            if (string.IsNullOrEmpty(tooleff)) tooleff = "無效果";
                            toolType = "增傷";
                            break;
                    }
                }
            }
            TPH.cardName.text = name;
            TPH.type.text = toolType;
            TPH.desc.text = discription;
            TPH.eff.text = tooleff;
        }
    }

    // 處理一般會「清空全場」的卡牌 (攻擊、補血、效果等)
    private void HandleStandardCard(int targetIndex, Transform targetDisplay)
    {
        List<GameObject> currentDisplay = gameManager.CardsInDisplay[targetIndex];

        if (gameManager.displaycount > 0 && currentDisplay.Count > 0 && currentDisplay[0].GetComponent<ToolDisplayController>().num == num)
        {
            ClearAllDisplays();
            gameManager.displaycount = 0;
        }
        else
        {
            ClearAllDisplays();
            AddCardToDisplay(targetIndex, targetDisplay);
            gameManager.displaycount = 1;
        }
    }
    private void HandleFemboyCard(string cardFace, Transform targetDisplay)
    {
        List<GameObject>[] CID = gameManager.CardsInDisplay;
        List<GameObject> currentDisplay = CID[5];
        int existingIndexThis = currentDisplay.FindIndex(card => card.GetComponent<ToolDisplayController>().num == num);
        int existingIndexSelf = currentDisplay.FindIndex(card => card.GetComponent<ToolDisplayController>().face == cardFace);
        int existingIndexCo = currentDisplay.FindIndex(card => card.GetComponent<ToolDisplayController>().face == ((cardFace == "femboy1")? "femboy2" : "femboy1"));

        if (existingIndexSelf >= 0)
        {
            currentDisplay[existingIndexSelf].GetComponent<ToolDisplayController>().kill();
            currentDisplay.RemoveAt(existingIndexSelf);
            if(existingIndexThis < 0)
            {
                AddCardToDisplay(5, targetDisplay);
            }
            else
            {
                gameManager.displaycount--;
            }
        }
        else if(existingIndexCo < 0)
        {
            HandleStandardCard(5, targetDisplay);
        }
        else
        {
            AddCardToDisplay(5, targetDisplay);
            gameManager.displaycount++;
        }
    }
    private void HandleToggleCard(int targetIndex, Transform targetDisplay)
    {
        List<GameObject> currentDisplay = gameManager.CardsInDisplay[targetIndex];

        // 尋找這張牌是否已經在陣列中
        int existingIndex = currentDisplay.FindIndex(card => card.GetComponent<ToolDisplayController>().num == num);

        if (existingIndex >= 0)
        {
            // 如果已經在裡面，就把它砍掉 (取消選取)
            currentDisplay[existingIndex].GetComponent<ToolDisplayController>().kill();
            currentDisplay.RemoveAt(existingIndex);
            gameManager.displaycount--;
        }
        else
        {
            // 如果不在裡面，就加進去
            AddCardToDisplay(targetIndex, targetDisplay);
            gameManager.displaycount++;
        }
    }
    private void HandleDefenseCard(Transform targetDisplay)
    {
        for(int i = 0; i < 6; ++i)
        {
            if (i == 2) continue;
            if(gameManager.CardsInDisplay[i].Count != 0)
            {
                
                HandleStandardCard(typeToIndex["defense"], targetDisplay);
                return;
            }
        }
        HandleToggleCard(typeToIndex["defense"], targetDisplay);
    }
    // 強化卡的邏輯其實跟防禦卡一樣是疊加的，只是多了一個前提條件
    private void HandleStrengthen(Transform targetDisplay)
    {
        // 必須先有普通攻擊卡 (Index 0) 才能打強化卡
        if (gameManager.CardsInDisplay[0].Count != 0)
        {
            HandleToggleCard(typeToIndex["strengthen"], targetDisplay);
        }
    }

    // 負責清空所有展示區的迴圈
    private void ClearAllDisplays()
    {
        for (int y = 0; y < 6; y++)
        {
            if (gameManager.CardsInDisplay[y].Count > 0)
            {
                foreach (GameObject o in gameManager.CardsInDisplay[y])
                {
                    o.GetComponent<ToolDisplayController>().kill();
                }
                gameManager.CardsInDisplay[y].Clear();
            }
        }
    }

    // 負責生成實體並加入展示區
    // 負責生成實體並加入展示區
    private void AddCardToDisplay(int targetIndex, Transform targetDisplay)
    {
        GameObject detail = Instantiate(discription, targetDisplay);
        gameManager.CardsInDisplay[targetIndex].Add(detail);

        ToolDisplayController tdc = detail.GetComponent<ToolDisplayController>();

        // 【關鍵修改】：必須「先」給定 tooltype 和 face，才能呼叫 init！
        tdc.tooltype = tooltype;
        tdc.num = num;
        tdc.face = face;
        tdc.forDisplay = false;
        TextAsset det = Resources.Load<TextAsset>("text/Tool/" + tooltype + "/" + face);

        // 【關鍵修改】：根據卡牌種類，給予準備區卡牌對應的基礎展示數值
        // (因為準備階段還沒加上角色倍率，可以先給基礎值或0)
        if (tooltype != "special")
        {
            int w = -5, s = 0, r = 0;
            if (det != null)
            {
                string[] inf = det.text.Split("\n");
                for (int i = 0; i < inf.Length; i++)
                {
                    inf[i] = inf[i].Trim();
                }
                int.TryParse(inf[0], out w);
                int.TryParse(inf[1], out s);
                int.TryParse(inf[2], out r);
            }
            tdc.init(w, s, r);
        }
        else
            tdc.init(0, 0, 0);
    }

    public void kill()
    {
        Destroy(gameObject);
    }
}