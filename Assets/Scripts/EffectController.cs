using UnityEngine;
using UnityEngine.EventSystems;


public class EffectController : MonoBehaviour, IPointerClickHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject prefab;
    public string nam, round, describe;
    Transform canva;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canva = GameObject.Find("Canvas").GetComponent<Transform>();
        describe = "try";
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        GameObject detail = Instantiate(prefab, canva);
        detail.GetComponent<EffectDiscription>().setLook(nam);
        detail.GetComponent<EffectDiscription>().setWord(nam, describe);
        detail.GetComponent<EffectDiscription>().setCD(round);

    }
    // Update is called once per frame
    public void kill()
    {
        Destroy(gameObject);
    }
    void Update()
    {

    }
    public void setName(string name)
    {
        this.nam = name;
    }
    public void setRound(string round)
    {
        this.round = round;
    }
}
