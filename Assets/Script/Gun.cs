using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle; //枪口位置（枪口火焰）
    public Bullet bullet; //公共子弹
    public float muzzleVelocity = 35; //枪口速度
    public float msBetweenShots = 100; //射击间隔

    float nextShotTime;  //私有变量记录时间（限制子弹创建频率）

    public void Shoot()
    {
        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + msBetweenShots / 1000; //除1000将秒转换为毫秒
            Bullet newBullet = Instantiate<Bullet>(bullet, muzzle.position, muzzle.rotation);  //创建子弹
            newBullet.SetSpeed(muzzleVelocity); //设置子弹速度
        }
    }
}
