using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{

    public float startingHp;
    protected float health;
    protected bool death;

    public event System.Action OnDeath;  //注册一个死亡事件

    protected virtual void Start()
    {
        health = startingHp;
    }

    //受伤
    public void TaskHit(float damage, Collider hit)
    {
        health -= damage;
        print($"hp == {health}");
        if (health <= 0 && !death)
        {
            Die();
        }
    }

    //死亡
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
