using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    public GameManager gm;
    float heath;
    public float maxHealth;
    public enum Type
    {
        Warrior,
        Archer,
        Bomber
    }
    public Type type;
    public Animator anim;
    public Ally closestTarget;
    public float defaultRange;
    float range;
    public float moveSpeed;
    public bool attack;
    public bool runTrigger, attackTrigger;
    public ParticleSystem bombParticle;
    public GameObject arrow;
    public Transform arrowDefaultPos;
    public ParticleSystem deadParticle;
    public Transform coinParent;
    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        anim = GetComponent<Animator>();
        if (type == Type.Warrior)
        {
            maxHealth = gm.warriorHealth;
        }
        if (type == Type.Archer)
        {
            maxHealth = gm.archerHealth;
        }
        if (type == Type.Bomber)
        {
            maxHealth = gm.bomberHealth;
        }
        heath = maxHealth;
    }
    public void GetDamage(int value)
    {
        heath -= value;
        if (heath <= 0)
        {
            for (int i = 0; i < coinParent.childCount; i++)
            {
                var Mycoin = coinParent.GetChild(i);
                Mycoin.gameObject.SetActive(true);
                Mycoin.parent = null;
                Mycoin.transform.rotation = Quaternion.Euler(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180));
                Mycoin.GetComponent<Rigidbody>().AddForce(Random.Range(-50f, 50f), Random.Range(200f, 400f), Random.Range(-50f, 50f));
               
            }
            gm.EarnMoney(10);
            gm.earnedMoney += 10;
            gm.earnedMoneyText.text = gm.earnedMoney.ToString();
            gm.earnedMoneyText2.text = gm.earnedMoney.ToString();
            deadParticle.transform.parent = null;
            deadParticle.Play();
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (attack)
        {
            FindClosestEnemy();

            if (closestTarget != null)
            {
                if (Vector3.Distance(transform.position, closestTarget.transform.position) > range)
                {
                    range = defaultRange;
                    transform.position = Vector3.MoveTowards(transform.position, closestTarget.transform.position, moveSpeed);
                    var lookPos = closestTarget.transform.position - transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 50);
                    if (!runTrigger)
                    {
                        runTrigger = true;
                        anim.SetTrigger("Run");
                        attackTrigger = false;
                    }

                }
                else
                {
                    range = defaultRange + defaultRange / 10;
                    if (!attackTrigger)
                    {
                        attackTrigger = true;
                        anim.SetTrigger("Attack");
                        runTrigger = false;
                    }

                    var lookPos = closestTarget.transform.position - transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 50);
                    if (type == Type.Bomber)
                    {
                        bombParticle.transform.parent = null;
                        bombParticle.Play();
                        SetDamage(gm.bomberDamage);
                        gm.EarnMoney(10);
                        Destroy(gameObject);
                    }


                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, gm.myBase.transform.position, moveSpeed);
                var lookPos = gm.myBase.transform.position - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 50);
                if (!runTrigger)
                {
                    runTrigger = true;
                    anim.SetTrigger("Run");
                    attackTrigger = false;
                }
                if (Vector3.Distance(transform.position, gm.myBase.position) < 0.2f)
                {
                    gameObject.SetActive(false);
                    if (gm.baseHealth > 0)
                    {
                        gm.baseHealth -= 1;
                        gm.baseHealthText.text = gm.baseHealth.ToString();
                        if (gm.baseHealth <= 0)
                        {
                            for (int i = 0; i < gm.fractureMyBase.childCount; i++)
                            {   
                                gm.fractureMyBase.GetChild(i).gameObject.AddComponent<Rigidbody>();
                                gm.fractureMyBase.GetChild(i).gameObject.AddComponent<BoxCollider>();
                                gm.fractureMyBase.GetChild(i).GetComponent<Rigidbody>().AddForce(Random.Range(-300, 300), Random.Range(0, 300), Random.Range(-300, 300));
                                gm.baseHealthText.enabled = false;
                                gm.explosionMyBase.Play();

                                DOVirtual.DelayedCall(2, () => { gm.failPanel.SetActive(true); });
                            }
                        }
                    }
                    
                    gm.BaseDotween();
                }
            }
        }

    }
    public void ArrowMove()
    {
        arrow.SetActive(true);
        arrow.transform.rotation = Quaternion.Euler(45, 0, 0);
        DOVirtual.DelayedCall(0.3f, () => { arrow.transform.rotation = Quaternion.Euler(-45, 0, 0); });
        if (closestTarget != null)
        {
            arrow.transform.DOJump(closestTarget.transform.position, 3, 1, 1).SetEase(Ease.Linear).OnComplete(() => {
                arrow.transform.position = arrowDefaultPos.position;
                arrow.SetActive(false);
                SetDamage(gm.archerDamage);
            });
        }

    }
    public void SetDamage(int damage)
    {
        if (type == Type.Warrior)
        {
            damage = gm.warriorDamage;
        }

        if (closestTarget != null)
        {
            closestTarget.GetComponent<Ally>().GetDamage(damage);
        }

    }

    void FindClosestEnemy()
    {
        float distanceClosestEnemy = Mathf.Infinity;
        Ally closestEnemy = null;
        Ally[] allEnemies = FindObjectsOfType<Ally>();
        if (allEnemies.Length != 0)
        {
            foreach (Ally currentEnemy in allEnemies)
            {
                float distanceToEnemy = (currentEnemy.transform.position - transform.position).sqrMagnitude;
                if (distanceToEnemy < distanceClosestEnemy)
                {
                    distanceClosestEnemy = distanceToEnemy;
                    closestEnemy = currentEnemy;
                    closestTarget = closestEnemy;
                }
            }

        }
    }

   
}
