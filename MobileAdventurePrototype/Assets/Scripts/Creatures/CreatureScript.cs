using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum CreatureLivingStatus
{
    ALIVE,
    DYING,
    DEAD,
}

public enum CreatureObjective
{
    NOTHING,
    WANDER,
    FOLLOW_TARGET,
    ATTACK_TARGET,
}

public enum CreatureAction //or CreatureState
{
    IDLE,
    MOVING_TO_POSITION,
    MOVING_TO_TARGET,
    MELEE_ATTACKING,
    SHOOTING,
    STUNNED,
    DASHING,
    USING_SKILL,
}

public enum AutoAttackType
{
    MELEE,
    RANGED,
}

[RequireComponent(typeof(Collider2D))]
public abstract class CreatureScript : MonoBehaviour, IHasHealthComponent
{

    [SerializeField] private Collider2D hurtBox;
    protected Rigidbody2D rigidbody;

    public event EventHandler OnHealthChanged;
    public event EventHandler OnDeath;
    public event EventHandler OnAttackEnd;
    public event EventHandler OnReachMoveTarget;

    int IHasHealthComponent.Health
    {
        get { return healthCurrent; }
        set { healthCurrent = value; }
    }

    protected int healthCurrent = 10;
    protected int healthMax = 10;

    public float moveSpeed = 3f;
    public float attackSpeed = 1f; // seconds between each attack

    public float globalActionCooldownCurrent;
    public float attackCooldownCurrent;

    public Vector2 Direction
    {
        get
        {
            if (velocity == Vector2.zero) { return lastNonZeroDirection; }
            return velocity.normalized;
        }
    }
    private Vector2 lastNonZeroDirection = Vector2.right;

    public Vector2 Position2D { get { return new Vector2(transform.position.x, transform.position.y); } }
    public Vector2 velocity;

    public CreatureLivingStatus currentLivingStatus = CreatureLivingStatus.ALIVE;
    public CreatureAction currentState = CreatureAction.IDLE;

    Vector2 targetPosition;
    bool movingTowardsTarget = false;
    Transform targetToFollow;
    IHasHealthComponent targetToAttack;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        MoveTowardsUpdate();
        UpdateEndPhase();
    }

    protected void MoveTowardsUpdate()
    {
        if (movingTowardsTarget && currentState == CreatureAction.MOVING_TO_POSITION)
        {
            MoveTowardsTargetPosition();
            if (Vector2.Distance(transform.position, targetPosition) < 0.05f)
            {
                movingTowardsTarget = false;
                velocity = Vector2.zero;
                OnReachMoveTarget?.Invoke(this, EventArgs.Empty);
                // FOR NOW:
                ChangeState(CreatureAction.IDLE);
            }
        }

    }

    protected void UpdateEndPhase()
    {
        //Vector3 positionChange = new Vector3(velocity.x, velocity.y) * Time.deltaTime;
        //transform.position += positionChange;
        rigidbody.velocity = velocity;
        if (velocity != Vector2.zero) { lastNonZeroDirection = Direction; }

        if (globalActionCooldownCurrent > 0f) { globalActionCooldownCurrent -= Time.deltaTime; }
        if (attackCooldownCurrent > 0f) { attackCooldownCurrent -= Time.deltaTime; }
    }

    public void ChangeState(CreatureAction newState)
    {
        currentState = newState;
    }


    public void SetMoveTargetPosition(Vector2 moveTowardsPosition)
    {
        targetPosition = moveTowardsPosition;
        movingTowardsTarget = true;
        ChangeState(CreatureAction.MOVING_TO_POSITION);
        //Debug.Log($"target: {targetPosition.ToString()}");
    }

    public void MoveTowardsTargetPosition()
    {
        float distance = (targetPosition - Position2D).magnitude;
        velocity = (targetPosition - Position2D).normalized;

        if (distance < moveSpeed * Time.deltaTime)
        {
            velocity *= distance;
            movingTowardsTarget = false;
            OnReachMoveTarget?.Invoke(this, EventArgs.Empty);
            // FOR NOW:
            ChangeState(CreatureAction.IDLE);
        }
        else
        {
            velocity *= moveSpeed;
        }
    }

    public virtual bool InflictDamage(IHasHealthComponent target, int damage)
    {
        Debug.Log($"before: {target.Health}");
        target.TakeDamage(damage);
        Debug.Log($"after: {target.Health}");
        return true;
    }

    public virtual bool InflictKnockback(Vector2 knockback)
    {
        velocity = knockback;
        return true;
    }

    public void Die()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
        Destroy(this, 0.1f);
    }

}
