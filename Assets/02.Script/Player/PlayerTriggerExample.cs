using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerTriggerExample : MonoBehaviour
{
    // Battle Scene 으로 전환할 Scene 이름
    public string battelSceneName = "InGame_BattleScene";


    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            Debug.Log("Enemy 충돌 -> 전투 씬 이동");

            SceneManager.LoadScene(battelSceneName);
        }
        else
        {
            Debug.Log("collision 충돌함");
        }
        
    }



}
