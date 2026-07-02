using UnityEngine;
using UnityEngine.EventSystems;


public class EffectController : MonoBehaviour, IPointerClickHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject prefab;
    public string nam, round, describe, face;
    Transform canva;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canva = GameObject.Find("Canvas").GetComponent<Transform>();
    }
    public void init(string name, string roud, string desc, string f)
    {
        nam = name;
        round = roud;
        describe = desc;
        face = f;
    }
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        GameObject detail = Instantiate(prefab, canva);
        detail.GetComponent<EffectDiscription>().setLook(face);
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
