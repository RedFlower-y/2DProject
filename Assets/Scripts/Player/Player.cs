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
    private Animator[] animators;   // 获取动画
    private bool isMoving;
    private bool inputDisable;      // 是否能操控

    // 动画使用工具
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
        // 转换场景时，停止移动
        if (!inputDisable)
            // 运用了刚体，所以需要在FixedUpdate中调用
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
        // 执行动作时的动画
        if(itemDetails.itemType != ItemType.Seed && itemDetails.itemType != ItemType.Commodity && itemDetails.itemType != ItemType.Furniture)
        {
            mouseX = mouseWorldPos.x - transform.position.x;
            mouseY = mouseWorldPos.y - (transform.position.y + Settings.playSize * 0.5f); // 砍树时动画错位修正

            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
                mouseY = 0;     // X轴的偏移大于Y轴的偏移，则判断左右
            else
                mouseX = 0;     // Y轴的偏移大于X轴的偏移，则判断上下

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

            // 人物的面朝方向
            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);
        }
        yield return new WaitForSeconds(0.45f);         // 取决于动画的帧率
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

        // 走路状态速度
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX = inputX * 0.5f;
            inputY = inputY * 0.5f;
        }
        movementInput = new Vector2(inputX, inputY);
        isMoving = movementInput != Vector2.zero;       // 如果没有键盘输入，则判断为没有移动
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
