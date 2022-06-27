using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Vector2 mapSize;

    [Range(0, 1)]
    public float outLinePercent;
    public int obstacleCount = 10; //障碍物数量
    public int seed = 10; //随机数种子

    List<Coord> allTileCoord; //瓦片坐标列表
    Queue<Coord> shuffledTileCoords; //随机瓦片队列

    void Start() {
        MapGenerate();
    }

    public void MapGenerate() {

        allTileCoord = new List<Coord>(); //初始化坐标列表
        for (int i = 0; i < mapSize.x; i++) {
            for (int j = 0; j < mapSize.y; j++) {
                allTileCoord.Add(new Coord(i, j)); //添加坐标
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray<Coord>(allTileCoord.ToArray(), seed)); //初始化随机队列

        //生成地图前，清空地图块
        string holeName = "Generated Map";
        if (transform.Find(holeName)) {
            //该函数只在写编辑器代码时使用，因为延时的销毁永远不会在编辑模式下调用
            DestroyImmediate(transform.Find(holeName).gameObject);
        }


        //重新添加地图块
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


    //根据瓦片坐标获取位置
    Vector3 GetTileCoordToPosition(int x, int y) {
        return new Vector3(-mapSize.x / 2 + .5f + x, 0, -mapSize.y / 2 + .5f + y);
    }


    //坐标结构体
    public struct Coord {
        public int x;
        public int y;
        //构造函数
        public Coord(int _x, int _y) {
            this.x = _x;
            this.y = _y;
        }
    }
}
