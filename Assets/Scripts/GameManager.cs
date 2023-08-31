using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
public class GameManager : MonoBehaviour
{
    public Transform nodes;
    public Transform enemyNodes;
    public GameObject warrior,bomber,archer;
    public GameObject enemyWarrior;
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
    private void Start()
    {
        for (int i = 0; i < lines.childCount; i++)
        {
            lr.Add(lines.GetChild(i).GetComponent<LineRenderer>());
        }
        LrEnabledFalse();
        turnCountText.text = turnCount.ToString();
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

}
