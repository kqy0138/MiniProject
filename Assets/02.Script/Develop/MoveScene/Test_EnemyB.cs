using UnityEngine;

public class Test_EnemyB : MonoBehaviour
{
    public int currentHP;
    public int attack;
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Init(Test_EnemyData data)
    {
        currentHP = data.maxHP;
        attack = data.attack;
        spriteRenderer.sprite = data.sprite;
    }
}
