using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.AStar;
using UnityEngine.SceneManagement;
using System;
using MFarm.Save;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class NPCMovement : MonoBehaviour, ISaveable
{
    public ScheduleDataList_SO scheduleData;
    private SortedSet<ScheduleDetails> scheduleSet;
    private ScheduleDetails currentSchedule;

    // 临时储存信息
    [SerializeField]private string currentScene;
    private string targetScnene;

    private Vector3Int  currentGridPosition;
    private Vector3Int  targetGridPosition;
    private Vector3Int  nextGridPosition;
    private Vector3     nextWorldPosition;

    public string StartScene { set { currentScene = value; } }

    [Header("移动属性")]
    public float normalSpeed = 2f;
    private float minSpeed = 1f;
    private float maxSpeed = 3f;
    private Vector2 dir;
    public bool isMoving;       // 动画控制

    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;
    private Animator animator;
    private Grid grid;

    private Stack<MovementStep> movementSteps;
    private Coroutine npcMoveRoutine;

    private bool isInitialised;             // 确保只在第一次加载场景的时候初始化NPC移动   
    private bool isNPCmove;                 // 行为控制
    private bool isSceneLoaded;             // 是否加载完了场景
    public  bool interactable;              // 是否可互动 将ScheduleDetails中的interactable传进来
    public  bool isFirstLoad;               // 是否是第一次加载这个NPC 存档相关

    private Season currentSeason;

    // 动画计时器相关
    private float   animationBreakTime;
    private bool    canPlayStopAnimation;
    private AnimationClip   stopAnimationClip;
    public  AnimationClip   blankAnimationClip;
    private AnimatorOverrideController animOverride;

    private TimeSpan GameTime => TimeManager.Instance.GameTime;

    public string GUID => GetComponent<DataGUID>().GUID;

    private void Awake()
    {
        rb              = GetComponent<Rigidbody2D>();
        spriteRenderer  = GetComponent<SpriteRenderer>();
        coll            = GetComponent<BoxCollider2D>();
        animator        = GetComponent<Animator>();
        movementSteps   = new Stack<MovementStep>();  

        // 初始化
        animOverride = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animOverride;

        scheduleSet = new SortedSet<ScheduleDetails>();
        foreach(var schedule in scheduleData.scheduleList)
        {
            scheduleSet.Add(schedule);
        }
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent  += OnAfterSceneLoadedEvent;
        EventHandler.GameMinuteEvent        += OnGameMinuteEvent;
        EventHandler.StartNewGameEvent      += OnStartNewGameEvent;
        EventHandler.EndGameEvent           += OnEndGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent  -= OnAfterSceneLoadedEvent;
        EventHandler.GameMinuteEvent        -= OnGameMinuteEvent;
        EventHandler.StartNewGameEvent      -= OnStartNewGameEvent;
        EventHandler.EndGameEvent           -= OnEndGameEvent;
    }

    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }

    private void Update()
    {
        if (isSceneLoaded)
            SwitchAnimation();

        // 动画计时器
        animationBreakTime -= Time.deltaTime;
        canPlayStopAnimation = animationBreakTime <= 0;
    }

    private void FixedUpdate()
    {
        if (isSceneLoaded)
            Movement();
    }

    private void OnBeforeSceneUnloadEvent()
    {
        isSceneLoaded = false;
    }

    private void OnAfterSceneLoadedEvent()
    {
        grid = FindObjectOfType<Grid>();
        CheckVisiable();

        if (!isInitialised)
        {
            InitNPC();
            isInitialised = true;       // 已经初始化NPC移动了，下次加载场景NPC就是按照时间表在移动了
        }

        isSceneLoaded = true;

        // 再次读取时 重新构建路径
        if (!isFirstLoad)
        {
            currentGridPosition = grid.WorldToCell(transform.position);
            var schedule = new ScheduleDetails(0, 0, 0, 0, currentSeason, targetScnene, (Vector2Int)targetGridPosition, stopAnimationClip, interactable);
            BuildPath(schedule);
            isFirstLoad = true;
        }
    }

    private void OnGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        int time = (hour * 100) + minute;
        currentSeason = season;

        ScheduleDetails matchSchedule = null;
        foreach(var schedule in scheduleSet)
        {
            if(schedule.Time == time)
            {
                if (schedule.day != day && schedule.day != 0)
                    continue;
                if (schedule.season != season)
                    continue;
                matchSchedule = schedule;
            }
            else if (schedule.Time > time)
            {
                break;
            }
        }

        if (matchSchedule != null)
            BuildPath(matchSchedule);
    }

    private void OnStartNewGameEvent(int index)
    {
        isInitialised = false;
        isFirstLoad = true;
    }

    private void OnEndGameEvent()
    {
        isSceneLoaded = false;
        isNPCmove = false;
        if (npcMoveRoutine != null)
            StopCoroutine(npcMoveRoutine);
    }

    /// <summary>
    /// 判断当前场景NPC是否可视
    /// </summary>
    private void CheckVisiable()
    {
        if (currentScene == SceneManager.GetActiveScene().name)
            SetActiveInScene();
        else
            SetInactiveInScene();
    }

    /// <summary>
    /// NPC初始化（仅在第一次加载场景时调用）
    /// </summary>
    private void InitNPC()
    {
        targetScnene = currentScene;

        // 保持在当前坐标的网格中心
        currentGridPosition = grid.WorldToCell(transform.position);
        transform.position = new Vector3(currentGridPosition.x + Settings.gridCellSize * 0.5f, currentGridPosition.y + Settings.gridCellSize * 0.5f, 0);

        targetGridPosition = currentGridPosition;       // 确保人物不动
    }

    /// <summary>
    /// NPC移动
    /// </summary>
    private void Movement()
    {
        if (!isNPCmove)
        {
            if (movementSteps.Count > 0)
            {
                // 取出NPC时间表中的对应步骤
                MovementStep step = movementSteps.Pop();
                currentScene = step.sceneName;
                CheckVisiable();
                nextGridPosition = (Vector3Int)step.gridCoordinate;

                TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);

                MoveToGridPosition(nextGridPosition, stepTime);
            }
            else if(!isMoving && canPlayStopAnimation)
            {
                // 不在移动中 且 当前可以播放特定点动画
                StartCoroutine(SetStopAnimation());
            }
        }
    }

    /// <summary>
    /// NPC移动 单独创建一个函数来控制移动的协程，方便单独停止移动的协程
    /// </summary>
    /// <param name="gridPos">网格坐标</param>
    /// <param name="stepTime">时间戳</param>
    private void MoveToGridPosition(Vector3Int gridPos,TimeSpan stepTime)
    {
        npcMoveRoutine = StartCoroutine(MoveRoutine(gridPos, stepTime));
    }

    /// <summary>
    /// 移动协程
    /// </summary>
    /// <param name="gridPos">网格坐标</param>
    /// <param name="stepTime">时间戳</param>
    /// <returns></returns>
    private IEnumerator MoveRoutine(Vector3Int gridPos, TimeSpan stepTime)
    {
        isNPCmove = true;
        nextWorldPosition = GetWorldPosition(gridPos);

        // 判断是否还有时间移动（即将NPC时间表中的对应时间戳和当前的游戏时间进行对比）
        if (stepTime > GameTime)
        {
            // 还有时间移动
            // 用来移动的时间差，以秒为单位
            float timeToMove = (float)(stepTime.TotalSeconds - GameTime.TotalSeconds);

            // 实际移动距离
            float distance = Vector3.Distance(transform.position, nextWorldPosition);

            float speed = Mathf.Max(minSpeed, (distance / timeToMove / Settings.secondThreshold));  // 保证大于最小速度

            if (speed <= maxSpeed)
            {
                // 当前速度足够移动
                while (Vector3.Distance(transform.position, nextWorldPosition) > Settings.pixelSize)
                {
                    dir = (nextWorldPosition - transform.position).normalized;

                    // 实际移动
                    Vector2 posOffset = new Vector2(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime);
                    rb.MovePosition(rb.position + posOffset);
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        // 没有时间移动，那么就瞬移到对应位置
        rb.position = nextWorldPosition;
        currentGridPosition = gridPos;
        nextGridPosition = currentGridPosition;

        isNPCmove = false;
    }

    /// <summary>
    /// 跟据AStar生成路径
    /// </summary>
    /// <param name="scheduleDetails">NPC行动表</param>
    public void BuildPath(ScheduleDetails scheduleDetails)
    {
        movementSteps.Clear();
        currentSchedule = scheduleDetails;
        targetScnene = scheduleDetails.targetScene;

        // 动画相关
        targetGridPosition = (Vector3Int)scheduleDetails.targetGridPosition;
        stopAnimationClip = scheduleDetails.clipAtStop;

        this.interactable = scheduleDetails.interactable;           // 将ScheduleDetails中的interactable传进来

        if (scheduleDetails.targetScene == currentScene)
        {
            // 目标坐标在当前场景中
            AStar.Instance.BuildPath(scheduleDetails.targetScene, (Vector2Int)currentGridPosition, scheduleDetails.targetGridPosition, movementSteps);
        }
        else if (scheduleDetails.targetScene != currentScene)
        {
            // 跨场景移动
            SceneRoute sceneRoute = NPCManager.Instance.GetSceneRoute(currentScene, scheduleDetails.targetScene);
            if (sceneRoute != null)
            {
                for (int i = 0; i < sceneRoute.scenePathList.Count; i++)
                {
                    Vector2Int fromPos, goToPos;
                    ScenePath path = sceneRoute.scenePathList[i];

                    if (path.fromGridCell.x >= Settings.maxGridSize || path.fromGridCell.y >= Settings.maxGridSize)
                    {
                        // 当前还没有移动到目标坐标的场景
                        fromPos = (Vector2Int)currentGridPosition;
                    }
                    else
                    {
                        // 当前已经移动到目标坐标的场景
                        fromPos = path.fromGridCell;
                    }

                    if (path.goToGridCell.x >= Settings.maxGridSize || path.goToGridCell.y >= Settings.maxGridSize)
                    {
                        // 当前已经移动到目标坐标的场景
                        goToPos = scheduleDetails.targetGridPosition;
                    }
                    else
                    {
                        // 当前还没有移动到目标坐标的场景 
                        goToPos = path.goToGridCell;
                    }
                    AStar.Instance.BuildPath(path.sceneName, fromPos, goToPos, movementSteps);
                }
            }
        }


        if (movementSteps.Count > 1)
        {
            // 更新每一步对应的时间戳
            UpdateTimeOnPath();
        }
    }

    /// <summary>
    /// 生成NPC行动表的时间戳
    /// </summary>
    private void UpdateTimeOnPath()
    {
        MovementStep previousStep = null;

        TimeSpan currentGameTime = GameTime;

        foreach(MovementStep step in movementSteps)
        {
            // 第一格
            if(previousStep == null)
                previousStep = step;

            // 走过当前格子的时间
            step.hour = currentGameTime.Hours;
            step.minute = currentGameTime.Minutes;
            step.second = currentGameTime.Seconds;

            // 走过每一个格子所需要的时间长度
            TimeSpan gridMovementStepTime;
            if (MoveInDiagonal(step, previousStep))
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonaSize / normalSpeed / Settings.secondThreshold));
            else
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.secondThreshold));

            // 走过下一个格子的时间
            currentGameTime = currentGameTime.Add(gridMovementStepTime);    

            previousStep = step;    // 向前走一格
        }
    }

    /// <summary>
    /// 判断是否走斜方向
    /// </summary>
    /// <param name="currentStep">当前这次行动</param>
    /// <param name="previousStep">上一个行动</param>
    /// <returns>true代表是走斜方向</returns>
    private bool MoveInDiagonal(MovementStep currentStep, MovementStep previousStep)
    {
        // x和y坐标都不相同 才是斜方向移动
        return (currentStep.gridCoordinate.x != previousStep.gridCoordinate.x) && (currentStep.gridCoordinate.y != previousStep.gridCoordinate.y);
    }

    /// <summary>
    /// 网格坐标返回对应世界坐标的中心点
    /// </summary>
    /// <param name="gridPos">网格坐标</param>
    /// <returns>世界坐标的中心点</returns>
    private Vector3 GetWorldPosition(Vector3Int gridPos)
    {
        Vector3 worldPos = grid.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize * 0.5f, worldPos.y + Settings.gridCellSize * 0.5f);
    }

    /// <summary>
    /// 切换动画
    /// </summary>
    private void SwitchAnimation()
    {
        isMoving = transform.position != GetWorldPosition(targetGridPosition);
        animator.SetBool("isMoving", isMoving);
        if(isMoving)
        {
            animator.SetBool("Exit", true);
            animator.SetFloat("DirX", dir.x);
            animator.SetFloat("DirY", dir.y);
        }
        else
        {
            animator.SetBool("Exit", false);
        }
    }

    /// <summary>
    /// 播放停止时的特定动画
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetStopAnimation()
    {
        // 强制面向镜头
        animator.SetFloat("DirX", 0);
        animator.SetFloat("DirY", -1);

        animationBreakTime = Settings.animationBreakTime;   // 重置等待时间
        if (stopAnimationClip != null)
        {
            animOverride[blankAnimationClip] = stopAnimationClip;
            animator.SetBool("EventAnimation", true);
            yield return null;
            animator.SetBool("EventAnimation", false);
        }
        else
        {
            animOverride[stopAnimationClip] = blankAnimationClip;
            animator.SetBool("EventAnimation", false);
        }
    }

    #region 设置NPC显示情况
    private void SetActiveInScene()
    {
        spriteRenderer.enabled = true;
        coll.enabled = true;
        // 影子开启
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void SetInactiveInScene()
    {
        spriteRenderer.enabled = false;
        coll.enabled = false;
        // 影子关闭
        transform.GetChild(0).gameObject.SetActive(false);
    }
    #endregion

    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.characterPosDict = new Dictionary<string, SerializableVector3>();

        saveData.characterPosDict.Add("targetGridPosition", new SerializableVector3(targetGridPosition));

        saveData.characterPosDict.Add("currentPosition", new SerializableVector3(transform.position));

        saveData.dataSceneName = currentScene;

        saveData.targetScene = this.targetScnene;

        if (stopAnimationClip != null)
        {
            saveData.animationInstanceID = stopAnimationClip.GetInstanceID();
        }

        saveData.interactable = this.interactable;

        saveData.timeDict = new Dictionary<string, int>();
        saveData.timeDict.Add("currentSeason", (int)currentSeason);

        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        isInitialised = true;           // 读取存档时，人物是已经加载过的状态

        isFirstLoad = false;            // 上一次储存时，NPC在移动，读取存档后，需要重新构建路径

        Vector3Int gridPos = (Vector3Int)saveData.characterPosDict["targetGridPosition"].ToVector2Int();
        targetGridPosition = gridPos;

        Vector3 pos = saveData.characterPosDict["currentPosition"].ToVector3();
        transform.position = pos;

        currentScene = saveData.dataSceneName;

        targetScnene = saveData.targetScene;

        if (saveData.animationInstanceID != 0)
        {
            this.stopAnimationClip = Resources.InstanceIDToObject(saveData.animationInstanceID) as AnimationClip;
        }

        this.interactable = saveData.interactable;

        this.currentSeason = (Season)saveData.timeDict["currentSeason"];
    }
}
