using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
}
