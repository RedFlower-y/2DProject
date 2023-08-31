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
        /// �����Ի���ջ
        /// </summary>
        private void FillDialogueStack()
        {
            dialogueStack = new Stack<DialoguePiece>();

            // ��Ϊ�Ƕ�ջ�����Է��Ž��Ի��浽ջ��
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
                // ����UI��ʾ�Ի�
                EventHandler.CallShowDialogueEvent(result);
                EventHandler.CallUpdateGameStateEvent(GameState.GamePause); // �Ի���ʼ ���ﲻ�����ƶ�

                yield return new WaitUntil(() => result.isDone);    // ֱ����ǰ��һ�仰��ʾ��Ϊֹ
                isTalking = false;
            }
            else
            {
                // �Ի��Ѿ�����
                EventHandler.CallUpdateGameStateEvent(GameState.GamePlay); // �Ի����� ���������ƶ�
                EventHandler.CallShowDialogueEvent(null);
                FillDialogueStack();                        // �������ɶԻ���ջ�������ظ��Ի�
                isTalking = false;

                if(OnFinishEvent != null)
                {
                    OnFinishEvent.Invoke();
                    canTalk = false;            // ��һ�ζԻ���������¼�û�����ʱ �������ٴζԻ�
                }
            }
        }
    }
}
