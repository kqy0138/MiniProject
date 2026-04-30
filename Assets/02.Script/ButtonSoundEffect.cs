using UnityEngine;

public class ButtonSoundEffect : MonoBehaviour
{

    [Header("이 씬에서 사용할 Effect")]
    [SerializeField] private AudioClip effectClip;

    public void PlayButtonSfx()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySfxOneShot(effectClip);
        }
    }

}
