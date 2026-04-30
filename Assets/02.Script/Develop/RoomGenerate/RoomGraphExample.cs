
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor; // Scene에 텍스트 표시용
#endif

// 방그래프 생성 로직
public class RoomGraphExample : MonoBehaviour
{
    // =========================
    // 기본 설정
    // =========================

    [Header("랜덤 시드 (같으면 같은 맵 생성)")]
    [SerializeField] private int seed;

    [Header("생성할 방 개수")]
    public int roomCount = 20;

    [Header("방 간격 (Scene에서 얼마나 떨어질지)")]
    public float spacingX = 22f; // 가로
    public float spacingY = 14f; // 세로

    [Tooltip("Scene View에서 방 네모 크기")]
    public float roomGizmoSize = 1.8f;

    [Header("방 타입 비율 (Start/Boss 제외 나머지 방 기준)")]
    [Range(0f, 1f)] public float combatRatio = 0.5f; // 전투방 비율
    [Range(0f, 1f)] public float shopRatio = 0.2f;   // 상점방 비율
    [Range(0f, 1f)] public float eventRatio = 0.2f;  // 이벤트방 비율


    [Header("Room Prefabs")]
    // Start 방 프리팹
    public GameObject startRoomPrefab;
    // 일반 방 프리팹
    public GameObject normalRoomPrefab;
    // 전투 방 프리팹
    public GameObject combatRoomPrefab;
    // 상점 방 프리팹
    public GameObject shopRoomPrefab;
    // 이벤트 방 프리팹
    public GameObject eventRoomPrefab;
    // 보스 방 프리팹
    public GameObject bossRoomPrefab;

    // 생성된 실제 방 오브젝트들을 저장
    // 나중에 삭제/관리용
    private List<GameObject> spawnedRooms = new List<GameObject>();


    [SerializeField] private DoorSpawnerExmple doorSpawner;


    // =========================
    // 1. 방 타입 정의
    // =========================
    public enum RoomType
    {
        None,    // 아직 타입이 정해지지 않은 상태
        Start,   // 시작방 (단 하나)
        Normal,  // 일반방 (나머지)
        Combat,  // 전투방
        Shop,    // 상점
        Event,   // 이벤트
        Boss     // 보스방 (단 하나)
        // Goal // 클리어 지점
    }

    // =========================
    // 2. 방 데이터 클래스
    // =========================
    public class Room
    {
        public Vector2Int pos;  // 방의 격자 좌표
        public RoomType type;   // 방의 타입
    }


    // =========================
    // 내부 데이터
    // =========================
    // 생성된 모든 방 리스트
    private List<Room> rooms = new List<Room>();


    // 중복 방 방지
    private HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

    // 방과 방 사이 연결 정보 (길)
    public List<(Room, Room)> connections = new List<(Room, Room)>();

    // 상하좌우 방향
    private Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    // =========================
    // 시작 시 생성
    // =========================

    void Start()
    {
        GenerateAll();

    }

    [ContextMenu("Generate All")]
    private void GenerateAll()
    {
        //seed = Random.Range(0,100);
        //// 랜덤 시드 적용 (같은 seed면 같은 결과)
        //Random.InitState(seed);

        Random.InitState(GameStateDataManager.Instance.mapSeed);
        seed = GameStateDataManager.Instance.mapSeed;

        // 그래프 생성
        GenerateGraph();
        // 방 타입 지정
        AssignRoomTypes();

        // 실제 방 생성
        SpawnRooms();

        if (doorSpawner != null)
        {
            doorSpawner.SpawnDoor();
        }
    }



    // =========================
    // 맵 그래프 생성
    // =========================
    [ContextMenu("1) Generate Room Graph")]
    void GenerateGraph()
    {
        // 이전 데이터 초기화
        rooms.Clear();
        occupied.Clear();
        connections.Clear();

        // 시작 방 생성 (0,0)
        Room start = new Room();
        start.pos = Vector2Int.zero;
        start.type = RoomType.Start;

        rooms.Add(start);
        occupied.Add(start.pos);

        int safety = 1000; // 무한 루프 방지

        // 목표 개수까지 반복
        while (rooms.Count < roomCount && safety > 0)
        {
            safety--;

            // 기존 방 중 하나 선택
            Room baseRoom = rooms[Random.Range(0, rooms.Count)];

            // 방향 선택
            Vector2Int dir = directions[Random.Range(0, directions.Length)];

            // 새 위치 계산
            Vector2Int newPos = baseRoom.pos + dir;

            // 이미 있으면 패스
            if (occupied.Contains(newPos))
                continue;

            // 새 방 생성
            Room newRoom = new Room();
            newRoom.pos = newPos;
            newRoom.type = RoomType.None;

            // 추가
            rooms.Add(newRoom);
            occupied.Add(newPos);

            // 연결 추가 (길 생성)
            connections.Add((baseRoom, newRoom));
        }

        Debug.Log("그래프+방 생성 완료: " + rooms.Count);
    }


    // =========================================================
    // 방 타입 배정
    // =========================================================
    [ContextMenu("2) Assign Room Types")]
    private void AssignRoomTypes()
    {
        if (rooms == null || rooms.Count == 0)
        {
            Debug.LogWarning("방 데이터가 없습니다.");
            return;
        }
        // ---------- 1) Start 방 ----------
        // (0,0) 좌표를 가진 방 찾기
        Room startRoom = rooms.Find(r => r.pos == Vector2Int.zero);

        if (startRoom == null)
        {
            Debug.LogError("Start room not found!");
            return;
        }

        startRoom.type = RoomType.Start;

        // ---------- 2) Boss 방 ----------
        // 시작방에서 가장 먼 방 찾기

        Room bossRoom = null;
        int maxDist = -1;


        foreach (var room in rooms)
        {
            if (room == startRoom) continue;

            // 맨해튼 거리 계산
            int dist = Mathf.Abs(room.pos.x) + Mathf.Abs(room.pos.y);

            if (dist > maxDist)
            {
                maxDist = dist;
                bossRoom = room;
            }
        }
        if (bossRoom != null)
        {
            bossRoom.type = RoomType.Boss;
        }
        else
        {
            Debug.LogWarning("Boss room not assigned");
        }

        // ---------- 3) 나머지 방 리스트 ----------
        List<Room> remaining = new List<Room>();

        foreach (var room in rooms)
        {
            if (room.type == RoomType.None)
                remaining.Add(room);
        }

        int total = remaining.Count;

        // ---------- 4) 비율 → 개수 변환 ----------
        int combatCount = Mathf.FloorToInt(total * combatRatio);
        int shopCount = Mathf.FloorToInt(total * shopRatio);
        int eventCount = Mathf.FloorToInt(total * eventRatio);

        // 남은 방 = Normal
        int normalCount = total - (combatCount + shopCount + eventCount);

        // 혹시 비율 합이 너무 커서 음수가 나오면 보정
        if (normalCount < 0)
        {
            normalCount = 0;
        }

        // ---------- 5) 랜덤 섞기 ----------

        // 후보 방 목록을 섞기
        Shuffle(remaining);

        // 섞인 순서대로 타입 배정
        int index = 0;

        // Combat 배정
        for (int i = 0; i < combatCount && index < remaining.Count; i++)
        {
            remaining[index++].type = RoomType.Combat;
        }

        // Shop 배정
        for (int i = 0; i < shopCount && index < remaining.Count; i++)
        {
            remaining[index++].type = RoomType.Shop;
        }

        // Event 배정
        for (int i = 0; i < eventCount && index < remaining.Count; i++)
        {
            remaining[index++].type = RoomType.Event;
        }

        // 나머지는 Normal
        for (; index < remaining.Count; index++)
        {
            remaining[index].type = RoomType.Normal;
        }

    }

    // =========================================================
    // 3. Room → Prefab 생성
    // =========================================================
    [ContextMenu("3) Spawn Rooms")]
    private void SpawnRooms()
    {
        // 기존에 생성된 방 제거
        ClearSpawnedRooms();

        // 모든 Room 데이터를 순회
        foreach (var room in rooms)
        {
            // RoomType에 맞는 Prefab 가져오기
            GameObject prefab = GetPrefabByType(room.type);

            // 프리팹이 없으면 스킵 (에러 방지)
            if (prefab == null)
            {
                Debug.LogWarning($"Prefab missing for {room.type}");
                continue;
            }

            // Grid 좌표 → 월드 좌표 변환
            Vector3 worldPos = GridToWorld(room.pos);

            // 실제 게임 오브젝트 생성
            GameObject instance = Instantiate(prefab, worldPos, Quaternion.identity);

            // 이름 설정 (디버깅 및 ID 확인)
            instance.name = $"{room.type}_Room_{room.pos}";

            RoomInstanceExample roomInstance = instance.GetComponent<RoomInstanceExample>();
            if (roomInstance != null)
            {
                roomInstance.Init(room.type);
                // spawner 가 방의 이름을 알 수 있도록
                RoomContentSpawner spawner = instance.GetComponentInChildren<RoomContentSpawner>();

                if (spawner != null)
                {
                    // 여기서 ID 전달
                    spawner.SetRoomID(instance.name); // 🔥 핵심
                    spawner.GenerateContent(room.type); // 🔥 여기로 이동

                }


            }



            // 리스트에 저장 (관리용)
            spawnedRooms.Add(instance);
        }
    }

    // =========================================================
    // 기존에 생성된 방 오브젝트 제거
    // =========================================================
    private void ClearSpawnedRooms()
    {
        // 생성된 오브젝트가 없으면 종료
        if (spawnedRooms == null) return;

        for (int i = 0; i < spawnedRooms.Count; i++)
        {
            if (spawnedRooms[i] != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(spawnedRooms[i]); // 에디터용 즉시 삭제
#else
            Destroy(spawnedRooms[i]); // 플레이 모드용
#endif
            }
        }

        // 리스트 초기화
        spawnedRooms.Clear();
    }

    // =========================================================
    // RoomType → Prefab 매핑 함수
    // =========================================================
    private GameObject GetPrefabByType(RoomType type)
    {
        switch (type)
        {
            case RoomType.Start:
                return startRoomPrefab;

            case RoomType.Normal:
                return normalRoomPrefab;

            case RoomType.Combat:
                return combatRoomPrefab;

            case RoomType.Shop:
                return shopRoomPrefab;

            case RoomType.Event:
                return eventRoomPrefab;

            case RoomType.Boss:
                return bossRoomPrefab;

            default:
                return null;
        }
    }

    // =========================================================
    // 리스트 셔플 함수 (랜덤 섞기)
    // =========================================================
    void Shuffle(List<Room> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }


    // =========================
    // Gizmo로 시각화
    // =========================

    void OnDrawGizmos()
    {
        // 실행 중이 아니면 그리지 않음 (Start 이후에만 보이게)
        if (rooms == null || rooms.Count == 0)
            return;
        if (connections == null)
            return;

        DrawConnections();
        DrawRooms();

    }

    // -------------------------
    // 연결선 그리기
    // -------------------------
    private void DrawConnections()
    {
        Gizmos.color = Color.gray;

        foreach (var conn in connections)
        {
            Vector3 a = GridToWorld(conn.Item1.pos);
            Vector3 b = GridToWorld(conn.Item2.pos);

            Gizmos.DrawLine(a, b);
        }

    }

    // -------------------------
    // 방 네모 + 텍스트
    // -------------------------
    private void DrawRooms()
    {

        for (int i = 0; i < rooms.Count; i++)
        {
            Room room = rooms[i];
            Vector3 pos = GridToWorld(room.pos);

            Gizmos.color = GetColorByRoomType(room.type);
            Gizmos.DrawCube(pos, Vector3.one * roomGizmoSize * 0.9f);

            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(pos, Vector3.one * roomGizmoSize);

#if UNITY_EDITOR
            // 텍스트 표시
            Handles.Label(
                pos + Vector3.up * 0.6f,
                $"{room.type}\n({room.pos.x},{room.pos.y})"
            );
#endif
        }

        //        foreach (var room in rooms)
        //        {
        //            Vector3 pos = GridToWorld(room.pos);

        //            // 타입별 색상
        //            switch (room.type)
        //            {
        //                case RoomType.Start: Gizmos.color = Color.green; break;
        //                case RoomType.Boss: Gizmos.color = Color.red; break;
        //                case RoomType.Combat: Gizmos.color = Color.white; break;
        //                case RoomType.Shop: Gizmos.color = Color.blue; break;
        //                case RoomType.Event: Gizmos.color = Color.yellow; break;
        //                case RoomType.Normal: Gizmos.color = Color.cyan; break;
        //                default: Gizmos.color = Color.magenta; break;
        //            }

        //            Gizmos.DrawCube(pos, Vector3.one * roomGizmoSize);

        //#if UNITY_EDITOR
        //            // 텍스트 표시
        //            Handles.Label(
        //                pos + Vector3.up * 0.6f,
        //                $"{room.type}\n({room.pos.x},{room.pos.y})"
        //            );
        //#endif
        //        }
    }

    private Color GetColorByRoomType(RoomType type)
    {
        switch (type)
        {
            case RoomType.Start:
                return Color.green;

            case RoomType.Normal:
                return new Color(0.7f, 0.9f, 1f); // 연한 하늘색

            case RoomType.Combat:
                return Color.red;

            case RoomType.Shop:
                return Color.yellow;

            case RoomType.Event:
                return new Color(1f, 0.4f, 1f); // 보라/핑크 계열

            case RoomType.Boss:
                return new Color(0.4f, 0f, 0f); // 어두운 빨강

            default:
                return Color.white;
        }
    }

    // -------------------------
    // 좌표 변환
    // -------------------------
    Vector3 GridToWorld(Vector2Int grid)
    {
        return new Vector3(grid.x * spacingX, grid.y * spacingY, 0f);
    }
}
