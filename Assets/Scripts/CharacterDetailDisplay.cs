using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterDetailDisplay : MonoBehaviour
{
    GameObject obj;
    public Image img;
    Vector2 max, min;
    public TMP_Text properties, passive, active;
    public Button close;
    public string face;
    void Start()
    {
        // obj = gameObject;
        // RectTransform rt = obj.GetComponent<RectTransform>();
        // rt.anchorMin = new Vector2(0.5f, 0.5f);
        // rt.anchorMax = new Vector2(0.5f, 0.5f);
        // rt.pivot = new Vector2(0.5f, 0.5f);
        // rt.anchoredPosition = new Vector2(0, 0);
        // Vector3[] corner = new Vector3[4];
        // rt.GetWorldCorners(corner);
        // max = corner[2];
        // min = corner[0];
        // Debug.Log(max.x.ToString() + ' ' + max.y.ToString() + ' ' + min.x.ToString() + ' ' + min.y.ToString());
    }

    void Update()
    {
        
    }
    public void set(string face)
    {
        img.sprite = Resources.Load<Sprite>("image/CCard/" + face);
        TextAsset det = Resources.Load<TextAsset>("text/CCard/" + face);
        string[] inf;
        inf = det.text.Split("\n");
        for (int i = 0; i < inf.Length; i++)
        {
            inf[i] = inf[i].Trim();
        }
        string[] p = {"智慧:", inf[0].ToString(), "體力:", inf[1].ToString(), "聲譽:", inf[2].ToString()};
        properties.text = System.String.Join(" ", p);
        passive.text = "被動 " + inf[3] + "\n" + inf[4];
        active.text = "主動 " + inf[5] + "\n" + inf[6];
    }
    public void kill()
    {
        Destroy(gameObject);
    }
}
