using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMagicSelect : MonoBehaviour
{
    public string spellName;
    public int spellCost;
    public Text nameText;
    public Text costText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Aqui já usa magia...
    public void Press()
    {
        if (BattleManager.instance.activeBattler[BattleManager.instance.currentTurn].currentMP >= spellCost)
        {
            BattleManager.instance.spellMenu.SetActive(false);
            BattleManager.instance.OpenTargetMenu(spellName);
            BattleManager.instance.activeBattler[BattleManager.instance.currentTurn].currentMP -= spellCost;
        }
        else
        {
            // Não tem mana bastante pra usar magia...
            BattleManager.instance.battleNotification.theText.text = "Nao tem mana, porra!";
            BattleManager.instance.battleNotification.Activate();
            BattleManager.instance.spellMenu.SetActive(false);
        }
        
    }
}
