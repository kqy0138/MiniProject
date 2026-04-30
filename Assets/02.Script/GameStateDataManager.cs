using System.Collections.Generic;
using UnityEngine;




/// <summary>
/// 게임의 상태 데이터를 저장하는 클래스
/// 이 클래스에서는 전체적인 게임의 상태를 저장하고 반환해야한다
/// </summary>
public class GameStateDataManager : MonoBehaviour
{
    public static GameStateDataManager Instance;

    // 상태
    public bool isNewRun;
    public bool hasSavedPosition;

    // 맵
    public int mapSeed;

    // 플레이어
    public Vector3 playerPosition;

    public int playerHP;

    // Enemy ID 저장
    public List<string> deadEnemies = new List<string>();


    private void Awake()
    {
        // [Singleton 1단계] 중복 인스턴스 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // [Singleton 2단계] 전역 참조 등록
        Instance = this;

        // [Singleton 3단계] 씬 변경 후에도 유지
        DontDestroyOnLoad(gameObject);
    }

    public void ResetData()
    {
        isNewRun = true;

        mapSeed = 0;

        playerPosition = Vector3.zero;
        playerHP = 0;

        deadEnemies.Clear();

        hasSavedPosition = false;

        playerHP = 0;

        deadEnemies.Clear();

    }



}
