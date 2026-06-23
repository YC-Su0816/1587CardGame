using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TargetSetting : MonoBehaviour, IPointerClickHandler
{
    public GameObject manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameObject.Find("GameSceneManager");
    }
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        int k = GetComponentInParent<PlayerPanelController>().NumInList;
        GameSceneManager gsm = manager.GetComponent<GameSceneManager>();
        if (gsm.status == 1)
        {
            PlayerPanelController ppc = gsm.PlayerPanels[k].GetComponent<PlayerPanelController>();
            if (ppc.isExist("disappear") || ppc.isExist("hide"))
            {

            }
            else
            {
                manager.GetComponent<GameSceneManager>().TargetSet(k);
            }
        }
            
    }
    // Update is called once per frame
 
}
