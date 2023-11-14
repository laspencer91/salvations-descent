using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
* This class contains a singleton instance of the GameplayManager. The gameplay manager is responsible for functionality like Level Stat tracking, begin and end level procedures, etc.
**/
public class GameManager : MonoBehaviour, ITriggerListener
{
    public static GameManager Instance;

    [FoldoutGroup("Level Completion")] public Trigger ListenForOnLevelCompleteTrigger;
    [FoldoutGroup("Level Completion")] public float ScreenFadeDuration = 15f;
    [FoldoutGroup("Level Completion")] public float TimeAfterLevelEndToEnableSpacebar = 2f;
    [FoldoutGroup("Game Over")] public Trigger ListenForOnPlayerDiedTrigger;
    
    private LevelStats levelStats = new LevelStats();

    private GameState gameState = GameState.InProgress;


    private float levelCompleteTimestamp = 0;

    private float levelStartTimestamp = 0;

    private void Awake() 
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            Debug.LogWarning("GameplayManager already exists, a second instance cannot be created!");
            return;
        }

        Instance = this;
        levelStartTimestamp = Time.time;
    }

    private void Update() 
    {
        if (gameState == GameState.LevelComplete)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Time.time - levelCompleteTimestamp >= TimeAfterLevelEndToEnableSpacebar)
                    RestartScene();
            }
        } 
        else if (gameState == GameState.GameOver)
        {   
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartScene();
            }
        }
    }

    public void OnTrigger(string triggerName)
    {
        if (ListenForOnLevelCompleteTrigger.Is(triggerName))
        {
            ExecuteOnLevelCompleteProcedure();
        }

        if (ListenForOnPlayerDiedTrigger.Is(triggerName))
        {
            ExecuteGameOverProcedure();
        }
    }

    public static GameState GetGameState()
    {
        return Instance.gameState;
    }

    public static void RecordShotFired()
    {
        Instance.levelStats.ShotsFired += 1;
    }

    public static void RecordHit()
    {
        Instance.levelStats.ShotsOnTarget += 1;
    }

    public static void RecordHeadshot()
    {
        Instance.levelStats.Headshots += 1;
    }

    private void ExecuteOnLevelCompleteProcedure()
    {
        gameState = GameState.LevelComplete;
        ScreenFade.StartFade(ScreenFadeType.LevelComplete, ScreenFadeDuration);

        levelCompleteTimestamp = Time.time;
        Instance.levelStats.Time = levelCompleteTimestamp - levelStartTimestamp;    
        LevelCompleteStatsDisplay.BeginLevelEndProcedure(Instance.levelStats);
    }

    private void ExecuteGameOverProcedure()
    {
        gameState = GameState.GameOver;
        ScreenFade.StartFade(ScreenFadeType.GameOver, ScreenFadeDuration);
        GameOverScreenDisplay.BeginGameOverProcedure();
    }

    private void RestartScene()
    {
        // Get the index of the current active scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // Reload the current scene
        SceneManager.LoadScene(currentSceneIndex);
    }
}

public enum GameState
{
    InProgress,
    LevelComplete,
    GameOver
}

public class LevelStats
{
    public float Time;

    public int ShotsFired = 0;

    public int ShotsOnTarget = 0;

    public int Headshots = 0;
}