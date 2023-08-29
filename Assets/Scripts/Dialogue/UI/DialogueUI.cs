using MFarm.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialogueBox;
    public Text dialogueText;
    public Image faceLeft, faceRight;
    public Text nameLeft, nameRight;
    public GameObject continueBox;

    private void Awake()
    {
        continueBox.SetActive(false);
    }

    private void OnEnable()
    {
        EventHandler.ShowDialogueEvent += OnShowDialogueEvent;
    }

    private void OnDisable()
    {
        EventHandler.ShowDialogueEvent -= OnShowDialogueEvent;
    }

    private void OnShowDialogueEvent(DialoguePiece dialoguePiece)
    {
        StartCoroutine(ShowDialogue(dialoguePiece));
    }

    /// <summary>
    /// ���Ի���ʾ����
    /// </summary>
    /// <param name="dialoguePiece">�Ի�����</param>
    /// <returns></returns>
    private IEnumerator ShowDialogue(DialoguePiece dialoguePiece)
    {
        if(dialoguePiece != null)
        {
            dialoguePiece.isDone = false;   // ��NPC�����ظ��Ի�

            dialogueBox.SetActive(true);
            continueBox.SetActive(false);

            dialogueText.text = string.Empty;

            if (dialoguePiece.name != string.Empty)
            {
                // ������
                if (dialoguePiece.onLeft)
                {
                    // ��NPC�ڽ�����ͷ�������
                    faceRight.gameObject.SetActive(false);

                    faceLeft.gameObject.SetActive(true);
                    faceLeft.sprite = dialoguePiece.faceImage;
                    nameLeft.text = dialoguePiece.name;
                }
                else
                {
                    // �������ڽ�����ͷ�����ұ�
                    faceLeft.gameObject.SetActive(false);

                    faceRight.gameObject.SetActive(true);
                    faceRight.sprite = dialoguePiece.faceImage;
                    nameRight.text = dialoguePiece.name;
                }
            }
            else
            {
                // û������
                faceRight.gameObject.SetActive(false);
                faceLeft.gameObject.SetActive(false);
                nameRight.gameObject.SetActive(false);
                nameLeft.gameObject.SetActive(false);
            }
            yield return dialogueText.DOText(dialoguePiece.dialogueText, 1f).WaitForCompletion();       // �ȴ��Ի����ּ������Ϊֹ

            dialoguePiece.isDone = true;

            if (dialoguePiece.hasToPause && dialoguePiece.isDone)
            {
                continueBox.SetActive(true);
            }
        }
        else
        {
            dialogueBox.SetActive(false);
            yield break;
        }
    }
}
