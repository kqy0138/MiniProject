using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 마우스를 감지하는 클래스
/// </summary>

// IPointerEnterHandler / IPointerExitHandler << 이건 유니티 제공 인터페이스
// 마우스가 올라왔을 때/ 나갔을 때를 감지한다
public class CardHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private BattleUI_Manager uiManager;
    private CardInstance cardData;

    public void Init(BattleUI_Manager manager, CardInstance card)
    {
        uiManager = manager;
        cardData = card;
    }

    /// <summary>
    /// 마우스 올라갔을 때
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        uiManager.ShowTooltip(cardData);
    }

    /// <summary>
    /// 마우스 나갔을 때
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        uiManager.HideTooltip();
    }
}
