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
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);           // 非异步加载,直接加载
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
        ///// 改成携程是为了在加载第一个场景后，就执行AfterSceneLoadedEvent事件，方便CursorManager中的currentGrid的获取
        ///// </summary>
        ///// <returns></returns>
        //private IEnumerator Start()
        //{
        //    // TODO:转换成开始新游戏
        //    ISaveable saveable = this;
        //    saveable.RegisterSaveable();

        //    fadeCanvasGroup = FindObjectOfType<CanvasGroup>();          // 找到挂载了CanvasGroup的Object
        //    yield return StartCoroutine(LoadSceneSetActive(startSceneName));
        //    EventHandler.CallAfterSceneLoadedEvent();
        //}

        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();

            fadeCanvasGroup = FindObjectOfType<CanvasGroup>();          // 找到挂载了CanvasGroup的Object
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
        /// 场景切换
        /// </summary>
        /// <param name="sceneName">目标场景</param>
        /// <param name="targetPosition">目标位置</param>
        /// <returns></returns>
        private IEnumerator Transition(string sceneName, Vector3 targetPosition)
        {
            EventHandler.CallBeforeSceneUnloadEvent();

            yield return Fade(1);

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());  // 卸载当前场景

            yield return LoadSceneSetActive(sceneName);                                 // 加载目标场景

            EventHandler.CallMoveToPosition(targetPosition);                            // 移动人物坐标

            EventHandler.CallAfterSceneLoadedEvent();

            yield return Fade(0);
        }

        /// <summary>
        /// 加载场景并设置为激活
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <returns></returns>
        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);    // 异步加载，叠加

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
        /// 淡入淡出场景
        /// </summary>
        /// <param name="targetAlpha">1是黑 0是透明</param>
        /// <returns></returns>
        private IEnumerator Fade(float targetAlpha)
        {
            isFade = true;

            fadeCanvasGroup.blocksRaycasts = true;  // Fade开始的时候，不让鼠标可以点
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
        /// 读取存档时的场景加载
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <returns></returns>
        private IEnumerator LoadSaveDataScene(string sceneName)
        {
            yield return Fade(1);

            if (SceneManager.GetActiveScene().name != "PersistentScene")
            {
                // 在游戏过程当中，加载新的游戏
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
            // 加载游戏进度场景
            StartCoroutine(LoadSaveDataScene(saveData.dataSceneName));
        }
    }
}

