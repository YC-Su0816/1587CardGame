using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PVIIISkill : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Dropdown pickPlayer;
    public TMP_Dropdown pickType;
    public TMP_Text guessedName;
    public TMP_Text guessedType;
    public PVIII parent;
    public Button confirm;
    void Start()
    {
        
    }
    public void confirmChoice()
    {
        parent.confirmChoice(guessedName.text, guessedType.text);
        Destroy(gameObject);
    }
    public void init(PVIII p, List<string> namelist, List<string> type)
    {
        parent = p;
        pickPlayer.ClearOptions();
        pickType.ClearOptions();
        pickPlayer.AddOptions(namelist);
        pickType.AddOptions(type);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
