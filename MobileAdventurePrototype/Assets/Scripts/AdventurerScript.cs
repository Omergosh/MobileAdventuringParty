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
}
