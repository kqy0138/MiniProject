using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전체 카드 목록을 관리하는 Catalog
/// - Inspector에서 직접 수정 가능
/// - 코드로 자동 생성도 가능
/// </summary>
public class CardCatalog : MonoBehaviour
{

    //public static CardCatalog Instance;

    [Header("전체 카드 목록")]
    public List<CardData> allCards = new List<CardData>(); // 전체 카드 목록
    
    private Dictionary<string, CardData> cardDict = new Dictionary<string, CardData>(); // 카드 ID 검색

    private void Awake()
    {
        //if (Instance != null && Instance != this)
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        //Instance = this;

        //DontDestroyOnLoad(gameObject);


        BuildDictionary();
        //EnsureCatalogNotEmptyForRuntime();
    }

    private void BuildDictionary()
    {
        cardDict = new Dictionary<string, CardData>();

        foreach (var card in allCards)
        {
            // null 체크
            if(card == null)
            {
                Debug.LogWarning("카드 데이터가 null 입니다");
                continue;
            }
            
            // ID 체크
            if (string.IsNullOrEmpty(card.cardId))
            {
                Debug.LogWarning("cardId 비어있음");
                continue;
            }
            
            // 중복 체크
            if (cardDict.ContainsKey(card.cardId))
            {
                Debug.LogWarning("중복 ID 발견: " + card.cardId);
            }
            cardDict.Add(card.cardId, card);
        }
    }


    // 카탈로그가 비어있는 경우 샘플 데이터 넣기
    // 현재 Inspector 기반으로 카드 생성하게 되어있음
    private void EnsureCatalogNotEmptyForRuntime()
    {
        if (allCards.Count <= 0)
        {
            // TODO : 샘플 데이터 넣기
            Debug.Log("카드 추가함");
            Debug.LogWarning("카드 데이터 없음 (Inspector 확인 필요)");
        }
        else
        {
            Debug.Log("카드 목록 있음");
            return;
        }
    }


    /// <summary>
    /// cardId로 카드 찾기
    /// </summary>
    public CardData GetCardById(string id)
    {
        // 기존에 리스트로 순회하던 것을 딕셔너리 조회로 수정
        //foreach (var card in allCards)
        //{
        //    if (card.cardId == id)
        //        return card;
        //}
        if (cardDict == null)
        {
            Debug.LogWarning("cardDic 초기화 되지 않음");
            return null;
        }

        if (cardDict.TryGetValue(id, out var cardData))
        {
            return cardData;
        }

        //Debug.LogWarning("카드 없음: " + id);
        return null;
    }

    // =========================
    // Inspector 데이터 검사용
    // =========================

    /// <summary>
    /// 카드 데이터 전체 검증
    /// </summary>
    [ContextMenu("Validate Cards")]
    public void ValidateCards()
    {
        HashSet<string> idSet = new HashSet<string>();

        foreach (var card in allCards)
        {
            // 1. null 체크
            if (card == null)
            {
                Debug.LogWarning("CardData가 null입니다.");
                continue;
            }

            // 2. ID 체크
            if (string.IsNullOrEmpty(card.cardId))
            {
                Debug.LogWarning("cardId 비어있음");
                continue;
            }

            // 3. 중복 체크
            if (idSet.Contains(card.cardId))
            {
                Debug.LogWarning("중복 ID 발견: " + card.cardId);
            }
            else
            {
                idSet.Add(card.cardId);
            }

            // 4. 필수값 체크
            if (string.IsNullOrEmpty(card.displayCardName))
            {
                Debug.LogWarning("cardName 없음: " + card.cardId);
            }

            if (card.abilityId == CardAbilityID.None)
            {
                Debug.LogWarning("abilityId 없음: " + card.cardId);
            }
        }

        Debug.Log("카드 검증 완료");
    }

}
