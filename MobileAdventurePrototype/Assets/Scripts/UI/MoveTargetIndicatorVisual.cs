using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTargetIndicatorVisual : MonoBehaviour
{
    public static MoveTargetIndicatorVisual current;

    SpriteRenderer spriteRenderer;

    public bool fadingOut = true;

    private const float fadeOutTime = 0.3f;
    private float fadeOutPerSecond;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        fadeOutPerSecond = 1.0f / fadeOutTime;
        //Destroy(gameObject, fadeOutTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (fadingOut)
        {
            FadeOut();
        }
    }

    public void FadeOut()
    {
        float newAlpha = spriteRenderer.color.a - (Time.deltaTime * fadeOutPerSecond);

        if (newAlpha > 0f)
        {
            spriteRenderer.color = new Color(
                spriteRenderer.color.r,
                spriteRenderer.color.g,
                spriteRenderer.color.b,
                newAlpha
                );
        }
        else
        {
            spriteRenderer.enabled = false;
            if (current == this) { current = null; }
            Destroy(gameObject);
        }
    }


}
