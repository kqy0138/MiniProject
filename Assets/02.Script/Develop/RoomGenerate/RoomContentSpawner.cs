
using System.Collections.Generic;
using UnityEngine;
using static RoomGraphExample;
using Random = UnityEngine.Random;

public class RoomContentSpawner : MonoBehaviour
{
    [Header("생성 범위 (방 내부 영역)")]
    // 오브젝트가 생성될 영역 크기 (중심 기준)
    private Vector2 spawnAreaSize = new Vector2(14f, 6f);

    private string roomID;
    private RoomGraphExample.RoomType roomType;

    [Header("적 Prefabs")]
    // 생성 가능한 적 프리팹 목록
    public List<GameObject> enemyPrefabs;

    [Header("장애물 Prefabs")]
    // 생성 가능한 장애물 프리팹 목록
    public List<GameObject> obstaclePrefabs;

    [Header("아이템 Prefabs")]
    // 생성 가능한 아이템 프리팹 목록
    public List<GameObject> itemPrefabs;

    [Header("생성 개수 범위")]
    // 최소~최대 생성 개수 범위
    public Vector2Int enemyCountRange = new Vector2Int(1, 2);
    public Vector2Int obstacleCountRange = new Vector2Int(1, 3);
    public Vector2Int itemCountRange = new Vector2Int(0, 1);

    [Header("겹침 방지")]
    // 충돌 검사 반경 (오브젝트 크기에 맞게 조절)
    public float checkRadius = 0.8f;
    // 최대 시도 횟수 (무한 루프 방지)
    public int maxTryCount = 30;


    // =========================
    // 외부에서 호출되는 메인 함수
    // =========================
    /// <summary>
    /// 방 타입에 따라 내부 콘텐츠 생성 시작
    /// (RoomInstance에서 호출됨)
    /// </summary>
    public void GenerateContent(RoomGraphExample.RoomType roomType)
    {
        // 방 타입별로 다르게 생성
        switch (roomType)
        {
            case RoomGraphExample.RoomType.Combat:
                SpawnObject(enemyPrefabs, enemyCountRange);
                SpawnObject(obstaclePrefabs, obstacleCountRange);
                
                break;

            case RoomGraphExample.RoomType.Event:
                SpawnBoss();
                break;

            case RoomGraphExample.RoomType.Normal:
                SpawnObject(obstaclePrefabs, obstacleCountRange);
                break;

            case RoomGraphExample.RoomType.Shop:
                SpawnObject(itemPrefabs, itemCountRange);
                break;

            case RoomGraphExample.RoomType.Boss:
                //SpawnBoss();
                break;
        }
    }

    

    // =========================
    // 공통 생성 함수
    // =========================

    void SpawnObject(List<GameObject> prefabList, Vector2Int countRange)
    {
        int count = Random.Range(countRange.x, countRange.y + 1);

        for(int i = 0; i < count; ++i)
        {
            GameObject prefab = GetRandomPrefab(prefabList);
            if (prefab == null) continue;

            Vector3 spawnPos;

            // 겹치지 않는 위치 찾기
            bool found = TryGetValiPosition(prefab, out spawnPos);

            if(found)
            {
                GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity, transform);

                OverworldEnemy enemy = obj.GetComponent< OverworldEnemy>();

                if (enemy != null)
                {
                    enemy.enemyID = roomID + "_" + i;
                }
            }
            else
            {
                Debug.Log("배치 실패(공간 부족)");
            }
        }

    }

    /// <summary>
    /// 겹치지 않는 위치를 찾는 함수
    /// </summary>
    
    private bool TryGetValiPosition(GameObject prefab, out Vector3 result)
    {
        // 🔥 prefab 크기 가져오기
        Collider2D col = prefab.GetComponentInChildren<Collider2D>();

        float radius = checkRadius;

        if (col != null)
        {
            radius = Mathf.Max(col.bounds.extents.x, col.bounds.extents.y);
        }

        for (int i = 0; i < maxTryCount; ++i)
        {
            Vector3 pos = GetRandomPosition(prefab);
            // Physics2D.OverlapCircle(pos, radius); 이 구간 개념 체크 / * 1.2f 해서 여유값 추가
            Collider2D hit = Physics2D.OverlapCircle(pos, radius * 1.2f);

            if (hit == null)
            {
                result = pos;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }



    /// <summary>
    /// 랜덤 위치 계산 (벽 나가지 않게 보정 포함)
    /// </summary>
    private Vector3 GetRandomPosition(GameObject prefab)
    {
        Collider2D col = prefab.GetComponentInChildren<Collider2D>();

        Vector2 size = Vector2.one;

        if(col != null)
        {
            size = col.bounds.size;
        }

        float paddingX = size.x / 2f;
        float paddingY = size.y / 2f;

        float x = Random.Range(
            -spawnAreaSize.x / 2 + paddingX,
            spawnAreaSize.x / 2 - paddingX);

        float y = Random.Range(
            -spawnAreaSize.y / 2 + paddingY,
            spawnAreaSize.y / 2 - paddingY);

        return transform.position + new Vector3(x, y, 0);
    }


    // 랜덤 프리팹 선택
    GameObject GetRandomPrefab(List<GameObject> list)
    {
        if(list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];

    }


    // =========================
    // 보스 생성
    // =========================
    void SpawnBoss()
    {
        if (enemyPrefabs.Count == 0) return;

        GameObject prefab = enemyPrefabs[0]; // 첫 번째를 보스로 사용 (임시)
        Instantiate(prefab, transform.position, Quaternion.identity, transform);
    }

    public void SetRoomID(string id)
    {
        roomID = id;

        TryGenerate();

    }

    private bool hasGenerated = false;

    private void TryGenerate()
    {
        // 둘 다 있어야 실행
        if (hasGenerated) return;
        if (string.IsNullOrEmpty(roomID)) return;
        if (roomType == RoomGraphExample.RoomType.None) return;

        hasGenerated = true;

        GenerateContent(roomType);
    }

    /*기존 랜덤 생성

    // =========================
    // 랜덤 위치 계산
    // =========================
    Vector3 GetRandomPosition()
    {
        float x = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float y = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);

        return transform.position + new Vector3(x, y, 0);
    }

    // =========================
    // 랜덤 프리팹 선택
    // =========================
    GameObject GetRandomPrefab(List<GameObject> list)
    {
        if (list == null || list.Count == 0) return null;

        return list[Random.Range(0, list.Count)];
    }
    
    // =========================
    // Enemy 생성
    // =========================
    void SpawnEnemies()
    {
        int count = Random.Range(enemyCountRange.x, enemyCountRange.y + 1);

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = GetRandomPrefab(enemyPrefabs);
            Vector3 pos = GetRandomPosition(prefab);

            Instantiate(prefab, pos, Quaternion.identity, transform);
        }
    }
    
    // =========================
    // 장애물 생성
    // =========================
    void SpawnObstacles()
    {
        int count = Random.Range(obstacleCountRange.x, obstacleCountRange.y + 1);

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = GetRandomPrefab(obstaclePrefabs);
            Vector3 pos = GetRandomPosition(prefab);

            Instantiate(prefab, pos, Quaternion.identity, transform);
        }
    }
    // =========================
    // 아이템 생성
    // =========================
    void SpawnItems()
    {
        int count = Random.Range(itemCountRange.x, itemCountRange.y + 1);

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = GetRandomPrefab(itemPrefabs);
            Vector3 pos = GetRandomPosition(prefab);

            Instantiate(prefab, pos, Quaternion.identity, transform);
        }
    }
    */


}