using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("音乐数据库")]
    public SoundDetailsList_SO soundDetailsData;
    public SceneSoundList_SO sceneSoundData;

    [Header("Audio Source")]
    public AudioSource ambientSoundSource;
    public AudioSource backgroundMusicSource;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Snapshots")]
    public AudioMixerSnapshot normalSnapshots;
    public AudioMixerSnapshot ambientSoundOnlySnapshots;
    public AudioMixerSnapshot muteSnapshots;
    private float musicTransitionSecond = 8f;                   // 声音缓慢增加到目标声音的时间

    public float MusicStartSecond => Random.Range(5f, 15f);     // 环境音加载完毕后开始 到启动背景音乐开始为止的等待时间
    private Coroutine soundRoutine;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneSoundItem sceneSound = sceneSoundData.GetSceneSoundItem(currentScene);
        if (sceneSound == null)
            return;

        SoundDatails ambientSound       = soundDetailsData.GetSoundDatails(sceneSound.ambientSound);
        SoundDatails backgroundMusic    = soundDetailsData.GetSoundDatails(sceneSound.backgroundMusic);

        if (soundRoutine != null)
            StopCoroutine(soundRoutine);
        soundRoutine = StartCoroutine(PlaySoundRoutine(backgroundMusic, ambientSound));
    }

    private IEnumerator PlaySoundRoutine(SoundDatails backGroundMusic,SoundDatails ambientSound)
    {
        if (backGroundMusic != null && ambientSound != null)
        {
            PlayAmbientSoundClip(ambientSound, musicTransitionSecond);
            yield return new WaitForSeconds(MusicStartSecond);
            PlayBackgroundMusicClip(backGroundMusic, musicTransitionSecond);
        }
    }

    /// <summary>
    /// 播放环境音效
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayAmbientSoundClip(SoundDatails soundDetails, float transitionTime)
    {
        audioMixer.SetFloat("AmbientSoundVolume", ConvertSoundVolume(soundDetails.soundVolume));
        ambientSoundSource.clip = soundDetails.soundClip;
        if (ambientSoundSource.isActiveAndEnabled)
            ambientSoundSource.Play();

        ambientSoundOnlySnapshots.TransitionTo(1f);
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayBackgroundMusicClip(SoundDatails soundDetails, float transitionTime)
    {
        audioMixer.SetFloat("BackgroundMusicVolume", ConvertSoundVolume(soundDetails.soundVolume));
        backgroundMusicSource.clip = soundDetails.soundClip;
        if (backgroundMusicSource.isActiveAndEnabled)
            backgroundMusicSource.Play();

        normalSnapshots.TransitionTo(transitionTime);
    }

    /// <summary>
    /// 将SoundDatails中的Volume转换为实际AudioMixer中的音量
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    private float ConvertSoundVolume(float amount)
    {
        return (amount * 100 - 80);
    }
}
