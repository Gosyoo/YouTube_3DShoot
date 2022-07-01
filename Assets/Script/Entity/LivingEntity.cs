using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable {

    public float startingHp;
    protected float health;
    protected bool death = false;

    public event System.Action OnDeath;  //ע��һ�������¼�

    public bool isDeath {
        get {
            return death;
        }
    }

    protected virtual void Start() {
        health = startingHp;
    }

    //����
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

    //����
    [ContextMenu("Self Destruct")]  //���ԣ��ڽű��һ��˵����һ����ť�����ֶ�����
    protected void Die() {
        if (OnDeath != null) {
            OnDeath(); //���������¼�
        }
        death = true;
        Destroy(gameObject);
    }
}
