using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public BattleState state;

    public float countdownTimer;
    public float countdownTimerMax = 3;

    public int waveNumber = 1;
    public float waveTimer;
    public float waveTimerMax = 5;

    private void Awake()
    {
        Instance = this;
        state = BattleState.WAITING_FOR_START;
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        switch (state)
        {
            case BattleState.WAITING_FOR_START:
                if (Input.anyKeyDown)
                {
                    StartBattle();
                }
                break;

            case BattleState.COUNTING_DOWN_TO_START:
                countdownTimer -= Time.deltaTime;
                if(countdownTimer <= 0f)
                {
                    state = BattleState.BATTLING;
                }
                break;
            case BattleState.COUNTING_DOWN_TO_NEXT_WAVE:
                waveTimer -= Time.deltaTime;
                if(waveTimer <= 0f)
                {
                    state = BattleState.BATTLING;
                }
                break;
            default:
                Debug.LogError("Invalid/undefined BattleManager state");
                break;
        }
    }

    public void StartBattle()
    {
        if(state == BattleState.WAITING_FOR_START)
        {
            countdownTimer = countdownTimerMax;
            state = BattleState.COUNTING_DOWN_TO_START;
        }
    }
}
