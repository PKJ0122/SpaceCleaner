using UnityEngine;

[CreateAssetMenu(fileName = "SoundData",menuName = "ScriptableObject/SoundData")]
public class SoundData : ScriptableObject
{
    public AudioClip[] Bgms;
    public AudioClip[] Sfxs;
}