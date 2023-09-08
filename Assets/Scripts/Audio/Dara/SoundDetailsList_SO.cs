using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDetailsList_SO", menuName = "Sound/SoundDetailsList")]
public class SoundDetailsList_SO : ScriptableObject
{
    public List<SoundDatails> soundDatails;

    public SoundDatails GetSoundDatails(SoundName name)
    {
        return soundDatails.Find(s => s.soundName == name);
    }
}

[System.Serializable]
public class SoundDatails
{
    public SoundName soundName;
    public AudioClip soundClip;
    [Range(0.1f, 1.5f)] public float soundPitchMin;     // 随机音调的最低值
    [Range(0.1f, 1.5f)] public float soundPitchMax;     // 随机音调的最高值
    [Range(0.1f, 1f)]   public float soundVolume;       // 音量
}
