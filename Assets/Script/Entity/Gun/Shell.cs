using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour {
    public Rigidbody myRigidbody;
    public float minForce; //最小力
    public float maxForce; //最大力

    public float liftTime = 3; //存在持续时间
    public float fadeTime = 1; //消失持续时间

    void Start() {
        float force = Random.Range(minForce, maxForce);
        myRigidbody.AddForce(force * transform.right); //添加力
        myRigidbody.AddTorque(force * Random.insideUnitSphere); //添加扭矩

        StartCoroutine(Fade());
    }

    IEnumerator Fade() {
        yield return new WaitForSeconds(liftTime);  //liftTime时间后再次调用

        float precent = 0;
        float speed = 1 / fadeTime;

        while (precent < 1) {
            precent += Time.deltaTime * speed;

            Material material = GetComponent<Renderer>().material;
            Color originColor = material.color;
            material.color = Color.Lerp(originColor, Color.clear, precent);

            yield return null;
        }

        Destroy(gameObject);  //销毁自身
    }

}
