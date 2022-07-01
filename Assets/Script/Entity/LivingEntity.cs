using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable {

    public float startingHp;
    protected float health;
    protected bool death = false;

    public event System.Action OnDeath;  //注册一个死亡事件

    public bool isDeath {
        get {
            return death;
        }
    }

    protected virtual void Start() {
        health = startingHp;
    }

    //受伤
    public virtual void TaskHit(float damage, Vector3 hitPos, Vector3 hitDirection) {
        TaskDamage(damage);
    }


    public virtual void TaskDamage(float damage) {
        health -= damage;
        //print($"hp == {health}");
        if (health <= 0 && !death) {
            Die();
        }
    }

    //死亡
    [ContextMenu("Self Destruct")]  //特性：在脚本右击菜单添加一个按钮可以手动销毁
    protected void Die() {
        if (OnDeath != null) {
            OnDeath(); //触发死亡事件
        }
        death = true;
        Destroy(gameObject);
    }
}
