using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MFarm.Save;

namespace MFarm.Transition
{
    public class TransitionManager : Singloten<TransitionManager>, ISaveable
    {
        [SceneName]
        public string startSceneName = string.Empty;

        private CanvasGroup fadeCanvasGroup;
        private bool isFade;

        public string GUID => GetComponent<DataGUID>().GUID;

        protected override void Awake()
        {
            base.Awake();
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);           // ���첽����,ֱ�Ӽ���
        }

        private void OnEnable()
        {
            EventHandler.TransitionEvent    += OnTransitionEvent;
            EventHandler.StartNewGameEvent  += OnStartNewGameEvent;
            EventHandler.EndGameEvent       += OnEndGameEvent;
        }

        private void OnDisable()
        {
            EventHandler.TransitionEvent    -= OnTransitionEvent;
            EventHandler.StartNewGameEvent  -= OnStartNewGameEvent;
            EventHandler.EndGameEvent       -= OnEndGameEvent;
        }

        ///// <summary>
        ///// �ĳ�Я����Ϊ���ڼ��ص�һ�������󣬾�ִ��AfterSceneLoadedEvent�¼�������CursorManager�е�currentGrid�Ļ�ȡ
        ///// </summary>
        ///// <returns></returns>
        //private IEnumerator Start()
        //{
        //    // TODO:ת���ɿ�ʼ����Ϸ
        //    ISaveable saveable = this;
        //    saveable.RegisterSaveable();

        //    fadeCanvasGroup = FindObjectOfType<CanvasGroup>();          // �ҵ�������CanvasGroup��Object
        //    yield return StartCoroutine(LoadSceneSetActive(startSceneName));
        //    EventHandler.CallAfterSceneLoadedEvent();
        //}

        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();

            fadeCanvasGroup = FindObjectOfType<CanvasGroup>();          // �ҵ�������CanvasGroup��Object
        }

        private void OnEndGameEvent()
        {
            StartCoroutine(UnloadScene());
        }

        private void OnStartNewGameEvent(int index)
        {
            StartCoroutine(LoadSaveDataScene(startSceneName));
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
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);    // �첽���أ�����

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            SceneManager.SetActiveScene(newScene);
        }

        private IEnumerator UnloadScene()
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return Fade(1f);
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            yield return Fade(0);
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

        /// <summary>
        /// ��ȡ�浵ʱ�ĳ�������
        /// </summary>
        /// <param name="sceneName">��������</param>
        /// <returns></returns>
        private IEnumerator LoadSaveDataScene(string sceneName)
        {
            yield return Fade(1);

            if (SceneManager.GetActiveScene().name != "PersistentScene")
            {
                // ����Ϸ���̵��У������µ���Ϸ
                EventHandler.CallBeforeSceneUnloadEvent();
                yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            }

            yield return LoadSceneSetActive(sceneName);
            EventHandler.CallAfterSceneLoadedEvent();
            yield return Fade(0);
        }

        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.dataSceneName = SceneManager.GetActiveScene().name;

            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            // ������Ϸ���ȳ���
            StartCoroutine(LoadSaveDataScene(saveData.dataSceneName));
        }
    }
}

