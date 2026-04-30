using System;
using UnityEngine;

public class DoorSpawnerExmple : MonoBehaviour
{
    [Header("Ref")]
    public RoomGraphExample dungeon;

    [Header("Door Prefab")]
    public GameObject doorPrefab;


    // 카메라 제어 스크립트 참조
    // Inspector에서 연결
    public CameraFollowExample cameraController;


    public void SpawnDoor()
    { 
        if (dungeon == null || dungeon.connections == null) return;

        foreach (var conn in dungeon.connections)
        {

            // 문은 하나만 생성
            CreateDoor(conn.Item1.pos, conn.Item2.pos);


        }
    }

    // =========================================================
    // 문 생성 함수 (양방향 이동용 단일 문)
    // =========================================================
    private void CreateDoor(Vector2Int roomA, Vector2Int roomB)
    {
        // 방향 계산
        Vector2Int dir = roomB - roomA;

        // 현재 방 중심 위치
        Vector3 center = GridToWorld(roomA);

        // 방향 기반 문 위치 계산
        Vector3 doorPos = center;

        float halfX = dungeon.spacingX / 2f;
        float halfY = dungeon.spacingY / 2f;

        // 방향별 위치 설정
        if (dir == Vector2Int.right)
        {
            doorPos += new Vector3(halfX, 0, 0);
        }
        else if (dir == Vector2Int.left)
        {
            doorPos += new Vector3(-halfX, 0, 0);
        }
        else if (dir == Vector2Int.up)
        {
            doorPos += new Vector3(0, halfY, 0);
        }
        else if (dir == Vector2Int.down)
        {
            doorPos += new Vector3(0, -halfY, 0);
        }



        // 문 생성
        GameObject door = Instantiate(doorPrefab, doorPos, Quaternion.identity);

        // 4️⃣ DoorTrigger 가져오기
        DoorTriggerExample trigger = door.GetComponent<DoorTriggerExample>();

        if (trigger != null)
        {
            // 도착 방 설정
            trigger.roomA = roomA;
            trigger.roomB = roomB;
            trigger.dir = dir;

            trigger.SetupCollider();

            // spacing 전달
            trigger.spacingX = dungeon.spacingX;
            trigger.spacingY = dungeon.spacingY;

            // 카메라 참조 전달 
            trigger.cameraController = cameraController;
        }
        // 디버깅용 이름
        door.name = $"Door_{roomA}_to_{roomB}";

    }


    private Vector3 GridToWorld(Vector2Int grid)
    {
        return new Vector3(
            grid.x * dungeon.spacingX,
            grid.y * dungeon.spacingY,
            0f);
    }


}
