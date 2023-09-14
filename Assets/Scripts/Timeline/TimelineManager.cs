using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : Singloten<TimelineManager>
{
    public PlayableDirector startDirector;
    private PlayableDirector currentDirector;

    private bool isPause;

    protected override void Awake()
    {
        base.Awake();
        currentDirector = startDirector;
    }

    private void Update()
    {
        if (isPause && Input.GetKeyDown(KeyCode.Space))
        {
            isPause = false;
            currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
        }
    }

    public void PauseTimeline(PlayableDirector director)
    {
        currentDirector = director;
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);  // 将根节点的playable的播放速度设置为0（暂停）     
        isPause = true;
    }
}
