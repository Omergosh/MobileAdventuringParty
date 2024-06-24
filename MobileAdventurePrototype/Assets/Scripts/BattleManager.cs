using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum BattleState
{
    WAITING_FOR_START,
    COUNTING_DOWN_TO_START,
    BATTLING,
    WAITING_FOR_NEXT_WAVE,
    COUNTING_DOWN_TO_NEXT_WAVE,
    ENDING,
    ENDED,
}

public enum ControlContextState
{
    NONE,
    MOVING,
    AIMING,
    MENU,
}

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public event EventHandler OnCountdownStart;
    public event EventHandler OnCountdownEnd;
    public event EventHandler<OnSelectedAdventurerChangedEventArgs> OnSelectedAdventurerChanged;
    public class OnSelectedAdventurerChangedEventArgs : EventArgs { public AdventurerScript adventurer; }
    public event EventHandler<OnCommandAdventurerMoveEventArgs> OnCommandAdventurerMove;
    public class OnCommandAdventurerMoveEventArgs : EventArgs {
        public AdventurerScript adventurer;
        public Vector2 targetPosition;
        // later refactor this with MoveToPositionCommand or something after Command pattern is implemented
    }

    private int lastHitboxID;

    public static int GetNewHitboxID()
    {
        Instance.lastHitboxID++;
        return Instance.lastHitboxID;
    }

    public List<AdventurerScript> adventurers;
    public List<EnemyScript> enemies;

    public BattleState battleState;
    public ControlContextState controlState;

    public float countdownTimer;
    public float countdownTimerMax = 3;

    public int waveNumber = 1;
    public float waveTimer;
    public float waveTimerMax = 5;

    public const string TAG_ADVENTURER = "Player";
    public const string TAG_ENEMY = "Enemy";

    public int selectedAdventurerIndex;
    [SerializeField] private LayerMask selectableFieldEntities;

    private void Awake()
    {
        Instance = this;
        battleState = BattleState.WAITING_FOR_START;
        controlState = ControlContextState.NONE;
    }

    private void OnEnable()
    {
        //TouchManager.Instance.OnTouchPress += ProcessTouchInputPress;
        TouchManager.Instance.OnTouchStart += TouchManager_OnTouchStart;
        TouchManager.Instance.OnTap += TouchManager_OnTap;
        TouchManager.Instance.OnHoldStart += TouchManager_OnHoldStart;
        TouchManager.Instance.OnTouchEnd += TouchManager_OnTouchEnd;
        TouchManager.Instance.OnSwipe += TouchManager_OnSwipe;

    }

    private void TouchManager_OnSwipe(object sender, EventArgs e)
    {
        // Player just swiped!
        switch (battleState)
        {
            case BattleState.BATTLING:
                // Selected adventurer attempts to dash
                Debug.Log("swipe adv");
                adventurers[selectedAdventurerIndex].TryDash(TouchManager.Instance.GetSwipeDirection());
                break;
        }
    }

    private void TouchManager_OnTouchEnd(object sender, EventArgs e)
    {

    }

    private void TouchManager_OnHoldStart(object sender, EventArgs e)
    {

    }

    private void TouchManager_OnTap(object sender, EventArgs e)
    {
        switch (battleState)
        {
            case BattleState.WAITING_FOR_START:
                StartBattle();
                break;
            case BattleState.BATTLING:
                // Select adventurer if they were tapped on!
                Vector3 newPosition = TouchManager.Instance.GetTouchPosition();
                Collider2D[] pressedAdventurers = Physics2D.OverlapPointAll(newPosition, selectableFieldEntities);
                pressedAdventurers = pressedAdventurers.Where(col => col.gameObject.GetComponent<AdventurerScript>() != null).ToArray();
                if (pressedAdventurers.Length > 0)
                {
                    Debug.Log("tapped adv");

                    pressedAdventurers = pressedAdventurers.OrderByDescending(
                        col => Vector2.Distance(newPosition, col.gameObject.transform.position)
                        )
                        .Reverse()
                        .ToArray();

                    // Attempt to select closest adventurer tapped on
                    selectedAdventurerIndex = adventurers.IndexOf(pressedAdventurers[0].gameObject.GetComponent<AdventurerScript>());
                    Debug.Log($"selected adventurer #{selectedAdventurerIndex}!");
                    OnSelectedAdventurerChanged?.Invoke(this, new OnSelectedAdventurerChangedEventArgs { adventurer = adventurers[selectedAdventurerIndex] });
                }
                else
                {
                    //// No new adventurer selected.
                    //// Move currently selected adventurer to target location!
                    //newPosition.z = adventurers[selectedAdventurerIndex].transform.position.z;
                    //adventurers[selectedAdventurerIndex].SetMoveTargetPosition(newPosition);
                    //OnCommandAdventurerMove?.Invoke(this, new OnCommandAdventurerMoveEventArgs
                    //{
                    //    adventurer = adventurers[selectedAdventurerIndex],
                    //    targetPosition = newPosition
                    //});
                    break;
                }
                break;
        }
    }

    private void TouchManager_OnTouchStart(object sender, EventArgs e)
    {
        // Player just started touching the screen
        switch (battleState)
        {
            case BattleState.WAITING_FOR_START:
                StartBattle();
                break;

            case BattleState.BATTLING:
                // Check if current touch started on top of the selected adventurer
                if (TouchOverEmptySpace())
                {
                    // Move currently selected adventurer to target location!
                    Vector3 newPosition = TouchManager.Instance.GetTouchPosition();
                    newPosition.z = adventurers[selectedAdventurerIndex].transform.position.z;
                    adventurers[selectedAdventurerIndex].SetMoveTargetPosition(newPosition);
                    OnCommandAdventurerMove?.Invoke(this, new OnCommandAdventurerMoveEventArgs
                    {
                        adventurer = adventurers[selectedAdventurerIndex],
                        targetPosition = newPosition
                    });
                }
                break;
        }
    }

    public bool TouchOverEmptySpace()
    {
        Vector3 touchPosition = TouchManager.Instance.GetTouchPosition();
        Collider2D[] pressedAdventurers = Physics2D.OverlapPointAll(touchPosition, selectableFieldEntities);
        return pressedAdventurers.Length == 0;
    }

    private void OnDisable()
    {
        //TouchManager.Instance.OnTouchPress -= ProcessTouchInputPress;
    }

    // Update is called once per frame
    private void Update()
    {
        switch (battleState)
        {
            case BattleState.WAITING_FOR_START:
                //if (Input.anyKeyDown)
                //{
                //    StartBattle();
                //}
                break;

            case BattleState.COUNTING_DOWN_TO_START:
                countdownTimer -= Time.deltaTime;
                if (countdownTimer <= 0f)
                {
                    battleState = BattleState.BATTLING;
                    OnCountdownEnd?.Invoke(this, EventArgs.Empty);
                }
                break;
            case BattleState.COUNTING_DOWN_TO_NEXT_WAVE:
                waveTimer -= Time.deltaTime;
                if (waveTimer <= 0f)
                {
                    battleState = BattleState.BATTLING;
                }
                break;
            case BattleState.BATTLING:
                break;
            default:
                Debug.LogError("Invalid/undefined BattleManager state");
                break;
        }
    }


    private void ProcessTouchInputPress(object sender, EventArgs e)
    {
        switch (battleState)
        {
            case BattleState.WAITING_FOR_START:
                StartBattle();
                break;
            case BattleState.BATTLING:
                Vector3 newPosition = TouchManager.Instance.GetTouchPosition();
                Collider2D[] pressedAdventurers = Physics2D.OverlapPointAll(newPosition, selectableFieldEntities);
                if (pressedAdventurers.Length > 0) { Debug.Log("tapped add"); }

                newPosition.z = adventurers[selectedAdventurerIndex].transform.position.z;
                //adventurers[selectedAdventurerIndex].transform.position = newPosition;
                adventurers[selectedAdventurerIndex].SetMoveTargetPosition(newPosition);
                break;
        }
    }

    public void StartBattle()
    {
        if (battleState == BattleState.WAITING_FOR_START)
        {
            countdownTimer = countdownTimerMax;
            battleState = BattleState.COUNTING_DOWN_TO_START;
            OnCountdownStart?.Invoke(this, EventArgs.Empty);
        }
    }

    public AdventurerScript GetSelectedAdventurer => adventurers[selectedAdventurerIndex];
    public bool IsSelectedAdventurer(AdventurerScript adventurer) => adventurers.Contains(adventurer);

}
