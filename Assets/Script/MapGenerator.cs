using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Vector2 mapSize;

    [Range(0, 1)]
    public float outLinePercent;
    public int obstacleCount = 10; //�ϰ�������
    public int seed = 10; //���������

    List<Coord> allTileCoord; //��Ƭ�����б�
    Queue<Coord> shuffledTileCoords; //�����Ƭ����

    void Start() {
        MapGenerate();
    }

    public void MapGenerate() {

        allTileCoord = new List<Coord>(); //��ʼ�������б�
        for (int i = 0; i < mapSize.x; i++) {
            for (int j = 0; j < mapSize.y; j++) {
                allTileCoord.Add(new Coord(i, j)); //�������
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray<Coord>(allTileCoord.ToArray(), seed)); //��ʼ���������

        //���ɵ�ͼǰ����յ�ͼ��
        string holeName = "Generated Map";
        if (transform.Find(holeName)) {
            //�ú���ֻ��д�༭������ʱʹ�ã���Ϊ��ʱ��������Զ�����ڱ༭ģʽ�µ���
            DestroyImmediate(transform.Find(holeName).gameObject);
        }


        //������ӵ�ͼ��
        Transform holeMap = new GameObject(holeName).transform;
        holeMap.parent = transform;

        for (int i = 0; i < mapSize.x; i++) {
            for (int j = 0; j < mapSize.y; j++) {
                Vector3 tilePosition = GetTileCoordToPosition(i, j);
                Transform newTile = Instantiate<Transform>(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90));
                newTile.localScale = Vector3.one * (1 - outLinePercent);
                newTile.parent = holeMap;
            }
        }

        for (int i = 0; i < obstacleCount; i++) {
            Coord obstacleCoord = shuffledTileCoords.Dequeue();
            shuffledTileCoords.Enqueue(obstacleCoord);
            Vector3 obstaclePosition = GetTileCoordToPosition(obstacleCoord.x, obstacleCoord.y);
            Transform newObstacle = Instantiate<Transform>(obstaclePrefab, obstaclePosition + Vector3.up * .5f, Quaternion.identity);
            newObstacle.parent = holeMap;
        }
    }


    //������Ƭ�����ȡλ��
    Vector3 GetTileCoordToPosition(int x, int y) {
        return new Vector3(-mapSize.x / 2 + .5f + x, 0, -mapSize.y / 2 + .5f + y);
    }


    //����ṹ��
    public struct Coord {
        public int x;
        public int y;
        //���캯��
        public Coord(int _x, int _y) {
            this.x = _x;
            this.y = _y;
        }
    }
}
