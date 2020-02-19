using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Type")]
    public bool isItem;
    public bool isWeapon;
    public bool isArmor;

    [Header("Item Details")]
    public string itemName;
    public string description;
    public int value;
    public Sprite itemSprite;

    [Header("Item Details")]
    public int amountToChange;
    public bool affectHP, affectMP, affectStr;

    [Header("Weapon/Armor Details")]
    public int weaponStrength;

    public int armorStrength;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Use(int charToUseOn)
    {
        CharStats selectedChar = GameManager.instance.playerStats[charToUseOn];

       // Debug.Log(GameManager.instance.playerStats[charToUseOn].charName);

        if (isItem)
        {
            if (affectHP)
            {
                selectedChar.currentHP += amountToChange;

                if (selectedChar.currentHP > selectedChar.maxHP)
                {
                    selectedChar.currentHP = selectedChar.maxHP;
                }

                if (BattleManager.instance.battleActive)
                {

                   // charToUseOn = BattleManager.instance.currentActiveBattler;


                    BattleManager.instance.activeBattler[charToUseOn].currentHp += amountToChange;

                    Debug.Log(BattleManager.instance.activeBattler[charToUseOn].charName);


                    if (BattleManager.instance.activeBattler[charToUseOn].currentHp > selectedChar.maxHP)
                    {
                        BattleManager.instance.activeBattler[charToUseOn].currentHp = selectedChar.maxHP;
                    }
                }
                GameManager.instance.RemoveItem(itemName);
            }
            

            if (selectedChar.currentMP != selectedChar.maxMP)
            {
                if (affectMP)
                {
                    selectedChar.currentMP += amountToChange;
                    if (selectedChar.currentMP > selectedChar.maxMP)
                    {
                        selectedChar.currentMP = selectedChar.maxMP;
                    }

                    if (BattleManager.instance.battleActive)
                    {
                        charToUseOn = BattleManager.instance.currentActiveBattler;
                        BattleManager.instance.activeBattler[charToUseOn].currentMP += amountToChange;
                        if (BattleManager.instance.activeBattler[charToUseOn].currentMP > selectedChar.maxMP)
                        {
                            BattleManager.instance.activeBattler[charToUseOn].currentMP = selectedChar.maxMP;
                        }
                    }

                    GameManager.instance.RemoveItem(itemName);
                }
            }

            if (affectStr)
            {
                selectedChar.strength += amountToChange;

                GameManager.instance.RemoveItem(itemName);
            }
        }

        if (isWeapon)
        {
            if (selectedChar.equippedWpn != "")
            {
                GameManager.instance.AddItem(selectedChar.equippedWpn);
            }

            selectedChar.equippedWpn = itemName;
            selectedChar.wpnPwr = weaponStrength;

            GameManager.instance.RemoveItem(itemName);
        }

        if (isArmor)
        {
            if (selectedChar.equippedArmr != "")
            {
                GameManager.instance.AddItem(selectedChar.equippedArmr);
            }

            selectedChar.equippedArmr = itemName;
            selectedChar.armrPwr = armorStrength;

            GameManager.instance.RemoveItem(itemName);
        }
    }
}
