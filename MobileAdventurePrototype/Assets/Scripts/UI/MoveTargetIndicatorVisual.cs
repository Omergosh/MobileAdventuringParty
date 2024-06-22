using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTargetIndicatorVisual : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    private const float fadeOutTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Destroy(gameObject, fadeOutTime);
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.color = new Color(
            spriteRenderer.color.r,
            spriteRenderer.color.g,
            spriteRenderer.color.b,
            spriteRenderer.color.a * (1f - (Time.deltaTime / fadeOutTime))
            );
    }
}
