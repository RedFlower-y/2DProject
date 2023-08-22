using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.AStar;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class NPCMovement : MonoBehaviour
{
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

    private Stack<MovementStep> movementSteps;

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
        CheckVisiable();
    }

    private void CheckVisiable()
    {
        if (currentScene == SceneManager.GetActiveScene().name)
            SetActiveInScene();
        else
            SetInactiveInScene();
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
