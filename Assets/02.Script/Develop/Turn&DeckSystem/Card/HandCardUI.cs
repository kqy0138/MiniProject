using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandCardUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;

    public void SetCard(CardInstance card)
    {
        nameText.text = card.data.displayCardName;
        icon.sprite = card.data.icon;
    }
}
