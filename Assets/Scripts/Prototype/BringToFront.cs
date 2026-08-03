using UnityEngine;

public class BringToFront : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;

    public int frontOrder = 10;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    private static int currentOrder = 10;

    private void OnMouseDown()
    {
        currentOrder++;

        spriteRenderer.sortingOrder = currentOrder;
    }

}