using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractive : MonoBehaviour
{
    private bool isAnimating;
    private WaitForSeconds pause = new WaitForSeconds(0.04f);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAnimating)
        {
            if (other.transform.position.x < transform.position.x)
            {
                // 人物在农作物左边 向右摇晃
                StartCoroutine(RotateRight());
            }
            else
            {
                // 人物在农作物右边 向左摇晃
                StartCoroutine(RotateLeft());
            }
            EventHandler.CallPlaySoundEvent(SoundName.Rustle);      // 播放掠过草丛时的声音
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isAnimating)
        {
            if (other.transform.position.x > transform.position.x)
            {
                // 人物在农作物左边 向右摇晃
                StartCoroutine(RotateRight());
            }
            else
            {
                // 人物在农作物右边 向左摇晃
                StartCoroutine(RotateLeft());
            }
            EventHandler.CallPlaySoundEvent(SoundName.Rustle);      // 播放掠过草丛时的声音
        }
    }

    private IEnumerator RotateLeft()
    {
        isAnimating = true;

        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(0).Rotate(0, 0, 2);
            yield return pause;
        }
        for (int i = 0; i < 5; i++)
        {
            transform.GetChild(0).Rotate(0, 0, -2);
            yield return pause;
        }
        transform.GetChild(0).Rotate(0, 0, 2);
        yield return pause;

        isAnimating = false;
    }

    private IEnumerator RotateRight()
    {
        isAnimating = true;

        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(0).Rotate(0, 0, -2);
            yield return pause;
        }
        for (int i = 0; i < 5; i++)
        {
            transform.GetChild(0).Rotate(0, 0, 2);
            yield return pause;
        }
        transform.GetChild(0).Rotate(0, 0, -2);
        yield return pause;

        isAnimating = false;
    }
}
