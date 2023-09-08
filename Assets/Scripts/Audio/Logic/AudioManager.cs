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

    public float MusicStartSecond => Random.Range(5f, 15f);
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
            PlayAmbientSoundClip(ambientSound);
            yield return new WaitForSeconds(MusicStartSecond);
            PlayBackgroundMusicClip(backGroundMusic);
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayBackgroundMusicClip(SoundDatails soundDetails)
    {
        backgroundMusicSource.clip = soundDetails.soundClip;
        if (backgroundMusicSource.isActiveAndEnabled)
            backgroundMusicSource.Play();
    }

    /// <summary>
    /// 播放环境音效
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayAmbientSoundClip(SoundDatails soundDetails)
    {
        ambientSoundSource.clip = soundDetails.soundClip;
        if (ambientSoundSource.isActiveAndEnabled)
            ambientSoundSource.Play();
    }
}
