using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class VolumeUI : MonoBehaviour
{
    [SerializeField] private Slider bgmslider;
    [SerializeField] private Slider sfxSlider;

    private void Awake()
    {
        // 🔹 초기화
        bgmslider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();

        // 🔹 이벤트 먼저 연결 (중요)
        bgmslider.onValueChanged.AddListener(OnBGMChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
    }




    private void Start()
    {
        // 🔹 1. 저장된 값 불러오기 (확실하게)
        float bgm = PlayerPrefs.GetFloat("BGM_VOLUME", 1f);
        float sfx = PlayerPrefs.GetFloat("SFX_VOLUME", 1f);


        // 🔹 2. Slider 값 먼저 설정
        bgmslider.value = bgm;
        sfxSlider.value = sfx;


        // 🔹 3. 실제 사운드에도 강제 적용 (동기화)
        SoundManager.Instance.SetBGMVolume(bgm);
        SoundManager.Instance.SetSFXVolume(sfx);

    }

    private void OnBGMChanged(float value)
    {
        SoundManager.Instance.SetBGMVolume(value);
    }

    private void OnSFXChanged(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
    }
}
