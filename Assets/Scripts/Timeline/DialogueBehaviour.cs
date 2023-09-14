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
}
