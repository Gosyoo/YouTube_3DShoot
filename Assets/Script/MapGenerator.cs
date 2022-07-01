using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navMeshFloor;
    public Transform navMeshMaskPrefab;
    public Vector2 maxNavMeshSize; //���Ѱ·���ذ��С

    public Map[] maps;
    public int mapIndex;
    Map currMap;

    List<Coord> allTileCoord; //��Ƭ�����б�
    Queue<Coord> shuffledTileCoords; //�����Ƭ����
    Queue<Coord> shuffledOpenTileCoords; //��ͨ����Ƭ�������

    Transform[,] tileMap; //��Ƭ��ͼ
    bool[,] obstacleMap; //�ϰ����ͼ


    void Awake() {
        FindObjectOfType<Spawner>().OnNextWave += OnNextMap;
    }

    void OnNextMap(int waveNum) {
        if (waveNum > 0 && waveNum <= maps.Length) {
            mapIndex = waveNum - 1;
            MapGenerate();
        }
    }

    //���ɵ�ͼ
    public void MapGenerate() {
        currMap = maps[mapIndex];
        System.Random prng = new System.Random(currMap.seed);
        GetComponent<BoxCollider>().size = new Vector3(currMap.mapSize.x * currMap.tileSize, 0.05f, currMap.mapSize.y * currMap.tileSize);
        tileMap = new Transform[currMap.mapSize.x, currMap.mapSize.y];

        allTileCoord = new List<Coord>(); //��ʼ�������б�
        for (int i = 0; i < currMap.mapSize.x; i++) {
            for (int j = 0; j < currMap.mapSize.y; j++) {
                allTileCoord.Add(new Coord(i, j)); //�������
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray<Coord>(allTileCoord.ToArray(), currMap.seed)); //��ʼ���������

        //���ɵ�ͼǰ����յ�ͼ��
        string holeName = "Generated Map";
        if (transform.Find(holeName)) {
            //�ú���ֻ��д�༭������ʱʹ�ã���Ϊ��ʱ��������Զ�����ڱ༭ģʽ�µ���
            DestroyImmediate(transform.Find(holeName).gameObject);
        }


        //�������ɵ�ͼ��
        Transform holeMap = new GameObject(holeName).transform;
        holeMap.parent = transform;

        for (int i = 0; i < currMap.mapSize.x; i++) {
            for (int j = 0; j < currMap.mapSize.y; j++) {
                Vector3 tilePosition = GetTileCoordToPosition(i, j);
                Transform newTile = Instantiate<Transform>(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90));
                newTile.localScale = Vector3.one * (1 - currMap.outLinePercent) * currMap.tileSize;
                newTile.parent = holeMap;

                tileMap[i, j] = newTile; //��ʼ����Ƭ��ͼ
            }
        }

        //����ϰ���
        int obstacleCount = (int)(currMap.mapSize.x * currMap.mapSize.y * currMap.obstaclePercent); //�ϰ�������
        int currObstacleCount = 0; //��ǰ�ϰ�������
        obstacleMap = new bool[currMap.mapSize.x, currMap.mapSize.y]; //��ʼ����ά����

        List<Coord> allOpenTileList = new List<Coord>(allTileCoord);

        //ע���ϰ�������������������ͨ��ͨ�����ϰ���������1
        for (int i = 0; i < obstacleCount; i++) {
            Coord obstacleCoord = GetRandomCoord();

            obstacleMap[obstacleCoord.x, obstacleCoord.y] = true; //�������������ϰ���
            currObstacleCount++;
            //���ǳ����㣨���ĵ㣩,��������ͨ��
            if (obstacleCoord != currMap.centerCoord && MapIsFullyAccessible(obstacleMap, currObstacleCount)) {

                //����߶�
                float obstacleHeight = Mathf.Lerp(currMap.minObstacleHeight, currMap.maxObstacleHeight, (float)prng.NextDouble());
                Vector3 obstaclePosition = GetTileCoordToPosition(obstacleCoord.x, obstacleCoord.y);
                Transform newObstacle = Instantiate<Transform>(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight / 2, Quaternion.identity);
                //�������ţ����ײ����м��������ţ��߶Ȳ���
                newObstacle.localScale = new Vector3((1 - currMap.outLinePercent) * currMap.tileSize, obstacleHeight, (1 - currMap.outLinePercent) * currMap.tileSize);

                float colorPrecent = obstacleCoord.y / (float)currMap.mapSize.y; //y�ὥ��
                setObstcaleColor(newObstacle, colorPrecent);

                newObstacle.parent = holeMap;
                allOpenTileList.Remove(obstacleCoord); //���ϰ�����Ƴ���ͨ����
            } else {
                obstacleMap[obstacleCoord.x, obstacleCoord.y] = false; //������������Ϊfalse
                currObstacleCount--;
            }
        }

        //��ʼ����ͨ��Ƭ�������
        shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray<Coord>(allOpenTileList.ToArray(), currMap.seed));

        //����navMeshPrefab��ת90��󣬴�С�����x��y�������x��z��
        navMeshFloor.localScale = new Vector3(maxNavMeshSize.x, maxNavMeshSize.y) * currMap.tileSize;

        //����Ѱ·��������
        Transform leftMask = Instantiate<Transform>(navMeshMaskPrefab, Vector3.left * (maxNavMeshSize.x + currMap.mapSize.x) / 4f * currMap.tileSize, Quaternion.identity, holeMap);
        leftMask.localScale = new Vector3((maxNavMeshSize.x - currMap.mapSize.x) / 2f, 1, currMap.mapSize.y) * currMap.tileSize;
        Transform rightMask = Instantiate<Transform>(navMeshMaskPrefab, Vector3.right * (maxNavMeshSize.x + currMap.mapSize.x) / 4f * currMap.tileSize, Quaternion.identity, holeMap);
        rightMask.localScale = new Vector3((maxNavMeshSize.x - currMap.mapSize.x) / 2f, 1, currMap.mapSize.y) * currMap.tileSize;
        Transform topMask = Instantiate<Transform>(navMeshMaskPrefab, Vector3.forward * (maxNavMeshSize.y + currMap.mapSize.y) / 4f * currMap.tileSize, Quaternion.identity, holeMap);
        topMask.localScale = new Vector3(maxNavMeshSize.y, 1, (maxNavMeshSize.y - currMap.mapSize.y) / 2f) * currMap.tileSize;
        Transform backMask = Instantiate<Transform>(navMeshMaskPrefab, Vector3.back * (maxNavMeshSize.y + currMap.mapSize.y) / 4f * currMap.tileSize, Quaternion.identity, holeMap);
        backMask.localScale = new Vector3(maxNavMeshSize.y, 1, (maxNavMeshSize.y - currMap.mapSize.y) / 2f) * currMap.tileSize;
    }


    //��ͼ�Ƿ���ͨ����ˮ��䣩
    private bool MapIsFullyAccessible(bool[,] obstacleMap, int currObstacleCount) {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)]; //������Ƕ�ά�����ڼ��õ��Ƿ����
        mapFlags[currMap.centerCoord.x, currMap.centerCoord.y] = true; //���ĵ�Ĭ�ϼ���
        Queue<Coord> queue = new Queue<Coord>(); //����һ����������ѭ���ж�
        queue.Enqueue(currMap.centerCoord); //���ĵ��Ȼ��ͨ�����Դ����ĵ㿪ʼ����

        int accessibleTileCount = 1; //��ͨ�������

        //�����Ԫ��,�ͼ�������
        while (queue.Count > 0) {
            Coord tile = queue.Dequeue(); //��ջһ��Ԫ��

            //�˷�����
            //  (-1,1)      (0,1)       (1,1) 
            //  (-1,0)      (0,0)       (1,0)
            //  (-1,-1)     (0,-1)      (1,-1)

            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    int nextX = tile.x + x;
                    int nextY = tile.y + y;
                    if (x == 0 || y == 0) {  //4����
                        //�߽��⣬��������ͼ��С
                        if (nextX >= 0 && nextX < obstacleMap.GetLength(0) && nextY >= 0 && nextY < obstacleMap.GetLength(1)) {
                            //û�б��������Ҳ����ϰ���
                            if (!mapFlags[nextX, nextY] && !obstacleMap[nextX, nextY]) {
                                mapFlags[nextX, nextY] = true; //���Ϊ�Ѽ��
                                queue.Enqueue(new Coord(nextX, nextY)); //�Ըõ�Ϊ����������
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)currMap.mapSize.x * (int)currMap.mapSize.y - currObstacleCount; //������ͨ�������
        return accessibleTileCount == targetAccessibleTileCount; //�����ͨ������������������������ͨ
    }


    //��ȡ�������
    private Coord GetRandomCoord() {
        Coord obstacleCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(obstacleCoord);
        return obstacleCoord;
    }

    //��ȡ�����Ƭ
    public Transform GetRandomTile() {
        Coord tileCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(tileCoord);
        return tileMap[tileCoord.x, tileCoord.y];
    }

    //����λ�û�ȡ��Ƭ
    public Transform GetTileFromPosition(Vector3 pos) {
        int x = Mathf.RoundToInt(pos.x / currMap.tileSize + (currMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(pos.z / currMap.tileSize + (currMap.mapSize.y - 1) / 2f);

        //ע��mathf.clamp ==> ���� ֵ �� �������С֮�� ���������ȡ���С����Сȡ��С
        x = Mathf.Clamp(x, 0, tileMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1) - 1);
        return tileMap[x, y];
    }

    //������Ƭ�����ȡλ��
    Vector3 GetTileCoordToPosition(int x, int y) {
        return new Vector3(-currMap.mapSize.x / 2f + .5f + x, 0, -currMap.mapSize.y / 2f + .5f + y) * currMap.tileSize;
    }

    /// <summary>
    /// �����ϰ�����ɫ
    /// </summary>
    /// <param name="obstacle">�ϰ���</param>
    /// <param name="colorPercent">������</param>
    void setObstcaleColor(Transform obstacle, float colorPercent) {
        Renderer renderer = obstacle.GetComponent<Renderer>();
        Material obstMaterial = new Material(renderer.sharedMaterial);
        obstMaterial.color = Color.Lerp(currMap.foregroundColour, currMap.backgroundColour, colorPercent); //����ɫ
        renderer.sharedMaterial = obstMaterial;
    }

    //����ṹ��
    [System.Serializable]
    public struct Coord {
        public int x;
        public int y;
        //���캯��
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
        public Coord mapSize;//��Ƭ��ͼ��С
        [Range(0, 1)]
        public float outLinePercent; //�м�����
        [Range(0, 1)]
        public float obstaclePercent; //�ϰ������
        [Min(1)]
        public float tileSize; //��Ƭ��С

        public int seed = 10; //���������
        public float minObstacleHeight; //��С�ϰ���߶�
        public float maxObstacleHeight; //����ϰ���߶�
        public Color foregroundColour;  //ǰ��ɫ
        public Color backgroundColour;  //����ɫ

        public Coord centerCoord {
            get {
                return new Coord((int)mapSize.x / 2, (int)mapSize.y / 2);
            }
        }
    }
}
