﻿using UnityEngine;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour
{
    [SerializeField]
    private Button button;
    [SerializeField]
    private Image waveImage;
    private void Awake() {
        waveImage.enabled = !GameManager.instance.soundMute;
        button.onClick.AddListener(() => {
            GameManager.instance.UserTriggerSoundSetting();
            GameManager.instance.PlaySound("Pop1");
            waveImage.enabled = !GameManager.instance.soundMute;
        });
    }
}