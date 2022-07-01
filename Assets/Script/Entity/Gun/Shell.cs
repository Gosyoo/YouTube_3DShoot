using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour {
    public Rigidbody myRigidbody;
    public float minForce; //��С��
    public float maxForce; //�����

    public float liftTime = 3; //���ڳ���ʱ��
    public float fadeTime = 1; //��ʧ����ʱ��

    void Start() {
        float force = Random.Range(minForce, maxForce);
        myRigidbody.AddForce(force * transform.right); //�����
        myRigidbody.AddTorque(force * Random.insideUnitSphere); //���Ť��

        StartCoroutine(Fade());
    }

    IEnumerator Fade() {
        yield return new WaitForSeconds(liftTime);  //liftTimeʱ����ٴε���

        float precent = 0;
        float speed = 1 / fadeTime;

        while (precent < 1) {
            precent += Time.deltaTime * speed;

            Material material = GetComponent<Renderer>().material;
            Color originColor = material.color;
            material.color = Color.Lerp(originColor, Color.clear, precent);

            yield return null;
        }

        Destroy(gameObject);  //��������
    }

}
