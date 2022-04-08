using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    NavMeshAgent navAgent;
    Transform target;  //Ŀ��

    protected override void Start()
    {
        base.Start();
        navAgent = GetComponent<NavMeshAgent>(); //����Ѱ·��
        target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(UpdataPath());
    }

    //void Update()
    //{

    //}

    IEnumerator UpdataPath()
    {
        float refleshTime = .25f;  //���ʱ��
        while (target != null) //�ֶ�Э��ˢ�� 
        {
            Vector3 targetPoint = new Vector3(target.position.x, 0, target.position.z);
            if (!death)  //��������ִ��
            {
                navAgent.SetDestination(targetPoint);
            }
            yield return new WaitForSeconds(refleshTime);
        }
    }
}
