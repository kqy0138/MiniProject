using UnityEngine;

public class Test_EnemyA : MonoBehaviour
{
    public int maxHP = 30;
    public int attack = 5;
    public Sprite sprite;


    public Test_EnemyData GetData()
    {
        Test_EnemyData data = new Test_EnemyData();

        data.maxHP = maxHP;
        data.attack = attack;
        data.sprite = sprite;

        return data;
    }
}
