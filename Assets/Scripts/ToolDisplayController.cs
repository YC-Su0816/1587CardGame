using System;
using System.Net.NetworkInformation;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ToolDisplayController : MonoBehaviour
{
    public int num;
    public bool forDisplay;
    public string face, toolname, discription, tooltype;
    public string tooleff;
    public GameObject manager;
    public GameSceneManager gsm;
    GameObject obj;
    Image image;
    TMP_Text N, D, E;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void init(int w, int s, int r)
    {
        manager = GameObject.Find("GameSceneManager");
        gsm = manager.GetComponent<GameSceneManager>();
        obj = gameObject;
        TextAsset det = Resources.Load<TextAsset>("text/Tool/" + tooltype + "/" + face);
        
        toolname = "";
        discription = "";
        tooleff = "";
        image = obj.GetComponentsInChildren<Image>()[1];
        image.sprite = Resources.Load<Sprite>("image/Tool/" + tooltype + "/" + face);
        switch (tooltype)
        {
            case "attack":
            case "medicine":
                if (det != null)
                {
                    string[] inf = det.text.Split("\n");
                    for (int i = 0; i < inf.Length; i++)
                    {
                        inf[i] = inf[i].Trim();
                    }
                    toolname = inf[3];
                    discription = inf[4];
                }
                else
                {
                    toolname = "雷電戟";
                    discription = "新版OP神器";
                }
                if (w < 0) tooleff += "傷智" + (-w) + " ";
                else if (w > 0) tooleff += "治智" + w + " ";

                if (s < 0) tooleff += "傷體" + (-s) + " ";
                else if (s > 0) tooleff += "治體" + s + " ";

                if (r < 0) tooleff += "傷譽" + (-r) + " ";
                else if (r > 0) tooleff += "治譽" + r + " ";

                tooleff = tooleff.Trim();
                if (string.IsNullOrEmpty(tooleff)) tooleff = "無效果";
                break;
            case "multiattack":
                if (det != null)
                {
                    string[] inf = det.text.Split("\n");
                    for (int i = 0; i < inf.Length; i++)
                    {
                        inf[i] = inf[i].Trim();
                    }
                    toolname = inf[3];
                    discription = inf[4];
                }
                else
                {
                    toolname = "雷電戟";
                    discription = "新版OP神器";
                }
                toolname += " (群)";
                if (w < 0) tooleff += "傷智" + (-w) + " ";
                else if (w > 0) tooleff += "治智" + w + " ";

                if (s < 0) tooleff += "傷體" + (-s) + " ";
                else if (s > 0) tooleff += "治體" + s + " ";

                if (r < 0) tooleff += "傷譽" + (-r) + " ";
                else if (r > 0) tooleff += "治譽" + r + " ";

                tooleff = tooleff.Trim();
                if (string.IsNullOrEmpty(tooleff)) tooleff = "無效果";
                break;
            case "defense":
                if (det != null)
                {
                    string[] inf = det.text.Split("\n");
                    for (int i = 0; i < inf.Length; i++)
                    {
                        inf[i] = inf[i].Trim();
                    }
                    toolname = inf[3];
                    discription = inf[4];
                }
                else
                {
                    toolname = "原石";
                    discription = "稀有材料";
                }
                if (!forDisplay || num == 0)
                {
                    if (w < 0) tooleff += "耗智" + (-w) + " ";
                    else if (w > 0) tooleff += "智防" + w + " ";

                    if (s < 0) tooleff += "耗體" + (-s) + " ";
                    else if (s > 0) tooleff += "體防" + s + " ";

                    if (r < 0) tooleff += "耗譽" + (-r) + " ";
                    else if (r > 0) tooleff += "譽防" + r + " ";
                }
                else
                {
                    tooleff = "";
                }
                    break;
            case "strengthen":
                if (det != null)
                {
                    string[] inf = det.text.Split("\n");
                    for (int i = 0; i < inf.Length; i++)
                    {
                        inf[i] = inf[i].Trim();
                    }
                    toolname = inf[3];
                    discription = inf[4];
                }
                else
                {
                    toolname = "力量藥水II";
                    discription = "奇妙的藥水";
                }
                if (!forDisplay)
                {
                    if (w < 0) tooleff += "增智傷" + (-w) + " ";
                    else if (w > 0) tooleff += "減智傷" + w + " ";

                    if (s < 0) tooleff += "增體傷" + (-s) + " ";
                    else if (s > 0) tooleff += "減體傷" + s + " ";

                    if (r < 0) tooleff += "增譽傷" + (-r) + " ";
                    else if (r > 0) tooleff += "減譽傷" + r + " ";
                }
                else
                {
                    tooleff = "";
                }
                    break;
            case "special":
                if (det != null)
                {
                    string[] inf = det.text.Split("\n");
                    for (int i = 0; i < inf.Length; i++)
                    {
                        inf[i] = inf[i].Trim();
                    }
                    if(forDisplay && (face == "femboy1" || face == "femboy2"))
                    {
                        if(w < 0)
                        {
                            if(face == gsm.DisplayFace[0])
                            {
                                toolname = "兄弟同心";
                                discription = inf[1];
                                if (w < 0) tooleff += "傷智" + (-w) + " ";
                                else if (w > 0) tooleff += "治智" + w + " ";

                                if (s < 0) tooleff += "傷體" + (-s) + " ";
                                else if (s > 0) tooleff += "治體" + s + " ";

                                if (r < 0) tooleff += "傷譽" + (-r) + " ";
                                else if (r > 0) tooleff += "治譽" + r + " ";

                            }
                            else
                            {
                                toolname = "誰跟你兄弟，是姐妹";
                                discription = inf[1];
                                tooleff = "造成傷害會失神，啾咪";
                            }
                        }
                        else
                        {
                            toolname = inf[0];
                            discription = inf[1];
                            tooleff = "碎片未湊齊，所以沒效果";
                        }
                    }
                    else if(face == "self_defense"){
                        toolname = inf[0];
                        discription = inf[1];
                        if (w < 0) tooleff += "傷智" + (-w) + " ";
                        else if (w > 0) tooleff += "治智" + w + " ";

                        if (s < 0) tooleff += "傷體" + (-s) + " ";
                        else if (s > 0) tooleff += "治體" + s + " ";

                        if (r < 0) tooleff += "傷譽" + (-r) + " ";
                        else if (r > 0) tooleff += "治譽" + r + " ";
                    }
                    else if(face == "test_reflect")
                    {
                        toolname = inf[0];
                        discription = inf[1];
                        if (w < 0) tooleff += "傷智" + (-w) + " ";
                        else if (w > 0) tooleff += "治智" + w + " ";

                        if (s < 0) tooleff += "傷體" + (-s) + " ";
                        else if (s > 0) tooleff += "治體" + s + " ";

                        if (r < 0) tooleff += "傷譽" + (-r) + " ";
                        else if (r > 0) tooleff += "治譽" + r + " ";
                    }
                    else
                    {
                        toolname = inf[0];
                        discription = inf[1];
                        tooleff = inf[2];
                    }
                }
                else
                {                   
                    toolname = "特殊卡牌";
                    discription = "擁有獨特的效果";
                    tooleff = "特殊發動";
                }
                
                break;
            case "unblockable":
                if(face == "self_defense")
                {
                    toolname = "防身術";
                    discription = "不要碰我！";
                    if (w < 0) tooleff += "傷智" + (-w) + " ";

                    if (s < 0) tooleff += "傷體" + (-s) + " ";

                    if (r < 0) tooleff += "傷譽" + (-r) + " "; 
                }
                if (det != null)
                {
                    string[] inf = det.text.Split("\n");
                    for (int i = 0; i < inf.Length; i++)
                    {
                        inf[i] = inf[i].Trim();
                    }
                    toolname = inf[0];
                    discription = inf[1];
                    tooleff = inf[2];
                }
                else
                {
                    toolname = "特殊卡牌";
                    discription = "擁有獨特的效果";
                    tooleff = "特殊發動";
                }

                break;
        }
        
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text text in texts)
        {
            if (text.name == "Toolname") N = text;
            else if (text.name == "Tooldiscription") D = text;
            else E = text;
        }
        N.text = toolname;
        D.text = discription;
        E.text = tooleff;
    }
    // Update is called once per frame
    void Update()
    {
        /*if (!forDisplay)
        {         
            if (manager.GetComponent<GameSceneManager>().status > 0)
            {
                if (num != manager.GetComponent<GameSceneManager>().cardpicking)
                {
                    Destroy(obj);
                }
            }
        }
        else
        {
            if (!manager.GetComponent<GameSceneManager>().displaytime)
            {
                Destroy(obj);
            }
        }*/
    }
    public void kill()
    {
        Destroy(obj);
    }
}
