using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void MoveTowards(Vector2 moveTowardsPosition)
    {
        float distance = (moveTowardsPosition - Position2D).magnitude;
        velocity = (moveTowardsPosition - Position2D).normalized;

        if(distance < speed)
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
