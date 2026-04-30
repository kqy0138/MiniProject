using System;
using UnityEngine;


// 카드 타입
public enum CardType
{
    None,
    Attack,
    Defense,
    Skill
}

// 카드 효과 ID
public enum CardAbilityID
{
    None,
    Attack,
    Defense,
    Draw,
    Skill_atk,
    Skill_def,
    Heal
}
/// <summary>
/// 카드 1장의 "데이터 구조"
/// Inspector에 표시되도록 Serializable 사용
/// </summary>
[Serializable]
public class CardData
{
    [Header("기본 정보")]
    [SerializeField] public string cardId;
    [SerializeField] public string displayCardName;
    [SerializeField] public Sprite icon;

    [TextArea]
    [SerializeField] public string description;

    [Header("카드 속성")]
    [SerializeField] public CardType cardType;
    [SerializeField] public int cardCost;

    [Header("효과 정보")]
    [SerializeField] public CardAbilityID abilityId;
    [SerializeField] public int value;


    public CardData(
          string cardId,
          string displayCardName,
          Sprite icon,
          string description,
          CardType cardType,
          int cardcost,
          CardAbilityID abilityId,
          int value)
          {
              this.cardId = cardId;
              this.displayCardName = displayCardName;
              this.icon = icon;
              this.description = description;
              this.cardType = cardType;
              this.cardCost = cardcost;
              this.abilityId = abilityId;
              this.value = value;
          }

}
