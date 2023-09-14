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
        // ���������Ի�����
        EventHandler.CallShowDialogueEvent(dialoguePiece);
        if (Application.isPlaying)
        {
            // ���ڲ��ŵ������
            if (dialoguePiece.hasToPause)
            {
                // ��ͣtimeline
                TimelineManager.Instance.PauseTimeline(director);
            }
            else
            {
                // �رնԻ�����
                EventHandler.CallShowDialogueEvent(null);
            }
        }
    }

    // ��Timeline�����ڼ� ÿһִ֡��
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (Application.isPlaying)
            TimelineManager.Instance.IsDone = dialoguePiece.isDone;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        // �رնԻ�����
        EventHandler.CallShowDialogueEvent(null);
    }

    public override void OnGraphStart(Playable playable)
    {
        // Timelineִ��ʱ��ʱ����ͣ
        EventHandler.CallUpdateGameStateEvent(GameState.GamePause);
    }

    public override void OnGraphStop(Playable playable)
    {
        // Timeline����ʱ��ʱ������
        EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
    }
}
