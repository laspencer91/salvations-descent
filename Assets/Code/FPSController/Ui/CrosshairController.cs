using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CrosshairController : ManagerBehavior<CrosshairController>
{
    public float ReturnToScaleSpeed = 100f;

    private RectTransform rectTransform;

    private Vector2 originalSize;

    protected override void SingletonAwake()
    {
        rectTransform = GetComponent<RectTransform>();
        Instance.originalSize = Instance.rectTransform.sizeDelta;
    }

    private void Update() 
    {
        if (rectTransform.sizeDelta != Instance.originalSize)
        {
            rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, Instance.originalSize, ReturnToScaleSpeed * Time.deltaTime);
        }
    }

    public static void SetCrosshairScale(float scale)
    {
        Instance.rectTransform.sizeDelta = Instance.originalSize * scale;
    }
}
