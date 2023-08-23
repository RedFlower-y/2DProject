using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.AStar;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class NPCMovement : MonoBehaviour
{
    public ScheduleDataList_SO scheduleData;
    private SortedSet<ScheduleDetails> scheduleSet;
    private ScheduleDetails currentSchedule;

    // ��ʱ������Ϣ
    [SerializeField]private string currentScene;
    private string targetScnene;

    private Vector3Int  currentGridPosition;
    private Vector3Int  targetGridPosition;
    private Vector3Int  nextGridPosition;
    private Vector3     nextWorldPosition;

    public string StartScene { set { currentScene = value; } }

    [Header("�ƶ�����")]
    public float normalSpeed = 2f;
    private float minSpeed = 1f;
    private float maxSpeed = 3f;
    private Vector2 dir;
    public bool isMoving;

    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;
    private Animator animator;
    private Grid grid;

    private Stack<MovementStep> movementSteps;

    private bool isInitialised;             // ȷ��ֻ�ڵ�һ�μ��س�����ʱ���ʼ��NPC�ƶ�   
    private bool isNPCmove;
    private bool isSceneLoaded;             // �Ƿ�������˳���

    private TimeSpan GameTime => TimeManager.Instance.GameTime;

    private void Awake()
    {
        rb              = GetComponent<Rigidbody2D>();
        spriteRenderer  = GetComponent<SpriteRenderer>();
        coll            = GetComponent<BoxCollider2D>();
        animator        = GetComponent<Animator>();
        movementSteps   = new Stack<MovementStep>();
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent  += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent  -= OnAfterSceneLoadedEvent;
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
            isInitialised = true;       // �Ѿ���ʼ��NPC�ƶ��ˣ��´μ��س���NPC���ǰ���ʱ������ƶ���
        }

        isSceneLoaded = true;
    }

    /// <summary>
    /// �жϵ�ǰ����NPC�Ƿ����
    /// </summary>
    private void CheckVisiable()
    {
        if (currentScene == SceneManager.GetActiveScene().name)
            SetActiveInScene();
        else
            SetInactiveInScene();
    }

    /// <summary>
    /// NPC��ʼ�������ڵ�һ�μ��س���ʱ���ã�
    /// </summary>
    private void InitNPC()
    {
        targetScnene = currentScene;

        // �����ڵ�ǰ�������������
        currentGridPosition = grid.WorldToCell(transform.position);
        transform.position = new Vector3(currentGridPosition.x + Settings.gridCellSize * 0.5f, currentGridPosition.y + Settings.gridCellSize * 0.5f, 0);

        targetGridPosition = currentGridPosition;       // ȷ�����ﲻ��
    }

    /// <summary>
    /// NPC�ƶ�
    /// </summary>
    private void Movement()
    {
        if (!isNPCmove)
        {
            if (movementSteps.Count > 0)
            {
                // ȡ��NPCʱ����еĶ�Ӧ����
                MovementStep step = movementSteps.Pop();
                currentScene = step.sceneName;
                CheckVisiable();
                nextGridPosition = (Vector3Int)step.gridCoordinate;

                TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);

                MoveToGridPosition(nextGridPosition, stepTime);
            }
        }
    }

    /// <summary>
    /// NPC�ƶ� ��������һ�������������ƶ���Э�̣����㵥��ֹͣ�ƶ���Э��
    /// </summary>
    /// <param name="gridPos">��������</param>
    /// <param name="stepTime">ʱ���</param>
    private void MoveToGridPosition(Vector3Int gridPos,TimeSpan stepTime)
    {
        StartCoroutine(MoveRoutine(gridPos, stepTime));
    }

    /// <summary>
    /// �ƶ�Э��
    /// </summary>
    /// <param name="gridPos">��������</param>
    /// <param name="stepTime">ʱ���</param>
    /// <returns></returns>
    private IEnumerator MoveRoutine(Vector3Int gridPos, TimeSpan stepTime)
    {
        isNPCmove = true;
        nextWorldPosition = GetWorldPosition(gridPos);

        // �ж��Ƿ���ʱ���ƶ�������NPCʱ����еĶ�Ӧʱ����͵�ǰ����Ϸʱ����жԱȣ�
        if (stepTime > GameTime)
        {
            // ����ʱ���ƶ�
            // �����ƶ���ʱ������Ϊ��λ
            float timeToMove = (float)(stepTime.TotalSeconds - GameTime.TotalSeconds);

            // ʵ���ƶ�����
            float distance = Vector3.Distance(transform.position, nextWorldPosition);

            float speed = Mathf.Max(minSpeed, (distance / timeToMove / Settings.secondThreshold));  // ��֤������С�ٶ�

            if (speed <= maxSpeed)
            {
                // ��ǰ�ٶ��㹻�ƶ�
                while (Vector3.Distance(transform.position, nextWorldPosition) > Settings.pixelSize)
                {
                    dir = (nextWorldPosition - transform.position).normalized;

                    // ʵ���ƶ�
                    Vector2 posOffset = new Vector2(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime);
                    rb.MovePosition(rb.position + posOffset);
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        // û��ʱ���ƶ�����ô��˲�Ƶ���Ӧλ��
        rb.position = nextWorldPosition;
        currentGridPosition = gridPos;
        nextGridPosition = currentGridPosition;

        isNPCmove = false;
    }

    /// <summary>
    /// ����AStar����·��
    /// </summary>
    /// <param name="scheduleDetails">NPC�ж���</param>
    public void BuildPath(ScheduleDetails scheduleDetails)
    {
        movementSteps.Clear();
        currentSchedule = scheduleDetails;

        if(scheduleDetails.targetScene == currentScene)
        {
            AStar.Instance.BuildPath(scheduleDetails.targetScene, (Vector2Int)currentGridPosition, scheduleDetails.targetGridPosition, movementSteps);
        }
        // TODO:�糡���ƶ�

        if (movementSteps.Count > 1)
        {
            // ����ÿһ����Ӧ��ʱ���
            UpdateTimeOnPath();
        }
    }

    /// <summary>
    /// ����NPC�ж����ʱ���
    /// </summary>
    private void UpdateTimeOnPath()
    {
        MovementStep previousStep = null;

        TimeSpan currentGameTime = GameTime;

        foreach(MovementStep step in movementSteps)
        {
            // ��һ��
            if(previousStep == null)
                previousStep = step;

            // �߹���ǰ���ӵ�ʱ��
            step.hour = currentGameTime.Hours;
            step.minute = currentGameTime.Minutes;
            step.second = currentGameTime.Seconds;

            // �߹�ÿһ����������Ҫ��ʱ�䳤��
            TimeSpan gridMovementStepTime;
            if (MoveInDiagonal(step, previousStep))
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonaSize / normalSpeed / Settings.secondThreshold));
            else
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.secondThreshold));

            // �߹���һ�����ӵ�ʱ��
            currentGameTime = currentGameTime.Add(gridMovementStepTime);    

            previousStep = step;    // ��ǰ��һ��
        }
    }

    /// <summary>
    /// �ж��Ƿ���б����
    /// </summary>
    /// <param name="currentStep">��ǰ����ж�</param>
    /// <param name="previousStep">��һ���ж�</param>
    /// <returns>true��������б����</returns>
    private bool MoveInDiagonal(MovementStep currentStep, MovementStep previousStep)
    {
        // x��y���궼����ͬ ����б�����ƶ�
        return (currentStep.gridCoordinate.x != previousStep.gridCoordinate.x) && (currentStep.gridCoordinate.y != previousStep.gridCoordinate.y);
    }

    /// <summary>
    /// �������귵�ض�Ӧ������������ĵ�
    /// </summary>
    /// <param name="gridPos">��������</param>
    /// <returns>������������ĵ�</returns>
    private Vector3 GetWorldPosition(Vector3Int gridPos)
    {
        Vector3 worldPos = grid.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize * 0.5f, worldPos.y + Settings.gridCellSize * 0.5f);
    }

    #region ����NPC��ʾ���
    private void SetActiveInScene()
    {
        spriteRenderer.enabled = true;
        coll.enabled = true;
        // TODO:Ӱ�ӹر�
        //transform.GetChild(0).gameObject.SetActive(true);
    }

    private void SetInactiveInScene()
    {
        spriteRenderer.enabled = false;
        coll.enabled = false;
        // TODO:Ӱ�ӹر�
        //transform.GetChild(0).gameObject.SetActive(false);
    }
    #endregion
}
