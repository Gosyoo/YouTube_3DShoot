using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public LayerMask layer; //�������Ĳ㼶
    public float damage = 1;
    float speed = 10;
    float liveTime = 3;
    float skinWidth = .1f;

    void Start() {
        Destroy(gameObject, liveTime);  //����
        //���ڹ��������ڲ�����ʱ��ʹ����ײ��⣬���߼���ⲻ��
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, layer);
        if (initialCollisions.Length > 0) {
            HitObject(initialCollisions[0]);
        }
    }

    public void SetSpeed(float newSpeed) {
        speed = newSpeed;
    }

    void FixedUpdate() {
        float moveDir = speed * Time.fixedDeltaTime; //һ��ˢ���ƶ��ľ���
        CheckCollection(moveDir);
        transform.Translate(Vector3.forward * moveDir);
    }

    //�����ײ
    void CheckCollection(float moveDir) {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;  //��ײ��⵽������
        Debug.DrawLine(transform.position, (transform.position + transform.forward), Color.red);

        //�������߼�⣨�������ǰ��һ֡�ƶ������ڵģ���Ϊlayer�㼶�ģ��ҿ����Ǵ���������ײ����
        //ע���÷�����Ҫ��FixedUpdata��ʹ��
        //Bug:����һ���ӵ�δ�����ǣ��÷����᲻�ټ�ⴥ������ֻ��ͬʱ����һ�����߼��ȥ��ⴥ������
        //�������Ŀ����-->����-->Auto Sync Transforms(ȷ����ѡ)  2018.2֮��Ĭ��Ϊfalse������Щ΢��������
        if (Physics.Raycast(ray, out hit, moveDir + skinWidth, layer, QueryTriggerInteraction.Collide)) {
            HitObject(hit);
        }
    }

    //ʹ�ô������������߼��
    //private void OnTriggerEnter(Collider other)
    //{
    //    HitObject(other);
    //}

    //���з���
    void HitObject(RaycastHit hit) {
        //print(hit.GetType().Name);
        IDamageable damageable = hit.collider.GetComponent<IDamageable>();  //��ȡ����ű�
        if (damageable != null) {
            damageable.TaskHit(damage, hit);
        }
        //TODO�������ӵ�
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
