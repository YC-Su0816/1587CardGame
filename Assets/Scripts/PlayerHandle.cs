using Photon.Pun;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerHandle : MonoBehaviourPunCallbacks
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PlayerBase p;
    public GameObject VIIIskillPrefab;
    public GameObject XVskillPrefab;
    public GameSceneManager manager;
    public PhotonView View => photonView;
    public string nickname;
    public string character;
    public void Init(string c) 
    {
        manager = gameObject.GetComponent<GameSceneManager>();
        character = c;
        switch (c)
        {
            case "1":
                p = new PI();
                break;
            case "2":
                p = new PII();
                break;
            case "3":
                p = new PIII();
                break;
            case "4":
                p = new PIV();
                break;
            case "5":
                p = new PV();
                break;
            case "6":
                p = new PVI();
                break;
            case "7":
                p = new PVII();
                break;
            case "8":
                p = new PVIII();
                break;
            case "9":
                p = new PIX();
                break;
            case "10":
                p = new PX();
                break;
            case "11":
                p = new PXI();
                break;
            case "12":
                p = new PXII();
                break;
            case "13":
                p = new PXIII();
                break;
            case "14":
                p = new PXIV();
                break;
            case "15":
                p = new PXV();
                break;
            case "16":
                p = new PXVI();
                break;
            case "17":
                p = new PXVII();
                break;
            case "18":
                p = new PXVIII();
                break;
            case "19":
                p = new PXIX();
                break;
            case "20":
                p = new PXX();
                break;
            case "21":
                p = new PXXI();
                break;
            case "22":
                p = new PXXII();
                break;
            case "23":
                p = new PXXIII();
                break;
            case "24":
                p = new PXXIV();
                break;
            case "25":
                p = new PXXV();
                break;
            case "26":
                p = new PXXVI();
                break;
            case "27":
                p = new PXXVII();
                break;
            case "28":
                p = new PXXVIII();
                break;
            case "29":
                p = new PXXIX();
                break;
            case "30":
                p = new PXXX();
                break;

            default:
                p = new PI();
                break;
        }
        p.getHandleManager(this, manager);
        p.Init();
    }
    public void getNickname(string nam)
    {
        nickname = nam;
    }
    public int[] getProperties()
    {
        int[] results = new int[3];
        results[0] = (int)PhotonNetwork.LocalPlayer.CustomProperties["Wisdom"];
        results[1] = (int)PhotonNetwork.LocalPlayer.CustomProperties["Strength"];
        results[2] = (int)PhotonNetwork.LocalPlayer.CustomProperties["Reputation"];
        return results;
    }
    public void useSkill()
    {
        p.useSkill();
    }
    public void newRound()
    {
        p.newRound();
    }
    public void endRound()
    {
        p.endRound();
    }
    void Start()
    {
        
    }
    public string[] characterDetailHelper()
    {
        return p.characterDetailHelper(character);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
