using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Coin : MonoBehaviour
{
    public GameManager gm;
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        DOVirtual.DelayedCall(3, () => { transform.DOMove(gm.myBase.position, 0.5f).OnComplete(() => {
            gameObject.SetActive(false);
        });
        });
    }

   
}
