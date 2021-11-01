using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum SoundType
{
    Click = 0,
    Fail = 1,
    Good = 2,
    Lose = 3,
    Sgo = 4,
    Shoot = 5
}

public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }

    private AudioSource _as;

    private void Awake()
    {
        if (Instance != null) return;
        else Instance = this;

        Init();
    }

    private void Init()
    {
        _as = GetComponent<AudioSource>();
    }

    public void PlaySound(SoundType _type)
    {
        //_as.clip = Resources.Load<AudioClip>("sound/" + _type.ToString());
        _as.PlayOneShot(Resources.Load<AudioClip>("sound/" + _type.ToString()));
    }

    public void Un_MuteAudio()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }

    public bool IsMuted()
    {
        return AudioListener.volume == 0;
    }

}
