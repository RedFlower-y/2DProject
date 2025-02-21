using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using MFarm.Dialogue;

[System.Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    private PlayableDirector director;
    public DialoguePiece dialoguePiece;

    public override void OnPlayableCreate(Playable playable)
    {
        director = playable.GetGraph().GetResolver() as PlayableDirector;
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        // 呼叫启动对话窗口
        EventHandler.CallShowDialogueEvent(dialoguePiece);
        if (Application.isPlaying)
        {
            // 正在播放的情况下
            if (dialoguePiece.hasToPause)
            {
                // 暂停timeline
                TimelineManager.Instance.PauseTimeline(director);
            }
            else
            {
                // 关闭对话窗口
                EventHandler.CallShowDialogueEvent(null);
            }
        }
    }

    // 在Timeline播放期间 每一帧执行
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (Application.isPlaying)
            TimelineManager.Instance.IsDone = dialoguePiece.isDone;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        // 关闭对话窗口
        EventHandler.CallShowDialogueEvent(null);
    }

    public override void OnGraphStart(Playable playable)
    {
        // Timeline执行时，时间暂停
        EventHandler.CallUpdateGameStateEvent(GameState.GamePause);
    }

    public override void OnGraphStop(Playable playable)
    {
        // Timeline结束时，时间流动
        EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
    }
}
