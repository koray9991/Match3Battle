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
        pink,
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
        if (color == Colors.bonus || gm.turnCount <= 0 || gm.enemyTurn)
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
        if (color == Colors.pink)
        {
            gm.currentColor = GameManager.Colours.pink;
        }
        transform.DOScale(transform.localScale * 1.5f, 0.1f).OnComplete(() => { transform.DOScale(transform.localScale / 1.5f, 0.1f); });
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
                DOVirtual.DelayedCall(3, () => {
                    gm.uiPart.SetActive(false);
                    gm.fightButton.SetActive(true);
                    gm.vCamMerge.gameObject.SetActive(false);
                    gm.vCamFight.gameObject.SetActive(true);
                });
                
            }
            for (int i = 0; i < gm.currentObjects.Count; i++)
            {

                for (int j = 0; j < gm.nodes.childCount; j++)
                {
                    if (gm.nodes.GetChild(j).GetComponent<Node>().isEmpty)
                    {
                       
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
                        if (gm.currentObjects[0].GetComponent<Object>().color == Colors.pink)
                        {
                            newArmy = Instantiate(gm.healthObject, gm.currentObjects[i].transform.position, Quaternion.identity);
                        }
                        if (gm.currentObjects[i].GetComponent<Object>().color == Colors.bonus)
                        {
                            newArmy.transform.localScale = newArmy.transform.localScale * 2;
                            newArmy.GetComponent<Ally>().isBig = true;
                        }

                        if(gm.currentObjects[0].GetComponent<Object>().color != Colors.pink)
                        {
                            var node = gm.nodes.GetChild(j).GetComponent<Node>();
                            node.isEmpty = false;
                            node.character = newArmy.transform;
                            newArmy.transform.DOJump(node.transform.position, 3, 1, 1).SetEase(Ease.Linear).OnComplete(() => {
                                newArmy.GetComponent<Animator>().SetTrigger("Idle");
                            });
                        }
                        else
                        {
                            gm.baseHealth += 1;
                            newArmy.transform.DOJump(gm.myBase.transform.position, 5, 1, 1).SetEase(Ease.Linear).OnComplete(() => {
                                
                                gm.baseHealthText.text = gm.baseHealth.ToString();
                            });
                        }
                       
                        break;
                    }
                }
                gm.currentObjects[i].GetComponent<Object>().grid.isEmpty = true;
                Destroy(gm.currentObjects[i].gameObject);
            }
            gm.enemyTurn = true;
            gm.enemyTurnCircle.SetActive(true);
            gm.enemyTurnAmount.fillAmount = 0;
            gm.enemyTurnLock.SetActive(true);
            DOTween.To(x => gm.enemyTurnAmount.fillAmount = x, gm.enemyTurnAmount.fillAmount, 1, 3).OnComplete(() => {
                gm.enemyRandom = Random.Range(0, gm.enemiesList.Count);
                gm.spawnEnemyCount = Random.Range(3, 7);
                for (int i = 0; i < gm.spawnEnemyCount; i++)
                {

                    for (int j = 0; j < gm.enemyNodes.childCount; j++)
                    {

                        if (gm.enemyNodes.GetChild(j).GetComponent<Node>().isEmpty)
                        {
                            var node = gm.enemyNodes.GetChild(j).GetComponent<Node>();
                            node.isEmpty = false;
                            var newArmy = gm.gameObject;
                            newArmy = Instantiate(gm.enemiesList[gm.enemyRandom], gm.enemySpawnPos.position, Quaternion.identity);
                            newArmy.transform.rotation = Quaternion.Euler(0, 180, 0);
                            newArmy.transform.DOJump(node.transform.position, 3, 1, 1).SetEase(Ease.Linear).OnComplete(() => {
                                newArmy.GetComponent<Animator>().SetTrigger("Idle");
                            });
                            break;
                        }
                    }

                }
                gm.enemyTurnCircle.SetActive(false);
                gm.enemyTurn = false;
                gm.enemyTurnLock.SetActive(false);
            });
           
               
              
          
        }
      
       
       
        gm.currentColor = GameManager.Colours.none;
        gm.currentObjects.Clear();
        gm.holded = false;
        gm.holdedObjectRowValue = 0;
        gm.holdedObjectColumnValue = 0;
        gm.moveCountForBonusColor += 1;
        gm.LrEnabledFalse();
        DOVirtual.DelayedCall(0.2f,()=> tiles.FallObjects());
        DOVirtual.DelayedCall(0.5f, () => tiles.NewInstant());
        DOVirtual.DelayedCall(1f, () => tiles.Sibling());
        gm.Bar();
       
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
                    transform.DOScale(transform.localScale * 1.5f, 0.1f).OnComplete(() => { transform.DOScale(transform.localScale / 1.5f, 0.1f); });
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
            gm.Bar();
           
        }
    }

   
}
