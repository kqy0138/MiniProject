using System.Collections.Generic;
using UnityEngine;


// 카드 영역 구조체
[System.Serializable]
public class ClickArea
{
    public Vector3 center;
    public Vector2 size;
}

/// <summary>
/// 덱 빌딩 클래스
/// 덱 구성, 드로우, 카드 버리기, 카드 섞기, 셔플, 중복 여부 판단
/// </summary>

public class DeckBuildManager : MonoBehaviour
{

    //public static DeckBuildManager Instance;

    public TurnManager turnManager;

    [Header("Deck Settings")]
    int maxCardCount = 30;
    int minCardCount = 10;
    int maxDuplicateCount = 3;

    [Header("Ref")]
    [SerializeField] private CardCatalog cardCatalog;  // 전체 카드DB
    [SerializeField] private PlayerCardInventory inventory;  // 플레이어 보유 카드

    [Header("Card Piles")]
    [SerializeField] private List<CardInstance> deck = new List<CardInstance>();   // 전투용 드로우 덱   
    [SerializeField] private List<CardInstance> hand = new List<CardInstance>();     // 현재 손패
    [SerializeField] private List<CardInstance> disCard = new List<CardInstance>();  // 버린 카드

    [Header("Cost System")]
    public int maxCost = 3;
    public int currentCost;

    [Header("Hand Settings")]
    public int maxHandSize = 4;

    // ==============================================================================

    //private void Awake()
    //{
    //    if (Instance != null && Instance != this)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }

    //    Instance = this;

    //    DontDestroyOnLoad(gameObject);
    //}

    /// <summary>
    /// hand 리스트를 밖에서 보기 위한 창구
    /// </summary>
    public List<CardInstance> GetHandCards()
    {
        return new List<CardInstance>(hand);
    }

    /// <summary>
    /// 현재 손패 개수 반환
    /// </summary>
    public int HandCount()
    {
        return hand.Count;
    }

    // ==============================
    // 🔹 카탈로그 <- 카드 인벤토리) -> 덱 연결
    // ==============================

    /// <summary>
    /// 🔥 시작 덱 구성 (Catalog + Inventory 기반)
    /// </summary>
    public void BuildStartingDeck()
    {
        deck.Clear();

        foreach (var item in inventory.ownedCardIds)
        {
            CardData data = cardCatalog.GetCardById(item.cardId);

            if (data == null)
            {
                Debug.LogWarning("카드 없음: " + item.cardId);
                continue;
            }

            // count 만큼 카드 생성
            for (int i = 0; i < item.count; i++)
            {
                // 중복 체크
                if (!IsDuplicateAllowed(data))
                    break;

                if (deck.Count >= maxCardCount)
                {
                    Debug.Log("덱 최대 초과");
                    break;
                }

                CardInstance instance = new CardInstance(data);
                deck.Add(instance);
            }
        }

        // 최소 카드 수 보정 (필요 시)
        if (deck.Count < minCardCount)
        {
            Debug.LogWarning("덱 카드 수 부족: " + deck.Count);
        }

        ShuffleCard();

        Debug.Log("덱 생성 완료: " + deck.Count);
    }

    public void StartBattleDeck()
    {
        deck.Clear();
        disCard.Clear();
        hand.Clear();

        InitializeBattelDeck();
    }

    // ==============================
    // 🔹 플레이어 턴 시작
    // ==============================
    /// <summary>
    /// 플레이어 턴 시작 드로우
    /// </summary>
    public void StartPlayDrawCard()
    {
        //// 전투 시작 시 1번만 실행되도록 조건 필요
        //if (deck.Count == 0 && disCard.Count == 0 && hand.Count == 0)
        //{
        //    Debug.Log("collectionDeck 생성됨");
        //    InitializeBattelDeck();
        //}

        //else
        if (hand.Count > 0)
        {
            Debug.LogWarning("턴 시작 시 hand가 비어있지 않음 → 자동 정리");
            AllDisCard();
        }

        Debug.Log("StartPlayerTurn");

        // 코스트 초기화
        currentCost = maxCost;

        // 손패가 비어있는 경우 최대 손패까지 드로우
        DrawCard(maxHandSize);
    }

    /// <summary>
    /// 전투 시작 시 초기화
    /// </summary>
    public void InitializeBattelDeck()
    {
        // 덱 생성 + 비용 초기화
        BuildStartingDeck();

        hand.Clear();
        disCard.Clear();

        currentCost = maxCost;

        StartPlayDrawCard();
    }


    /// <summary>
    /// 🔥 여러 장 드로우
    /// </summary>
    public void DrawCard(int count)
    {

        for (int i = 0; i < count; i++)
        {
            // 🔥 손패 꽉 찼으면 중단
            if (hand.Count >= maxHandSize)
            {
                Debug.Log("손패 꽉 찼음");
                return;
            }

            DrawCard();
        }
    }

    // 🔹 카드 드로우
    /// <summary>
    /// 카드 1장 드로우
    /// </summary>
    public void DrawCard()
    {
        // 손패 꽉 찼으면 중단
        if (hand.Count >= maxHandSize)
        {
            return;
        }

        // 🔥 덱이 비면 discard 섞어서 채움
        if (deck.Count == 0)
        {
            ReShuffleDiscardIntoDeck();
        }

        if (deck.Count == 0)
        {
            Debug.Log("드로우 불가: 카드 없음");
            return;
        }

        CardInstance card = deck[0];
        deck.RemoveAt(0);

        hand.Add(card);

        //Debug.Log("드로우: " + card.data.displayCardName);
    }

    /// <summary>
    /// 카드 사용
    /// </summary>
    public bool UseCard(CardInstance card)
    {
        if (card == null)
        {
            Debug.Log("카드가 null");
            return false;
        }

        // 코스트 체크
        if (currentCost < card.data.cardCost)
        {
            Debug.Log("코스트 부족");
            return false;
        }

        currentCost -= card.data.cardCost;

        hand.Remove(card);
        disCard.Add(card);

        return true;
    }




    /// <summary>
    /// 카드 1장 버리기
    /// </summary>
    public void DisCard(CardInstance card) // 🔥 파라미터 추가
    {
        if (hand.Contains(card))
        {
            hand.Remove(card);
            disCard.Add(card);
        }
    }


    /// <summary>
    /// 손패 전체 버리기
    /// </summary>
    public void AllDisCard()
    {
        foreach (var card in hand)
        {
            disCard.Add(card);
        }

        hand.Clear();
    }

    // 🔹 덱 리필 + 셔플
    [ContextMenu("덱 리필+셔플")]
    /// <summary>
    /// discard → deck 리필
    /// </summary>
    public void ReShuffleDiscardIntoDeck()
    {
        if (disCard.Count == 0) return;

        deck.AddRange(disCard);
        disCard.Clear();

        ShuffleCard();

        //Debug.Log("discard → deck 셔플");
    }

    // 🔹 덱 셔플
    [ContextMenu("덱 셔플")]
    /// <summary>
    /// 덱 셔플
    /// </summary>
    public void ShuffleCard()
    {
        // 🔥 최소 2장 이상일 때만 셔플
        if (deck.Count < 2) return;

        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);

            CardInstance temp = deck[i];
            deck[i] = deck[rand];
            deck[rand] = temp;
        }
    }

    /// <summary>
    /// 🔥 카드 중복 허용 여부 체크
    /// </summary>
    public bool IsDuplicateAllowed(CardData data)
    {
        int count = 0;

        foreach (var card in deck)
        {
            if (card.data.cardId == data.cardId)
                count++;
        }

        return count < maxDuplicateCount;
    }



}
