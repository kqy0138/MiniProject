using System;
using UnityEngine;

public enum DIRECTION
{
    Left, LeftUp, LeftDown,
    Right, RightUp, RightDown,
    Up, Down, None

}
public static class Direction8
{
    public static Vector2 ToVector2(DIRECTION direction)
    {

        return direction switch
        {
            DIRECTION.Left => Vector2.left,
            DIRECTION.Right => Vector2.right,
            DIRECTION.Up => Vector2.up,
            DIRECTION.Down => Vector2.down,
            DIRECTION.LeftUp => new Vector2(-1f, 1f).normalized,
            DIRECTION.LeftDown => new Vector2(-1f, -1f).normalized,
            DIRECTION.RightUp => new Vector2(1f, 1f).normalized,
            DIRECTION.RightDown => new Vector2(1f, -1f).normalized,
            _ => Vector2.zero

        };
    }
}


public class PlayerMove2D : MonoBehaviour
{

    [SerializeField] public float moveSpeed;

    [Header("Ref")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInputReader playerInputReader;
    [SerializeField] private Transform battleSpawnPoint;
    [SerializeField] private CameraFollowExample cameraController;


    DIRECTION currentdirection;
    public DIRECTION Currnentdirection => currentdirection;

    Vector2 inputDirection;

    // ==============================================================================


    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (playerInputReader == null)
        {
            playerInputReader = GetComponent<PlayerInputReader>();
        }

    }

    private void Start()
    {
        currentdirection = DIRECTION.None;

        // 현재 씬이 전투 씬인지 확인
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "InGame_BattleScene")
        {

            SetBattlePosition();
           
            //// 전투 씬이면 위치 초기화
            //rb.position = Vector2.zero;
            //rb.linearVelocity = Vector2.zero;

            //Debug.Log("Battle Scene -> Player 위치 초기화 (0, 0)");
        }
        else
        {

            // 탐험 씬이면 저장된 위치 로드
            LoadPosition();

        }

    }

    private void SetBattlePosition()
    {

        GameObject spawn = GameObject.Find("playerposition");
        
        if (spawn != null)
        {
            rb.position = battleSpawnPoint.position;
            Debug.Log("Battle Player Spawn: " + spawn.transform.position);
        }
        else
        {
            Debug.LogError("BattlePlayerSpawn 없음!");
        }
    }

    private void Update()
    {
        inputDirection = playerInputReader != null ? playerInputReader.MoveVector : Vector2.zero;
        UpdateDirection();
    }

    private void FixedUpdate()
    {
        //Vector2 newposition = rb.position * moveSpeed * Time.deltaTime;
        rb.linearVelocity = inputDirection.normalized * moveSpeed;
    }

    private void UpdateDirection()
    {

        if (inputDirection.sqrMagnitude < 0.01f) return;

        // 다음과 같이 바꿔서 써도 된다
        const float deadzoneValue = 0.1f;

        float x = inputDirection.x;
        float y = inputDirection.y;

        if (x > deadzoneValue)
        {
            if (y > deadzoneValue) currentdirection = DIRECTION.RightUp;
            else if (y < -1 * deadzoneValue) currentdirection = DIRECTION.RightDown;
            else currentdirection = DIRECTION.Right;
        }
        else if (x < -1 * deadzoneValue)
        {
            if (y > deadzoneValue) currentdirection = DIRECTION.LeftUp;
            else if (y < -1 * deadzoneValue) currentdirection = DIRECTION.LeftDown;
            else currentdirection = DIRECTION.Left;
        }
        else
        {
            if (y > deadzoneValue) currentdirection = DIRECTION.Up;
            else if (y < -1 * deadzoneValue) currentdirection = DIRECTION.Down;
        }
    }


    private void LoadPosition()
    {
        var state = GameStateDataManager.Instance;
        if (state == null) return;

        rb.position = state.playerPosition + Vector3.left * 0.5f;
        rb.linearVelocity = Vector2.zero;

        if(cameraController != null )
        {
            cameraController.SnapTo(rb.position);
        }

        Debug.Log("Player Position Loaded: " + rb.position);

    }
}
