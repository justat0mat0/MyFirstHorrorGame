using UnityEngine;
using DG.Tweening;


public class FloatArrowHorizontal : MonoBehaviour
{

    [Header("Horizontal Float")]
    public float distance = 0.15f;
    public float duration = 1.2f;


    [Header("Alpha Float")]
    [Range(0f, 1f)]
    public float maxAlpha = 1f;

    [Range(0f, 1f)]
    public float minAlpha = 0.5f;



    private Vector3 startPos;
    private SpriteRenderer spriteRenderer;



    void Start()
    {
        startPos = transform.position;

        spriteRenderer = GetComponent<SpriteRenderer>();


        // ³õÊ¼Í¸Ã÷¶È
        Color c = spriteRenderer.color;
        c.a = maxAlpha;
        spriteRenderer.color = c;



        // ×óÓÒÆ¯¸¡
        transform.DOMoveX(
            startPos.x + distance,
            duration
        )
        .SetEase(Ease.InOutSine)
        .SetLoops(-1, LoopType.Yoyo);



        // Í¸Ã÷¶ÈºôÎü
        spriteRenderer
            .DOFade(
                minAlpha,
                duration
            )
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

    }

}