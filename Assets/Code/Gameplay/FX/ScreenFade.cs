using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScreenFade : MonoBehaviour
{
    // Singleton instance
    public static ScreenFade Instance { get; private set; }

    public float fadeDuration = 1f;

    private Image fadeImage;
    private Color originalColor;
    private Color targetColor;
    private float currentAlpha;
    private float fadeStartTime;
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
        originalColor = fadeImage.color;
        currentAlpha = originalColor.a;
    }

    public static void StartFade()
    {
        Instance._StartFade();
    }

    private void _StartFade()
    {
        targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        fadeStartTime = Time.time;
        isFading = true;
    }

    private void Update()
    {
        if (isFading)
        {
            float elapsedTime = Time.time - fadeStartTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            currentAlpha = Mathf.Lerp(originalColor.a, targetColor.a, t);
            fadeImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlpha);

            if (t >= 1f)
            {
                isFading = false;
            }
        }
    }
}
