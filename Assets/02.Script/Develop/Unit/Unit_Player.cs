using UnityEngine;

public class Unit_Player : MonoBehaviour
{
    /// <summary>
    /// 플레이어의 기본 정보
    /// </summary>
    [Header("Palyer 기본정보")]
    public string UnitName;
    public int maxHP = 30;
    public int currentHP;
    public int currentShield; // 임시 방어막 변수
    public int currentCost = 3;
    

    [Header("Ref")]
    [SerializeField] PlayerCardInventory inventory;
    [SerializeField] BattleManager battleManager;

    private void Start()
    {
        currentHP = maxHP;
    }

    public bool IsDead => currentHP <= 0;

}