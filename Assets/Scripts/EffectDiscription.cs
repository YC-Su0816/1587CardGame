using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class EffectDiscription : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    GameObject obj;
    public Image img;
    Vector2 max, min;
    public TMP_Text nam, desciption, cd;
    void Start()
    {
        obj = gameObject;
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0, 0);
        Vector3[] corner = new Vector3[4];
        rt.GetWorldCorners(corner);
        max = corner[2];
        min = corner[0];
        Debug.Log(max.x.ToString() + ' ' + max.y.ToString() + ' ' + min.x.ToString() + ' ' + min.y.ToString());
    }

        // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(mousepos.x.ToString() + ' ' + mousepos.y.ToString());
            if (mousepos.x >= min.x && mousepos.x <= max.x && mousepos.y >= min.y && mousepos.y <= max.y)
            {

            }
            else
            {
                Destroy(gameObject);
            }

        }
    }
    public void setLook(string face)
    {
        img.sprite = Resources.Load<Sprite>("Tool/effect/" + face);
    }
    public void setWord(string inam, string des)
    {
        nam.text = inam;
        desciption.text = des;
    }
    public void setCD(string coold)
    {
        cd.text = coold;
    }
}
