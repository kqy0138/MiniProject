
[System.Serializable]

public class CardInstance
{
    public CardData data;// 🔹 원본 데이터 (Catalog 참조)
    public int currentValue;
    public bool isUpgraded;

    public CardInstance(CardData data)
    {
        this.data = data;
        this.currentValue = data.value;
        this.isUpgraded = false;
    }
}