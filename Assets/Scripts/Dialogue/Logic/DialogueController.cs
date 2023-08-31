using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MFarm.Dialogue
{
    [RequireComponent(typeof(NPCMovement))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class DialogueController : MonoBehaviour
    {
        private NPCMovement npc => GetComponent<NPCMovement>();

        public UnityEvent OnFinishEvent;

        public List<DialoguePiece> dialogueList = new List<DialoguePiece>();

        private Stack<DialoguePiece> dialogueStack;

        private bool canTalk;
        private bool isTalking;

        private GameObject UISign;

        private void Awake()
        {
            UISign = transform.GetChild(1).gameObject;
            FillDialogueStack();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canTalk = !npc.isMoving && npc.interactable;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canTalk = false;
            }
        }

        private void Update()
        {
            UISign.SetActive(canTalk);

            if (canTalk && Input.GetKeyDown(KeyCode.Space) && !isTalking)
            {
                StartCoroutine(DialogueRoutine());
            }
        }

        /// <summary>
        /// 构建对话堆栈
        /// </summary>
        private void FillDialogueStack()
        {
            dialogueStack = new Stack<DialoguePiece>();

            // 因为是堆栈，所以反着将对话存到栈里
            for (int i = dialogueList.Count - 1; i >= 0; i--)
            {
                dialogueList[i].isDone = false;
                dialogueStack.Push(dialogueList[i]);    
            }
        }

       
        private IEnumerator DialogueRoutine()
        {
            isTalking = true;
            if (dialogueStack.TryPop(out DialoguePiece result))
            {
                // 传递UI显示对话
                EventHandler.CallShowDialogueEvent(result);
                EventHandler.CallUpdateGameStateEvent(GameState.GamePause); // 对话开始 人物不允许移动

                yield return new WaitUntil(() => result.isDone);    // 直到当前这一句话显示完为止
                isTalking = false;
            }
            else
            {
                // 对话已经读完
                EventHandler.CallUpdateGameStateEvent(GameState.GamePlay); // 对话结束 人物允许移动
                EventHandler.CallShowDialogueEvent(null);
                FillDialogueStack();                        // 重新生成对话堆栈，用于重复对话
                isTalking = false;

                if(OnFinishEvent != null)
                {
                    OnFinishEvent.Invoke();
                    canTalk = false;            // 在一次对话结束后的事件没有完成时 不允许再次对话
                }
            }
        }
    }
}
