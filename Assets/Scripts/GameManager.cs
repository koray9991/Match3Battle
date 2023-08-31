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
    }

    public void LrEnabledFalse()
    {
        for (int i = 0; i < lr.Count; i++)
        {
            lr[i].enabled = false;
        }
    }
    public void FightButton()
    {
        vCamMerge.gameObject.SetActive(false);
        vCamFight.gameObject.SetActive(true);
    
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
