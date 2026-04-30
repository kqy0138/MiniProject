using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 전투 진행 UI 갱신 클래스
/// </summary>
public class BattleUI_Manager : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private DeckBuildManager deckManager;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private Unit_Player player;
    [SerializeField] private EnemyBase enemy;

    [Header("화면 UI")]
    [SerializeField] private TextMeshProUGUI turnCountText;

    [Header("Player UI")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI playerHpText;
    [SerializeField] private Slider playerHPSlider;
    
    [Header("Enemy UI")]
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI enemyPatternText;
    [SerializeField] private TextMeshProUGUI enemyHpText;
    [SerializeField] private Slider enemyHPSlider;

    [Header("툴팁 패널")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;


    private void Start()
    {
        RefreshALL();
    }

    private void Update()
    {
        // 매 프레임 갱신 -> 나중에 이벤트 방식으로 변경 
        RefreshCostUI();
        RefreshHPUI();
        RefreshTurnUI();
    }

    void RefreshALL()
    {

        // 기본적으로 UI들은 시작할때 한 번 갱신한다

        CurrentUI();

        RefreshTurnUI();
        RefreshCostUI();
        RefreshHPUI();
        RefreshPattentUI();

    }


    /// <summary>
    /// 카드 설명 툴팁 보이기
    /// </summary>
    public void ShowTooltip(CardInstance card)
    {
        tooltipPanel.SetActive(true);

        tooltipText.text = $"{card.data.displayCardName}" +
            $" / cost:[{card.data.cardCost}] / value:[{card.data.value}]\n{card.data.description}";
    }

    /// <summary>
    /// 카드 설명 툴팁 숨기기
    /// </summary>
    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    /// <summary>
    /// 변동이 없는 기본 데이터 표시
    /// </summary>
    void CurrentUI()
    {
        playerNameText.text = player.UnitName;
        enemyNameText.text = enemy.UnitName;

    }


    /// <summary>
    /// 현재 코스트 갱신
    /// </summary>
    void RefreshCostUI()
    {
        int current = deckManager.currentCost;
        int max = deckManager.maxCost;

        costText.text = $"Cost: {current} / {max}";
        
    }

    void RefreshTurnUI()
    {
        int turn = turnManager.turnCount;
        turnCountText.text = $"Turn Count : {turn}";
    }    


    /// <summary>
    /// 현재 체력 갱신
    /// </summary>
    private void RefreshHPUI()
    {
        // Player HP
        playerHpText.text = $"{player.maxHP}/{player.currentHP}";
        float playerRatio = (float)player.currentHP / player.maxHP;
        playerHPSlider.value = playerRatio;


        // Enemy HP
        enemyHpText.text = $"{enemy.maxHp}/{enemy.currentHP}";
        float enemyRatio = (float)enemy.currentHP / enemy.maxHp;
        enemyHPSlider.value = enemyRatio;

        //Debug.Log($"Enemy현재HP/{enemy.currentHP}, 최대HP{enemy.maxHp}, 슬라이더 value,{enemyHPSlider.value}");
    }

    /// <summary>
    ///  다음 턴에 할 패턴 표시
    /// </summary>
    public void RefreshPattentUI()
    {
        EnemyStatePatterns patterns = enemy.nextPattern;

        enemyPatternText.text = GetPatternText(patterns);


    }


    private string GetPatternText(EnemyStatePatterns pattern)
    {
        switch (pattern)
        {
            case EnemyStatePatterns.BaseAttack:
                return "곧 달려들것 같다\n[기본 공격]";

            case EnemyStatePatterns.StrongAttack:
                return "심상치 않은 분위기가 느껴진다\n[강한 공격]";

            case EnemyStatePatterns.BaseDefence:
                return "잔뜩 경계하고있다\n[방어력 증가]";

            case EnemyStatePatterns.Idle:
                return "이쪽을 살펴보는것 같다\n[대기]";

            default:
                return "None(=BaseAttack)";
        }
    }

    /// <summary>
    /// Enemy 데이터 연결
    /// </summary>
    public void SetEnemy(EnemyBase enemyRef) 
    {
        enemy = enemyRef;
    }

    /// <summary>
    /// 탐험 씬으로 복귀하기
    /// </summary>
    public void OnClick_ReturnAdventureMap()
    {
        Debug.Log("탐험 씬으로 복귀");
        SceneManager.LoadScene("InGame_RoomTest");
    }
    

}
