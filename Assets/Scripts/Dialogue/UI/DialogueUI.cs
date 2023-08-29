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
    /// 将对话显示出来
    /// </summary>
    /// <param name="dialoguePiece">对话数据</param>
    /// <returns></returns>
    private IEnumerator ShowDialogue(DialoguePiece dialoguePiece)
    {
        if(dialoguePiece != null)
        {
            dialoguePiece.isDone = false;   // 让NPC可以重复对话

            dialogueBox.SetActive(true);
            continueBox.SetActive(false);

            dialogueText.text = string.Empty;

            if (dialoguePiece.name != string.Empty)
            {
                // 有名字
                if (dialoguePiece.onLeft)
                {
                    // 是NPC在讲话，头像是左边
                    faceRight.gameObject.SetActive(false);

                    faceLeft.gameObject.SetActive(true);
                    faceLeft.sprite = dialoguePiece.faceImage;
                    nameLeft.text = dialoguePiece.name;
                }
                else
                {
                    // 是主角在讲话，头像是右边
                    faceLeft.gameObject.SetActive(false);

                    faceRight.gameObject.SetActive(true);
                    faceRight.sprite = dialoguePiece.faceImage;
                    nameRight.text = dialoguePiece.name;
                }
            }
            else
            {
                // 没有名字
                faceRight.gameObject.SetActive(false);
                faceLeft.gameObject.SetActive(false);
                nameRight.gameObject.SetActive(false);
                nameLeft.gameObject.SetActive(false);
            }
            yield return dialogueText.DOText(dialoguePiece.dialogueText, 1f).WaitForCompletion();       // 等待对话逐字加载完成为止

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
