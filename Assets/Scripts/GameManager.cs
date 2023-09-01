using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using DG.Tweening;
public class GameManager : MonoBehaviour
{
    public Transform nodes;
    public Transform enemyNodes;
    public GameObject warrior,bomber,archer;
    public GameObject enemyWarrior, enemyArcher, enemyBomber;
    public int bonusColorRandomSpawnCount;
    public int moveCountForBonusColor;

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

    public float baseHealth, enemyBaseHealth;
    public TextMeshPro baseHealthText, enemyBaseHealthText;

    public bool fightMode;
    public int aliveAllies, aliveEnemies;
    public GameObject winPanel;

    public int warriorCount, archerCount, bomberCount;
    public TextMeshProUGUI warriorCountText, archerCountText, bomberCountText;

    public int enemyWarriorCount, enemyArcherCount, enemyBomberCount;
    public TextMeshProUGUI enemyWarriorCountText, enemyArcherCountText, enemyBomberCountText;

    public List<GameObject> warriorList, archerList, bomberList;
    public List<GameObject> enemyWarriorList, enemyArcherList, enemyBomberList;

    float countControlTimer;
    public GameObject healthObject;
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
    }
    private void Update()
    {

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
        enemyBase.transform.DOPunchScale(Vector3.one*0.1f, 0.1f, 1, 1).OnComplete(() => { enemyBase.transform.localScale = Vector3.one*0.3f; });
    }
    public void BaseDotween()
    {
        myBase.transform.DOPunchScale(Vector3.one*0.1f, 0.1f, 1, 1).OnComplete(() => { myBase.transform.localScale = Vector3.one * 0.3f; });
    }
}
