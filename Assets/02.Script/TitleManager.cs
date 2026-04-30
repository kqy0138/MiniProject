using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{

    public void StartNewRun()
    {
        var state = GameStateDataManager.Instance;

        state.ResetData();
        state.isNewRun = true;
        state.mapSeed = Random.Range(0, 10000);

        //state.deadEnemies.Clear();

        SceneManager.LoadScene("InGame_RoomTest");
    }


    public void ContinueRun()
    {
        var state = GameStateDataManager.Instance;
        
        state.isNewRun = false;

        SceneManager.LoadScene("InGame_RoomTest");
    }
}
