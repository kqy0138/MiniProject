using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Enemy 충돌 처리를 관리하는 클래스
/// 여기서 데이터를 복사하고 BattleContext에 전달한다
/// </summary>
public class EnemyTrigger : MonoBehaviour
{
    private OverworldEnemy enemy;


    private void Awake()
    {
        enemy = GetComponent<OverworldEnemy>();
        
        if (enemy == null)
        {
            Debug.LogError("OverworldEnemy 없음!");
        }
    }

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;
        if (!collision.CompareTag("Player")) return;

        var state = GameStateDataManager.Instance;
        if (state == null) return;

        if(state.deadEnemies.Contains(enemy.enemyID))
        {
            Debug.Log("Dead Enemy Trigger 무시:" + enemy.enemyID);
            return;
        }

        hasTriggered = true;

        // 여기서 플레이어 위치 저장
        GameStateDataManager.Instance.playerPosition = collision.transform.position;
        Debug.Log("Player Position Saved: " + collision.transform.position);


        //// 여기서 Enemy 데이터 복사하고 전달
        EnemyData data = enemy.GetData();

        if (BattleContext.Instance == null)
        {
            Debug.LogError("BattleContext 없음!");
            return;
        }
        BattleContext.Instance.currentEnemyData = data;

        Debug.Log("[EnemyTrigger] EnemyData 저장 완료");

        SceneManager.LoadScene("InGame_BattleScene");
    }

}
