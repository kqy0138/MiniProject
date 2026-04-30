using UnityEngine;

public class Test_TurnManager : MonoBehaviour
{
    // 현재 턴의 주체 (플레이어 or 적)
    public enum TurnType
    {
        None,
        Player,
        Enemy
    }

    // 턴 내부의 단계 (페이즈)
    public enum PhaseType
    {
        None,
        Start,
        Main,
        End
    }

    // Main Phase에서 선택 가능한 행동
    public enum MainType
    {
        None,
        Attack,
        Skill,
        Defence,
        UseItem
    }

    private TurnType currentTurn;
    private PhaseType currentPhase;


    [Header("Ref")]
    [SerializeField] public Unit_Player playerUnit;
    [SerializeField] public EnemyBase enemyUnit;

    // DeckBuildManager (카드 드로우 담당)
    [SerializeField] public DeckBuildManager deckManager;
    // BattleManager (전투 처리 담당)
    [SerializeField] public Test_BattleManager battleManager;
    [SerializeField] public BattleUI_Manager battleUIManager;

    public bool IsPlayerTurn => currentTurn == TurnType.Player;


    // ================================
    // 전투 시작
    // ================================

    private void Awake()
    {
        if (playerUnit == null) Debug.LogWarning("playerUnit 연결 안됨 (Inspector에서 연결 필요)");
        if (deckManager == null) Debug.LogWarning("deckManager 연결 안됨 (Inspector에서 연결 필요)");
        if (battleManager == null) Debug.LogWarning("battleManager 연결 안됨 (Inspector에서 연결 필요)");

        enemyUnit = gameObject.GetComponent<EnemyBase>();

        //if (enemyUnit == null) Debug.LogWarning("enemyUnit 연결 안됨 (Inspector에서 연결 필요)");
    }

    void Start()
    {
        StartBattle();
    }


    public void StartBattle()
    {
        // 첫 턴은 플레이어부터 시작
        currentTurn = TurnType.Player;

        Debug.Log("전투 시작");

        StartTurn();
    }

    // ================================
    // 턴 시작
    // ================================

    void StartTurn()
    {
        Debug.Log(currentTurn + " 턴 시작");

        // 현재 턴이 플레이어일때
        if (currentTurn == TurnType.Player)
        {
            // player 턴 시작
            StartPlayerTurn();
        }

        // 현재 턴이 Enemy 일 때
        else //if (currentTurn == TurnType.Enemy)
        {
            // Enemy 턴 시작
            StartEnemyTurn();

        }

    }


    // ================================
    // Phase 흐름 관리
    // ================================

    // Player 턴 시작
    private void StartPlayerTurn()
    {
        currentPhase = PhaseType.Start;

        Debug.Log(currentTurn + " Start Phase");

        // 플레이어 턴일 경우 카드 드로우
        if (currentTurn == TurnType.Player)
        {
            // 카드 4장 드로우
            deckManager.StartPlayDrawCard();

        }

        currentPhase = PhaseType.Main;
        MainPhase();


    }

    // Enemy 턴 시작
    private void StartEnemyTurn()
    {
        currentPhase = PhaseType.Start;

        Debug.Log(currentTurn + " Start Phase");

        // Queue 에서 패턴 가져오기
        enemyUnit.PrepareTurn();
        //// Enemy 상태가 None 이면
        //if (enemyUnit.currentPattern == EnemyStatePatterns.None)
        //{
        //    // 기본 공격
        //    enemyUnit.currentPattern = EnemyStatePatterns.BaseAttack;
        //    Debug.Log("Enemy 기본 패턴 설정"); // 디버깅 추가
        //}

        //battleUIManager.RefreshPattentUI();

        NextPhase();
    }

    void MainPhase()
    {
        Debug.Log(currentTurn + " Main Phase");

        if (currentTurn == TurnType.Player)
        {

            Debug.Log("플레이어 입력 대기 중");
        }
        else
        {
            // 적은 자동 행동
            EnemyAction();
        }
    }

    public void EndPhase()
    {
        Debug.Log(currentTurn + " End Phase");

        // 플레이어 턴 종료 시 카드 버림
        if (currentTurn == TurnType.Player)
        {
            deckManager.AllDisCard();
        }
    }

    // ================================
    // Phase 이동 로직
    // ================================

    void NextPhase()
    {
        if (currentTurn == TurnType.Player)
        {
            Debug.LogWarning("Player 턴에서는 NextPhase 호출 금지");
            return;
        }


        if (currentPhase == PhaseType.Start)
        {
            currentPhase = PhaseType.Main;
            MainPhase();
        }
        else if (currentPhase == PhaseType.Main)
        {
            currentPhase = PhaseType.End;
            EndPhase();
        }
    }

    // ================================
    // 플레이어 행동 선택
    // ================================

    // 선택한 카드에 따라 Action 실행
    public void OnSelectCard(CardType cardtype)
    {
        // Main Phase에서만 실행 가능
        if (currentPhase != PhaseType.Main || currentTurn != TurnType.Player) return;
        Debug.Log("플레이어 선택: " + cardtype);

        switch (cardtype)
        {
            case CardType.None:
                Debug.Log("타입이 설정되지 않은 카드");
                break;

            case CardType.Attack:

                break;

            case CardType.Defense:
                break;

            case CardType.Skill:
                break;

            default:
                Debug.Log("카드 타입 미설정");
                return;

        }
        // 행동 후 End Phase로 이동
        //NextPhase();
    }

    // ================================
    // 적 행동
    // ================================

    void EnemyAction()
    {
        Debug.Log("Enemy 행동");

        //battleManager.ExecuteEnemyTurn();

        // 행동 끝나면 End Phase -> End Turn -> 자동으로 턴 종료
        EndTurn();


    }

    // ================================
    // 턴 종료
    // ================================

    public void EndTurn()
    {
        Debug.Log($"[EndTurn 호출됨] 현재 턴: {currentTurn}"); // 🔥 핵심 로그
        //Debug.Log(currentTurn + " 턴 종료");

        // 턴 전환
        if (currentTurn == TurnType.Player)
        {
            currentTurn = TurnType.Enemy;
        }
        else
        {
            currentTurn = TurnType.Player;
        }


        // 다음 턴 시작
        StartTurn();
    }

}
