using UnityEngine;

public class Test_BattleContext : MonoBehaviour
{

    public static Test_BattleContext Instance;

    public Test_EnemyData currentEnemyData;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 넘어가도 유지
        }

        else
        {
            Destroy(gameObject);
        }

    }



}
