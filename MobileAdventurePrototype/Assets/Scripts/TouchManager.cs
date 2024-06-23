using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    public Camera mainCamera;

    private PlayerInput playerInput;
    private PlayerControls playerControls;

    private InputAction touchContactAction;
    private InputAction touchPositionAction;
    public InputAction touchPressAction;
    public InputAction touchTapAction;
    public InputAction touchHoldAction;
    public InputAction touchPointMoveAction;

    public event EventHandler OnTouchStart;
    public event EventHandler OnTouchEnd;
    public event EventHandler OnSwipe;
    public event EventHandler OnTap;
    public event EventHandler OnHoldStart;
    //public event EventHandler OnHoldRelease;

    //public event EventHandler OnTouchPress;
    //public event EventHandler OnTouchTap;
    //public event EventHandler OnTouchHoldStart;
    //public event EventHandler OnTouchHoldRelease;

    public Vector2 swipeStartPointWorld;
    public Vector2 swipeEndPointWorld;
    public Vector2 swipeStartPointScreen;
    public Vector2 swipeEndPointScreen;
    [SerializeField] public Vector2 TouchMoveDelta { get; private set; }
    private float touchStartTime;
    private float touchEndTime;
    private float holdCheckTime;
    private bool touchStarted;
    private bool holdStarted;

    // config
    public const float holdTimeThreshold = 0.4f; // anything less is a tap or swipe
    public const float holdCancelDistanceFromStartThreshold = 0.2f;
    public const float minSwipeDistance = 200f;
    //public const float minSwipeDistanceWorld = 1.5f;
    public const float swipeCameraPercentageThreshold = 0.05f;
    public const float directionThreshold = 0.9f;

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
        //playerControls.Enable(); // (done in OnEnable)

        touchContactAction = playerControls.BattleControls.TouchContact;
        touchPositionAction = playerControls.BattleControls.TouchPosition;
        touchTapAction = playerControls.BattleControls.TouchTap;
        touchHoldAction = playerControls.BattleControls.TouchHold;
        touchPointMoveAction = playerControls.BattleControls.TouchPointMove;
        //touchPressAction = playerControls.BattleControls.TouchPress;
    }

    private void OnEnable()
    {
        playerControls.Enable();
        touchContactAction.started += TouchPrimaryStart;
        touchContactAction.canceled += TouchPrimaryEnd;
        touchTapAction.performed += TouchTapAction_performed;
        touchPointMoveAction.performed += TouchPointMove;
        touchHoldAction.performed += TouchHoldAction_performed;
        //touchPressAction.performed += TouchPressed;
        //touchPressAction.canceled += TouchPressAction_canceled;
    }

    private void OnDisable()
    {
        playerControls.Disable();
        touchContactAction.started -= TouchPrimaryStart;
        touchContactAction.canceled -= TouchPrimaryEnd;
        touchTapAction.performed -= TouchTapAction_performed;
        touchPointMoveAction.performed -= TouchPointMove;
        touchHoldAction.performed -= TouchHoldAction_performed;
        //touchPressAction.performed -= TouchPressed;
        //touchPressAction.canceled -= TouchPressAction_canceled;
    }

    private void TouchTapAction_performed(InputAction.CallbackContext obj)
    {
        OnTap?.Invoke(this, EventArgs.Empty);
    }

    private void TouchPrimaryStart(InputAction.CallbackContext context)
    {
        swipeStartPointScreen = touchPositionAction.ReadValue<Vector2>();
        swipeStartPointWorld = GetTouchPosition2D();
        TouchMoveDelta = Vector2.zero;
        touchStartTime = Time.time;
        touchStarted = true;
        holdStarted = false;
        holdCheckTime = 0f;
        OnTouchStart?.Invoke(this, EventArgs.Empty);
        //Debug.Log($"startpoint: {swipeStartPoint}");
    }

    private void TouchPrimaryEnd(InputAction.CallbackContext context)
    {
        swipeEndPointScreen = touchPositionAction.ReadValue<Vector2>();
        swipeEndPointWorld = GetTouchPosition2D();
        TouchMoveDelta = Vector2.zero;
        touchEndTime = Time.time;
        OnTouchEnd?.Invoke(this, EventArgs.Empty);

        if (touchEndTime - touchStartTime < holdTimeThreshold)
        {
            //Debug.Log(Vector2.Distance(swipeStartPointScreen, swipeEndPointScreen));
            //if (Vector2.Distance(swipeStartPointScreen, swipeEndPointScreen) >= minSwipeDistance)
            if (IsSwipeValid(swipeStartPointScreen, swipeEndPointScreen))
            {
                // Swipe detected!
                OnSwipe?.Invoke(this, EventArgs.Empty);
                Debug.Log("swipe");
            }
        }

        touchStarted = false;
        holdStarted = false;
        holdCheckTime = 0f;
    }

    private void TouchPointMove(InputAction.CallbackContext context)
    {
        TouchMoveDelta = context.ReadValue<Vector2>();
        //Debug.Log($"m o v e: {context.ReadValue<Vector2>()}");
    }

    //private void TouchPressAction_canceled(InputAction.CallbackContext context)
    //{
    //    //Debug.Log($"duration: {context.duration}");
    //    Debug.Log("release");
    //}

    private void TouchHoldAction_performed(InputAction.CallbackContext context)
    {
        Debug.Log("H O L D");
    }

    //private void TouchPressed(InputAction.CallbackContext context)
    //{
    //    // Detect if pressing on UI (guard clause?)

    //    // Otherwise, trigger event
    //    OnTouchPress?.Invoke(this, EventArgs.Empty);

    //    //context.ReadValueAsButton
    //    //Vector3 position = Camera.main.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>());
    //    //position.z = playerTransform.position.z;
    //    //playerTransform.position = position;
    //    //Debug.Log(position);
    //}

    public float GetCameraPercentageDistance(Vector2 start, Vector2 end)
    {
        // Used to test if a potential swipe is valid based on the distance traveled compared to current Camera size
        float cameraDiagonal = Mathf.Sqrt(Mathf.Pow(Camera.main.scaledPixelWidth, 2) + Mathf.Pow(Camera.main.scaledPixelHeight, 2));
        float screenDistancePixels = (end - start).magnitude;
        float screenDistancePercentage = screenDistancePixels / cameraDiagonal;
        //float screenDiffX = Mathf.Abs(start.x - end.x);
        //float screenDiffY = Mathf.Abs(start.y - end.y);

        // APPARENTLY not a reliable approach at all to try and get a physical screen size. Oh well.
        //float screenDistancePixels = (end - start).magnitude;
        //float screenDistancePhysical = screenDistancePixels / Screen.dpi;
        //return screenDistancePhysical;

        //Debug.Log(screenDistancePercentage);
        return screenDistancePercentage;
    }

    public bool IsSwipeValid(Vector2 start, Vector2 end)
    {
        // Used to test if a potential swipe is valid based on the distance traveled compared to current Camera size
        return GetCameraPercentageDistance(start, end) >= swipeCameraPercentageThreshold;
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

    public Vector2 GetSwipeDirection()
    {
        return (swipeEndPointWorld - swipeStartPointWorld).normalized;
    }

}
