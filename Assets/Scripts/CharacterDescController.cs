using UnityEngine;
using UnityEngine.EventSystems;
public class CharacterDescController : MonoBehaviour, IPointerClickHandler
{
    public GameObject prefab;
    public string face;
    Transform canva;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canva = GameObject.Find("Canvas").GetComponent<Transform>();
    }
    public void init(string f)
    {
        face = f;
    }
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        GameObject detail = Instantiate(prefab, canva);
        detail.GetComponent<CharacterDetailDisplay>().set(face);
    }
    // Update is called once per frame
    public void kill()
    {
        Destroy(gameObject);
    }
    void Update()
    {

    }
}
