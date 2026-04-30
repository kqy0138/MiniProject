using UnityEngine;

[System.Serializable]
/// <summary>
/// Enemy 가 갖고 있는, 변동되지 않는 기본 뼈대 정보
/// </summary>
public class EnemyData
{

    public string enemyID; // 적 개체의 ID

    public string UnitName; // 유닛 이름
    public int maxHP; // 최대 체력
    public int basicDamage; // 기본 데미지
    public Sprite sprite; // 이미지

    public EnemyStatePatterns[] patterns; // 행동 패턴
    public GameObject enemyPrefab; // Enemy의 프리팹

}
