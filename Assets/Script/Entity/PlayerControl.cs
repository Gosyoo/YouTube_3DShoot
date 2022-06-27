using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour {
    Rigidbody myRigidbody;
    Vector3 velocity;

    void Start() {
        myRigidbody = GetComponent<Rigidbody>();
    }


    public void FixedUpdate() {
        //�ƶ��߼���FixedUpdata��ÿִ֡��
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);  //�����ƶ�
    }

    //�ƶ� ʹ��Updata������ֵ�������ֵΪ��׼���� * �ٶȣ�
    public void Move(Vector3 _velocity) {
        velocity = _velocity;
    }

    public void lookAt(Vector3 point) {
        Vector3 heightCorrectedPoint = new Vector3(point.x, transform.position.y, point.z);
        transform.LookAt(heightCorrectedPoint);
    }

}
