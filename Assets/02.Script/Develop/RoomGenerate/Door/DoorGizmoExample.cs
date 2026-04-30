using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

// 방과 방 사이 연결하는 문 생성 로직
public class DoorGizmoExample : MonoBehaviour
{
    // =========================
    // 연결할 대상 (방 생성 스크립트)
    // =========================

    [Header("Dungeon Generator 참조")]
    public RoomGraphExample dungeon;

    [Header("문 Gizmo 설정")]
    public float doorSize = 0.5f;

    // 문 색상
    public Color doorColor = Color.magenta;

    // 연결 선 색상
    public Color lineColor = Color.gray;


    // =========================
    // Gizmo 그리기
    // =========================
    private void OnDrawGizmos()
    {
        // dungeon이 연결되지 않았으면 종료
        if (dungeon == null) return;

        // connections가 없으면 종료
        if (dungeon.connections == null || dungeon.connections.Count == 0) return;

        DrawDoors();

    }


    // =========================
    // 문 + 연결선 그리기
    // =========================

    private void DrawDoors()
    {
        foreach (var conn in dungeon.connections)
        {
            Vector3 a = GridToWorld(conn.Item1.pos);
            Vector3 b = GridToWorld(conn.Item2.pos);

            // 연결선(길)
            Gizmos.color = lineColor;
            Gizmos.DrawLine(a, b);

            // 문 위치 계산(중간 지점)
            Vector3 doorPos = (a + b) / 2f;

            // 문 표시 (작은 큐브)
            Gizmos.color = doorColor;
            Gizmos.DrawCube(doorPos, Vector3.one * doorSize);
#if UNITY_EDITOR
            // 문 위치에 텍스트 표시 (디버깅용)
            Handles.Label(doorPos + Vector3.up * 0.3f, "Door");
#endif

        }

    }

    // =========================
    // 좌표 변환 함수
    // =========================
    Vector3 GridToWorld(Vector2Int grid)
    {
        // dungeon의 spacing을 사용해서 동일한 좌표계 유지
        return new Vector3(
            grid.x * dungeon.spacingX,
            grid.y * dungeon.spacingY,
            0f
        );
    }
}
