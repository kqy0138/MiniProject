using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 전투에 사용될 Enemy
/// </summary>
public class EnemyBase : MonoBehaviour
{
    // 이름, 최대체력, 기본 공격력, 기본 이미지, 패턴의 종류
    // 위 데이터들은 전투중 변동X
    // Init 으로 처음에 생성할거임 (데이터 복붙)

    public string enemyID;
    public string UnitName; // 이름
    public int maxHp; // 최대 체력
    public int currentHP; // 현재 체력
    public int currentdamage; // 현재 공격력
    public SpriteRenderer spriteRenderer; // 기본 이미지

    public int currentShield; // 임시 방어막 변수


    public EnemyStatePatterns[] pattern; // 고정적인 행동 패턴
    public EnemyStatePatterns currentPattern; // 현재 행동
    public EnemyStatePatterns nextPattern; // 다음에 할 행동

    public int minPatternCount = 10; // 최소 패턴 개수

    public bool IsDead => currentHP <= 0; // 사망여부

                                     // Enemy 행동 패턴 Queue
    public Queue<EnemyStatePatterns> patternsQueue = new Queue<EnemyStatePatterns>();
    // 인스펙터 확인용 List
    public List<EnemyStatePatterns> patternsQueuelist = new List<EnemyStatePatterns>();

    /// ------------------------------------------------------------------------

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer 없음!");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    
    /// <summary>
    /// 초기화
    /// </summary>
    public void Init(EnemyData data)
    {
        enemyID = data.enemyID;
        UnitName  = data.UnitName;
        maxHp = data.maxHP;
        currentdamage = data.basicDamage;
        spriteRenderer.sprite = data.sprite;
        pattern = data.patterns;

        currentHP = maxHp;

        GeneratePatternQueue(minPatternCount); 

        currentPattern = EnemyStatePatterns.None;
        nextPattern = GetNextPattern();
        Debug.Log("[EnemyBase - Init] 호출됨");
    }


    /// <summary>
    /// 패턴 Queue 생성
    /// </summary>
    public void GeneratePatternQueue(int count)
    {
        count = minPatternCount;


        patternsQueue.Clear();
        patternsQueuelist.Clear();

        for (int i = 0; i < count; i++)
        {
            if (pattern == null || pattern.Length == 0)
            {
                Debug.LogError("패턴 데이터 없음!");
                return;
            }

            int randomIndex = Random.Range(0, pattern.Length);
            EnemyStatePatterns patterns = pattern[randomIndex];

            patternsQueue.Enqueue(patterns);
            patternsQueuelist.Add(patterns);
        }

#if UNITY_EDITOR
        Debug.Log($"[Enemy] 패턴 생성 완료");
#endif
    }

    /// <summary>
    /// Enemy 상태 출력
    /// 다음 패턴 가져오기
    /// </summary>
    private EnemyStatePatterns GetNextPattern()
    {
        if (patternsQueue.Count == 0)
        {
            //Debug.Log("[Enemy] 패턴 부족 -> 재설정");
            GeneratePatternQueue(minPatternCount);
        }

        if (patternsQueue.Count == 0)
        {
            Debug.LogError("패턴 생성 실패");
            return EnemyStatePatterns.Idle;
        }


        EnemyStatePatterns next = patternsQueue.Dequeue();
        patternsQueuelist.RemoveAt(0);

        return next;
    }

    /// <summary>
    /// 현재 패턴 / 다음 패턴 준비
    /// </summary>
    public void PrepareTurn()
    {
        currentPattern = nextPattern;
        nextPattern = GetNextPattern();

        Debug.Log($"[Enemy] 현재 패턴: {currentPattern}");
        Debug.Log($"[Enemy] 다음 패턴: {nextPattern}");

    }

}
