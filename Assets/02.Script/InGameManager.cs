using UnityEngine;


[DisallowMultipleComponent]
public class InGameManager : MonoBehaviour
{

    public static InGameManager Instance { get; private set; }

    private void Awake()
    {

        // [Singleton 1단계] 중복 인스턴스 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;
    }


}
