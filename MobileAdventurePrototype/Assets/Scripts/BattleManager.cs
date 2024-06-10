using System;
using System.Collections;
using System.Collections.Generic;
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
    }

    private void OnEnable()
    {
        TouchManager.Instance.OnTouchPress += ProcessTouchInputPress;
    }

    private void OnDisable()
    {
        TouchManager.Instance.OnTouchPress -= ProcessTouchInputPress;
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
                if(countdownTimer <= 0f)
                {
                    battleState = BattleState.BATTLING;
                }
                break;
            case BattleState.COUNTING_DOWN_TO_NEXT_WAVE:
                waveTimer -= Time.deltaTime;
                if(waveTimer <= 0f)
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
        if(battleState == BattleState.WAITING_FOR_START)
        {
            countdownTimer = countdownTimerMax;
            battleState = BattleState.COUNTING_DOWN_TO_START;
        }
    }
}
