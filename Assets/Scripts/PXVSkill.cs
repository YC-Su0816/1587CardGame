using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PXVSkill : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_Dropdown pickPlayer;
    public TMP_Text guessedName;
    public PXV parent;
    public Button confirm;
    void Start()
    {
        
    }
    public void confirmChoice()
    {
        parent.confirmChoice(guessedName.text);
        Destroy(gameObject);
    }
    public void init(PXV p, List<string> namelist)
    {
        parent = p;
        pickPlayer.ClearOptions();
        pickPlayer.AddOptions(namelist);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
