using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    public bool battleActive;
    public GameObject battleScene;
    public Transform[] characterPositions;
    public Transform[] enemyPositions;
    public BattleChar[] characterPrefabs;
    public BattleChar[] enemyPrefabs;
    public List<BattleChar> activeBattler = new List<BattleChar>();
    public int currentActiveBattler;
    public int currentTurn;
    public bool turnWaiting;
    public GameObject uiButtonHolder;
    public BattleMove[] movesList;
    public GameObject enemyAttackEffect;
    public DamageNumber damageText;

    public Text[] charName, charHP, charMP;

    public GameObject targetMenu;
    public BattleTargetButtons[] targetButtons;
    public GameObject spellMenu;
    public BattleMagicSelect[] spellButtons;
    public BattleNotification battleNotification;
    public int chanceToFlee = 35;
    public bool fleeRollSuccessful = false;
    public bool allEnemiesDead = false;
    public bool allCharactersDead = false;
    public float endBattleSceneTimer = 1.5f;
    public string gameOverScene;
    public int rewardExp;
    public string[] rewardItems;
    public bool ableToFlee;
    public GameObject fleeButton;

    // Item Screen
    [Header("Item Menu")]
    public GameObject itemMenu;
    public ItemButton[] itemButtons;
    public string selectedItem;
    public Item activeItem;
    public Text itemName;
    public Text itemDescription;
    public Text useButtonText;

    // Items for char
    public GameObject itemCharChoiceBattleMenu;
    public Text[] itemCharChoiceBattleNames;

    private bool fleeing;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (battleActive)
        {
            for (int i = 0; i < activeBattler.Count; i++)
            {
                charName[i].color = Color.white;
            }

            if (turnWaiting)
            {
                if (activeBattler[currentTurn].isPlayer)
                {
                    if (activeBattler[currentTurn].charName == "Tim")
                    {
                        currentActiveBattler = 0;
                    }
                    else if (activeBattler[currentTurn].charName == "Woody")
                    {
                        currentActiveBattler = 1;
                    }

                    uiButtonHolder.SetActive(true);
                }
                else
                {
                    uiButtonHolder.SetActive(false);
                    // Start enemy attack turns
                    StartCoroutine(EnemyMoveCoroutine());
                }

                charName[currentActiveBattler].color = Color.grey;
            }
        }

        //TODO: Redo flee success roll notification
        //if (allEnemiesDead || allCharactersDead
        //                   || fleeRollSuccessful)
        //{
        //    endBattleSceneTimer -= Time.deltaTime;
        //    if (fleeRollSuccessful)
        //    {
        //        battleNotification.notificationText.text = "Flee success!";
        //        battleNotification.Activate();
        //    }

        //    if (endBattleSceneTimer <= 0)
        //    {
        //        battleActive = false;
        //        battleScene.SetActive(false);
        //        GameManager.instance.battleActive = false;
        //        AudioManager.instance.PlayBGM(3);
        //        fleeRollSuccessful = false;
        //        allEnemiesDead = false;
        //        allCharactersDead = false;
        //    }
        //}
    }

    public void BattleStart(string[] enemiesToSpawn, bool fleeStatus)
    {
        ableToFlee = fleeStatus;

        if (!battleActive)
        {
            battleActive = true;
            GameManager.instance.battleActive = true;
            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
            battleScene.SetActive(true);
            AudioManager.instance.PlayBGM(0);

            // Spawn Characters
            for (int i = 0; i < characterPositions.Length; i++)
            {
                if (GameManager.instance.playerStats[i].gameObject.activeInHierarchy)
                {
                    for (int j = 0; j < characterPrefabs.Length; j++)
                    {
                        if (characterPrefabs[j].charName == GameManager.instance.playerStats[i].charName)
                        {
                            BattleChar battleCharacter = Instantiate(characterPrefabs[j], characterPositions[i].position, characterPositions[i].rotation);
                            battleCharacter.transform.parent = characterPositions[i];
                            activeBattler.Add(battleCharacter);

                            CharStats character = GameManager.instance.playerStats[i];
                            activeBattler[i].currentHp = character.currentHP;
                            activeBattler[i].maxHP = character.maxHP;
                            activeBattler[i].currentMP = character.currentMP;
                            activeBattler[i].maxMP = character.maxMP;
                            activeBattler[i].strength = character.strength;
                            activeBattler[i].defence = character.defence;
                            activeBattler[i].wpnPower = character.wpnPwr;
                            activeBattler[i].armrPower = character.armrPwr;
                        }
                    }
                }
            }

            // Spawn Enemies
            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                if (enemiesToSpawn[i] != "")
                {
                    for (int j = 0; j < enemyPrefabs.Length; j++)
                    {
                        if (enemyPrefabs[j].charName == enemiesToSpawn[i])
                        {
                            BattleChar battleEnemy = Instantiate(enemyPrefabs[j], enemyPositions[i].position, enemyPositions[i].rotation);
                            battleEnemy.transform.parent = enemyPositions[i];
                            activeBattler.Add(battleEnemy);
                        }
                    }
                }
            }

            turnWaiting = true;
            currentTurn = UnityEngine.Random.Range(0, activeBattler.Count);
            UpdateUICharInfo();
        }
    }

    private void NextTurn()
    {
        currentTurn++;
        if (currentTurn >= activeBattler.Count)
        {
            currentTurn = 0;
        }

        turnWaiting = true;
        UpdateUICharInfo();
        UpdateBattle();
    }

    public void UpdateBattle()
    {
        allEnemiesDead = true;
        allCharactersDead = true;

        for (int i = 0; i < activeBattler.Count; i++)
        {
            if (activeBattler[i].currentHp <= 0)
            {
                activeBattler[i].currentHp = 0;
            }

            // Handle dead battler character
            if (activeBattler[i].currentHp == 0)
            {
                if (activeBattler[i].isPlayer)
                {
                    activeBattler[i].theSprite.sprite = activeBattler[i].deadSprite;
                }
                else
                {
                    activeBattler[i].EnemyFade();
                }
            }
            else
            {
                if (activeBattler[i].isPlayer)
                {
                    allCharactersDead = false;
                    activeBattler[i].theSprite.sprite = activeBattler[i].aliveSprite;
                }
                else
                {
                    allEnemiesDead = false;
                }
            }
        }

        if (allEnemiesDead || allCharactersDead)
        {
            if (allEnemiesDead)
            {
                //TODO: End battle in victory
                StartCoroutine(EndBattleCoroutine());
                //AudioManager.instance.StopMusic();
            }
            else
            {
                //TODO: End battle in failure
                StartCoroutine(GameOverCoroutine());
            }

            if (endBattleSceneTimer <= 0)
            {
                battleScene.SetActive(false);
                GameManager.instance.battleActive = false;
                battleActive = false;
            }
        }
        else
        {
            // Check if current active battler has HP
            while (activeBattler[currentTurn].currentHp == 0)
            {
                currentTurn++;
                if (currentTurn >= activeBattler.Count)
                {
                    currentTurn = 0;
                }
            }
        }
    }

    public void CharacterAttack(string moveName, int selectedTarget)
    {
        int movePower = 0;
        for (int i = 0; i < movesList.Length; i++)
        {
            if (movesList[i].moveName == moveName)
            {
                // Instantiate attack effect on appropriate character
                Instantiate(movesList[i].theEffect,
                            activeBattler[selectedTarget].transform.position,
                            activeBattler[selectedTarget].transform.rotation);
                // Grab move power of appropriate move
                movePower = movesList[i].movePower;
            }
        }

        // Disable buttons to prevent double attacks
        uiButtonHolder.SetActive(false);
        targetMenu.SetActive(false);

        // Effect to show which enemy just attacked
        Instantiate(enemyAttackEffect,
                    activeBattler[currentTurn].transform.position,
                    activeBattler[currentTurn].transform.rotation);
        DealDamage(selectedTarget, movePower);
        NextTurn();
    }

    // A co-routine is something that can happen outside the normal order of Unity
    public IEnumerator EnemyMoveCoroutine()
    {
        // Setting spacing between enemy attacks
        turnWaiting = false;
        yield return new WaitForSeconds(1f);
        EnemyAttack();
        yield return new WaitForSeconds(1f);
        NextTurn();
    }

    public void EnemyAttack()
    {
        List<int> characters = new List<int>();
        for (int i = 0; i < activeBattler.Count; i++)
        {
            if (activeBattler[i].isPlayer && activeBattler[i].currentHp > 0)
            {
                characters.Add(i);
            }
        }

        int selectedTarget = characters[Random.Range(0, characters.Count)];
        int selectAttack = Random.Range(0, activeBattler[currentTurn].movesAvailable.Length);
        int movePower = 0;
        if (activeBattler[currentTurn].currentHp > 0)
        {
            // Pick random character for enemy to attack
            for (int i = 0; i < movesList.Length; i++)
            {
                if (movesList[i].moveName == activeBattler[currentTurn].movesAvailable[selectAttack])
                {
                    // Instantiate attack effect on appropriate character
                    Instantiate(movesList[i].theEffect,
                                activeBattler[selectedTarget].transform.position,
                                activeBattler[selectedTarget].transform.rotation);
                    // Grab move power of appropriate move
                    movePower = movesList[i].movePower;
                }
            }

            // Effect to show which enemy just attacked
            Instantiate(enemyAttackEffect,
                        activeBattler[currentTurn].transform.position,
                        activeBattler[currentTurn].transform.rotation);

            DealDamage(selectedTarget, movePower);
        }
    }

    public void DealDamage(int target, int movePower)
    {
        // High level boss with a bunch of attack power will do a ton more damage.
        float attackPower = activeBattler[currentTurn].strength + activeBattler[currentTurn].wpnPower;
        float defensePower = activeBattler[target].defence + activeBattler[target].armrPower;
        float randomNum = Random.Range(0.9f, 1.1f);
        float damageCalculation;

        if (defensePower != 0f)
        {
            damageCalculation = (attackPower / defensePower) * movePower * randomNum;
        }
        else
        {
            damageCalculation = 0.1f;
        }

        int damageToGive = Mathf.RoundToInt(damageCalculation);

        /*Debug.Log(activeBattler[currentTurn].charName + " is dealing "
                                                      + damageToGive + " (Mathf.RoundToInt(" + damageCalculation + ")) damage to "
                                                      + activeBattler[target].charName);*/

        activeBattler[target].currentHp -= damageToGive;

        if (activeBattler[target].isPlayer)
        {
            GameManager.instance.playerStats[target].currentHP -= damageToGive;
        }

        Instantiate(damageText, activeBattler[target].transform.position, activeBattler[target].transform.rotation).SetDamage(damageToGive);
        UpdateUICharInfo();
    }

    public void UpdateUICharInfo()
    {
        for (int i = 0; i < charName.Length; i++)
        {
            if (activeBattler.Count > i)
            {
                BattleChar charInfo = activeBattler[i];

                charName[i].gameObject.SetActive(true);
                charName[i].text = charInfo.charName;

                charHP[i].text = Mathf.Clamp(charInfo.currentHp, 0, int.MaxValue) + " / " + charInfo.maxHP;
                charMP[i].text = Mathf.Clamp(charInfo.currentMP, 0, int.MaxValue) + " / " + charInfo.maxMP;
                GameManager.instance.playerStats[i].currentHP = activeBattler[i].currentHp;
                GameManager.instance.playerStats[i].currentMP = activeBattler[i].currentMP;

                if (activeBattler[i].isPlayer)
                {
                    // 
                    
                }
                else
                {
                    charName[i].gameObject.SetActive(false);
                }
            }
            else
            {
                charName[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenTargetMenu(string moveName)
    {
        targetMenu.SetActive(true);
        List<int> enemy = new List<int>();
        for (int i = 0; i < activeBattler.Count; i++)
        {
            if (!activeBattler[i].isPlayer)
            {
                enemy.Add(i);
            }
        }

        for (int i = 0; i < targetButtons.Length; i++)
        {
            if (enemy.Count > i && activeBattler[enemy[i]].currentHp > 0)
            {
                targetButtons[i].gameObject.SetActive(true);

                targetButtons[i].moveName = moveName;
                targetButtons[i].activeBattleTarget = enemy[i];
                targetButtons[i].targetName.text = activeBattler[enemy[i]].charName;
            }
            else
            {
                targetButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenSpellMenu()
    {
        // Bring up spell menu
        spellMenu.SetActive(true);
        // Find out which spells character has and display button
        for (int i = 0; i < spellButtons.Length; i++)
        {
            if (activeBattler[currentTurn].movesAvailable.Length > i)
            {
                spellButtons[i].gameObject.SetActive(true);
                spellButtons[i].spellName = activeBattler[currentTurn].movesAvailable[i];
                spellButtons[i].nameText.text = spellButtons[i].spellName;
                // Find out cost of spell to use
                for (int j = 0; j < movesList.Length; j++)
                {
                    if (movesList[j].moveName == spellButtons[i].spellName)
                    {
                        spellButtons[i].spellCost = movesList[j].moveCost;
                        spellButtons[i].costText.text = spellButtons[i].spellCost.ToString();
                    }
                }
            }
            else
            {
                spellButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // ---------------------BEGIN OF ITEM SECTION--------------------------------------

    public void OpenItemMenu()
    {
        GameManager.instance.SortItems();
        OpenItemBattleCharChoice();
        itemMenu.SetActive(true);
        ShowItems();
    }

    public void ShowItems()
    {
        GameManager.instance.SortItems();

        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i].buttonValue = i;

            if (GameManager.instance.itemsHeld[i] != "")
            {
                itemButtons[i].buttonImage.gameObject.SetActive(true);
                itemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[i]).itemSprite;
                itemButtons[i].amountText.text = GameManager.instance.numberOfItems[i].ToString();
            }
            else
            {
                itemButtons[i].buttonImage.gameObject.SetActive(false);
                itemButtons[i].amountText.text = "";
            }
        }
    }

    // Added
    public void OpenItemBattleCharChoice()
    {
        itemCharChoiceBattleMenu.SetActive(true);

        for (int i = 0; i < itemCharChoiceBattleNames.Length; i++)
        {
            itemCharChoiceBattleNames[i].text = GameManager.instance.playerStats[i].charName;
            itemCharChoiceBattleNames[i].transform.parent.gameObject.SetActive(GameManager.instance.playerStats[i].gameObject.activeInHierarchy);
        }
    }

    public void CloseItemBattleCharChoice()
    {
        itemCharChoiceBattleMenu.SetActive(false);
        itemMenu.SetActive(false);
    }

    public void SelectItem(Item selectedItem)
    {
        if (selectedItem != null)
        {
            activeItem = selectedItem;
            if (selectedItem.isItem)
            {
                // useButtonText.text = "Use";
            }

            if (activeItem.isWeapon || activeItem.isArmor)
            {
                useButtonText.text = "Equip";
            }

            itemName.text = activeItem.itemName;
            itemDescription.text = activeItem.description;
        }     
    }

    /*public void UseItem()
    {
        activeItem.Use();
        GameManager.instance.SortItems();
        UpdateUICharInfo();
        CloseItemBattleCharChoice();

       
    }*/

    public void UseItem(int selected)
    {
        activeItem.Use(selected);
        GameManager.instance.SortItems();
        UpdateUICharInfo();
        CloseItemBattleCharChoice();
        NextTurn();
    }

    public void CloseItemMenu()
    {
        itemMenu.SetActive(false);
    }

    // ------------------------END OF ITEM SECTION---------------------------------------------

    public void Flee()
    {
        int fleeRoll = Random.Range(0, 100);
        if (fleeRoll < chanceToFlee)
        {
            // End Battle
            //TODO: Redo succesfull flee notification
            //fleeRollSuccessful = true;
            fleeing = true;
            StartCoroutine(EndBattleCoroutine());
        }
        else
        {
            // Not successful
            NextTurn();
            battleNotification.theText.text = "Couldn't flee!";
            battleNotification.Activate();
        }
    }

    public IEnumerator EndBattleCoroutine()
    {
        battleActive = false;
        GameManager.instance.battleActive = false;
        uiButtonHolder.SetActive(false);
        targetMenu.SetActive(false);
        spellMenu.SetActive(false);
        //TODO: Close item menu, too
        //itemMenu.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < activeBattler.Count; i++)
        {
            if (activeBattler[i].isPlayer)
            {
                for (int j = 0; j < GameManager.instance.playerStats.Length; j++)
                {
                    if (activeBattler[i].charName == GameManager.instance.playerStats[j].charName)
                    {
                        GameManager.instance.playerStats[j].currentHP = activeBattler[i].currentHp;
                        GameManager.instance.playerStats[j].currentMP = activeBattler[i].currentMP;
                    }
                }
            }

            // Remove characters from memory
            Destroy(activeBattler[i].gameObject);
        }

        UIFade.instance.FadeFromBlack();
        battleScene.SetActive(false);
        activeBattler.Clear();
        currentTurn = 0;
        AudioManager.instance.StopMusic();
        AudioManager.instance.PlayBGM(FindObjectOfType<CameraController>().musicToPlay);


        if (fleeing)
        {
            GameManager.instance.battleActive = false;
            AudioManager.instance.PlayBGM(FindObjectOfType<CameraController>().musicToPlay);
            fleeing = false;
        }
        else
        {
            BattleReward.instance.OpenRewardScreen(rewardExp, rewardItems);
        }
    }

    public IEnumerator GameOverCoroutine()
    {
        battleActive = false;
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(1.5f);
        battleScene.SetActive(false);
        SceneManager.LoadScene(gameOverScene);
    }
}

