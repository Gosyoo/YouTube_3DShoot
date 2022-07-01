using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
    public Transform muzzle; //ǹ��λ��
    public Bullet bullet; //�����ӵ�

    public Transform shellSpawn; //�׿�λ��
    public Shell shell; //����

    MuzzleFlash muzzleFlash; //ǹ�ڻ���

    public float muzzleVelocity = 35; //ǹ���ٶ�
    public float msBetweenShots = 100; //������

    float nextShotTime;  //˽�б�����¼ʱ�䣨�����ӵ�����Ƶ�ʣ�

    void Start() {
        muzzleFlash = GetComponent<MuzzleFlash>();
    }

    public void Shoot() {
        if (Time.time > nextShotTime) {
            nextShotTime = Time.time + msBetweenShots / 1000; //��1000����ת��Ϊ����
            Bullet newBullet = Instantiate<Bullet>(bullet, muzzle.position, muzzle.rotation);  //�����ӵ�
            newBullet.SetSpeed(muzzleVelocity); //�����ӵ��ٶ�

            Instantiate(shell, shellSpawn.position, shellSpawn.rotation);
            muzzleFlash.Activate();
        }
    }
}
