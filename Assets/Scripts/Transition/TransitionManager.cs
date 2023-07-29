using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFarm.Transition
{
    public class TransitionManager : MonoBehaviour
    {
        [SceneName]
        public string startSceneName = string.Empty;

        private CanvasGroup fadeCanvasGroup;
        private bool isFade;


        /// <summary>
        /// �ĳ�Я����Ϊ���ڼ��ص�һ�������󣬾�ִ��AfterSceneLoadedEvent�¼�������CursorManager�е�currentGrid�Ļ�ȡ
        /// </summary>
        /// <returns></returns>
        private IEnumerator Start()
        {
            fadeCanvasGroup = FindObjectOfType<CanvasGroup>();          // �ҵ�������CanvasGroup��Object
            yield return StartCoroutine(LoadSceneSetActive(startSceneName));
            EventHandler.CallAfterSceneLoadedEvent();
        }
        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
        }

        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }

        private void OnTransitionEvent(string secneToGo, Vector3 positionToGo)
        {
            if (!isFade)
                StartCoroutine(Transition(secneToGo, positionToGo));
        }

        /// <summary>
        /// �����л�
        /// </summary>
        /// <param name="sceneName">Ŀ�곡��</param>
        /// <param name="targetPosition">Ŀ��λ��</param>
        /// <returns></returns>
        private IEnumerator Transition(string sceneName, Vector3 targetPosition)
        {
            EventHandler.CallBeforeSceneUnloadEvent();

            yield return Fade(1);

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());  // ж�ص�ǰ����

            yield return LoadSceneSetActive(sceneName);                                 // ����Ŀ�곡��

            EventHandler.CallMoveToPosition(targetPosition);                            // �ƶ���������

            EventHandler.CallAfterSceneLoadedEvent();

            yield return Fade(0);
        }

        /// <summary>
        /// ���س���������Ϊ����
        /// </summary>
        /// <param name="sceneName">������</param>
        /// <returns></returns>
        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);    // �첽���أ����Ӽ���

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            SceneManager.SetActiveScene(newScene);
        }

        /// <summary>
        /// ���뵭������
        /// </summary>
        /// <param name="targetAlpha">1�Ǻ� 0��͸��</param>
        /// <returns></returns>
        private IEnumerator Fade(float targetAlpha)
        {
            isFade = true;

            fadeCanvasGroup.blocksRaycasts = true;  // Fade��ʼ��ʱ�򣬲��������Ե�
            float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / Settings.fadeDuration;

            while(!Mathf.Approximately(fadeCanvasGroup.alpha,targetAlpha))
            {
                fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
                yield return null;
            }
            fadeCanvasGroup.blocksRaycasts = false;
            isFade = false;
        }
    }
}

