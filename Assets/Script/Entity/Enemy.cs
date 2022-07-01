using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity {
    NavMeshAgent navAgent;
    Transform target;  //Ŀ��
    Material skinMaterial; //����
    LivingEntity targetEntity; //Ŀ��ʵ��

    public ParticleSystem enemyDeathEff; //����������Ч������ϵͳ��
    public enum State { IDLE, ATTACK, CHASE }; //״̬��
    State currState;  //��ǰ״̬

    Color originColor;

    float attackDistance = 0.5f; //��������
    float attackIntervalTime = 1; //�������ʱ��
    float nextAttackTime; //��һ�ι���ʱ��

    float myColldierRadius; //������ײ�뾶;
    float targetColliderRadius; //Ŀ����ײ�뾶

    float damage = 1; //�˺�
    bool hasTarget;  //�Ƿ���Ŀ��

    protected override void Start() {
        base.Start();
        currState = Enemy.State.IDLE;  //��ʼ��״̬
        navAgent = GetComponent<NavMeshAgent>(); //����Ѱ·��

        if (GameObject.FindGameObjectWithTag("Player")) {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            hasTarget = true;

            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath; //�����¼�

            StartCoroutine(UpdataPath());  //ִ��Ѱ·Э��
            myColldierRadius = GetComponent<CapsuleCollider>().radius;
            targetColliderRadius = target.GetComponent<CapsuleCollider>().radius;
            currState = Enemy.State.CHASE;
        }

        skinMaterial = GetComponent<Renderer>().material;
        originColor = skinMaterial.color;
    }

    //����Ŀ������
    void OnTargetDeath() {
        hasTarget = false;
        currState = Enemy.State.IDLE;
    }

    void Update() {
        if (Time.time > nextAttackTime && hasTarget) {
            //������Ŀ��ľ����ƽ��������ģ��ƽ����
            float SquDis = (target.position - transform.position).sqrMagnitude;
            //���������ƽ����
            float SquAttackDis = Mathf.Pow(attackDistance + myColldierRadius + targetColliderRadius, 2);
            //���㹥������  ����
            if (SquDis <= SquAttackDis) {
                StartCoroutine(Attack());  //ִ�й���Э��
                nextAttackTime = Time.time + attackIntervalTime; //��һ�ι�����ʱ��
            }
        }
    }


    IEnumerator Attack() {
        currState = Enemy.State.ATTACK;
        //navAgent.enabled = false;
        skinMaterial.color = Color.red;

        Vector3 originPos = transform.position; //������ʼλ��
        Vector3 dirToTarget = (target.position - transform.position).normalized;//�ӵ�ǰָ��Ŀ��ĵ�λ����
        Vector3 targetPos = target.position - myColldierRadius * dirToTarget; //����Ŀ��λ�� 

        float percent = 0;  //���̣����ȣ�
        float atkSpeed = 3;  //�����ٶ�
        float interpolation; //��ֵ
        bool hasAttack = false; //�Ƿ���Թ���

        while (percent <= 1) {

            if (percent >= .5f && !hasAttack) {
                hasAttack = true;
                targetEntity.TaskDamage(damage);
            }

            percent += Time.deltaTime * atkSpeed; //��ʱ����� -- ����1 ����
            interpolation = (-Mathf.Pow(percent, 2) + percent) * 4; //(-x^2 + x) * 4 ==> -4x^2 + 4x = y
            transform.position = Vector3.Lerp(originPos, targetPos, interpolation);
            yield return null;
        }

        //TODO�����ƶ�������Ѱ·����Ľ���/ʹ�û���� Ѱ·������ʱ������Ѱ·�����Զ�Ӷ�����Ѱ·���������ɹ��Ĵ���
        //navAgent.enabled = true;  
        currState = Enemy.State.CHASE;
        skinMaterial.color = originColor;
    }

    //����Ѱ·
    IEnumerator UpdataPath() {
        float refleshTime = .25f;  //���ʱ��
        while (hasTarget) //�ֶ�Э��ˢ�� 
        {
            if (currState == Enemy.State.CHASE) {
                Vector3 dirToTarget = (target.position - transform.position).normalized;//�ӵ�ǰָ��Ŀ��ĵ�λ����
                Vector3 targetPoint = target.position - (myColldierRadius + targetColliderRadius + attackDistance / 2) * dirToTarget; //��ȥ˫���뾶������

                if (!death && navAgent && navAgent.enabled == true)  //û������  ���� ״̬Ϊ׷�� ʱִ��
                {
                    navAgent.SetDestination(targetPoint); //��Ѱ·���������ʱ���ᱨ��
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
