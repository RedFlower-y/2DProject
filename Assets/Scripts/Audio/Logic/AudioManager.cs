using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : Singloten<AudioManager>
{
    [Header("�������ݿ�")]
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
    private float musicTransitionSecond = 8f;                   // �����������ӵ�Ŀ��������ʱ��

    public float MusicStartSecond => Random.Range(5f, 15f);     // ������������Ϻ�ʼ �������������ֿ�ʼΪֹ�ĵȴ�ʱ��
    private Coroutine soundRoutine;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent  += OnAfterSceneLoadedEvent;
        EventHandler.PlaySoundEvent         += OnPlaySoundEvent;
        EventHandler.EndGameEvent           += OnEndGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent  -= OnAfterSceneLoadedEvent;
        EventHandler.PlaySoundEvent         -= OnPlaySoundEvent;
        EventHandler.EndGameEvent           -= OnEndGameEvent;
    }

    private void OnEndGameEvent()
    {
        if (soundRoutine != null)
            StopCoroutine(soundRoutine);
        muteSnapshots.TransitionTo(1f);
    }

    private void OnAfterSceneLoadedEvent()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneSoundItem sceneSound = sceneSoundData.GetSceneSoundItem(currentScene);
        if (sceneSound == null)
            return;

        SoundDetails ambientSound       = soundDetailsData.GetSoundDetails(sceneSound.ambientSound);
        SoundDetails backgroundMusic    = soundDetailsData.GetSoundDetails(sceneSound.backgroundMusic);

        if (soundRoutine != null)
            StopCoroutine(soundRoutine);
        soundRoutine = StartCoroutine(PlaySoundRoutine(backgroundMusic, ambientSound));
    }

    private void OnPlaySoundEvent(SoundName soundName)
    {
        var soundDetails = soundDetailsData.GetSoundDetails(soundName);
        if (soundDetails != null)
            EventHandler.CallInitSoundEffect(soundDetails);
    }

    /// <summary>
    ///  �Ȳ��Ż�������Ȼ�������ű�������
    /// </summary>
    /// <param name="backGroundMusic">��������</param>
    /// <param name="ambientSound">������</param>
    /// <returns></returns>
    private IEnumerator PlaySoundRoutine(SoundDetails backGroundMusic,SoundDetails ambientSound)
    {
        if (backGroundMusic != null && ambientSound != null)
        {
            PlayAmbientSoundClip(ambientSound, musicTransitionSecond);
            yield return new WaitForSeconds(MusicStartSecond);
            PlayBackgroundMusicClip(backGroundMusic, musicTransitionSecond);
        }
    }

    /// <summary>
    /// ���Ż�����Ч
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayAmbientSoundClip(SoundDetails soundDetails, float transitionTime)
    {
        audioMixer.SetFloat("AmbientSoundVolume", ConvertSoundVolume(soundDetails.soundVolume));
        ambientSoundSource.clip = soundDetails.soundClip;
        if (ambientSoundSource.isActiveAndEnabled)
            ambientSoundSource.Play();

        ambientSoundOnlySnapshots.TransitionTo(1f);
    }

    /// <summary>
    /// ���ű�������
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayBackgroundMusicClip(SoundDetails soundDetails, float transitionTime)
    {
        audioMixer.SetFloat("BackgroundMusicVolume", ConvertSoundVolume(soundDetails.soundVolume));
        backgroundMusicSource.clip = soundDetails.soundClip;
        if (backgroundMusicSource.isActiveAndEnabled)
            backgroundMusicSource.Play();

        normalSnapshots.TransitionTo(transitionTime);
    }

    /// <summary>
    /// ��SoundDatails�е�Volumeת��Ϊʵ��AudioMixer�е�����
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    private float ConvertSoundVolume(float amount)
    {
        return (amount * 100 - 80);
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", (value * 100 - 80));
    }
}
