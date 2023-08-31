using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
public class Object : MonoBehaviour ,IPointerDownHandler,IPointerUpHandler,IPointerEnterHandler
{
    public GameManager gm;
    public Tiles tiles;
    public Grid grid;
    public GameObject underGrid;
    public enum Colors
    {
        blue,
        red,
        green,
        bonus
    }
    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        tiles = FindObjectOfType<Tiles>();
    }

    public Colors color;
    public int row, column;
    public TextMeshProUGUI text;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (color == Colors.bonus || gm.turnCount <= 0)
        {
            return;
        }
        if (color == Colors.blue)
        {
            gm.currentColor = GameManager.Colours.blue;
        }
        if (color == Colors.red)
        {
            gm.currentColor = GameManager.Colours.red;
        }
        if (color == Colors.green)
        {
            gm.currentColor = GameManager.Colours.green;
        }
      
        gm.currentObjects.Add(gameObject);
        gm.holded = true;
        gm.holdedObjectRowValue = row;
        gm.holdedObjectColumnValue = column;
    }




    public void OnPointerUp(PointerEventData eventData)
    {
        if (gm.currentObjects.Count > 2)
        {
            gm.turnCount -= 1;
            gm.turnCountText.text = gm.turnCount.ToString();
            if (gm.turnCount <= 0)
            {
                gm.uiPart.SetActive(false);
                gm.fightButton.SetActive(true);
            }
            for (int i = 0; i < gm.currentObjects.Count; i++)
            {

                for (int j = 0; j < gm.nodes.childCount; j++)
                {
                    if (gm.nodes.GetChild(j).GetComponent<Node>().isEmpty)
                    {
                        var node = gm.nodes.GetChild(j).GetComponent<Node>();
                        node.isEmpty = false;
                        var newArmy = gameObject;
                        if (gm.currentObjects[0].GetComponent<Object>().color == Colors.blue)
                        {
                            newArmy = Instantiate(gm.warrior, gm.currentObjects[i].transform.position, Quaternion.identity);
                        }
                        if (gm.currentObjects[0].GetComponent<Object>().color == Colors.red)
                        {
                            newArmy = Instantiate(gm.bomber, gm.currentObjects[i].transform.position, Quaternion.identity);
                        }
                        if (gm.currentObjects[0].GetComponent<Object>().color == Colors.green)
                        {
                            newArmy = Instantiate(gm.archer, gm.currentObjects[i].transform.position, Quaternion.identity);
                        }
                        if (gm.currentObjects[i].GetComponent<Object>().color == Colors.bonus)
                        {
                            newArmy.transform.localScale = newArmy.transform.localScale * 2;
                        }
                        node.character = newArmy.transform;
                        newArmy.transform.DOJump(node.transform.position, 5, 1, 1).SetEase(Ease.Linear).OnComplete(() => {
                            newArmy.GetComponent<Animator>().SetTrigger("Idle");
                        });
                        break;
                    }
                }
                gm.currentObjects[i].GetComponent<Object>().grid.isEmpty = true;
                Destroy(gm.currentObjects[i].gameObject);
            }
           
        }
        for (int i = 0; i < gm.spawnEnemyCount; i++)
        {
            for (int j = 0; j < gm.enemyNodes.childCount; j++)
            {
                if (gm.enemyNodes.GetChild(j).GetComponent<Node>().isEmpty)
                {
                    var node = gm.enemyNodes.GetChild(j).GetComponent<Node>();
                    node.isEmpty = false;
                    var newArmy = gameObject;
                    newArmy = Instantiate(gm.enemyWarrior, gm.enemySpawnPos.position, Quaternion.identity);
                    newArmy.transform.rotation = Quaternion.Euler(0, 180, 0);
                    newArmy.transform.DOJump(node.transform.position, 5, 1, 1).SetEase(Ease.Linear).OnComplete(() => {
                        newArmy.GetComponent<Animator>().SetTrigger("Idle");
                    });
                    break;
                }
            }
                    
        }
        gm.currentColor = GameManager.Colours.none;
        gm.currentObjects.Clear();
        gm.holded = false;
        gm.holdedObjectRowValue = 0;
        gm.holdedObjectColumnValue = 0;
        gm.moveCountForBonusColor += 1;
        gm.LrEnabledFalse();
        DOVirtual.DelayedCall(0.1f,()=> tiles.FallObjects());
        DOVirtual.DelayedCall(0.15f, () => tiles.NewInstant());
        DOVirtual.DelayedCall(0.2f, () => tiles.Sibling());

       
        // 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gm.holded)
        {
            if((gm.currentColor.ToString()== color.ToString() || color==Colors.bonus) && !gm.currentObjects.Contains(gameObject))
            {
                if(Mathf.Abs(gm.holdedObjectRowValue - row) < 2 && Mathf.Abs(gm.holdedObjectColumnValue - column) < 2)
                {
                 
                    gm.currentObjects.Add(gameObject);
                    gm.holdedObjectRowValue = row;
                    gm.holdedObjectColumnValue = column;
                    gm.lr[gm.currentObjects.Count - 1].enabled = true;
                    gm.lr[gm.currentObjects.Count - 1].SetPosition(0, gm.currentObjects[gm.currentObjects.Count - 2].transform.position+new Vector3(0,0,0.1f));
                    gm.lr[gm.currentObjects.Count - 1].SetPosition(1, gm.currentObjects[gm.currentObjects.Count - 1].transform.position + new Vector3(0, 0, 0.1f));
                }
                       
            }
            if (gm.currentObjects.Count > 1)
            {
                if (gameObject == gm.currentObjects[gm.currentObjects.Count - 2])
                {
                    gm.lr[gm.currentObjects.Count - 1].enabled = false;
                    gm.currentObjects.RemoveAt(gm.currentObjects.Count - 1);
                    gm.holdedObjectRowValue = row;
                    gm.holdedObjectColumnValue = column;
                    
                }
            }
          
        }
    }

   
}
