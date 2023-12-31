using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
public class Ally : MonoBehaviour
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
    public Enemy closestTarget;
    public float defaultRange;
    float range;
    public float moveSpeed;
    public bool attack;
    public bool runTrigger,attackTrigger;
    public ParticleSystem bombParticle;
    public GameObject arrow;
    public Transform arrowDefaultPos;
    public ParticleSystem deadParticle;
    public bool isBig;
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
            anim.SetFloat("speed", Random.Range(0.7f, 1.3f));
        }
        if (type == Type.Bomber)
        {
            maxHealth = gm.bomberHealth;
        }
        if (isBig)
        {
            maxHealth = maxHealth * 2;
        }
        heath = maxHealth;
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
                        Destroy(gameObject);
                    }
                }
            }
            else
            {
             
                  transform.position = Vector3.MoveTowards(transform.position, gm.enemyBase.transform.position, moveSpeed);
                var lookPos = gm.enemyBase.transform.position - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 50);
                if (!runTrigger)
                {
                    runTrigger = true;
                    anim.SetTrigger("Run");
                    attackTrigger = false;
                }

                if (Vector3.Distance(transform.position, gm.enemyBase.position) < 0.2f)
                {
                    gameObject.SetActive(false);
                    if (gm.enemyBaseHealth > 0)
                    {
                        gm.enemyBaseHealth -= 1;
                        gm.enemyBaseHealthText.text = gm.enemyBaseHealth.ToString();
                        if (gm.enemyBaseHealth <= 0)
                        {
                            for (int i = 0; i < gm.fractureEnemyBase.childCount; i++)
                            {
                                gm.fractureEnemyBase.GetChild(i).gameObject.AddComponent<Rigidbody>();
                                gm.fractureEnemyBase.GetChild(i).gameObject.AddComponent<BoxCollider>();
                                gm.fractureEnemyBase.GetChild(i).GetComponent<Rigidbody>().AddForce(Random.Range(-300, 300), Random.Range(0, 300), Random.Range(-300, 300));
                                gm.enemyBaseHealthText.enabled = false;
                                gm.explosionEnemyBase.Play();
                            }  
                        }
                    }
                    gm.EnemyBaseDotween();
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

        if(closestTarget != null)
        {
            if (closestTarget.GetComponent<Enemy>())
            {
                closestTarget.GetComponent<Enemy>().GetDamage(damage);
            }
           
        }
        
    }


    void FindClosestEnemy()
    {
        float distanceClosestEnemy = Mathf.Infinity;
        Enemy closestEnemy = null;
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        if (allEnemies.Length != 0)
        {
            foreach (Enemy currentEnemy in allEnemies)
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

    public void GetDamage(int value)
    {
        heath -= value;
        if (heath <= 0)
        {
           
            deadParticle.transform.parent = null;
            deadParticle.Play();
            anim.SetTrigger("Death");
            gameObject.AddComponent<Destroy>();
            Destroy(GetComponent<Ally>());
         //   Destroy(gameObject);
        }
    }
}