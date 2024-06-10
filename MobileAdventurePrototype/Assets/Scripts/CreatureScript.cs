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

[RequireComponent(typeof(Collider2D))]
public abstract class CreatureScript : MonoBehaviour
{
    
    [SerializeField] private Collider2D hurtBox;

    public int Health { get; private set; }
    public float speed = 3f;

    public Vector2 velocity;
    public Vector2 Direction { get { return velocity.normalized; } }
    public Vector2 Position2D { get { return new Vector2(transform.position.x, transform.position.y); } }

    Vector2 targetPosition;
    bool movingTowardsTarget = false;

    private void FixedUpdate()
    {
        if (movingTowardsTarget)
        {
            MoveTowardsTargetPosition();
            if (Vector2.Distance(transform.position, targetPosition) < 0.05f)
            {
                movingTowardsTarget = false;
                velocity = Vector2.zero;
            }
        }

        Vector3 positionChange = new Vector3(velocity.x, velocity.y);
        //positionChange *= Time.deltaTime;
        transform.position += positionChange;
    }


    public void SetMoveTargetPosition(Vector2 moveTowardsPosition)
    {
        targetPosition = moveTowardsPosition;
        movingTowardsTarget = true;
        //Debug.Log($"target: {targetPosition.ToString()}");
    }

    public void MoveTowardsTargetPosition()
    {
        float distance = (targetPosition - Position2D).magnitude;
        velocity = (targetPosition - Position2D).normalized;
        velocity *= Time.deltaTime;

        if(distance < speed * Time.deltaTime)
        {
            velocity *= distance;
        }
        else
        {
            velocity *= speed;
        }
    }

    public virtual bool InflictDamage(int damage)
    {
        Health -= damage;
        return true;
    }

    public virtual bool InflictKnockback(Vector2 knockback)
    {
        velocity = knockback;
        return true;
    }

}
