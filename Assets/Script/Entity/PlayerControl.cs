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
        //移动逻辑在FixedUpdata中每帧执行
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);  //增量移动
    }

    //移动 使用Updata给方向赋值（方向的值为标准向量 * 速度）
    public void Move(Vector3 _velocity) {
        velocity = _velocity;
    }

    public void lookAt(Vector3 point) {
        Vector3 heightCorrectedPoint = new Vector3(point.x, transform.position.y, point.z);
        transform.LookAt(heightCorrectedPoint);
    }

}
