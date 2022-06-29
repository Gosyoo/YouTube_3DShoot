using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������
public class Spawner : MonoBehaviour {
    [Header("����")]
    public Wave[] wave;
    public Enemy enemy; //����Ԥ����

    Wave currWave; //��ǰ��
    int waveIndex; //����
    int enemiesToSpawn; //��ǰ����������
    int enemyAliveNumber;  //���˴������ 

    float nextTime; //��һֻ��ʱ��

    MapGenerator map;

    [Header("����������ʱ��")]
    public float timeBetweenCampingChecks = 2;

    void Start() {
        map = FindObjectOfType<MapGenerator>();

        NextWave();
    }

    void Update() {
        if (enemiesToSpawn > 0 && Time.time > nextTime) {
            enemiesToSpawn--;
            nextTime = Time.time + currWave.timeBetweenToSpawner;

            StartCoroutine(GenerateEnemy());
        }
    }

    IEnumerator GenerateEnemy() {
        float flashTime = 0; //��˸ʱ��
        float flashDelay = 1; //��˸��ʱ�������
        float flashSpeed = 4; //��˸�ٶ�
        //ע����flashDelay �� flashSpeed ��ͬ������˸������

        Transform tile = map.GetRandomTile(); //��������Ƭ

        Material material = tile.GetComponent<Renderer>().material;
        Color originColor = material.color;  //ԭʼ��ɫ
        Color flashColor = Color.red; //��˸��ɫ

        while (flashTime < flashDelay) {

            //Mathf.pingpong: ��ÿ�Ρ�flashTime * flashCount������ֵ��0-1��Ȼ���ٴ�ѭ��0-1
            material.color = Color.Lerp(originColor, flashColor, Mathf.PingPong(flashTime * flashSpeed, 1));

            flashTime += Time.deltaTime;
            yield return null;
        }

        Enemy currEnemy = Instantiate<Enemy>(enemy, tile.position + Vector3.up, Quaternion.identity);  //���ɵ���
        currEnemy.OnDeath += OnEnemyDeath;  //���������¼���
    }

    void OnEnemyDeath() {
        enemyAliveNumber--;
        if (enemyAliveNumber <= 0) {
            NextWave();
        }
    }

    void NextWave() {
        waveIndex++;
        if (waveIndex - 1 < wave.Length) {
            currWave = wave[waveIndex - 1];
            enemiesToSpawn = currWave.enemyCount;
            enemyAliveNumber = enemiesToSpawn;
            print("wave == " + waveIndex);
        }
    }

    //�ڲ���  ����
    [System.Serializable]
    public class Wave {
        [Header("��������")]
        public int enemyCount;
        [Header("���ɼ��ʱ��")]
        public float timeBetweenToSpawner;
    }
}
