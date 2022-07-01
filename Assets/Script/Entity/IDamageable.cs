using UnityEngine;

public interface IDamageable {

    void TaskHit(float damage, Vector3 hitPos ,Vector3 hitDirection);

    void TaskDamage(float damage);
}
