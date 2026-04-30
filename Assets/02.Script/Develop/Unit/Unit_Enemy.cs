using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;



// 각 유닛의 데이터를 제공하는 스크립트
// EnemyBase로 전투 및 행동 관련 로직 이동
[System.Serializable]
public class Unit_Enemy : MonoBehaviour
{
    [Header("Enemy 기본정보")]
    public string UnitName;
    public int maxHP;
    public int currentHP;
    public int currentdamage;
    public EnemyStatePatterns currentPattern;
    public EnemyStatePatterns nextPattern; // 다음에 할 행동
    public int minPatternCount = 10; // 최소 패턴 개수
    public Sprite sprite;

    [SerializeField] public EnemyStatePatterns[] enemypatterns = new EnemyStatePatterns[5];

    // Enemy 행동 패턴 Queue
    public Queue<EnemyStatePatterns> patternsQueue = new Queue<EnemyStatePatterns>();
    // 인스펙터 확인용 List
    public List<EnemyStatePatterns> patternsQueuelist = new List<EnemyStatePatterns>();


    public bool IsDead => currentHP <= 0;

    private void Awake()
    {

        currentHP = maxHP;
        currentPattern = EnemyStatePatterns.None;

        nextPattern = GetNextPattern();
    }

    public EnemyData GetData()
    {
        EnemyData data = new EnemyData();

        data.UnitName = UnitName;
        data.maxHP = maxHP;
        data.basicDamage = currentdamage;
        data.sprite = sprite;

        data.patterns = enemypatterns;

        return data;
    }



    // 패턴 Queue 생성
    public void GeneratePatternQueue(int count)
    {
        count = minPatternCount;

        patternsQueue.Clear();
        patternsQueuelist.Clear();

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, enemypatterns.Length);
            EnemyStatePatterns patterns = enemypatterns[randomIndex];

            patternsQueue.Enqueue(patterns);
            patternsQueuelist.Add(patterns);
        }

#if UNITY_EDITOR
        Debug.Log($"[Enemy] 패턴 생성 완료");
#endif
    }

    // Enemy 상태 출력
    // 다음 패턴 가져오기
    public EnemyStatePatterns GetNextPattern()
    {
        if(patternsQueue.Count == 0)
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

    public void PrepareTurn()
    {
        currentPattern = nextPattern;
        nextPattern = GetNextPattern();

        //Debug.Log($"[Enemy] 현재 패턴: {currentPattern}");
        //Debug.Log($"[Enemy] 다음 패턴: {nextPattern}");

    }



}
