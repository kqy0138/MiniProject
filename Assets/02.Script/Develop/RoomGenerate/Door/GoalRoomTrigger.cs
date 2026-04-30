using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalRoomTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision ==  collision.CompareTag("Player"))
        {
            SceneManager.LoadScene("InGame_ClearAdventure");
        }

    }

}
