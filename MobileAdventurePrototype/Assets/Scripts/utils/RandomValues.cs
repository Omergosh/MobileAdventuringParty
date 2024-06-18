using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomValues : MonoBehaviour
{
    public static Vector2 RandomDirection2()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
