using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DashType
{
    ATTACK,
    DEFEND,
    SKILL,

}

public abstract class AdventurerScript : CreatureScript
{
    public DashType dashType;
    protected float dashCooldownMax = 2f; // time after dash before another dash can occur. AKA dash cooldown max
    protected float dashSpeed = 10f;
    protected float dashDuration = 0.4f; //dashTimeSpentAtMaxSpeed
    protected float dashTimeCurrent; //dashTimeSpentAtMaxSpeed

    // TEST CODE
    public float testTimeUntilDash = 1f;

    private void FixedUpdate()
    {
        DashUpdate();
        MoveTowardsUpdate();
        UpdateEndPhase();

        // TEST CODE
        if (BattleManager.Instance.battleState == BattleState.BATTLING
            && testTimeUntilDash > 0f)
        {
            testTimeUntilDash -= Time.deltaTime;
            if (testTimeUntilDash < 0)
            {
                Vector2 testDirection = RandomValues.RandomDirection2();
                Debug.Log(testDirection);
                Dash(testDirection);
            }
        }
    }

    protected virtual void DashUpdate()
    {
        if (currentState == CreatureAction.DASHING)
        {
            if (dashTimeCurrent > 0f)
            {
                dashTimeCurrent -= Time.deltaTime;
                //Debug.Log("still dashing");
            }
            else
            {
                Debug.Log("STOP dashing");
                ChangeState(CreatureAction.IDLE);
                dashTimeCurrent = 0f;
                velocity = Vector2.zero;
            }
        }
    }

    public virtual bool Dash(Vector2 dashDirection)
    {
        ChangeState(CreatureAction.DASHING);
        globalActionCooldownCurrent = dashCooldownMax;

        dashTimeCurrent = dashDuration;
        velocity = dashDirection.normalized * dashSpeed;

        return false;
    }
}
