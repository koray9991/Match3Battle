using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Tiles : MonoBehaviour
{
    public GameManager gm;
    public Transform gridParent;
    public List<GameObject> colors;
    public GameObject bonusColor;
    public Transform objects;
    public int totalRow, totalColumn;
    int currentRow, currentColumn;
    public List<Transform> instantPoints;
    public LineRenderer lr;
    
    private void Start()
    {
        currentColumn = 1;
        currentRow = 1;
        for (int i = 0; i < gridParent.childCount; i++)
        {
            var newColor = Instantiate(colors[Random.Range(0,colors.Count)], transform.position, Quaternion.identity);
            newColor.transform.parent = objects;
            newColor.transform.localRotation = Quaternion.Euler(0, 0, 0);
            newColor.transform.position = instantPoints[currentColumn - 1].position;
            newColor.transform.DOMove(gridParent.GetChild(i).transform.position,1).SetEase(Ease.OutBounce);
            newColor.GetComponent<Object>().grid = gridParent.GetChild(i).GetComponent<Grid>();
            var myGrid = newColor.GetComponent<Object>().grid;
            myGrid.isEmpty = false;
            newColor.GetComponent<Object>().row = currentRow;
            newColor.GetComponent<Object>().column = currentColumn;
            myGrid.row = currentRow;
            myGrid.column = currentColumn;
            newColor.GetComponent<Object>().text.text = currentRow + "," + currentColumn;
            currentColumn += 1;
            if (currentColumn > totalColumn)
            {
                currentColumn = 1;
                currentRow += 1;
            }
            gm.bonusColorRandomSpawnCount = Random.Range(2, 7);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            NewInstant();
        }
    }
    public void FallObjects()
    {
        for (int i = objects.transform.childCount-1; i >= 0; i--)
        {
            if (objects.GetChild(i).GetComponent<Object>().row < totalRow)
            {
                for (int j = 0; j < gridParent.childCount; j++)
                {
                    if(gridParent.GetChild(j).GetComponent<Grid>().column == objects.GetChild(i).GetComponent<Object>().column)
                    {
                        for (int k = totalRow; k > 0; k--)
                        {
                            if (gridParent.GetChild(j).GetComponent<Grid>().row == objects.GetChild(i).GetComponent<Object>().row + k)
                            {
                                var underGrid = gridParent.GetChild(j).GetComponent<Grid>();
                                if (underGrid.isEmpty)
                                {
                                    var myObject = objects.GetChild(i);
                                    myObject.GetComponent<Object>().grid.isEmpty = true;
                                    myObject.transform.DOMove(underGrid.transform.position, 1).SetEase(Ease.OutBounce);
                                    myObject.GetComponent<Object>().grid = underGrid;
                                    underGrid.isEmpty = false;
                                    myObject.GetComponent<Object>().row = underGrid.row;
                                    myObject.GetComponent<Object>().underGrid = underGrid.gameObject;
                                }
                            }
                        }

                       
                       
                    }
                }
            }
        }
    }
    public void NewInstant()
    {
        for (int i = 0; i < gridParent.childCount; i++)
        {
            if (gridParent.GetChild(i).GetComponent<Grid>().isEmpty)
            {
                var newColor = gameObject;
                if (gm.moveCountForBonusColor == gm.bonusColorRandomSpawnCount)
                {
                    gm.moveCountForBonusColor = 0;
                    gm.bonusColorRandomSpawnCount = Random.Range(2, 7);
                    newColor = Instantiate(bonusColor, transform.position, Quaternion.identity);
                }
                else
                {
                    newColor = Instantiate(colors[Random.Range(0, colors.Count)], transform.position, Quaternion.identity);
                }
               
                newColor.transform.parent = objects;
                newColor.transform.localRotation = Quaternion.Euler(0, 0, 0);
                newColor.transform.SetSiblingIndex(0);
                newColor.GetComponent<Object>().grid = gridParent.GetChild(i).GetComponent<Grid>();
                var myGrid = newColor.GetComponent<Object>().grid;
                myGrid.isEmpty = false;
                newColor.GetComponent<Object>().row = myGrid.row;
                newColor.GetComponent<Object>().column = myGrid.column;
                newColor.GetComponent<Object>().text.text = newColor.GetComponent<Object>().row + "," + newColor.GetComponent<Object>().column;
                newColor.transform.position = instantPoints[myGrid.column-1].position;
                newColor.transform.DOMove(gridParent.GetChild(i).transform.position, 1).SetEase(Ease.OutBounce);
            }
           
        }
    }

    public void Sibling()
    {
        for (int i = 0; i < objects.childCount; i++)
        {
            var myObject = objects.GetChild(i).GetComponent<Object>();
            myObject.transform.SetSiblingIndex(myObject.grid.transform.GetSiblingIndex()); 
        }
    }
}
