using UnityEngine;
using UnityEngine.EventSystems;

public class CardPanel : MonoBehaviour, IPointerClickHandler
{
    public GameObject manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        int s = manager.GetComponent<GameSceneManager>().status;
        if(manager.GetComponent<GameSceneManager>().displaycount != 0)
        {
            if (s == 1 && this.name == "FromBoard")
            {
                manager.GetComponent<GameSceneManager>().status = 3;
                manager.GetComponent<GameSceneManager>().PlayCard();
            }
            else if(s == 2 && this.name == "ToBoard")
            {
                manager.GetComponent<GameSceneManager>().status = 0;
                manager.GetComponent<GameSceneManager>().RespondCard();
            }
        }
        else
        {
            if (s == 1 && this.name == "FromBoard")
            {
                manager.GetComponent<GameSceneManager>().status = 0;
                manager.GetComponent<GameSceneManager>().PlayCard();
            }
            if (s == 2 && this.name == "ToBoard")
            {
                manager.GetComponent<GameSceneManager>().status = 0;
                manager.GetComponent<GameSceneManager>().RespondCard();
            }
        }
        
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
