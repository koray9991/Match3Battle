using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Enemy : MonoBehaviour
{
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

    private void Start()
    {
        anim = GetComponent<Animator>();
        heath = maxHealth;
    }
    public void GetDamage(int value)
    {
        heath -= value;
        if (heath <= 0)
        {
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
                        SetDamage(30);
                        Destroy(gameObject);
                    }


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
            arrow.transform.DOJump(closestTarget.transform.position, 3, 1, 1).OnComplete(() => {
                arrow.transform.position = arrowDefaultPos.position;
                arrow.SetActive(false);
                SetDamage(3);
            });
        }

    }
    public void SetDamage(int damage)
    {
        if (type == Type.Warrior)
        {
            damage = 5;
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
