using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleCardUI_Manager : MonoBehaviour
{

    [Header("Ref")]
    [SerializeField] private DeckBuildManager deckManager;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private BattleUI_Manager battleUIManager; 

    private CardInstance cardInstance;

    public Button drawButton;
    public Button endTurnButton;

    public List<Button> cardButtons = new List<Button>();
    public List<TextMeshProUGUI> cardText = new List<TextMeshProUGUI>();
    public List<Image> cardImage = new List<Image>();


    void Start()
    { 
        // 버튼 이벤트 연결
        drawButton.onClick.AddListener(OnClickDraw);
        endTurnButton.onClick.AddListener(OnClickEndTurn);

        RefreshHandUI();
    }

    /// <summary>
    ///  Draw 버튼 클릭
    /// </summary>
    void OnClickDraw()
    {
        int need = deckManager.maxHandSize - deckManager.HandCount();
        deckManager.DrawCard(need);
        RefreshHandUI(); // 드로우 후 UI 갱신
    }

    /// <summary>
    ///  End Turn 버튼 클릭
    /// </summary>
    void OnClickEndTurn()
    {

        if(!turnManager.IsPlayerTurn)
        {
            Debug.Log("Player 턴이 아님 -> EndTurn 무시");
            return;
        }

        deckManager.AllDisCard();

        RefreshHandUI(); // 턴 종료 후 UI 갱신


    }


    /// <summary>
    /// 카드 버튼 클릭 연결
    /// </summary>
    public void OnClickCard(int index)
    {
        List<CardInstance> hand = deckManager.GetHandCards();

        if (index >= hand.Count) return;

        CardInstance card = hand[index];

        bool used = deckManager.UseCard(card);

        if(used)
        {
            battleManager.ExecuteCard(card);

            RefreshHandUI(); // 카드 사용 후 UI 갱신
        }
        
    }

    /// <summary>
    /// 손패 UI 갱신
    /// </summary>
    void RefreshHandUI()
    {
        List<CardInstance> hand = deckManager.GetHandCards();

        for (int i = 0; i < cardButtons.Count; i++)
        {
            if (i < hand.Count)
            {
                cardButtons[i].gameObject.SetActive(true);

                // 카드 이름 표시
                cardText[i].text = hand[i].data.displayCardName;

                // 카드 이미지 적용
                cardImage[i].sprite = hand[i].data.icon;

                int index = i;

                // 기존 이벤트 제거 후 다시 등록 (중요)
                cardButtons[i].onClick.RemoveAllListeners();
                cardButtons[i].onClick.AddListener(() => OnClickCard(index));

                CardHoverHandler hover = cardButtons[i].GetComponent<CardHoverHandler>();

                if (hover == null)
                {
                    hover = cardButtons[i].gameObject.AddComponent<CardHoverHandler>(); // 🔥 자동 추가
                }

                // 🔥 카드 데이터 연결
                hover.Init(battleUIManager, hand[i]);



                // Player 턴이 아니면 카드 막기
                if (!turnManager.IsPlayerTurn)
                {
                    cardButtons[i].interactable = false;
                }
                // 현재 cost 가 card cost 보다 작으면 버튼 막기
                else if(deckManager.currentCost < hand[i].data.cardCost)
                {
                    cardButtons[i].interactable = false;
                }
                else
                {
                    cardButtons[i].interactable = true;
                }
            }
            else
            {
                cardButtons[i].gameObject.SetActive(false);
            }
        }
    }





}
