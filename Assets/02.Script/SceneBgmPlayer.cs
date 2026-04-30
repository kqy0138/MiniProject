using UnityEngine;

public class SceneBgmPlayer : MonoBehaviour
{
    [Header("이 씬에서 사용할 BGM")]
    [SerializeField] private AudioClip bgmClip;


    private void Start()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBgm(bgmClip);
            
        }
    }

}