using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity {
    NavMeshAgent navAgent;
    Transform target;  //目标
    Material skinMaterial; //材质
    LivingEntity targetEntity; //目标实体

    public ParticleSystem enemyDeathEff; //敌人死亡特效（粒子系统）
    public enum State { IDLE, ATTACK, CHASE }; //状态机
    State currState;  //当前状态

    Color originColor;

    float attackDistance = 0.5f; //攻击距离
    float attackIntervalTime = 1; //攻击间隔时间
    float nextAttackTime; //下一次攻击时间

    float myColldierRadius; //自身碰撞半径;
    float targetColliderRadius; //目标碰撞半径

    float damage = 1; //伤害
    bool hasTarget;  //是否有目标

    protected override void Start() {
        base.Start();
        currState = Enemy.State.IDLE;  //初始化状态
        navAgent = GetComponent<NavMeshAgent>(); //网格寻路器

        if (GameObject.FindGameObjectWithTag("Player")) {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            hasTarget = true;

            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath; //监听事件

            StartCoroutine(UpdataPath());  //执行寻路协程
            myColldierRadius = GetComponent<CapsuleCollider>().radius;
            targetColliderRadius = target.GetComponent<CapsuleCollider>().radius;
            currState = Enemy.State.CHASE;
        }

        skinMaterial = GetComponent<Renderer>().material;
        originColor = skinMaterial.color;
    }

    //攻击目标死亡
    void OnTargetDeath() {
        hasTarget = false;
        currState = Enemy.State.IDLE;
    }

    void Update() {
        if (Time.time > nextAttackTime && hasTarget) {
            //自身与目标的距离的平方（向量模的平方）
            float SquDis = (target.position - transform.position).sqrMagnitude;
            //攻击距离的平方；
            float SquAttackDis = Mathf.Pow(attackDistance + myColldierRadius + targetColliderRadius, 2);
            //满足攻击条件  攻击
            if (SquDis <= SquAttackDis) {
                StartCoroutine(Attack());  //执行攻击协程
                nextAttackTime = Time.time + attackIntervalTime; //下一次攻击的时机
            }
        }
    }


    IEnumerator Attack() {
        currState = Enemy.State.ATTACK;
        //navAgent.enabled = false;
        skinMaterial.color = Color.red;

        Vector3 originPos = transform.position; //攻击起始位置
        Vector3 dirToTarget = (target.position - transform.position).normalized;//从当前指向目标的单位向量
        Vector3 targetPos = target.position - myColldierRadius * dirToTarget; //攻击目标位置 

        float percent = 0;  //过程（进度）
        float atkSpeed = 3;  //攻击速度
        float interpolation; //差值
        bool hasAttack = false; //是否可以攻击

        while (percent <= 1) {

            if (percent >= .5f && !hasAttack) {
                hasAttack = true;
                targetEntity.TaskDamage(damage);
            }

            percent += Time.deltaTime * atkSpeed; //按时间递增 -- 超过1 结束
            interpolation = (-Mathf.Pow(percent, 2) + percent) * 4; //(-x^2 + x) * 4 ==> -4x^2 + 4x = y
            transform.position = Vector3.Lerp(originPos, targetPos, interpolation);
            yield return null;
        }

        //TODO：在移动中设置寻路代理的禁用/使用会造成 寻路代理创建时检测距离寻路网格过远从而导致寻路代理创建不成功的错误
        //navAgent.enabled = true;  
        currState = Enemy.State.CHASE;
        skinMaterial.color = originColor;
    }

    //更新寻路
    IEnumerator UpdataPath() {
        float refleshTime = .25f;  //间隔时间
        while (hasTarget) //手动协程刷新 
        {
            if (currState == Enemy.State.CHASE) {
                Vector3 dirToTarget = (target.position - transform.position).normalized;//从当前指向目标的单位向量
                Vector3 targetPoint = target.position - (myColldierRadius + targetColliderRadius + attackDistance / 2) * dirToTarget; //减去双方半径，贴边

                if (!death && navAgent && navAgent.enabled == true)  //没有死亡  并且 状态为追击 时执行
                {
                    navAgent.SetDestination(targetPoint); //当寻路组件被禁用时，会报错
                }
            }
            yield return new WaitForSeconds(refleshTime);
        }
    }

    public override void TaskHit(float damage, Vector3 hitPos, Vector3 hitDirection) {
        base.TaskHit(damage, hitPos, hitDirection);
        if (isDeath) {
            Destroy(Instantiate(enemyDeathEff.gameObject, hitPos, Quaternion.FromToRotation(Vector3.forward, hitDirection)), enemyDeathEff.main.startLifetimeMultiplier);
        }
    }
}
