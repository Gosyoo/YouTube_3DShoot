using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//生成器
public class Spawner : MonoBehaviour {
    [Header("波数")]
    public Wave[] wave;
    public Enemy enemy; //敌人预制体

    Wave currWave; //当前波
    int waveIndex; //波次
    int enemiesToSpawn; //当前波生成数量
    int enemyAliveNumber;  //敌人存活数量 

    float nextTime; //下一只的时间

    MapGenerator map;

    [Header("检测滞留间隔时间")]
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
        float flashTime = 0; //闪烁时间
        float flashDelay = 1; //闪烁延时（间隔）
        float flashSpeed = 4; //闪烁速度
        //注：（flashDelay 与 flashSpeed 共同决定闪烁次数）

        Transform tile = map.GetRandomTile(); //获得随机瓦片

        Material material = tile.GetComponent<Renderer>().material;
        Color originColor = material.color;  //原始颜色
        Color flashColor = Color.red; //闪烁颜色

        while (flashTime < flashDelay) {

            //Mathf.pingpong: 以每次【flashTime * flashCount】的增值从0-1，然后再次循环0-1
            material.color = Color.Lerp(originColor, flashColor, Mathf.PingPong(flashTime * flashSpeed, 1));

            flashTime += Time.deltaTime;
            yield return null;
        }

        Enemy currEnemy = Instantiate<Enemy>(enemy, tile.position + Vector3.up, Quaternion.identity);  //生成敌人
        currEnemy.OnDeath += OnEnemyDeath;  //订阅死亡事件；
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

    //内部类  波数
    [System.Serializable]
    public class Wave {
        [Header("生成数量")]
        public int enemyCount;
        [Header("生成间隔时间")]
        public float timeBetweenToSpawner;
    }
}
