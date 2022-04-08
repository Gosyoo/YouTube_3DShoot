using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������
public class Spawner : MonoBehaviour
{
    //����
    public Wave[] wave;
    public Enemy enemy; //����Ԥ����

    Wave currWave; //��ǰ��
    int waveIndex; //����
    int enemiesToSpawn; //��ǰ����������
    int enemyAliveNumber;  //���˴������ 

    float nextTime; //��һֻ��ʱ��

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

            Enemy currEnemy = Instantiate<Enemy>(enemy, Vector3.zero, Quaternion.identity);  //ԭ�����ɵ���
            //currEnemy.GetComponent<Material>().color = currWave.color;
            currEnemy.OnDeath += OnEnemyDeath;  //���������¼���
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

    //�ڲ���  ����
    [System.Serializable]
    public class Wave
    {
        /// <summary>
        /// ��������
        /// </summary>
        public int enemyCount;
        /// <summary>
        /// ���ɼ��ʱ��
        /// </summary>
        public float timeBetweenToSpawner;
        /// <summary>
        /// ������ɫ
        /// </summary>
        //public Color color;
    }
}
