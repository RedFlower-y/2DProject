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
}
