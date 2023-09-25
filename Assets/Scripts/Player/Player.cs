using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Save;

public class Player : MonoBehaviour, ISaveable
{
    private Rigidbody2D rb;
    public float speed;
    private float inputX;
    private float inputY;
    private Vector2 movementInput;
    private Animator[] animators;   // ��ȡ����
    private bool isMoving;
    private bool inputDisable;      // �Ƿ��ܲٿ�

    // ����ʹ�ù���
    private float mouseX;
    private float mouseY;
    private bool useTool;

    public string GUID => GetComponent<DataGUID>().GUID;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();

        inputDisable = true;
    }

    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent  += OnAfterSceneLoadedEvent;
        EventHandler.MoveToPosition         += OnMoveToPosition;
        EventHandler.MouseClickedEvent      += OnMouseClickedEvent;
        EventHandler.UpdateGameStateEvent   += OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent      += OnStartNewGameEvent;
        EventHandler.EndGameEvent           += OnEndGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent  -= OnAfterSceneLoadedEvent;
        EventHandler.MoveToPosition         -= OnMoveToPosition;
        EventHandler.MouseClickedEvent      -= OnMouseClickedEvent;
        EventHandler.UpdateGameStateEvent   -= OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent      -= OnStartNewGameEvent;
        EventHandler.EndGameEvent           -= OnEndGameEvent;
    }


    private void Update()
    {
        if (!inputDisable)
            PlayerInput();
        else
            isMoving = false;
        SwitchAnimation();
    }

    private void FixedUpdate()
    {
        // ת������ʱ��ֹͣ�ƶ�
        if (!inputDisable)
            // �����˸��壬������Ҫ��FixedUpdate�е���
            Movement();
    }
    private void OnEndGameEvent()
    {
        inputDisable = true;
    }
    private void OnStartNewGameEvent(int index)
    {
        inputDisable = false;
        transform.position = Settings.playerStartPos;
    }

    private void OnUpdateGameStateEvent(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.GamePlay:
                inputDisable = false;
                break;
            case GameState.GamePause:
                inputDisable = true;
                break;
        }
    }

    private void OnMouseClickedEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        // ִ�ж���ʱ�Ķ���
        if(itemDetails.itemType != ItemType.Seed && itemDetails.itemType != ItemType.Commodity && itemDetails.itemType != ItemType.Furniture)
        {
            mouseX = mouseWorldPos.x - transform.position.x;
            mouseY = mouseWorldPos.y - (transform.position.y + Settings.playSize * 0.5f); // ����ʱ������λ����

            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
                mouseY = 0;     // X���ƫ�ƴ���Y���ƫ�ƣ����ж�����
            else
                mouseX = 0;     // Y���ƫ�ƴ���X���ƫ�ƣ����ж�����

            StartCoroutine(UseToolRoutine(mouseWorldPos, itemDetails));
        }
        else
        {
            EventHandler.CallExecuteActionAfterAnimationEvent(mouseWorldPos, itemDetails);
        }     
    }

    private IEnumerator UseToolRoutine(Vector3 mouseWorldPos,ItemDetails itemDetails)
    {
        useTool = true;
        inputDisable = true;
        yield return null;
        foreach(var anim in animators)
        {
            anim.SetTrigger("useTool");

            // ������泯����
            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);
        }
        yield return new WaitForSeconds(0.45f);         // ȡ���ڶ�����֡��
        EventHandler.CallExecuteActionAfterAnimationEvent(mouseWorldPos, itemDetails);
        yield return new WaitForSeconds(0.25f);

        useTool = false;
        inputDisable = false;
    }


    private void OnMoveToPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    private void OnAfterSceneLoadedEvent()
    {
        inputDisable = false;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        inputDisable = true;
    }

    

    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if(inputX != 0 && inputY != 0)
        {
            inputX = inputX * 0.6f;
            inputY = inputY * 0.6f;
        }

        // ��·״̬�ٶ�
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX = inputX * 0.5f;
            inputY = inputY * 0.5f;
        }
        movementInput = new Vector2(inputX, inputY);
        isMoving = movementInput != Vector2.zero;       // ���û�м������룬���ж�Ϊû���ƶ�
    }

    private void Movement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }    

    private void SwitchAnimation()
    {
        foreach ( var anim in animators )
        {
            anim.SetBool("IsMoving", isMoving);
            anim.SetFloat("mouseY", mouseY);
            anim.SetFloat("mouseX", mouseX);

            if(isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
        }
    }

    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.characterPosDict = new Dictionary<string, SerializableVector3>();
        saveData.characterPosDict.Add(this.name, new SerializableVector3(transform.position));

        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        var targetPositon = saveData.characterPosDict[this.name].ToVector3();
        transform.position = targetPositon;
    }
}
