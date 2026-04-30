using UnityEngine;
using UnityEngine.SceneManagement;

public class Test_EnemyTrigger : MonoBehaviour
{
    private Test_EnemyA enemy;

    private void Awake()
    {
        enemy = GetComponent<Test_EnemyA>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. 데이터 복사
            Test_EnemyData data = enemy.GetData();

            // 2. 저장
            Test_BattleContext.Instance.currentEnemyData = data;

            // 3. 씬 이동
            SceneManager.LoadScene("TestScene4_ObjMoveScene2");
        }
    }
}
