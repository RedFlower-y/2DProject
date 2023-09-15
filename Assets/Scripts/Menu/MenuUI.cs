using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject[] panel;

    public void SwitchPanel(int index)
    {
        for (int i = 0; i < panel.Length; i++)
        {
            if (i == index)
            {
                panel[i].transform.SetAsLastSibling();      // 将指定Panel的绘制顺序调到最后
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("EXIT GAME");
    }
}
