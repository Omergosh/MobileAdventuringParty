using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    public Camera mainCamera;

    private PlayerInput playerInput;
    private PlayerControls playerControls;

    private InputAction touchPositionAction;
    public InputAction touchPressAction;
    public InputAction touchHoldAction;
    public InputAction touchPointMoveAction;

    public event EventHandler OnTouchPress;
    public event EventHandler OnTouchTap;
    public event EventHandler OnTouchHoldStart;
    public event EventHandler OnTouchHoldRelease;
    public event EventHandler OnSwipe;
    public Vector2 swipeStartPoint;
    public Vector2 swipeEndPoint;

    public static TouchManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;

        mainCamera = Camera.main;
        playerInput = GetComponent<PlayerInput>();
        playerControls = new PlayerControls();
        playerControls.Enable();

        touchPressAction = playerControls.BattleControls.TouchPress;
        touchPositionAction = playerControls.BattleControls.TouchPosition;
        touchHoldAction = playerControls.BattleControls.TouchHold;
        touchPointMoveAction= playerControls.BattleControls.TouchPointMove;
    }

    private void OnEnable()
    {
        touchPressAction.performed += TouchPressed;
        touchPressAction.canceled += TouchPressAction_canceled;
        touchHoldAction.performed += TouchHoldAction_performed;
        touchPointMoveAction.performed += TouchPointMove;
    }

    private void TouchPressAction_canceled(InputAction.CallbackContext context)
    {
        //Debug.Log($"duration: {context.duration}");
        Debug.Log("release");
    }

    private void TouchPointMove(InputAction.CallbackContext context)
    {
        //Debug.Log($"m o v e: {context.ReadValue<Vector2>()}");
    }

    private void TouchHoldAction_performed(InputAction.CallbackContext context)
    {
        Debug.Log("H O L D");
    }

    private void OnDisable()
    {
        touchPressAction.performed -= TouchPressed;
    }

    private void TouchPressed(InputAction.CallbackContext context)
    {
        // Detect if pressing on UI (guard clause?)

        // Otherwise, trigger event
        OnTouchPress?.Invoke(this, EventArgs.Empty);

        //context.ReadValueAsButton
        //Vector3 position = Camera.main.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>());
        //position.z = playerTransform.position.z;
        //playerTransform.position = position;
        //Debug.Log(position);
    }

    public Vector3 GetTouchPosition()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>());
        position.z = 0f;
        return position;
    }

    public Vector2 GetTouchPosition2D()
    {
        Vector2 position = Camera.main.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>());
        return position;
    }
}
