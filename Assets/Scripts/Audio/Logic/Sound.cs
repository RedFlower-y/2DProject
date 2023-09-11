using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    /// <summary>
    /// ����������soundDetails �Ž�AudioSource��
    /// </summary>
    /// <param name="soundDatails"></param>
    public void SetSound(SoundDetails soundDatails)
    {
        audioSource.clip = soundDatails.soundClip;
        audioSource.volume = soundDatails.soundVolume;
        audioSource.pitch = Random.Range(soundDatails.soundPitchMin, soundDatails.soundPitchMax);
    }
}
