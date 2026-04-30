using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카드 ID, 보유 개수
/// </summary>
[System.Serializable]
public class CardInventoryItem
{
    public string cardId;  // 카드 ID
    public int count;      // 보유 개수
}

/// <summary>
/// 플레이어의 카드 인벤토리
/// 소지하고 있는 카드 ID 관리
/// </summary>
public class PlayerCardInventory : MonoBehaviour
{

    // 플레이어가 소유하고 있는 카드 ID
    [Header("플레이어 보유 카드 ID 목록")]
    public List<CardInventoryItem> ownedCardIds = new List<CardInventoryItem>();

    // 조회용 Dictionary
    private Dictionary<string, CardInventoryItem> inventoryDict;

    [SerializeField] private CardCatalog catalog;


    private void Awake()
    {
        BuildDictionary();
    }

    // List -> Dictoinary 변환
    private void BuildDictionary()
    {
        inventoryDict = new Dictionary<string, CardInventoryItem>();

        foreach(var item in ownedCardIds)
        {
            if(item==null)
            {
                Debug.LogWarning("Inventory Item Null");
                continue;
            }

            if(string.IsNullOrEmpty(item.cardId))
            {
                Debug.LogWarning("cardId 없음");
                continue;
            }
            
            if(inventoryDict.ContainsKey(item.cardId))
            {
                Debug.LogWarning("중복 카드 ID : " + item.cardId);
            }

            inventoryDict.Add(item.cardId, item);
        }
    }

    // Dictioinary 조회
    public bool HasCard(string cardId)
    {
        return inventoryDict.ContainsKey(cardId);
    }


    /// <summary>
    /// 카드 개수 반환
    /// </summary>
    public int GetCardCount(string cardId)
    {
        if(inventoryDict.TryGetValue(cardId, out var item))
        {
            return item.count;
        }
        return 0;
    }


    /// <summary>
    ///  카드 추가
    /// </summary>
    public void AddCard(string cardId, int amount = 1)
    {
        if(string.IsNullOrEmpty(cardId))
        {
            Debug.LogWarning("잘못된 cardId");
            return;
        }


        //CardCatalog catalog = FindObjectOfType<CardCatalog>();
        if(catalog != null && catalog.GetCardById(cardId) == null)
        {
            Debug.LogWarning("catalog에 없는 카드 : " + cardId);
        }

        // Dictionary 기준으로 처리
        if(inventoryDict.TryGetValue(cardId, out var item))
        {
            item.count += amount;
        }
        else
        {
            // 처음 추가되는 카드
            CardInventoryItem newCard = new CardInventoryItem
            {
                cardId = cardId,
                count = amount
            };

            ownedCardIds.Add(newCard);  // Ispector용 List
            inventoryDict.Add(cardId, newCard); // Dictionary 동기화
        }

    }

}
