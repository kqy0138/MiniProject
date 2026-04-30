using System.Collections.Generic;
using UnityEngine;

public class _MyRoomGraphGizmoExample : MonoBehaviour
{

    /*
    // 방 데이터 구조
    [System.Serializable]
    public class RoomNode
    {
        public Vector2 position;
        public List<int> connections = new List<int>();
    }

    public List<RoomNode> nodes = new List<RoomNode>();

    void Start()
    {
        nodes.Clear();

        for (int i = 0; i < 5; i++)
        {
            RoomNode node = new RoomNode();
            node.position = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
            nodes.Add(node);
        }

        // 간단 연결
        for (int i = 0; i < nodes.Count - 1; i++)
        {
            nodes[i].connections.Add(i + 1);
        }
    }

    void OnDrawGizmos()
    {
        if (nodes == null) return;

        // 1. 방(네모) 그리기
        Gizmos.color = Color.green;
        foreach (var node in nodes)
        {
            Vector3 pos = new Vector3(node.position.x, 0, node.position.y);
            Gizmos.DrawCube(pos, Vector3.one);
        }

        // 2. 연결선 그리기
        Gizmos.color = Color.white;
        for (int i = 0; i < nodes.Count; i++)
        {
            foreach (int targetIndex in nodes[i].connections)
            {
                if (targetIndex < 0 || targetIndex >= nodes.Count) continue;

                Vector3 from = new Vector3(nodes[i].position.x, 0, nodes[i].position.y);
                Vector3 to = new Vector3(nodes[targetIndex].position.x, 0, nodes[targetIndex].position.y);

                Gizmos.DrawLine(from, to);
            }
        }
    }
    */

    // =========================================================
    // Inspector 설정값
    // =========================================================

    [Header("맵 생성 설정")]

    [Tooltip("생성할 방의 총 개수")]
    public int roomCount = 10;

    [Tooltip("방과 방 사이의 거리 (Scene에서 얼마나 떨어져 보일지)")]
    public float spacing = 4f;

    [Tooltip("Scene View에서 보이는 방 네모 크기")]
    public float size = 1.5f;


    // =========================================================
    // 방 타입 정의
    // =========================================================

    /// <summary>
    /// 각 방이 어떤 역할을 가지는지 정의
    /// </summary>
    enum RoomType
    {
        None,
        Start,   // 시작방
        Normal,  // 일반방
        Combat,  // 전투방
        Shop,    // 상점
        Event,   // 이벤트
        Boss     // 보스방
    }


    // =========================================================
    // 방 데이터 구조
    // =========================================================

    /// <summary>
    /// 방 하나의 정보
    /// - 위치 (좌표)
    /// - 타입 (역할)
    /// </summary>
    class Room
    {
        public Vector2Int pos;     // 격자 좌표
        public RoomType type;      // 방 종류
    }


    // =========================================================
    // 내부 데이터 저장
    // =========================================================

    /// <summary>
    /// 생성된 모든 방 데이터 리스트
    /// </summary>
    List<Room> rooms = new List<Room>();

    /// <summary>
    /// 이미 생성된 좌표를 빠르게 체크하기 위한 집합 (중복 방 방지)
    /// </summary>
    HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();


    /// <summary>
    /// 방을 확장할 때 사용하는 방향 배열 (상하좌우)
    /// </summary>
    Vector2Int[] dirs = {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };


    private void Start()
    {
        // 게임 시작 시 맵 생성
        GenerateMap();
    }


    // =========================================================
    // 전체 맵 생성 흐름
    // =========================================================

    /// <summary>
    /// 맵 생성의 전체 흐름
    /// 1. 방 위치 생성
    /// 2. 방 타입 지정
    /// </summary>
    void GenerateMap()
    {
        // 이전 데이터 초기화
        rooms.Clear();
        occupied.Clear();

        GenerateRooms();      // 1단계: 구조 생성
        AssignRoomTypes();    // 2단계: 의미 부여
    }


    // =========================================================
    // 1단계: 방 좌표 생성 (그래프 생성)
    // =========================================================

    /// <summary>
    /// 랜덤 워크 방식으로 방을 하나씩 확장하며 생성
    /// - 기존 방에서 상하좌우로 확장
    /// - 중복 좌표는 생성하지 않음
    /// </summary>
    void GenerateRooms()
    {
        // 시작 방 (항상 원점)
        Vector2Int start = Vector2Int.zero;

        rooms.Add(new Room { pos = start });
        occupied.Add(start);

        // 목표 개수까지 반복 생성
        while (rooms.Count < roomCount)
        {
            // 기존 방 중 하나를 랜덤 선택
            Vector2Int baseRoom = rooms[Random.Range(0, rooms.Count)].pos;

            // 방향 선택 (상하좌우)
            Vector2Int dir = dirs[Random.Range(0, dirs.Length)];

            // 새로운 방 위치 계산
            Vector2Int newPos = baseRoom + dir;

            // 이미 존재하는 위치라면 스킵 (중복 방지)
            if (occupied.Contains(newPos))
                continue;

            // 새로운 방 추가
            rooms.Add(new Room { pos = newPos });
            occupied.Add(newPos);
        }
    }


    // =========================================================
    // 2단계: 방 타입 지정
    // =========================================================

    /// <summary>
    /// 각 방에 역할(타입)을 부여
    /// - 시작방은 첫 번째 방
    /// - 가장 먼 방은 보스방
    /// - 나머지는 확률로 분배
    /// </summary>
    void AssignRoomTypes()
    {
        // 시작방 지정
        rooms[0].type = RoomType.Start;

        Vector2Int startPos = rooms[0].pos;

        int maxDist = -1;
        int bossIndex = 0;

        // 시작점에서 가장 먼 방 찾기
        for (int i = 1; i < rooms.Count; i++)
        {
            int dist = Mathf.Abs(rooms[i].pos.x - startPos.x)
                     + Mathf.Abs(rooms[i].pos.y - startPos.y);

            if (dist > maxDist)
            {
                maxDist = dist;
                bossIndex = i;
            }
        }

        // 보스방 지정
        rooms[bossIndex].type = RoomType.Boss;

        // 나머지 방 타입 랜덤 분배
        for (int i = 1; i < rooms.Count; i++)
        {
            if (i == bossIndex) continue;

            float r = Random.value;

            if (r < 0.7f)
                rooms[i].type = RoomType.Combat;
            else
                rooms[i].type = RoomType.Shop;
        }
    }


    // =========================================================
    // Gizmo 그리기 (Scene View 디버깅용)
    // =========================================================

    /// <summary>
    /// Scene View에서 방과 연결을 시각적으로 표시
    /// </summary>
    void OnDrawGizmos()
    {
        if (rooms == null || rooms.Count == 0)
            return;

        DrawConnections(); // 선 먼저
        DrawRooms();       // 방 나중
    }


    /// <summary>
    /// 방을 네모로 표시
    /// </summary>
    void DrawRooms()
    {
        foreach (var room in rooms)
        {
            // 방 타입에 따른 색상 지정
            Gizmos.color = GetColor(room.type);

            Vector3 pos = GridToWorld(room.pos);

            // 채워진 큐브
            Gizmos.DrawCube(pos, Vector3.one * size * 0.9f);

            // 테두리
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(pos, Vector3.one * size);
        }
    }


    /// <summary>
    /// 인접한 방끼리 선으로 연결 표시
    /// </summary>
    void DrawConnections()
    {
        Gizmos.color = Color.white;

        foreach (var room in rooms)
        {
            foreach (var dir in dirs)
            {
                Vector2Int neighbor = room.pos + dir;

                // 인접 방이 없으면 스킵
                if (!occupied.Contains(neighbor))
                    continue;

                // 중복 선 방지
                if (neighbor.x < room.pos.x) continue;
                if (neighbor.y < room.pos.y) continue;

                Vector3 from = GridToWorld(room.pos);
                Vector3 to = GridToWorld(neighbor);

                Gizmos.DrawLine(from, to);
            }
        }
    }


    // =========================================================
    // 유틸 함수
    // =========================================================

    /// <summary>
    /// 방 타입에 따른 Gizmo 색상 반환
    /// </summary>
    Color GetColor(RoomType type)
    {
        switch (type)
        {
            case RoomType.Start: return Color.green;
            case RoomType.Boss: return Color.red;
            case RoomType.Combat: return Color.cyan;
            case RoomType.Shop: return Color.yellow;
        }
        return Color.white;
    }


    /// <summary>
    /// 격자 좌표를 Unity 월드 좌표로 변환
    /// </summary>
    Vector3 GridToWorld(Vector2Int pos)
    {
        return new Vector3(pos.x * spacing, pos.y * spacing, 0);
    }



}
