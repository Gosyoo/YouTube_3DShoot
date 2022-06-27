using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public LayerMask layer; //攻击检测的层级
    public float damage = 1;
    float speed = 10;
    float liveTime = 3;
    float skinWidth = .1f;

    void Start() {
        Destroy(gameObject, liveTime);  //销毁
        //当在攻击对象内部生成时，使用碰撞检测，射线检测检测不到
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, layer);
        if (initialCollisions.Length > 0) {
            HitObject(initialCollisions[0]);
        }
    }

    public void SetSpeed(float newSpeed) {
        speed = newSpeed;
    }

    void FixedUpdate() {
        float moveDir = speed * Time.fixedDeltaTime; //一次刷新移动的距离
        CheckCollection(moveDir);
        transform.Translate(Vector3.forward * moveDir);
    }

    //检测碰撞
    void CheckCollection(float moveDir) {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;  //碰撞检测到的物体
        Debug.DrawLine(transform.position, (transform.position + transform.forward), Color.red);

        //物理射线检测（检测射线前下一帧移动距离内的，且为layer层级的，且可以是触发器的碰撞器）
        //注：该方法需要在FixedUpdata中使用
        //Bug:当有一发子弹未销毁是，该方法会不再检测触发器，只能同时存在一个射线检测去检测触发器；
        //解决：项目设置-->物理-->Auto Sync Transforms(确定勾选)  2018.2之后默认为false，会有些微性能消耗
        if (Physics.Raycast(ray, out hit, moveDir + skinWidth, layer, QueryTriggerInteraction.Collide)) {
            HitObject(hit);
        }
    }

    //使用触发检测代替射线检测
    //private void OnTriggerEnter(Collider other)
    //{
    //    HitObject(other);
    //}

    //击中方法
    void HitObject(RaycastHit hit) {
        //print(hit.GetType().Name);
        IDamageable damageable = hit.collider.GetComponent<IDamageable>();  //获取对象脚本
        if (damageable != null) {
            damageable.TaskHit(damage, hit);
        }
        //TODO：销毁子弹
        Destroy(gameObject);
    }

    void HitObject(Collider collider) {
        IDamageable damageable = collider.GetComponent<IDamageable>();
        if (damageable != null) {
            damageable.TaskDamage(damage);
        }
        GameObject.Destroy(gameObject);
    }
}
