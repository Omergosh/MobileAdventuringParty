using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTargetIndicatorVisual : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    private const float fadeOutTime = 0.3f;
    private float fadeOutPerSecond;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        fadeOutPerSecond = 1.0f / fadeOutTime;
        Destroy(gameObject, fadeOutTime);
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.color = new Color(
            spriteRenderer.color.r,
            spriteRenderer.color.g,
            spriteRenderer.color.b,
            spriteRenderer.color.a - (Time.deltaTime * fadeOutPerSecond)
            );
    }
}
