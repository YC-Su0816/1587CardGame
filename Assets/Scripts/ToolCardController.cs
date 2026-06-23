//using Photon.Pun;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;
//using UnityEngine.UIElements;
//using UnityEngine.XR;

//public class ToolCardController : MonoBehaviour, IPointerClickHandler
//{
//    GameObject obj;

//    public int num;
//    public string face, tooltype;
//    public GameObject discription, manager;
//    public Transform displayfrom, displayto;

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        GameObject canva = GameObject.Find("Canvas");
//        manager = GameObject.Find("GameSceneManager");
//        displayfrom = canva.GetComponent<Transform>().Find("Round").GetComponent<Transform>().Find("FromBoard");
//        displayto = canva.GetComponent<Transform>().Find("Round").GetComponent<Transform>().Find("ToBoard");
//        obj = gameObject;
//        obj.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Tool/"+ tooltype + "/" + face);

//    }

//    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
//    {
//        int sta = manager.GetComponent<GameSceneManager>().status;

//        if (sta == 1)
//        {
//            if (tooltype == "attack")
//            {
//                if (manager.GetComponent<GameSceneManager>().displaycount > 0)
//                {
//                    List<GameObject> L = manager.GetComponent<GameSceneManager>().CardsInDisplay[0];

//                    if (L.Count > 0)
//                    {
//                        Debug.Log(L);
//                        Debug.Log(L.Count);
//                        Debug.Log(L[0].GetComponent<ToolDisplayController>().num);
//                        if (L[0].GetComponent<ToolDisplayController>().num == num)
//                        {
//                            for (int y = 0; y < 7; y++)
//                            {
//                                if (manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Count > 0)
//                                {
//                                    foreach (GameObject o in manager.GetComponent<GameSceneManager>().CardsInDisplay[y])
//                                    {
//                                        o.GetComponent<ToolDisplayController>().kill();
//                                    }
//                                    manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Clear();
//                                }
//                            }
//                            manager.GetComponent<GameSceneManager>().displaycount = 0;
//                        }
//                        else
//                        {
//                            for (int y = 0; y < 7; y++)
//                            {
//                                if (manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Count > 0)
//                                {
//                                    foreach (GameObject o in manager.GetComponent<GameSceneManager>().CardsInDisplay[y])
//                                    {
//                                        o.GetComponent<ToolDisplayController>().kill();
//                                    }
//                                    manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Clear();
//                                }
//                            }
//                            GameObject detail = Instantiate(discription, displayfrom);
//                            manager.GetComponent<GameSceneManager>().CardsInDisplay[0].Add(detail);
//                            manager.GetComponent<GameSceneManager>().displaycount = 1;
//                            detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                            detail.GetComponent<ToolDisplayController>().num = num;
//                            detail.GetComponent<ToolDisplayController>().face = face;
//                            detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                        }
//                    }
//                    else
//                    {
//                        for (int y = 0; y < 7; y++)
//                        {
//                            if (manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Count > 0)
//                            {
//                                foreach (GameObject o in manager.GetComponent<GameSceneManager>().CardsInDisplay[y])
//                                {
//                                    o.GetComponent<ToolDisplayController>().kill();
//                                }
//                                manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Clear();
//                            }
//                        }
//                        GameObject detail = Instantiate(discription, displayfrom);
//                        manager.GetComponent<GameSceneManager>().CardsInDisplay[0].Add(detail);
//                        manager.GetComponent<GameSceneManager>().displaycount = 1;
//                        detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                        detail.GetComponent<ToolDisplayController>().num = num;
//                        detail.GetComponent<ToolDisplayController>().face = face;
//                        detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                    }
//                }
//                else
//                {
//                    GameObject detail = Instantiate(discription, displayfrom);
//                    manager.GetComponent<GameSceneManager>().CardsInDisplay[0].Add(detail);
//                    manager.GetComponent<GameSceneManager>().displaycount = 1;
//                    detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                    detail.GetComponent<ToolDisplayController>().num = num;
//                    detail.GetComponent<ToolDisplayController>().face = face;
//                    detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                }
//            }
//            else if (tooltype == "multiattack")
//            {
//                if (manager.GetComponent<GameSceneManager>().displaycount > 0)
//                {
//                    List<GameObject> L = manager.GetComponent<GameSceneManager>().CardsInDisplay[1];

//                    if (L.Count > 0)
//                    {
//                        if (L[0].GetComponent<ToolDisplayController>().num == num)
//                        {
//                            for (int y = 0; y < 7; y++)
//                            {
//                                if (manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Count > 0)
//                                {
//                                    foreach (GameObject o in manager.GetComponent<GameSceneManager>().CardsInDisplay[y])
//                                    {
//                                        o.GetComponent<ToolDisplayController>().kill();

//                                    }
//                                    manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Clear();
//                                }
//                            }
//                            manager.GetComponent<GameSceneManager>().displaycount = 0;
//                        }

//                        else
//                        {
//                            for (int y = 0; y < 7; y++)
//                            {
//                                if (manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Count > 0)
//                                {
//                                    foreach (GameObject o in manager.GetComponent<GameSceneManager>().CardsInDisplay[y])
//                                    {
//                                        o.GetComponent<ToolDisplayController>().kill();
//                                    }
//                                    manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Clear();
//                                }
//                            }
//                            GameObject detail = Instantiate(discription, displayfrom);
//                            manager.GetComponent<GameSceneManager>().CardsInDisplay[1].Add(detail);
//                            manager.GetComponent<GameSceneManager>().displaycount = 1;
//                            detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                            detail.GetComponent<ToolDisplayController>().num = num;
//                            detail.GetComponent<ToolDisplayController>().face = face;
//                            detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                        }
//                    }
//                    else
//                    {
//                        for (int y = 0; y < 7; y++)
//                        {
//                            if (manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Count > 0)
//                            {
//                                foreach (GameObject o in manager.GetComponent<GameSceneManager>().CardsInDisplay[y])
//                                {
//                                    o.GetComponent<ToolDisplayController>().kill();
//                                }
//                                manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Clear();
//                            }
//                        }
//                        GameObject detail = Instantiate(discription, displayfrom);
//                        manager.GetComponent<GameSceneManager>().CardsInDisplay[1].Add(detail);
//                        manager.GetComponent<GameSceneManager>().displaycount = 1;
//                        detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                        detail.GetComponent<ToolDisplayController>().num = num;
//                        detail.GetComponent<ToolDisplayController>().face = face;
//                        detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                    }
//                }
//                else
//                {
//                    GameObject detail = Instantiate(discription, displayfrom);
//                    manager.GetComponent<GameSceneManager>().CardsInDisplay[1].Add(detail);
//                    manager.GetComponent<GameSceneManager>().displaycount = 1;
//                    detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                    detail.GetComponent<ToolDisplayController>().num = num;
//                    detail.GetComponent<ToolDisplayController>().face = face;
//                    detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                }
//            }
//            else if (tooltype == "medicine")
//            {
//                if (manager.GetComponent<GameSceneManager>().displaycount > 0)
//                {
//                    List<GameObject> L = manager.GetComponent<GameSceneManager>().CardsInDisplay[5];

//                    if (L.Count > 0)
//                    {
//                        if (L[0].GetComponent<ToolDisplayController>().num == num)
//                        {
//                            for (int y = 0; y < 7; y++)
//                            {
//                                if (manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Count > 0)
//                                {
//                                    foreach (GameObject o in manager.GetComponent<GameSceneManager>().CardsInDisplay[y])
//                                    {
//                                        o.GetComponent<ToolDisplayController>().kill();
//                                    }
//                                    manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Clear();
//                                }
//                            }
//                            manager.GetComponent<GameSceneManager>().displaycount = 0;
//                        }

//                        else
//                        {
//                            for (int y = 0; y < 7; y++)
//                            {
//                                if (manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Count > 0)
//                                {
//                                    foreach (GameObject o in manager.GetComponent<GameSceneManager>().CardsInDisplay[y])
//                                    {
//                                        o.GetComponent<ToolDisplayController>().kill();
//                                    }
//                                    manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Clear();
//                                }
//                            }
//                            GameObject detail = Instantiate(discription, displayfrom);
//                            manager.GetComponent<GameSceneManager>().CardsInDisplay[5].Add(detail);
//                            manager.GetComponent<GameSceneManager>().displaycount = 1;
//                            detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                            detail.GetComponent<ToolDisplayController>().num = num;
//                            detail.GetComponent<ToolDisplayController>().face = face;
//                            detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                        }
//                    }
//                    else
//                    {
//                        for (int y = 0; y < 7; y++)
//                        {
//                            if (manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Count > 0)
//                            {
//                                foreach (GameObject o in manager.GetComponent<GameSceneManager>().CardsInDisplay[y])
//                                {
//                                    o.GetComponent<ToolDisplayController>().kill();
//                                }
//                                manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Clear();
//                            }
//                        }
//                        GameObject detail = Instantiate(discription, displayfrom);
//                        manager.GetComponent<GameSceneManager>().CardsInDisplay[5].Add(detail);
//                        manager.GetComponent<GameSceneManager>().displaycount = 1;
//                        detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                        detail.GetComponent<ToolDisplayController>().num = num;
//                        detail.GetComponent<ToolDisplayController>().face = face;
//                        detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                    }
//                }
//                else
//                {
//                    GameObject detail = Instantiate(discription, displayfrom);
//                    manager.GetComponent<GameSceneManager>().CardsInDisplay[5].Add(detail);
//                    manager.GetComponent<GameSceneManager>().displaycount = 1;
//                    detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                    detail.GetComponent<ToolDisplayController>().num = num;
//                    detail.GetComponent<ToolDisplayController>().face = face;
//                    detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                }
//            }
//            //------------
//            else if (tooltype == "effect")
//            {
//                if (manager.GetComponent<GameSceneManager>().displaycount > 0)
//                {
//                    List<GameObject> L = manager.GetComponent<GameSceneManager>().CardsInDisplay[3];
//                    if (L.Count > 0)
//                    {
//                        if (L[0].GetComponent<ToolDisplayController>().num == num)
//                        {
//                            for (int y = 0; y < 7; y++)
//                            {
//                                if (manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Count > 0)
//                                {
//                                    foreach (GameObject o in manager.GetComponent<GameSceneManager>().CardsInDisplay[y])
//                                    {
//                                        o.GetComponent<ToolDisplayController>().kill();
//                                    }
//                                    manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Clear();
//                                }
//                            }
//                            manager.GetComponent<GameSceneManager>().displaycount = 0;
//                        }

//                        else
//                        {
//                            for (int y = 0; y < 7; y++)
//                            {
//                                if (manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Count > 0)
//                                {
//                                    foreach (GameObject o in manager.GetComponent<GameSceneManager>().CardsInDisplay[y])
//                                    {
//                                        o.GetComponent<ToolDisplayController>().kill();
//                                    }
//                                    manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Clear();
//                                }
//                            }
//                            GameObject detail = Instantiate(discription, displayfrom);
//                            manager.GetComponent<GameSceneManager>().CardsInDisplay[3].Add(detail);
//                            manager.GetComponent<GameSceneManager>().displaycount = 1;
//                            detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                            detail.GetComponent<ToolDisplayController>().num = num;
//                            detail.GetComponent<ToolDisplayController>().face = face;
//                            detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                        }
//                    }
//                    else
//                    {
//                        for (int y = 0; y < 7; y++)
//                        {
//                            if (manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Count > 0)
//                            {
//                                foreach (GameObject o in manager.GetComponent<GameSceneManager>().CardsInDisplay[y])
//                                {
//                                    o.GetComponent<ToolDisplayController>().kill();
//                                }
//                                manager.GetComponent<GameSceneManager>().CardsInDisplay[y].Clear();
//                            }
//                        }
//                        GameObject detail = Instantiate(discription, displayfrom);
//                        manager.GetComponent<GameSceneManager>().CardsInDisplay[3].Add(detail);
//                        manager.GetComponent<GameSceneManager>().displaycount = 1;
//                        detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                        detail.GetComponent<ToolDisplayController>().num = num;
//                        detail.GetComponent<ToolDisplayController>().face = face;
//                        detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                    }
//                }
//                else
//                {
//                    GameObject detail = Instantiate(discription, displayfrom);
//                    manager.GetComponent<GameSceneManager>().CardsInDisplay[3].Add(detail);
//                    manager.GetComponent<GameSceneManager>().displaycount = 1;
//                    detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                    detail.GetComponent<ToolDisplayController>().num = num;
//                    detail.GetComponent<ToolDisplayController>().face = face;
//                    detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                }
//            }
//            //------------
//            else if (tooltype == "strengthen")
//            {
//                List<GameObject> L = manager.GetComponent<GameSceneManager>().CardsInDisplay[0];
//                if (L.Count != 0)
//                {
//                    List<GameObject> S = manager.GetComponent<GameSceneManager>().CardsInDisplay[6];
//                    if (S.Count != 0)
//                    {
//                        int i = 0;
//                        foreach (GameObject o in S)
//                        {
//                            if (o.GetComponent<ToolDisplayController>().num == num)
//                            {
//                                break;
//                            }
//                            else
//                            {
//                                i++;
//                            }
//                        }
//                        if (i < S.Count)
//                        {
//                            manager.GetComponent<GameSceneManager>().CardsInDisplay[6][i].GetComponent<ToolDisplayController>().kill();
//                            manager.GetComponent<GameSceneManager>().CardsInDisplay[6].Remove(manager.GetComponent<GameSceneManager>().CardsInDisplay[6][i]);
//                            manager.GetComponent<GameSceneManager>().displaycount--;
//                        }
//                        else
//                        {
//                            GameObject detail = Instantiate(discription, displayfrom);
//                            manager.GetComponent<GameSceneManager>().CardsInDisplay[6].Add(detail);
//                            manager.GetComponent<GameSceneManager>().displaycount++;
//                            detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                            detail.GetComponent<ToolDisplayController>().num = num;
//                            detail.GetComponent<ToolDisplayController>().face = face;
//                            detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                        }
//                    }
//                    else
//                    {
//                        GameObject detail = Instantiate(discription, displayfrom);
//                        manager.GetComponent<GameSceneManager>().CardsInDisplay[6].Add(detail);
//                        manager.GetComponent<GameSceneManager>().displaycount++;
//                        detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                        detail.GetComponent<ToolDisplayController>().num = num;
//                        detail.GetComponent<ToolDisplayController>().face = face;
//                        detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                    }

//                }
//            }

//        }
//        else if (sta == 2)
//        {
//            if (manager.GetComponent<GameSceneManager>().DisplayType[0] == "effect" )
//            {
//                if (tooltype == "effectdefense")
//                {

//                    List<GameObject> S = manager.GetComponent<GameSceneManager>().CardsInDisplay[4];
//                    if (S.Count != 0)
//                    {
//                        int i = 0;
//                        foreach (GameObject o in S)
//                        {
//                            if (o.GetComponent<ToolDisplayController>().num == num)
//                            {
//                                break;
//                            }
//                            else
//                            {
//                                i++;
//                            }
//                        }
//                        if (i < S.Count)
//                        {
//                            manager.GetComponent<GameSceneManager>().CardsInDisplay[4][i].GetComponent<ToolDisplayController>().kill();
//                            manager.GetComponent<GameSceneManager>().CardsInDisplay[4].Remove(manager.GetComponent<GameSceneManager>().CardsInDisplay[4][i]);
//                            manager.GetComponent<GameSceneManager>().displaycount--;
//                        }
//                        else
//                        {
//                            GameObject detail = Instantiate(discription, displayto);
//                            manager.GetComponent<GameSceneManager>().CardsInDisplay[4].Add(detail);
//                            manager.GetComponent<GameSceneManager>().displaycount++;
//                            detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                            detail.GetComponent<ToolDisplayController>().num = num;
//                            detail.GetComponent<ToolDisplayController>().face = face;
//                            detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                        }
//                    }
//                    else
//                    {
//                        GameObject detail = Instantiate(discription, displayto);
//                        manager.GetComponent<GameSceneManager>().CardsInDisplay[4].Add(detail);
//                        manager.GetComponent<GameSceneManager>().displaycount++;
//                        detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                        detail.GetComponent<ToolDisplayController>().num = num;
//                        detail.GetComponent<ToolDisplayController>().face = face;
//                        detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                    }
//                }

//            }
//            else
//            {
//                if (tooltype == "defense")
//                {

//                    List<GameObject> S = manager.GetComponent<GameSceneManager>().CardsInDisplay[2];
//                    if (S.Count != 0)
//                    {
//                        int i = 0;
//                        foreach (GameObject o in S)
//                        {
//                            if (o.GetComponent<ToolDisplayController>().num == num)
//                            {
//                                break;
//                            }
//                            else
//                            {
//                                i++;
//                            }
//                        }
//                        if (i < S.Count)
//                        {
//                            manager.GetComponent<GameSceneManager>().CardsInDisplay[2][i].GetComponent<ToolDisplayController>().kill();
//                            manager.GetComponent<GameSceneManager>().CardsInDisplay[2].Remove(manager.GetComponent<GameSceneManager>().CardsInDisplay[2][i]);
//                            manager.GetComponent<GameSceneManager>().displaycount--;
//                        }
//                        else
//                        {
//                            GameObject detail = Instantiate(discription, displayto);
//                            manager.GetComponent<GameSceneManager>().CardsInDisplay[2].Add(detail);
//                            manager.GetComponent<GameSceneManager>().displaycount++;
//                            detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                            detail.GetComponent<ToolDisplayController>().num = num;
//                            detail.GetComponent<ToolDisplayController>().face = face;
//                            detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                        }
//                    }
//                    else
//                    {
//                        GameObject detail = Instantiate(discription, displayto);
//                        manager.GetComponent<GameSceneManager>().CardsInDisplay[2].Add(detail);
//                        manager.GetComponent<GameSceneManager>().displaycount++;
//                        detail.GetComponent<ToolDisplayController>().tooltype = tooltype;
//                        detail.GetComponent<ToolDisplayController>().num = num;
//                        detail.GetComponent<ToolDisplayController>().face = face;
//                        detail.GetComponent<ToolDisplayController>().forDisplay = false;
//                    }
//                }
//            }
//        }
//        manager.GetComponent<GameSceneManager>().RefreshDisplay();
//    }
//    // Update is called once per frame
//    public void kill()
//    {
//        Destroy(obj);
//    }
//    void Update()
//    {

//    }
//}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolCardController : MonoBehaviour, IPointerClickHandler
{
    public int num;
    public string face, tooltype;
    public GameObject discription;

    // 將原本散落的 Find 和 GetComponent 集中快取，減少效能浪費
    private GameSceneManager gameManager;
    private Transform displayfrom, displayto;

    // 建立卡牌類型與陣列 Index 的對應表 (類似 Hash Map / Lookup Table)
    private readonly Dictionary<string, int> typeToIndex = new Dictionary<string, int>
    {
        { "attack", 0 },
        { "multiattack", 1 },
        { "defense", 2 },
        { "effect", 3 },
        { "effectdefense", 4 },
        { "medicine", 5 },
        { "strengthen", 6 }
    };

    void Start()
    {
        // 只在初始化時找一次並存成參考 (Reference)
        GameObject canva = GameObject.Find("Canvas");
        gameManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();

        // 善用 transform.Find 可以直接找子物件，不需動用全域尋找
        Transform roundTransform = canva.transform.Find("Round");
        displayfrom = roundTransform.Find("FromBoard");
        displayto = roundTransform.Find("ToBoard");

        GetComponent<Image>().sprite = Resources.Load<Sprite>("Tool/" + tooltype + "/" + face);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int sta = gameManager.status;

        if (sta == 1) // 1: 出牌階段
        {
            if (tooltype == "strengthen")
            {
                HandleStrengthen(displayfrom);
            }
            // 【修正點】：明確限制在 sta == 1 時，只有這四種基礎牌可以觸發 HandleStandardCard
            else if (tooltype == "attack" || tooltype == "multiattack" || tooltype == "medicine" || tooltype == "effect")
            {
                HandleStandardCard(typeToIndex[tooltype], displayfrom);
            }
            // 如果在 sta == 1 點到 defense，因為不符合上述條件，什麼事都不會發生，成功防堵！
        }
        else if (sta == 2) // 2: 回應階段
        {
            // 加上 Count > 0 的檢查比較保險，避免 DisplayType 是空的時候發生 OutOfRangeException
            if (gameManager.DisplayType.Count > 0 && gameManager.DisplayType[0] == "effect")
            {
                if (tooltype == "effectdefense")
                    HandleToggleCard(4, displayto);
            }
            else
            {
                if (tooltype == "defense")
                    HandleToggleCard(2, displayto);
            }
        }

        gameManager.RefreshDisplay();
    }

    // 處理一般會「清空全場」的卡牌 (攻擊、補血、效果等)
    private void HandleStandardCard(int targetIndex, Transform targetDisplay)
    {
        List<GameObject> currentDisplay = gameManager.CardsInDisplay[targetIndex];

        // 如果點擊的是同一張已經在展示區的牌，就取消選取並清空
        if (gameManager.displaycount > 0 && currentDisplay.Count > 0 && currentDisplay[0].GetComponent<ToolDisplayController>().num == num)
        {
            ClearAllDisplays();
            gameManager.displaycount = 0;
        }
        else
        {
            // 否則清空展示區，並放入這張新牌
            ClearAllDisplays();
            AddCardToDisplay(targetIndex, targetDisplay);
            gameManager.displaycount = 1;
        }
    }

    // 處理可以「多張疊加」或「反覆開關」的卡牌 (防禦、效果防禦等)
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

    // 強化卡的邏輯其實跟防禦卡一樣是疊加的，只是多了一個前提條件
    private void HandleStrengthen(Transform targetDisplay)
    {
        // 必須先有普通攻擊卡 (Index 0) 才能打強化卡
        if (gameManager.CardsInDisplay[0].Count != 0)
        {
            HandleToggleCard(6, targetDisplay);
        }
    }

    // 負責清空所有展示區的迴圈
    private void ClearAllDisplays()
    {
        for (int y = 0; y < 7; y++)
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

        // 【關鍵修改】：根據卡牌種類，給予準備區卡牌對應的基礎展示數值
        // (因為準備階段還沒加上角色倍率，可以先給基礎值或0)
        if (tooltype == "attack" || tooltype == "multiattack")
            tdc.init(-5, 0, 0);
        else if (tooltype == "medicine" || tooltype == "strengthen")
            tdc.init(5, 0, 0);
        else
            tdc.init(0, 0, 0);
    }

    public void kill()
    {
        Destroy(gameObject);
    }
}