using UnityEngine;

public class Test_BattleManager : MonoBehaviour
{

    public EnemyBase enemyPrefab;
    public Unit_Player player;

    private void Start()
    {
        // 1. 데이터 가져오기
        EnemyData data = BattleContext.Instance.currentEnemyData;

        // 2. 생성
        EnemyBase enemy = Instantiate(enemyPrefab);

        // 3. 초기화
        enemy.Init(data);

        // 배틀전 생성한 패턴 Queue에 넣기
        // enemy.GeneratePatternQueue(enemyPrefab.minPatternCount);
        //Pattern(enemyPrefab.minPatternCount);
    }

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
                Attack(player, enemyPrefab, card.currentValue);
                Debug.Log("공격: " + data.value);
                break;

            case (CardAbilityID.Defense):
                Defence(player, enemyPrefab, card.currentValue);
                Debug.Log("방어: " + data.value);
                break;


            case (CardAbilityID.Skill_atk):
                Skill(player, enemyPrefab, card.currentValue, card.data.abilityId);
                Debug.Log("스킬공격: " + data.value);
                break;

            case (CardAbilityID.Skill_def):
                Skill(player, enemyPrefab, card.currentValue, card.data.abilityId);
                Debug.Log("스킬방어: " + data.value);
                break;

            case (CardAbilityID.Heal):
                Skill(player, enemyPrefab, card.currentValue, card.data.abilityId);
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

        

    }

    public void Defence(Unit_Player player, EnemyBase enemy, int value)
    {
        // TODO :: 기본 방어
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

                Debug.Log("방어 스킬 발동");
                break;


            case CardAbilityID.Heal:
                Debug.Log("회복 스킬 발동");
                player.currentHP += value;
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




}
