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
    public void set(string face, GameSceneManager m)
    {
        img.sprite = Resources.Load<Sprite>("image/CCard/" + face);
        string[] inf = m.player.characterDetailHelper();
        properties.text = inf[0];
        passive.text = inf[1];
        active.text = inf[2];
    }
    public void kill()
    {
        Destroy(gameObject);
    }
}
