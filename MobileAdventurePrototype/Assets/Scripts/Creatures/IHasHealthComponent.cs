using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



public interface IHasHealthComponent
{

    public event EventHandler OnHealthChanged;
    public event EventHandler OnDeath;

    public int Health { get; protected set; }

    public virtual bool TakeDamage(int damage)
    {
        Health -= damage;
        Die();
        return true;
    }

    public abstract void Die();


}
