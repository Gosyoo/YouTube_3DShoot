using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public LayerMask layer; //�������Ĳ㼶
    public float damage = 1;
    float speed = 10;

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void FixedUpdate()
    {
        float moveDir = speed * Time.fixedDeltaTime; //һ��ˢ���ƶ��ľ���
        //CheckCollection(moveDir);
        transform.Translate(Vector3.forward * moveDir);
    }

    //�����ײ
    void CheckCollection(float moveDir)
    {
        Ray ray = new Ray(transform.position, Vector3.forward);
        RaycastHit hit;  //��ײ��⵽������

        //�������߼�⣨�������ǰ��һ֡�ƶ������ڵģ���Ϊlayer�㼶�ģ��ҿ����Ǵ���������ײ����
        //ע���÷�����Ҫ��FixedUpdata��ʹ��
        //Bug:����һ���ӵ�δ�����ǣ��÷����᲻�ټ�ⴥ������ֻ��ͬʱ����һ�����߼��ȥ��ⴥ������
        if (Physics.Raycast(ray, out hit, moveDir, layer, QueryTriggerInteraction.Collide))
        {
            //�����������ִ��
            //HitObject(hit);
        }
    }

    //ʹ�ô������������߼��
    private void OnTriggerEnter(Collider other)
    {
        HitObject(other);
    }

    //���з���
    void HitObject(Collider hit)
    {
        //print(hit.GetType().Name);
        IDamageable damageable = hit.GetComponent<IDamageable>();  //��ȡ����ű�
        if (damageable != null)
        {
            damageable.TaskHit(damage, hit);
        }
        //TODO�������ӵ�
        Destroy(gameObject);
    }
}
