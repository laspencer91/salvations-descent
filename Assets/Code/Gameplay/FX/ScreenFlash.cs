using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScreenFlash : MonoBehaviour
{
    // Singleton instance
    public static ScreenFlash Instance { get; private set; }

    public Color TakeDamageColor;

    public Color ItemPickupColor;

    private Image flashImage;

    private Color originalColor;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        flashImage = GetComponent<Image>();

        Instance = this;
        originalColor = flashImage.color;
    }

    public static void FlashScreen(FlashType flashType, float flashDuration = 0.05f)
    {
        ScreenFlash.Instance._FlashScreen(flashType, flashDuration);
    }

    private void _FlashScreen(FlashType flashType, float flashDuration, bool smoothFade = true)
    {
        Color flashColor = Color.white;
        switch (flashType)
        {
            case FlashType.Damage:
            flashColor = TakeDamageColor;
            break;
            case FlashType.ItemPickup:
            flashColor = ItemPickupColor;
            break;
        }

        if (smoothFade)
        {
            StartCoroutine(SmoothFlashRoutine(flashColor, flashDuration));       
        }
        else
        {
            StartCoroutine(FlashRoutine(flashColor, flashDuration));           
        }
    }

    private IEnumerator FlashRoutine(Color flashColor, float flashDuration)
    {
        flashImage.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        flashImage.color = originalColor;
    }

    private IEnumerator SmoothFlashRoutine(Color flashColor, float flashDuration)
{
    Color startColor = flashImage.color;
    Color endColor = flashColor;

    float elapsedTime = 0f;
    while (elapsedTime < flashDuration)
    {
        flashImage.color = Color.Lerp(startColor, endColor, elapsedTime / flashDuration);

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    flashImage.color = originalColor;
}
}

public enum FlashType
{
    Damage,
    ItemPickup
}
