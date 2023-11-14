using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreenDisplay : ManagerBehavior<GameOverScreenDisplay>
{
    public float ScreenFadeDuration = 0.4f;

    private CanvasGroup canvasGroup;

    protected override void SingletonAwake() 
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public static void BeginGameOverProcedure()
    {
        Instance.StartCoroutine(Instance.FadeCanvasGroup(Instance.canvasGroup, 1, Instance.ScreenFadeDuration));
    }
    
    private System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float fadeDuration)
    {
        float startAlpha = canvasGroup.alpha;
        float timeElapsed = 0.0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        // Ensure the alpha value is set to the target value exactly when the fade is finished
        canvasGroup.alpha = targetAlpha;
    }  
}
