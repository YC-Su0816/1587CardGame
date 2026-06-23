using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolDisplayController : MonoBehaviour
{
    public int num;
    public bool forDisplay;
    public string face, toolname, discription, tooltype;
    public string tooleff;
    public GameObject manager;
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
        if (manager.GetComponent<GameSceneManager>().displaytime)
        {
            forDisplay = true;
        }
        obj = gameObject;
        toolname = "";
        discription = "";
        tooleff = "";
        image = obj.GetComponentsInChildren<Image>()[1];
        image.sprite = Resources.Load<Sprite>("Tool/" + tooltype + "/" + face);
        switch (tooltype)
        {
            case "attack":
            case "medicine":
                toolname = "雷電戟" + "(群)";
                discription = "新版OP神器";
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
                toolname = "雷電戟" + "(群)";
                discription = "新版OP神器";
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
                toolname = "原石";
                discription = "稀有材料";
                if (w > 0) tooleff += "智防" + w + " ";

                if (s > 0) tooleff += "體防" + s + " ";

                if (r > 0) tooleff += "譽防" + r + " ";

                tooleff = tooleff.Trim();
                if (string.IsNullOrEmpty(tooleff)) tooleff = "無效果";
                break;
            case "effect":
                toolname = "紅姐";
                discription = "男莖大屠殺";
                tooleff = "AIDS";
                break;
            case "effectdefense":
                toolname = "老子";
                discription = "一生二，二生三，三生萬物";
                tooleff = "免疫";
                break;
            case "strengthen":
                toolname = "力量藥水II";
                discription = "奇妙的藥水";
                if (w < 0) tooleff += "減智傷" + (-w) + " ";
                else if (w > 0) tooleff += "增智傷" + w + " ";

                if (s < 0) tooleff += "減體傷" + (-s) + " ";
                else if (s > 0) tooleff += "增體傷" + s + " ";

                if (r < 0) tooleff += "減譽傷" + (-r) + " ";
                else if (r > 0) tooleff += "增譽傷" + r + " ";

                tooleff = tooleff.Trim();
                if (string.IsNullOrEmpty(tooleff)) tooleff = "無效果";
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
