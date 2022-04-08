using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{

    public float startingHp;
    protected float health;
    protected bool death;

    public event System.Action OnDeath;  //ע��һ�������¼�

    protected virtual void Start()
    {
        health = startingHp;
    }

    //����
    public void TaskHit(float damage, Collider hit)
    {
        health -= damage;
        print($"hp == {health}");
        if (health <= 0 && !death)
        {
            Die();
        }
    }

    //����
    protected void Die()
    {
        if (OnDeath != null)
        {
            OnDeath();
        }
        death = true;
        Destroy(gameObject);
    }
}
