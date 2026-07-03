// using Unity.VisualScripting;
// using UnityEngine;
// using UnityEngine.UI;


// public class CharacterCard : MonoBehaviour
// {
//     public Button detailbutton;
//     public string card;
//     float norm = 1f/6f, pick = 0.2f, width, normScale, pickScale;
//     Vector3 max,min;
//     Vector3 normal = new Vector3(0.25f, 0.25f, 0.25f);
//     Vector3 picked = new Vector3(0.3f, 0.3f, 0.3f);
//     Vector3 normalize;
//     GameObject obj;
//     GameObject manager;
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         detailbutton.gameObject.SetActive(false);
//         manager = GameObject.Find("PickingSceneManager");
//         obj = gameObject; 
//         max = obj.GetComponent<Renderer>().bounds.max;
//         min = obj.GetComponent<Renderer>().bounds.min;
//         float screenWidth = Screen.width;
//         normalize = Camera.main.ScreenToWorldPoint(Vector3.one*screenWidth);
//         width = obj.GetComponent<SpriteRenderer>().sprite.rect.width;
//         normScale = norm * (screenWidth / width);
//         pickScale = pick * (screenWidth / width);
//         obj.GetComponent<Transform>().localScale = Vector3.one * normScale;
//         //Camera cam = obj.GetComponent<Camera>();
//         //float screenWorldWidth = cam.orthographicSize * 2f * cam.aspect;
//         //float normWidth = screenWorldWidth * norm;
//         //float pickWidth = screenWorldWidth * pick;
//         //float spriteWidth = obj.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
//         //normScale = normWidth / width;
//         //pickScale = pickWidth / width;
//         Debug.Log(max.x.ToString() +' '+ max.y.ToString() + ' ' + min.x.ToString() + ' ' + min.y.ToString());
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         bool s = manager.GetComponent<PickingSceneManager>().sss;
//         if (!s && Input.GetMouseButtonUp(0))
//         {
//             Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//             Debug.Log(mousepos.x.ToString()+' '+ mousepos.y.ToString());
//             if (mousepos.x >= min.x && mousepos.x <= max.x && mousepos.y >= min.y && mousepos.y <= max.y)
//             {
//                 //obj.GetComponent<Transform>().localScale = picked;
//                 obj.GetComponent<Transform>().localScale = Vector3.one * pickScale;
//                 Debug.Log("有點到");
//                 manager.GetComponent<PickingSceneManager>().picking = obj.name;
//                 detailbutton.transform.position = Camera.main.WorldToScreenPoint(new Vector3((max.x+min.x)/2,max.y-0.3f,0));
//                 detailbutton.gameObject.SetActive(true);
//             }
//             else
//             {
//                 detailbutton.gameObject.SetActive(false);
//                 if (manager.GetComponent<PickingSceneManager>().picking == obj.name)
//                 {
//                     manager.GetComponent<PickingSceneManager>().picking = "nobody";
//                 }
//                 //obj.GetComponent<Transform>().localScale = normal;

//                 obj.GetComponent<Transform>().localScale = Vector3.one * normScale;
//                 Debug.Log("沒點到");
//             }

//         }
//     }
    
// }

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour
{
    public Button detailbutton;
    public string card;

    [Header("卡片統一顯示大小 (世界座標)")]
    public float targetWorldWidth = 3f;  // 自行調整成你想要的卡片寬度
    public float targetWorldHeight = 4.5f; // 自行調整成你想要的卡片高度
    public float pickScaleMultiplier = 1.1f; // 點擊時放大的倍率

    public Vector3 normalScale;
    public Vector3 pickedScale;

    Vector3 max, min;
    GameObject obj;
    GameObject manager;

    void Awake()
    {
        // 建議在 Awake 抓取參考，確保其他腳本呼叫時物件已準備好
        manager = GameObject.Find("PickingSceneManager");
        obj = gameObject;
    }

    void Start()
    {
        detailbutton.gameObject.SetActive(false);
    }

    // 新增此函式：在 Photon 派發圖片後，由 Manager 主動呼叫來重設大小
    public void SetupSize()
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr.sprite == null) return;

        // 1. 先將縮放歸一，取得原始 Sprite 在世界座標下的真實 Bounds 大小
        transform.localScale = Vector3.one;
        float originalWidth = sr.bounds.size.x;
        float originalHeight = sr.bounds.size.y;

        // 2. 計算 X 軸與 Y 軸各自需要的縮放比例，強制對齊目標長寬
        float scaleX = targetWorldWidth / originalWidth;
        float scaleY = targetWorldHeight / originalHeight;

        normalScale = new Vector3(scaleX, scaleY, 1f);
        pickedScale = normalScale * pickScaleMultiplier;

        // 3. 套用正常大小
        transform.localScale = normalScale;

        // 4. 重新計算點擊判定的邊界 (依據縮放後的新大小)
        max = sr.bounds.max;
        min = sr.bounds.min;
    }

    void Update()
    {
        bool s = manager.GetComponent<PickingSceneManager>().sss;
        if (!s && Input.GetMouseButtonUp(0))
        {
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // 使用 min 和 max 來判斷點擊
            if (mousepos.x >= min.x && mousepos.x <= max.x && mousepos.y >= min.y && mousepos.y <= max.y)
            {
                obj.transform.localScale = pickedScale;
                manager.GetComponent<PickingSceneManager>().picking = obj.name;
                
                // 動態調整按鈕位置到底部
                detailbutton.transform.position = Camera.main.WorldToScreenPoint(new Vector3((max.x + min.x) / 2, max.y + 0.3f, 0));
                detailbutton.gameObject.SetActive(true);
            }
            else
            {
                detailbutton.gameObject.SetActive(false);
                if (manager.GetComponent<PickingSceneManager>().picking == obj.name)
                {
                    manager.GetComponent<PickingSceneManager>().picking = "nobody";
                }
                obj.transform.localScale = normalScale;
            }
        }
    }
}