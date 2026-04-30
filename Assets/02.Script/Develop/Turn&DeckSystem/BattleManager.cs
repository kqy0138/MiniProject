
using System;
using UnityEngine;

/// <summary>
/// 전투 진행용 클래스
/// </summary>

public class BattleManager : MonoBehaviour
{

    //public static BattleManager Instance;


    [Header("Ref")]
    [SerializeField] private Unit_Player player;
    [SerializeField] private EnemyBase enemyPrefab; // 프리팹 용도 (나중에 data.enemyPrefab 대체 가능)
    private EnemyBase currentEnemy; // 실제 전투에 사용될 Enemy 인스턴스
    [SerializeField] private Transform enemySpawnPoint; // Enemy 위치

    [SerializeField] private TurnManager turnManager;
    [SerializeField] private BattleUI_Manager battleUIManager;
    [SerializeField] private DeckBuildManager deckManager;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject resultUI;



    [SerializeField] private EnemyData debugEnemyData; // 테스트용 더미 데이터


    public bool IsBattleEnd = false;

    // ========================================================================


    private void Awake()
    {
        //if (Instance != null && Instance != this)
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        //Instance = this;

        //DontDestroyOnLoad(gameObject);


        if (BattleContext.Instance.currentEnemyData == null)
        {

            Debug.LogError("EnemyData 없음!");
            return;
        }

        // 데이터 가져오기
        EnemyData data = BattleContext.Instance.currentEnemyData;
#if UNITY_EDITOR
        
        // 👉 에디터에서만 동작
        if (data == null)
        {
            Debug.LogWarning("디버그용 EnemyData 사용");
            
            if (debugEnemyData == null)
            {
                Debug.LogError("debugEnemyData도 없음. 인스펙터에 넣기");
                return;
            }
            data = debugEnemyData;
        }
#endif

        // 생성된 enemy를 currentEnemy에 저장
        currentEnemy = Instantiate(enemyPrefab, enemySpawnPoint.position, Quaternion.identity);

        
        currentEnemy.Init(data);
        // 데이터 가져오기 완료되면 데이터 비움
        BattleContext.Instance.currentEnemyData = null;

        // UI 에 전투용 currnetEnemy 전달
        battleUIManager.SetEnemy(currentEnemy);
        turnManager.SetEnemy(currentEnemy);
    }
    private void Start()
    {
        turnManager.StartBattle();
        deckManager.StartBattleDeck();
    }

    #region Player 행동

    public void ExecuteCard(CardInstance card)
    {

        if (card == null)
        {
            Debug.Log("card null");
            return;
        }

        CardData data = card.data;

        Debug.Log("카드 실행: " + data.displayCardName);

        switch (data.abilityId)
        {
            case (CardAbilityID.None):
                Debug.Log("능력 Id 정의되지 않음");
                break;

            case (CardAbilityID.Attack):
                Attack(player, currentEnemy, card.currentValue);
                Debug.Log("공격: " + data.value);
                break;

            case (CardAbilityID.Defense):
                Defence(player, card.currentValue);
                Debug.Log("방어: " + data.value);
                break;

            case (CardAbilityID.Draw):
                deckManager.DrawCard();
                break;

            case (CardAbilityID.Skill_atk):
                Skill(player, currentEnemy, card.currentValue, card.data.abilityId);
                Debug.Log("스킬공격: " + data.value);
                break;

            case (CardAbilityID.Skill_def):
                Skill(player, currentEnemy, card.currentValue, card.data.abilityId);
                Debug.Log("스킬방어: " + data.value);
                break;

            case (CardAbilityID.Heal):
                Skill(player, currentEnemy, card.currentValue, card.data.abilityId);
                Debug.Log("회복: " + data.value);
                break;

            default:
                Debug.Log("정의되지 않은 능력: " + data.abilityId);
                break;
        }

        

    }


    public void Attack(Unit_Player player, EnemyBase enemy, int damage)
    {
        enemy.currentHP -= damage;
        Debug.Log($"적에게 Attack {damage}");

        if (enemy.currentHP <= 0)
        {
            enemy.currentHP = 0;
            Debug.Log("Enemy 처치함");
        }

        CheckBattleEnd();

    }


    public void Defence(Unit_Player player, int value)
    {
        player.currentShield += value;
        Debug.Log($"방어력 증가: {value}");
    }

    public void Skill(Unit_Player player, EnemyBase enemy, int value, CardAbilityID Id)
    {

        switch (Id)
        {
            case CardAbilityID.Skill_atk:

                Debug.Log("공격 스킬 발동");
                enemy.currentHP -= value;
                break;

            case CardAbilityID.Skill_def:
                player.currentShield += value;
                Debug.Log("방어 스킬 발동");
                break;


            case CardAbilityID.Heal:
                Debug.Log("회복 스킬 발동");
                player.currentHP += value;
                if(player.currentHP > player.maxHP)
                {
                    player.currentHP = player.maxHP;
                }
                break;
        }

        if (enemy.currentHP <= 0)
        {
            enemy.currentHP = 0;
            Debug.Log("Enemy 처치함");
        }
    }

    public void UseItem(Unit_Player player)
    {
        // TODO :: 아이템 사용
    }



    #endregion

    #region Enemy 행동


    // ============================================================
    // Enemy 턴 실행 
    // ============================================================
    public void ExecuteEnemyTurn()
    {
        battleUIManager.RefreshPattentUI();

        switch (currentEnemy.currentPattern)
        {

            case EnemyStatePatterns.BaseAttack:
                BaickAttack(player, currentEnemy.currentdamage);
                break;

            case EnemyStatePatterns.StrongAttack:
                StrongAttack(player, currentEnemy.currentdamage);
                break;

            case EnemyStatePatterns.BaseDefence:
                BaseDefence(currentEnemy, currentEnemy.currentShield);
                break;

            case EnemyStatePatterns.Idle:
                Idle();
                break;

            case EnemyStatePatterns.None:
                Debug.LogWarning("정의되지 않은 상태 -> 기본 공격처리");
                BaickAttack(player, currentEnemy.currentdamage);
                break;

        }
    }


    // ============================================================
    // Enemy 행동들
    // ============================================================



    public void ApplyDamage(Unit_Player player, int damage)
    {
        int remainingDamage = damage;

        if (player.currentShield > 0)
        {
            if (player.currentShield >= remainingDamage)
            {
                player.currentShield -= remainingDamage;
                remainingDamage = 0;
            }
            else
            {
                remainingDamage -= player.currentShield;
                player.currentShield = 0;
            }
        }

        if (remainingDamage > 0)
        {
            player.currentHP -= remainingDamage;
        }
    }



    public void BaickAttack(Unit_Player player, int damage)
    {
        ApplyDamage(player, damage);

        //player.currentHP -= damage;
        Debug.Log($"{player.gameObject.name} 가 데미지 {damage} 받음");

        //if (player.currentHP <= 0)
        //{
        //    player.currentHP = 0;
        //}
        CheckBattleEnd();
    }




    public void StrongAttack(Unit_Player player, int damage)
    {
        int strongDamage = damage * 2;

        ApplyDamage(player, strongDamage);
        //int damage = currentEnemy.currentdamage * 2;
        //player.currentHP -= damage;

        Debug.Log($"강한 공격! {strongDamage} 데미지");

        if (player.currentHP <= 0)
        {
            player.currentHP = 0;
        }
        CheckBattleEnd();
    }

    
    
    public void ApplyDamage2(EnemyBase enemy, int damage)
    {
        int remainingDamage = damage;

        if (enemy.currentShield > 0)
        {
            if (enemy.currentShield >= remainingDamage)
            {
                enemy.currentShield -= remainingDamage;
                remainingDamage = 0;
            }
            else
            {
                remainingDamage -= enemy.currentShield;
                enemy.currentShield = 0;
            }
        }

        if (remainingDamage > 0)
        {
            enemy.currentHP -= remainingDamage;
        }
    }


     
     

    public void BaseDefence(EnemyBase enemy, int value)
    {
        // TODO :: 기본 방어
        enemy.currentShield += value;
        Debug.Log($"Enemy 방어력 증가: {value}");
    }

    public void Idle()
    {
        // TODO :: 대기
        Debug.Log("Enemy 대기중");
    }


    #endregion


    // 배틀 종료 검사
    public void CheckBattleEnd()
    {
        if (IsBattleEnd)
        {
            return;
        }

        // 플레이어 승리
        if (currentEnemy.currentHP <= 0)
        {
            IsBattleEnd = true;

            SaveDeadEnemy();

            Debug.Log("승리 → 씬 이동");
            gameUI.SetActive(false);
            resultUI.SetActive(true);
        }

        // 플레이어 패배 (죽음)
        else if (player.currentHP <= 0)
        {
            IsBattleEnd = true;
            Debug.Log("패배 → 씬 이동");
            GameSceneManager.Instance.LoadSceneByName("InGame_BattleLose_GameOver");
        }
    }

    private void SaveDeadEnemy()
    {
        string deadID = currentEnemy.enemyID;

        var state = GameStateDataManager.Instance;

        if(!state.deadEnemies.Contains(deadID))
        {
            state.deadEnemies.Add(deadID);
        }
        Debug.Log("Dead Enemy Saved: " + deadID);
    }
}

