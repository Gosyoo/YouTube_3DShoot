using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public LayerMask layer; //攻击检测的层级
    public float damage = 1;
    float speed = 10;

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void FixedUpdate()
    {
        float moveDir = speed * Time.fixedDeltaTime; //一次刷新移动的距离
        //CheckCollection(moveDir);
        transform.Translate(Vector3.forward * moveDir);
    }

    //检测碰撞
    void CheckCollection(float moveDir)
    {
        Ray ray = new Ray(transform.position, Vector3.forward);
        RaycastHit hit;  //碰撞检测到的物体

        //物理射线检测（检测射线前下一帧移动距离内的，且为layer层级的，且可以是触发器的碰撞器）
        //注：该方法需要在FixedUpdata中使用
        //Bug:当有一发子弹未销毁是，该方法会不再检测触发器，只能同时存在一个射线检测去检测触发器；
        if (Physics.Raycast(ray, out hit, moveDir, layer, QueryTriggerInteraction.Collide))
        {
            //如果有碰到，执行
            //HitObject(hit);
        }
    }

    //使用触发检测代替射线检测
    private void OnTriggerEnter(Collider other)
    {
        HitObject(other);
    }

    //击中方法
    void HitObject(Collider hit)
    {
        //print(hit.GetType().Name);
        IDamageable damageable = hit.GetComponent<IDamageable>();  //获取对象脚本
        if (damageable != null)
        {
            damageable.TaskHit(damage, hit);
        }
        //TODO：销毁子弹
        Destroy(gameObject);
    }
}
