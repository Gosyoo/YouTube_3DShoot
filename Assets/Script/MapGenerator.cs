using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navMeshFloor;
    public Transform navMeshMaskPrefab;
    public Vector2 maxNavMeshSize; //最大寻路网地板大小

    public Map[] maps;
    public int mapIndex;
    Map currMap;

    List<Coord> allTileCoord; //瓦片坐标列表
    Queue<Coord> shuffledTileCoords; //随机瓦片队列
    Queue<Coord> shuffledOpenTileCoords; //连通的瓦片随机队列

    Transform[,] tileMap; //瓦片地图
    bool[,] obstacleMap; //障碍物地图


    void Awake() {
        FindObjectOfType<Spawner>().OnNextWave += OnNextMap;
    }

    void OnNextMap(int waveNum) {
        if (waveNum > 0 && waveNum <= maps.Length) {
            mapIndex = waveNum - 1;
            MapGenerate();
        }
    }

    //生成地图
    public void MapGenerate() {
        currMap = maps[mapIndex];
        System.Random prng = new System.Random(currMap.seed);
        GetComponent<BoxCollider>().size = new Vector3(currMap.mapSize.x * currMap.tileSize, 0.05f, currMap.mapSize.y * currMap.tileSize);
        tileMap = new Transform[currMap.mapSize.x, currMap.mapSize.y];

        allTileCoord = new List<Coord>(); //初始化坐标列表
        for (int i = 0; i < currMap.mapSize.x; i++) {
            for (int j = 0; j < currMap.mapSize.y; j++) {
                allTileCoord.Add(new Coord(i, j)); //添加坐标
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray<Coord>(allTileCoord.ToArray(), currMap.seed)); //初始化随机队列

        //生成地图前，清空地图块
        string holeName = "Generated Map";
        if (transform.Find(holeName)) {
            //该函数只在写编辑器代码时使用，因为延时的销毁永远不会在编辑模式下调用
            DestroyImmediate(transform.Find(holeName).gameObject);
        }


        //重新生成地图块
        Transform holeMap = new GameObject(holeName).transform;
        holeMap.parent = transform;

        for (int i = 0; i < currMap.mapSize.x; i++) {
            for (int j = 0; j < currMap.mapSize.y; j++) {
                Vector3 tilePosition = GetTileCoordToPosition(i, j);
                Transform newTile = Instantiate<Transform>(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90));
                newTile.localScale = Vector3.one * (1 - currMap.outLinePercent) * currMap.tileSize;
                newTile.parent = holeMap;

                tileMap[i, j] = newTile; //初始化瓦片地图
            }
        }

        //添加障碍物
        int obstacleCount = (int)(currMap.mapSize.x * currMap.mapSize.y * currMap.obstaclePercent); //障碍物数量
        int currObstacleCount = 0; //当前障碍物数量
        obstacleMap = new bool[currMap.mapSize.x, currMap.mapSize.y]; //初始化二维数组

        List<Coord> allOpenTileList = new List<Coord>(allTileCoord);

        //注：障碍物数量不定，因检查连通不通过则障碍物数量减1
        for (int i = 0; i < obstacleCount; i++) {
            Coord obstacleCoord = GetRandomCoord();

            obstacleMap[obstacleCoord.x, obstacleCoord.y] = true; //假设该随机点是障碍物
            currObstacleCount++;
            //不是出生点（中心点）,且满足连通性
            if (obstacleCoord != currMap.centerCoord && MapIsFullyAccessible(obstacleMap, currObstacleCount)) {

                //随机高度
                float obstacleHeight = Mathf.Lerp(currMap.minObstacleHeight, currMap.maxObstacleHeight, (float)prng.NextDouble());
                Vector3 obstaclePosition = GetTileCoordToPosition(obstacleCoord.x, obstacleCoord.y);
                Transform newObstacle = Instantiate<Transform>(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight / 2, Quaternion.identity);
                //设置缩放，仅底部按行间距比例缩放，高度不变
                newObstacle.localScale = new Vector3((1 - currMap.outLinePercent) * currMap.tileSize, obstacleHeight, (1 - currMap.outLinePercent) * currMap.tileSize);

                float colorPrecent = obstacleCoord.y / (float)currMap.mapSize.y; //y轴渐变
                setObstcaleColor(newObstacle, colorPrecent);

                newObstacle.parent = holeMap;
                allOpenTileList.Remove(obstacleCoord); //是障碍物就移除连通队列
            } else {
                obstacleMap[obstacleCoord.x, obstacleCoord.y] = false; //不满足则设置为false
                currObstacleCount--;
            }
        }

        //初始化连通瓦片随机队列
        shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray<Coord>(allOpenTileList.ToArray(), currMap.seed));

        //由于navMeshPrefab旋转90°后，大小需调整x，y轴而不是x，z轴
        navMeshFloor.localScale = new Vector3(maxNavMeshSize.x, maxNavMeshSize.y) * currMap.tileSize;

        //创建寻路网格遮罩
        Transform leftMask = Instantiate<Transform>(navMeshMaskPrefab, Vector3.left * (maxNavMeshSize.x + currMap.mapSize.x) / 4f * currMap.tileSize, Quaternion.identity, holeMap);
        leftMask.localScale = new Vector3((maxNavMeshSize.x - currMap.mapSize.x) / 2f, 1, currMap.mapSize.y) * currMap.tileSize;
        Transform rightMask = Instantiate<Transform>(navMeshMaskPrefab, Vector3.right * (maxNavMeshSize.x + currMap.mapSize.x) / 4f * currMap.tileSize, Quaternion.identity, holeMap);
        rightMask.localScale = new Vector3((maxNavMeshSize.x - currMap.mapSize.x) / 2f, 1, currMap.mapSize.y) * currMap.tileSize;
        Transform topMask = Instantiate<Transform>(navMeshMaskPrefab, Vector3.forward * (maxNavMeshSize.y + currMap.mapSize.y) / 4f * currMap.tileSize, Quaternion.identity, holeMap);
        topMask.localScale = new Vector3(maxNavMeshSize.y, 1, (maxNavMeshSize.y - currMap.mapSize.y) / 2f) * currMap.tileSize;
        Transform backMask = Instantiate<Transform>(navMeshMaskPrefab, Vector3.back * (maxNavMeshSize.y + currMap.mapSize.y) / 4f * currMap.tileSize, Quaternion.identity, holeMap);
        backMask.localScale = new Vector3(maxNavMeshSize.y, 1, (maxNavMeshSize.y - currMap.mapSize.y) / 2f) * currMap.tileSize;
    }


    //地图是否连通（洪水填充）
    private bool MapIsFullyAccessible(bool[,] obstacleMap, int currObstacleCount) {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)]; //创建标记二维表，用于检测该点是否检测过
        mapFlags[currMap.centerCoord.x, currMap.centerCoord.y] = true; //中心点默认检测过
        Queue<Coord> queue = new Queue<Coord>(); //创建一个队列用于循环判断
        queue.Enqueue(currMap.centerCoord); //中心点必然连通，所以从中心点开始蔓延

        int accessibleTileCount = 1; //连通点的数量

        //如果有元素,就继续蔓延
        while (queue.Count > 0) {
            Coord tile = queue.Dequeue(); //出栈一个元素

            //八方向检测
            //  (-1,1)      (0,1)       (1,1) 
            //  (-1,0)      (0,0)       (1,0)
            //  (-1,-1)     (0,-1)      (1,-1)

            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    int nextX = tile.x + x;
                    int nextY = tile.y + y;
                    if (x == 0 || y == 0) {  //4方向
                        //边界检测，不超过地图大小
                        if (nextX >= 0 && nextX < obstacleMap.GetLength(0) && nextY >= 0 && nextY < obstacleMap.GetLength(1)) {
                            //没有被检测过并且不是障碍物
                            if (!mapFlags[nextX, nextY] && !obstacleMap[nextX, nextY]) {
                                mapFlags[nextX, nextY] = true; //标记为已检测
                                queue.Enqueue(new Coord(nextX, nextY)); //以该点为起点继续蔓延
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)currMap.mapSize.x * (int)currMap.mapSize.y - currObstacleCount; //期望连通点的数量
        return accessibleTileCount == targetAccessibleTileCount; //如果连通点数量等于期望数量，则连通
    }


    //获取随机坐标
    private Coord GetRandomCoord() {
        Coord obstacleCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(obstacleCoord);
        return obstacleCoord;
    }

    //获取随机瓦片
    public Transform GetRandomTile() {
        Coord tileCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(tileCoord);
        return tileMap[tileCoord.x, tileCoord.y];
    }

    //根据位置获取瓦片
    public Transform GetTileFromPosition(Vector3 pos) {
        int x = Mathf.RoundToInt(pos.x / currMap.tileSize + (currMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(pos.z / currMap.tileSize + (currMap.mapSize.y - 1) / 2f);

        //注：mathf.clamp ==> 限制 值 在 最大与最小之间 ，大于最大取最大，小于最小取最小
        x = Mathf.Clamp(x, 0, tileMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1) - 1);
        return tileMap[x, y];
    }

    //根据瓦片坐标获取位置
    Vector3 GetTileCoordToPosition(int x, int y) {
        return new Vector3(-currMap.mapSize.x / 2f + .5f + x, 0, -currMap.mapSize.y / 2f + .5f + y) * currMap.tileSize;
    }

    /// <summary>
    /// 设置障碍物颜色
    /// </summary>
    /// <param name="obstacle">障碍物</param>
    /// <param name="colorPercent">渐变率</param>
    void setObstcaleColor(Transform obstacle, float colorPercent) {
        Renderer renderer = obstacle.GetComponent<Renderer>();
        Material obstMaterial = new Material(renderer.sharedMaterial);
        obstMaterial.color = Color.Lerp(currMap.foregroundColour, currMap.backgroundColour, colorPercent); //渐变色
        renderer.sharedMaterial = obstMaterial;
    }

    //坐标结构体
    [System.Serializable]
    public struct Coord {
        public int x;
        public int y;
        //构造函数
        public Coord(int _x, int _y) {
            this.x = _x;
            this.y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2) {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2) {
            return !(c1 == c2);
        }
    }

    [System.Serializable]
    public class Map {
        public Coord mapSize;//瓦片地图大小
        [Range(0, 1)]
        public float outLinePercent; //行间距比例
        [Range(0, 1)]
        public float obstaclePercent; //障碍物比例
        [Min(1)]
        public float tileSize; //瓦片大小

        public int seed = 10; //随机数种子
        public float minObstacleHeight; //最小障碍物高度
        public float maxObstacleHeight; //最大障碍物高度
        public Color foregroundColour;  //前景色
        public Color backgroundColour;  //背景色

        public Coord centerCoord {
            get {
                return new Coord((int)mapSize.x / 2, (int)mapSize.y / 2);
            }
        }
    }
}
