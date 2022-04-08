using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    NavMeshAgent navAgent;
    Transform target;  //目标

    protected override void Start()
    {
        base.Start();
        navAgent = GetComponent<NavMeshAgent>(); //网格寻路器
        target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(UpdataPath());
    }

    //void Update()
    //{

    //}

    IEnumerator UpdataPath()
    {
        float refleshTime = .25f;  //间隔时间
        while (target != null) //手动协程刷新 
        {
            Vector3 targetPoint = new Vector3(target.position.x, 0, target.position.z);
            if (!death)  //死亡不在执行
            {
                navAgent.SetDestination(targetPoint);
            }
            yield return new WaitForSeconds(refleshTime);
        }
    }
}
