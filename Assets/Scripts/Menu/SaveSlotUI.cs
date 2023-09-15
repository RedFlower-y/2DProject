using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public Text dataTime, dataScene;
    private Button currentButton;
    private int index => transform.GetSiblingIndex();   // ªÒ»°±£¥Ê¿∏±‡∫≈

    private void Awake()
    {
        currentButton = GetComponent<Button>();
        currentButton.onClick.AddListener(LoadGameData);
    }

    private void LoadGameData()
    {
        Debug.Log(index);
    }
}
