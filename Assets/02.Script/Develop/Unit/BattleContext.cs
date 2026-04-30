using UnityEngine;

/// <summary>
/// Enemy 데이터 복사용 클래스
/// </summary>

public class BattleContext : MonoBehaviour
{
    public static BattleContext Instance;

    [Header("현재 전투에 사용할 Enemy 데이터")]
    [Tooltip("필드에서 충돌한 Enemy의 값이 여기에 복사됨")]
    public EnemyData currentEnemyData;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("[BattleContext] 중복 생성 → 삭제");
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            //Debug.Log("[BattleContext] 생성됨");
        }
        
    }
}
