using UnityEngine;

public interface IDamageable {

    void TaskHit(float damage, RaycastHit hit);

    void TaskDamage(float damage);
}
