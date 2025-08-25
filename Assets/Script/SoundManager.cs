using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{
    const float MUTE = -80f;
    const float NOT_MUTE = 0f;

    AudioSource[] _audioSources;
    SoundData _soundData;

    int _currentChannel;
    int _maxChannel;


    protected override void Awake()
    {
        base.Awake();
        _audioSources = GetComponentsInChildren<AudioSource>();
        _soundData = Resources.Load<SoundData>("SoundData");
        _maxChannel = _audioSources.Length - 1;

        AudioMixer mixer = Resources.Load<AudioMixer>("AudioMixer");

        for (int i = 0; i < _audioSources.Length; i++)
        {
            AudioSource source = _audioSources[i];

            if (i == 0)
            {
                source.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
                source.loop = true;
                continue;
            }

            source.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
            source.loop = false;
        }

        Setting.Instance.OnBgmMuteChange += v =>
        {
            mixer.SetFloat("BGM", v ? MUTE : NOT_MUTE);
        };
        Setting.Instance.OnSfxMuteChange += v =>
        {
            mixer.SetFloat("SFX", v ? MUTE : NOT_MUTE);
        };

        SceneManager.sceneLoaded += (s, l) =>
        {
            if (Enum.TryParse(s.name, out BGM_List bgm))
            {
                BGM_Play(bgm);
            }
        };
    }

    public void BGM_Play(BGM_List bgm)
    {
        AudioSource audioSource = _audioSources[0];
        audioSource.clip = _soundData.Bgms[(int)bgm];
        audioSource.Play();
    }

    public void SFX_Play(SFX_List sfx)
    {
        if (++_currentChannel > _maxChannel) _currentChannel = 1;

        AudioSource audioSource = _audioSources[_currentChannel];
        audioSource.clip = _soundData.Sfxs[(int)sfx];
        audioSource.Play();
    }
}