using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class CharacterCard : MonoBehaviour
{
    public Button detailbutton;
    public string card;
    float norm = 1f/6f, pick = 0.2f, width, normScale, pickScale;
    Vector3 max,min;
    Vector3 normal = new Vector3(0.25f, 0.25f, 0.25f);
    Vector3 picked = new Vector3(0.3f, 0.3f, 0.3f);
    Vector3 normalize;
    GameObject obj;
    GameObject manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        detailbutton.gameObject.SetActive(false);
        manager = GameObject.Find("PickingSceneManager");
        obj = gameObject; 
        max = obj.GetComponent<Renderer>().bounds.max;
        min = obj.GetComponent<Renderer>().bounds.min;
        float screenWidth = Screen.width;
        normalize = Camera.main.ScreenToWorldPoint(Vector3.one*screenWidth);
        width = obj.GetComponent<SpriteRenderer>().sprite.rect.width;
        normScale = norm * (screenWidth / width);
        pickScale = pick * (screenWidth / width);
        obj.GetComponent<Transform>().localScale = Vector3.one * normScale;
        //Camera cam = obj.GetComponent<Camera>();
        //float screenWorldWidth = cam.orthographicSize * 2f * cam.aspect;
        //float normWidth = screenWorldWidth * norm;
        //float pickWidth = screenWorldWidth * pick;
        //float spriteWidth = obj.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        //normScale = normWidth / width;
        //pickScale = pickWidth / width;
        Debug.Log(max.x.ToString() +' '+ max.y.ToString() + ' ' + min.x.ToString() + ' ' + min.y.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        bool s = manager.GetComponent<PickingSceneManager>().sss;
        if (!s && Input.GetMouseButtonUp(0))
        {
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(mousepos.x.ToString()+' '+ mousepos.y.ToString());
            if (mousepos.x >= min.x && mousepos.x <= max.x && mousepos.y >= min.y && mousepos.y <= max.y)
            {
                //obj.GetComponent<Transform>().localScale = picked;
                obj.GetComponent<Transform>().localScale = Vector3.one * pickScale;
                Debug.Log("有點到");
                manager.GetComponent<PickingSceneManager>().picking = obj.name;
                detailbutton.transform.position = Camera.main.WorldToScreenPoint(new Vector3((max.x+min.x)/2,max.y-0.3f,0));
                detailbutton.gameObject.SetActive(true);
            }
            else
            {
                detailbutton.gameObject.SetActive(false);
                if (manager.GetComponent<PickingSceneManager>().picking == obj.name)
                {
                    manager.GetComponent<PickingSceneManager>().picking = "nobody";
                }
                //obj.GetComponent<Transform>().localScale = normal;

                obj.GetComponent<Transform>().localScale = Vector3.one * normScale;
                Debug.Log("沒點到");
            }

        }
    }
    
}

