using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������
public class Spawner : MonoBehaviour {
    [Header("����")]
    public Wave[] wave;
    public Enemy enemy; //����Ԥ����

    LivingEntity playerEnity; //��ɫʵ��
    Transform PlayerT;

    Wave currWave; //��ǰ��
    int waveIndex; //����
    int enemiesToSpawn; //��ǰ����������
    int enemyAliveNumber;  //���˴������ 
    float nextTime; //��һֻ��ʱ��

    MapGenerator map;

    [Header("����������ʱ��")]
    public float timeBetweenCampingChecks = 2f;
    float nextCampCheckTime; // ��һ����������ʱ��

    [Header("����������")]
    public float campThresholdDistance = 1.5f;
    Vector3 campPositionOld; //��ɫ��һ����������λ��

    bool isCamping; //�Ƿ�����
    bool isDisable; //�Ƿ�������ɵ���

    public event System.Action<int> OnNextWave; //ע�Ὺʼ��һ�ֵ��¼�

    void Start() {
        map = FindObjectOfType<MapGenerator>();

        playerEnity = FindObjectOfType<Player>();
        PlayerT = playerEnity.transform;
        playerEnity.OnDeath += OnPlayerDeath;  //��ɫ�����������ɵ���

        nextCampCheckTime = Time.time + timeBetweenCampingChecks;
        campPositionOld = PlayerT.position;

        NextWave();
    }

    void Update() {

        if (!isDisable) {

            //�������
            if (Time.time > nextCampCheckTime) {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks; //������һ�μ��ʱ��

                isCamping = Vector3.Distance(PlayerT.position, campPositionOld) < campThresholdDistance; //�������Ƿ�Ϊ����
                campPositionOld = PlayerT.position; //����λ��(ע�������֮���ٸ���λ��)
            }

            //��������
            if (enemiesToSpawn > 0 && Time.time > nextTime) {
                enemiesToSpawn--;
                nextTime = Time.time + currWave.timeBetweenToSpawner;

                StartCoroutine(GenerateEnemy());
            }
        }
    }

    IEnumerator GenerateEnemy() {
        float flashTime = 0; //��˸ʱ��
        float flashDelay = 1; //��˸��ʱ�������
        float flashSpeed = 4; //��˸�ٶ�
        //ע����flashDelay �� flashSpeed ��ͬ������˸������

        Transform tile = map.GetRandomTile(); //��������Ƭ
        //���������������Ƭ����λ��
        if (isCamping) {
            tile = map.GetTileFromPosition(PlayerT.position); // �ڽ�ɫλ������
        }

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

    void OnPlayerDeath() {
        isDisable = true;
    }

    void OnEnemyDeath() {
        enemyAliveNumber--;
        if (enemyAliveNumber <= 0) {
            NextWave();
        }
    }

    //�������λ��
    void ResetPlayerPosition() {
        Vector3 resetPos = map.GetTileFromPosition(Vector3.zero).position;
        PlayerT.position = resetPos + Vector3.up * 3; //�������λ�� ��ԭ���Ϸ�һ��
    }

    void NextWave() {
        waveIndex++;
        if (waveIndex - 1 < wave.Length) {
            currWave = wave[waveIndex - 1];
            enemiesToSpawn = currWave.enemyCount;
            enemyAliveNumber = enemiesToSpawn;
            print("wave == " + waveIndex);

            if (OnNextWave != null) {
                OnNextWave(waveIndex); //�����¼�
            }
            ResetPlayerPosition();
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
