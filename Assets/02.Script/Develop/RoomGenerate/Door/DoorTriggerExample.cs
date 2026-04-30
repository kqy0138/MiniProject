using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerExample : MonoBehaviour
{
    [SerializeField]
    private LayerMask playerLayer;

    // -----------------------------
    // 이 문이 연결하는 두 방의 좌표
    // -----------------------------
    public Vector2Int roomA; // 한쪽 방
    public Vector2Int roomB; // 반대쪽 방

    // 방 간 거리 (RoomGraph에서 전달받음)
    public float spacingX;
    public float spacingY;

    // 중복 이동 방지용 - 이동 중인지 체크
    private bool isMoving = false;

    // ---------------------------------
    // 문 안쪽으로 들어가도록 거리 설정
    // (값은 상황에 맞게 조절)
    // ---------------------------------
    // enterOffset -> 문에서 얼마나 안쪽으로 들어갈지 거리
    public float enterOffset = 1.5f;

    // 외부에서 전달받는 카메라 컨트롤러
    public CameraFollowExample cameraController;

    [HideInInspector]
    public Vector2Int dir;


    // ======================================================================================





    // 플레이어가 충돌했을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 방어 코드
        // collider 가 없으면 return
        if (collision == null) return;
        // 이동중이면 return
        if (isMoving) return;


        // 해당 오브젝트의 레이어가 playerLayer인지 판단
        // Player만 반응 (Layer 기반)
        if ((playerLayer.value & (1 << collision.gameObject.layer)) == 0) return;

        isMoving = true;

        Transform player = collision.transform;

        // -----------------------------
        // 플레이어 위치 기준으로
        // 더 가까운 방을 "현재 위치"로 판단
        // -----------------------------
        Vector3 posA = GridToWorld(roomA);
        Vector3 posB = GridToWorld(roomB);

        //Distance 개념 체크
        float distA = Vector3.Distance(player.position, posA);
        float distB = Vector3.Distance(player.position, posB);

        // 더 가까운 쪽이 현재 방
        // → 반대쪽으로 이동
        Vector2Int targetRoom = distA < distB ? roomB : roomA;


        // playerLayer 라면 이동시킴
        MovePlayer(player, targetRoom);

        // 잠깐 후 다시 이동 가능
        Invoke(nameof(ResetMove), 0.2f);

    }

    // =========================================
    // 플레이어를 "문 앞 위치"로 이동
    // =========================================
    private void MovePlayer(Transform player, Vector2Int targetRoomPos)
    {

        isMoving = true;

        // 이동 전 위치 저장 (디버깅용)
        Vector3 beforePos = player.position;

        // =========================================
        // 현재 위치 기준으로 방향 계산
        // =========================================
        Vector2Int currentRoom;

        float distA = Vector3.Distance(player.position, GridToWorld(roomA));
        float distB = Vector3.Distance(player.position, GridToWorld(roomB));

        currentRoom = distA < distB ? roomA : roomB;

        Vector2Int dirgrid = targetRoomPos - currentRoom;

        Vector3 dir = new Vector3(dirgrid.x, dirgrid.y,0).normalized;

        // doorPos = 현재 문 위치(기준점)
        Vector3 doorPos = transform.position;

        // =========================================
        // 최종 이동 위치 계산
        // =========================================
        Vector3 targetPos = doorPos + dir * enterOffset;

        // 이동 적용
        player.position = targetPos;

        // 카메라 이동
        if (cameraController != null)
        {
            Vector3 roomCenter = GridToWorld(targetRoomPos);
            cameraController.MoveTo(roomCenter);
        }

        // =========================================
        // 이동 종료 처리 (짧은 딜레이 후 해제)
        // =========================================
        Invoke(nameof(ResetMove), 0.2f);

    }


    // Grid → World 변환
    private Vector3 GridToWorld(Vector2Int grid)
    {
        return new Vector3(
            grid.x * spacingX,
            grid.y * spacingY,
            0f);
    }

    private void ResetMove()
    {
        isMoving = false;
    }



    // =========================================
    // Gizmo로 문 충돌 범위 시각화
    // =========================================
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null) return;

        // =========================================
        // 실제 Collider 기준으로 그림
        // -----------------------------------------
        // → Gizmo == 실제 충돌 범위
        // =========================================
        Vector3 center = transform.position + (Vector3)col.offset;
        Vector3 size = col.size;

        Gizmos.DrawWireCube(center, size);

    }


    // =========================================
    // Collider 자동 설정 함수
    // -----------------------------------------
    // 역할:
    // - 방향에 따라 Collider 크기 / 위치 설정
    // - Gizmo와 동일한 기준 사용
    // =========================================

    public void SetupCollider()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null) return;

        // =========================================
        // 기본 크기 설정 (여기서 조절)
        // =========================================
        float width = 3.5f;   // 가로 길이
        float height = 1.0f;  // 세로 길이

        // =========================================
        // 방향에 따라 Collider 크기 변경
        // =========================================
        if (dir == Vector2Int.left || dir == Vector2Int.right)
        {
            // 👉 가로형 (좌/우 이동)
            col.size = new Vector2(width, height);
        }
        else if (dir == Vector2Int.up || dir == Vector2Int.down)
        {
            // 👉 세로형 (상/하 이동)
            col.size = new Vector2(height, width);
        }

        // =========================================
        // offset 초기화 (중심 기준)
        // -----------------------------------------
        // 필요하면 문을 벽 안쪽으로 밀 수 있음
        // =========================================
        col.offset = Vector2.zero;

        // Trigger 보장
        col.isTrigger = true;
    }




}