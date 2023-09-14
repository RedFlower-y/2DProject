using System;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : Singloten<TimelineManager>
{
    public PlayableDirector startDirector;
    private PlayableDirector currentDirector;

    private bool isPause;

    public bool IsDone { set => isDone = value; }   // ֻ��
    private bool isDone;

    protected override void Awake()
    {
        base.Awake();
        currentDirector = startDirector;
    }

    private void OnEnable()
    {
        //currentDirector.played              += TimelinePlayer;
        //currentDirector.stopped             += TimelineStopped;
        EventHandler.AfterSceneLoadedEvent  += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent  -= OnAfterSceneLoadedEvent;
    }

    private void Update()
    {
        if (isPause && Input.GetKeyDown(KeyCode.Space) && isDone)
        {
            isPause = false;
            currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
        }
    }

    private void OnAfterSceneLoadedEvent()
    {
        currentDirector = FindObjectOfType<PlayableDirector>();
        if (currentDirector != null)
            currentDirector.Play();
    }

    public void PauseTimeline(PlayableDirector director)
    {
        currentDirector = director;
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);  // �����ڵ��playable�Ĳ����ٶ�����Ϊ0����ͣ��     
        isPause = true;
    }

    //private void TimelinePlayer(PlayableDirector director)
    //{
    //    if (director != null)
    //        EventHandler.CallUpdateGameStateEvent(GameState.GamePause);
    //}
    //private void TimelineStopped(PlayableDirector director)
    //{
    //    if (director != null)
    //    {
    //        EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
    //        director.gameObject.SetActive(false);
    //    }
    //}

    

}
