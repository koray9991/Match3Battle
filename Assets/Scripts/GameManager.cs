using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public Transform nodes;
    public Transform enemyNodes;
    public GameObject warrior,bomber,archer;
    public GameObject enemyWarrior, enemyArcher, enemyBomber;
    public int bonusColorRandomSpawnCount;
    public int moveCountForBonusColor;
    public List<GameObject> enemiesList;
   public enum Colours
    {
        none,
        blue,
        red,
        green,
        pink,
        bonus
    }
    public Colours currentColor;
    public List<GameObject> currentObjects;
    public bool holded;
    public int holdedObjectRowValue, holdedObjectColumnValue;
    public Transform lines;
    public List<LineRenderer> lr;
    public int turnCount;
    public TextMeshPro turnCountText;
    public GameObject uiPart;
    public GameObject fightButton;
    public CinemachineVirtualCamera vCamMerge, vCamFight;
    public List<Ally> allies;
    public List<Enemy> enemies;
    public int spawnEnemyCount;
    public Transform enemySpawnPos;
    public int enemyRandom;

    public int warriorHealth, archerHealth, bomberHealth;
    public int warriorDamage, archerDamage, bomberDamage;

    public Transform myBase,enemyBase;

    public Transform fractureEnemyBase,fractureMyBase;

    public ParticleSystem explosionEnemyBase,explosionMyBase;

    public float baseHealth, enemyBaseHealth;
    public TextMeshPro baseHealthText, enemyBaseHealthText;

    public bool fightMode;
    public int aliveAllies, aliveEnemies;
    public GameObject winPanel,failPanel;

    public int warriorCount, archerCount, bomberCount;
    public TextMeshProUGUI warriorCountText, archerCountText, bomberCountText;

    public int enemyWarriorCount, enemyArcherCount, enemyBomberCount;
    public TextMeshProUGUI enemyWarriorCountText, enemyArcherCountText, enemyBomberCountText;

    public List<GameObject> warriorList, archerList, bomberList;
    public List<GameObject> enemyWarriorList, enemyArcherList, enemyBomberList;

    float countControlTimer;
    public GameObject healthObject;
    public List<Image> purpleImages,greenImages,yellowImages,pinkImages;
    public int money;
    public TextMeshPro moneyText;
    public TextMeshProUGUI moneyTextGUI,moneyTextGUI2;

    public int warriorLevel;
    public TextMeshProUGUI warriorLevelText;
    public int warriorCost;
    public TextMeshProUGUI warriorCostText;

    public int archerLevel;
    public TextMeshProUGUI archerLevelText;
    public int archerCost;
    public TextMeshProUGUI archerCostText;

    public int bomberLevel;
    public TextMeshProUGUI bomberLevelText;
    public int bomberCost;
    public TextMeshProUGUI bomberCostText;

    public int earnedMoney;
    public TextMeshProUGUI earnedMoneyText,earnedMoneyText2;

    public bool enemyTurn;
    public GameObject enemyTurnCircle;
    public Image enemyTurnAmount;
    public GameObject enemyTurnLock;
    
    private void Start()
    {
        for (int i = 0; i < lines.childCount; i++)
        {
            lr.Add(lines.GetChild(i).GetComponent<LineRenderer>());
        }
        LrEnabledFalse();
        turnCountText.text = turnCount.ToString();
        baseHealthText.text = baseHealth.ToString();
        enemyBaseHealthText.text = enemyBaseHealth.ToString();
        countControlTimer = 1;
        EarnMoney(0);

        Prefs();
    }
    public void EarnMoney(int value)
    {
        money=PlayerPrefs.GetInt("money");
        money += value;
        PlayerPrefs.SetInt("money", money);
        moneyText.text = money.ToString();
        moneyTextGUI.text = money.ToString();

    }
    public void NextLevelButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Prefs()
    {
        warriorLevel = PlayerPrefs.GetInt("warriorLevel");
        if (warriorLevel == 0) { warriorLevel = 1; PlayerPrefs.SetInt("warriorLevel", warriorLevel); }
        warriorLevelText.text = "LEVEL " + warriorLevel;

        warriorCost = PlayerPrefs.GetInt("warriorCost");
        if (warriorCost == 0) { warriorCost = 500; PlayerPrefs.SetInt("warriorCost", warriorCost); }
        warriorCostText.text = warriorCost.ToString();

        warriorHealth = PlayerPrefs.GetInt("warriorHealth");
        if (warriorHealth == 0) { warriorHealth = 50; PlayerPrefs.SetInt("warriorHealth", warriorHealth); }

        warriorDamage = PlayerPrefs.GetInt("warriorDamage");
        if (warriorDamage == 0) { warriorDamage = 4; PlayerPrefs.SetInt("warriorDamage", warriorDamage); }


        archerLevel = PlayerPrefs.GetInt("archerLevel");
        if (archerLevel == 0) { archerLevel = 1; PlayerPrefs.SetInt("archerLevel", archerLevel); }
        archerLevelText.text = "LEVEL " + archerLevel;

        archerCost = PlayerPrefs.GetInt("archerCost");
        if (archerCost == 0) { archerCost = 500; PlayerPrefs.SetInt("archerCost", archerCost); }
        archerCostText.text = archerCost.ToString();

        archerHealth = PlayerPrefs.GetInt("archerHealth");
        if (archerHealth == 0) { archerHealth = 20; PlayerPrefs.SetInt("archerHealth", archerHealth); }

        archerDamage = PlayerPrefs.GetInt("archerDamage");
        if (archerDamage == 0) { archerDamage = 6; PlayerPrefs.SetInt("archerDamage", archerDamage); }





        bomberLevel = PlayerPrefs.GetInt("bomberLevel");
        if (bomberLevel == 0) { bomberLevel = 1; PlayerPrefs.SetInt("bomberLevel", bomberLevel); }
        bomberLevelText.text = "LEVEL " + bomberLevel;

        bomberCost = PlayerPrefs.GetInt("bomberCost");
        if (bomberCost == 0) { bomberCost = 500; PlayerPrefs.SetInt("bomberCost", bomberCost); }
        bomberCostText.text = bomberCost.ToString();

        bomberHealth = PlayerPrefs.GetInt("bomberHealth");
        if (bomberHealth == 0) { bomberHealth = 20; PlayerPrefs.SetInt("bomberHealth", bomberHealth); }

        bomberDamage = PlayerPrefs.GetInt("bomberDamage");
        if (bomberDamage == 0) { bomberDamage = 50; PlayerPrefs.SetInt("bomberDamage", bomberDamage); }
    }
    public void WarriorUpgrade()
    {
        if (money >= warriorCost)
        {
            EarnMoney(-warriorCost);

            warriorLevel += 1;
            PlayerPrefs.SetInt("warriorLevel", warriorLevel);
            warriorLevelText.text = "LEVEL " + warriorLevel;
            warriorCost += 100;
            PlayerPrefs.SetInt("warriorCost", warriorCost);
            warriorCostText.text = warriorCost.ToString();

            warriorHealth += 5; 
            PlayerPrefs.SetInt("warriorHealth", warriorHealth);

            warriorDamage += 1; 
            PlayerPrefs.SetInt("warriorDamage", warriorDamage);
        }
    }
    public void ArcherUpgrade()
    {
        if (money >= archerCost)
        {
            EarnMoney(-archerCost);

            archerLevel += 1;
            PlayerPrefs.SetInt("archerLevel", archerLevel);
            archerLevelText.text = "LEVEL " + archerLevel;
            archerCost += 100;
            PlayerPrefs.SetInt("archerCost", archerCost);
            archerCostText.text = archerCost.ToString();

            archerHealth += 4;
            PlayerPrefs.SetInt("archerHealth", archerHealth);

            archerDamage += 1;
            PlayerPrefs.SetInt("archerDamage", archerDamage);
        }
    }

    public void BomberUpgrade()
    {
        if (money >= bomberCost)
        {
            EarnMoney(-bomberCost);

            bomberLevel += 1;
            PlayerPrefs.SetInt("bomberLevel", bomberLevel);
            bomberLevelText.text = "LEVEL " + bomberLevel;
            bomberCost += 100;
            PlayerPrefs.SetInt("bomberCost", bomberCost);
            bomberCostText.text = bomberCost.ToString();

            bomberHealth += 4;
            PlayerPrefs.SetInt("bomberHealth", bomberHealth);

            bomberDamage += 1;
            PlayerPrefs.SetInt("bomberDamage", bomberDamage);
        }
    }
    public void Bar()
    {
        switch (currentColor)
        {
            case Colours.none:
                purpleImages[0].fillAmount = currentObjects.Count;
                purpleImages[1].fillAmount = currentObjects.Count;
                greenImages[0].fillAmount = (float)currentObjects.Count / 20;
                greenImages[1].fillAmount = (float)currentObjects.Count / 20;
                yellowImages[0].fillAmount = (float)currentObjects.Count / 20;
                yellowImages[1].fillAmount = (float)currentObjects.Count / 20;
                pinkImages[0].fillAmount = (float)currentObjects.Count / 20;
                pinkImages[1].fillAmount = (float)currentObjects.Count / 20;
                break;
            case Colours.blue:
                purpleImages[0].fillAmount = (float)currentObjects.Count / 20;
                purpleImages[1].fillAmount = (float)currentObjects.Count / 20;
                break;
            case Colours.green:
                greenImages[0].fillAmount = (float)currentObjects.Count / 20;
                greenImages[1].fillAmount = (float)currentObjects.Count / 20;
                break;
            case Colours.red:
                yellowImages[0].fillAmount = (float)currentObjects.Count / 20;
                yellowImages[1].fillAmount = (float)currentObjects.Count / 20;
                break;
            case Colours.pink:
                pinkImages[0].fillAmount = (float)currentObjects.Count / 20;
                pinkImages[1].fillAmount = (float)currentObjects.Count / 20;
                break;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            EarnMoney(1000);
        }
       
        
      
        CountControl();
        if (fightMode)
        {
            if (aliveEnemies==0 && aliveAllies == 0)
            {
                fightMode = false;
                vCamMerge.gameObject.SetActive(true);
                vCamFight.gameObject.SetActive(false);

                fightButton.SetActive(false);

                allies.Clear();
                enemies.Clear();

                turnCount = 4;
                turnCountText.text = turnCount.ToString();
                DOVirtual.DelayedCall(2, () => {
                    if (enemyBaseHealth <= 0)
                    {
                        winPanel.SetActive(true);
                    }
                    else
                    {
                        uiPart.SetActive(true);
                    }
                    
                });


                for (int i = 0; i <nodes.childCount ; i++)
                {
                    nodes.GetChild(i).GetComponent<Node>().isEmpty = true;
                }
                for (int i = 0; i < enemyNodes.childCount; i++)
                {
                    enemyNodes.GetChild(i).GetComponent<Node>().isEmpty = true;
                }

             
            }
        }
    }
    public void LrEnabledFalse()
    {
        for (int i = 0; i < lr.Count; i++)
        {
            lr[i].enabled = false;
        }
    }
    public void CountControl()
    {
        countControlTimer += Time.deltaTime;
        if (countControlTimer > 0.5f)
        {
            countControlTimer = 0;
            warriorList.Clear();
            archerList.Clear();
            bomberList.Clear();
            enemyWarriorList.Clear();
            enemyArcherList.Clear();
            enemyBomberList.Clear();
            aliveAllies = FindObjectsOfType<Ally>().Length;
            aliveEnemies = FindObjectsOfType<Enemy>().Length;

            foreach (Ally ally in FindObjectsOfType<Ally>())
            {
                if (ally.type == Ally.Type.Warrior)
                {
                    warriorList.Add(ally.gameObject);
                }
                if (ally.type == Ally.Type.Archer)
                {
                    archerList.Add(ally.gameObject);
                }
                if (ally.type == Ally.Type.Bomber)
                {
                    bomberList.Add(ally.gameObject);
                }
                
            }
            warriorCount = warriorList.Count;
            warriorCountText.text = warriorCount.ToString();
            archerCount = archerList.Count;
            archerCountText.text = archerCount.ToString();
            bomberCount = bomberList.Count;
            bomberCountText.text = bomberCount.ToString();
            foreach (Enemy enemy in FindObjectsOfType<Enemy>())
            {
                if (enemy.type == Enemy.Type.Warrior)
                {
                    enemyWarriorList.Add(enemy.gameObject);
                }
                if (enemy.type == Enemy.Type.Archer)
                {
                    enemyArcherList.Add(enemy.gameObject);
                }
                if (enemy.type == Enemy.Type.Bomber)
                {
                    enemyBomberList.Add(enemy.gameObject);
                }
              
            }
            enemyWarriorCount = enemyWarriorList.Count;
            enemyWarriorCountText.text = enemyWarriorCount.ToString();
            enemyArcherCount = enemyArcherList.Count;
            enemyArcherCountText.text = enemyArcherCount.ToString();
            enemyBomberCount = enemyBomberList.Count;
            enemyBomberCountText.text = enemyBomberCount.ToString();
        }
    }
    public void FightButton()
    {
        fightMode = true;
      
    
        fightButton.SetActive(false);

        foreach (Ally ally in FindObjectsOfType<Ally>())
        {
            allies.Add(ally);
        }
        for (int i = 0; i < allies.Count; i++)
        {
            allies[i].attack=true;
        }
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemies.Add(enemy);
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].attack = true;
        }
    }
    public void EnemyBaseDotween()
    {
        if (enemyBaseHealth > 0)
        {
            enemyBase.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f, 1, 1).OnComplete(() => { enemyBase.transform.localScale = Vector3.one * 0.3f; });
        }
       
    }
    public void BaseDotween()
    {
        if (baseHealth > 0)
        {
            myBase.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f, 1, 1).OnComplete(() => { myBase.transform.localScale = Vector3.one * 0.3f; });
        }
        
    }
}
