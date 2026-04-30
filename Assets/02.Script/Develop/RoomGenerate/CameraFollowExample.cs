using UnityEngine;

// 카메라를 특정 위치로 이동시키는 스크립트
public class CameraFollowExample : MonoBehaviour
{
    // 카메라 이동 속도 (부드러운 이동용)
    public float moveSpeed = 10f;

    // 목표 위치
    private Vector3 targetPos;

    // 즉시 이동 여부
    private bool isMoving = false;

    [SerializeField] private Transform player;

    private void Start()
    {
        //여기서 플레이어 Position 가져오면 플레이어가 아직 위치 복원 전일 수 있음
       // start에서는 카메라를 건드리지 않고 플레이어가 직접 호출하도록 구조 변경
        //if (player != null)
        //{
        //    Vector3 startPos = player.position;
        //    startPos.z = transform.position.z;

        //    transform.position = startPos;
        //    targetPos = startPos;

        //    isMoving = false;
        //}
    }


    private void Update()
    {
        if (!isMoving) return;

        // 부드럽게 이동
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * moveSpeed
        );

        // 거의 도착하면 멈춤
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            transform.position = targetPos;
            isMoving = false;
        }
    }


    // =========================================
    // 외부에서 호출하는 카메라 이동 함수
    // =========================================
    public void MoveTo(Vector3 pos)
    {
        targetPos = new Vector3(pos.x, pos.y, transform.position.z);
        isMoving = true;
    }

    public void SnapTo(Vector3 pos)
    {
        targetPos = new Vector3(pos.x, pos.y, transform.position.z);
        transform.position = targetPos;
        isMoving = false;
    }
}