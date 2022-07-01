using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//生成器
public class Spawner : MonoBehaviour {
    [Header("波数")]
    public Wave[] wave;
    public Enemy enemy; //敌人预制体

    LivingEntity playerEnity; //角色实体
    Transform PlayerT;

    Wave currWave; //当前波
    int waveIndex; //波次
    int enemiesToSpawn; //当前波生成数量
    int enemyAliveNumber;  //敌人存活数量 
    float nextTime; //下一只的时间

    MapGenerator map;

    [Header("检测滞留间隔时间")]
    public float timeBetweenCampingChecks = 2f;
    float nextCampCheckTime; // 下一次滞留检测的时间

    [Header("滞留检测距离")]
    public float campThresholdDistance = 1.5f;
    Vector3 campPositionOld; //角色上一次滞留检测的位置

    bool isCamping; //是否滞留
    bool isDisable; //是否禁用生成敌人

    public event System.Action<int> OnNextWave; //注册开始下一轮的事件

    void Start() {
        map = FindObjectOfType<MapGenerator>();

        playerEnity = FindObjectOfType<Player>();
        PlayerT = playerEnity.transform;
        playerEnity.OnDeath += OnPlayerDeath;  //角色死亡禁用生成敌人

        nextCampCheckTime = Time.time + timeBetweenCampingChecks;
        campPositionOld = PlayerT.position;

        NextWave();
    }

    void Update() {

        if (!isDisable) {

            //滞留检测
            if (Time.time > nextCampCheckTime) {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks; //更新下一次检测时间

                isCamping = Vector3.Distance(PlayerT.position, campPositionOld) < campThresholdDistance; //检测距离是否为滞留
                campPositionOld = PlayerT.position; //更新位置(注：检测完之后再更新位置)
            }

            //敌人生成
            if (enemiesToSpawn > 0 && Time.time > nextTime) {
                enemiesToSpawn--;
                nextTime = Time.time + currWave.timeBetweenToSpawner;

                StartCoroutine(GenerateEnemy());
            }
        }
    }

    IEnumerator GenerateEnemy() {
        float flashTime = 0; //闪烁时间
        float flashDelay = 1; //闪烁延时（间隔）
        float flashSpeed = 4; //闪烁速度
        //注：（flashDelay 与 flashSpeed 共同决定闪烁次数）

        Transform tile = map.GetRandomTile(); //获得随机瓦片
        //如果滞留，更新瓦片生成位置
        if (isCamping) {
            tile = map.GetTileFromPosition(PlayerT.position); // 在角色位置生成
        }

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

    void OnPlayerDeath() {
        isDisable = true;
    }

    void OnEnemyDeath() {
        enemyAliveNumber--;
        if (enemyAliveNumber <= 0) {
            NextWave();
        }
    }

    //重置玩家位置
    void ResetPlayerPosition() {
        Vector3 resetPos = map.GetTileFromPosition(Vector3.zero).position;
        PlayerT.position = resetPos + Vector3.up * 3; //玩家重置位置 在原点上方一点
    }

    void NextWave() {
        waveIndex++;
        if (waveIndex - 1 < wave.Length) {
            currWave = wave[waveIndex - 1];
            enemiesToSpawn = currWave.enemyCount;
            enemyAliveNumber = enemiesToSpawn;
            print("wave == " + waveIndex);

            if (OnNextWave != null) {
                OnNextWave(waveIndex); //触发事件
            }
            ResetPlayerPosition();
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
