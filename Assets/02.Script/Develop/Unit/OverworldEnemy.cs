using System;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Enemy의 행동 패턴
/// </summary>
public enum EnemyStatePatterns
{
    None,
    BaseAttack,
    StrongAttack,
    BaseDefence,
    Idle,
}

/// <summary>
/// 필드에 존재하는 Enemy 가 갖고 있는 정보 클래스
/// EnemyData 를 뼈대로 사용한다
/// </summary>
public class OverworldEnemy : MonoBehaviour
{
    public string enemyID; // 적 개체의 ID

    public string UnitName;
    public int maxHP;
    public int basicDamage;
    public Sprite sprite;

    public EnemyStatePatterns[] patterns; // Enemy 행동 패턴 

    public GameObject battlePrefab;

    private void Start()
    {
        CheckDeadID();
    }

    private void CheckDeadID()
    {
        var state = GameStateDataManager.Instance;

        if (state == null) return;

        if(state.deadEnemies.Contains(enemyID))
        {
            Debug.Log("이미 죽은 Enemy 제거" + enemyID);
            Destroy(gameObject);
            
        }
    }

    public EnemyData GetData()
    {
        EnemyData data = new EnemyData();

        data.enemyID = enemyID;
        data.UnitName = UnitName;
        data.maxHP = maxHP;
        data.basicDamage = basicDamage;
        data.sprite = sprite;
        data.patterns = patterns;

        data.enemyPrefab = battlePrefab;

        return data;
    }

}
