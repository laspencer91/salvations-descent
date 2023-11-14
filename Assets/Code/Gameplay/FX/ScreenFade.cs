using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScreenFade : MonoBehaviour
{
    // Singleton instance
    public static ScreenFade Instance { get; private set; }

    public Color LevelCompleteFadeColor;

    public Color GameOverFadeColor;

    private Image fadeImage;
    private Color targetColor;
    private float currentAlpha;
    private float fadeStartTime;
    private float fadeDuration = 1f;
    private bool isFading;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        fadeImage = GetComponent<Image>();
        currentAlpha = fadeImage.color.a;
    }

    [Button]
    public static void StartFade(ScreenFadeType fadeType, float fadeDuration = 1)
    {
        Instance._StartFade(fadeType, fadeDuration);
    }

    private void _StartFade(ScreenFadeType fadeType, float fadeDuration = 1)
    {
        switch (fadeType)
        {
            case ScreenFadeType.LevelComplete:
                targetColor = LevelCompleteFadeColor;
                break;
            case ScreenFadeType.GameOver:
                targetColor = GameOverFadeColor;
                break;
            case ScreenFadeType.None:
                targetColor = Color.clear;
                break;
        }
        this.fadeDuration = fadeDuration;
        fadeStartTime = Time.time;
        isFading = true;
    }

    private void Update()
    {
        if (isFading)
        {
            float elapsedTime = Time.time - fadeStartTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            Color currentColor = fadeImage.color;
            currentAlpha = Mathf.Lerp(currentColor.a, targetColor.a, t);
            fadeImage.color = new Color(targetColor.r, targetColor.g, targetColor.b, currentAlpha);

            if (t >= 1f)
            {
                isFading = false;
            }
        }
    }
}

public enum ScreenFadeType
{
    None,
    LevelComplete,
    GameOver
}