using _Systems.Audio;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup), typeof(TimelineBehavior))]
public class LevelCompleteStatsDisplay : ManagerBehavior<LevelCompleteStatsDisplay>
{
    [Required]
    public AudioEvent OnStatLabelShowAudioEvent;

    public TextMeshProUGUI TimeLabel;
    public TextMeshProUGUI ShotsFiredLabel;
    public TextMeshProUGUI AccuracyLabel;
    public TextMeshProUGUI HeadshotsLabel;

    public TextMeshProUGUI TimeCounter;
    public TextMeshProUGUI TotalShotsFiredCounter;
    public TextMeshProUGUI AccuracyCounter;
    public TextMeshProUGUI HeadshotsCounter;

    private CanvasGroup canvasGroup;

    private TimelineBehavior timelineBehavior;

    protected override void SingletonAwake() 
    {
        canvasGroup = GetComponent<CanvasGroup>();
        timelineBehavior = GetComponent<TimelineBehavior>();
    }

    public static void BeginLevelEndProcedure(LevelStats levelStats)
    {
        Instance.SetStats(levelStats);
        Instance.StartCoroutine(Instance.FadeCanvasGroup(Instance.canvasGroup, 1, .4f));
        Instance.timelineBehavior.Run();
    }

    public void SetStats(LevelStats levelStats)
    {
        TimeCounter.text = FormatTime(levelStats.Time);
        TotalShotsFiredCounter.text = levelStats.ShotsFired.ToString();
        AccuracyCounter.text = ((int)((float)levelStats.ShotsOnTarget / (float)levelStats.ShotsFired * 100)).ToString() + "%";
        HeadshotsCounter.text = levelStats.Headshots.ToString();
    }

    public void ShowTimeLabel() 
    {
        TimeLabel.gameObject.SetActive(true);
        OnStatLabelShowAudioEvent.Play2DSound();
    }

    public void ShowAccuracyLabel() 
    {
        AccuracyLabel.gameObject.SetActive(true);
        OnStatLabelShowAudioEvent.Play2DSound();
    }

    public void ShowShotsFiredLabel() 
    {
        ShotsFiredLabel.gameObject.SetActive(true);
        OnStatLabelShowAudioEvent.Play2DSound();
    }

    public void ShowHeadshotsLabel() 
    {
        HeadshotsLabel.gameObject.SetActive(true);
        OnStatLabelShowAudioEvent.Play2DSound();
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

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 1000) % 1000);

        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
