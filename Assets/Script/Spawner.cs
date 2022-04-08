using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//生成器
public class Spawner : MonoBehaviour
{
    //波数
    public Wave[] wave;
    public Enemy enemy; //敌人预制体

    Wave currWave; //当前波
    int waveIndex; //波次
    int enemiesToSpawn; //当前波生成数量
    int enemyAliveNumber;  //敌人存活数量 

    float nextTime; //下一只的时间

    void Start()
    {
        NextWave();
    }

    void Update()
    {
        if (enemiesToSpawn > 0 && Time.time > nextTime)
        {
            enemiesToSpawn--;
            nextTime = Time.time + currWave.timeBetweenToSpawner;

            Enemy currEnemy = Instantiate<Enemy>(enemy, Vector3.zero, Quaternion.identity);  //原点生成敌人
            //currEnemy.GetComponent<Material>().color = currWave.color;
            currEnemy.OnDeath += OnEnemyDeath;  //订阅死亡事件；
        }
    }

    void OnEnemyDeath()
    {
        //print("sssssssssss");
        enemyAliveNumber--;
        if (enemyAliveNumber <= 0)
        {
            NextWave();
        }
    }

    void NextWave()
    {
        waveIndex++;
        if (waveIndex - 1 < wave.Length)
        {
            currWave = wave[waveIndex - 1];
            enemiesToSpawn = currWave.enemyCount;
            enemyAliveNumber = enemiesToSpawn;
            print("wave == " + waveIndex);
        }
    }

    //内部类  波数
    [System.Serializable]
    public class Wave
    {
        /// <summary>
        /// 生成数量
        /// </summary>
        public int enemyCount;
        /// <summary>
        /// 生成间隔时间
        /// </summary>
        public float timeBetweenToSpawner;
        /// <summary>
        /// 敌人颜色
        /// </summary>
        //public Color color;
    }
}
