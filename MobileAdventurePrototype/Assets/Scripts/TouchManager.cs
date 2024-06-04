using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : MonoBehaviour
{
    [SerializeField] Transform playerTransform;

    private PlayerInput playerInput;
    private PlayerControls playerControls;

    private InputAction touchPositionAction;
    public InputAction touchPressAction;

    public static TouchManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;

        playerInput = GetComponent<PlayerInput>();
        
        playerControls = new PlayerControls();
        playerControls.Enable();

        touchPressAction = playerControls.BattleControls.TouchPress;
        touchPositionAction = playerControls.BattleControls.TouchPosition;
    }

    private void OnEnable()
    {
        touchPressAction.performed += TouchPressed;
    }

    private void OnDisable()
    {
        touchPressAction.performed -= TouchPressed;
    }

    private void TouchPressed(InputAction.CallbackContext context)
    {
        //context.ReadValueAsButton
        Vector3 position = Camera.main.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>());
        position.z = playerTransform.position.z;
        //position.z = 0f;
        playerTransform.position = position;
        Debug.Log(position);
    }
}
