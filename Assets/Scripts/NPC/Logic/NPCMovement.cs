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

    private Vector3Int currentGridPosition;
    private Vector3Int targetGridPosition;

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

    private TimeSpan GameTime => TimeManager.Instance.GameTime;

    private void Awake()
    {
        rb              = GetComponent<Rigidbody2D>();
        spriteRenderer  = GetComponent<SpriteRenderer>();
        coll            = GetComponent<BoxCollider2D>();
        animator        = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
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
