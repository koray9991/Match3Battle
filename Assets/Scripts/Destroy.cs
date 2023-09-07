using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Destroy : MonoBehaviour
{
   
    void Start()
    {
        DOVirtual.DelayedCall(3, () => { Destroy(gameObject); });
    }

   
}
