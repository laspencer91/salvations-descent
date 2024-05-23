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
    
    private LevelStats _levelStats = new LevelStats();

    private GameState _gameState = GameState.InProgress;


    private float _levelCompleteTimestamp = 0;

    private float _levelStartTimestamp = 0;

    private void Awake() 
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            Debug.LogWarning("GameplayManager already exists, a second instance cannot be created!");
            return;
        }

        Instance = this;
        _levelStartTimestamp = Time.time;
    }

    private void Update() 
    {
        if (_gameState == GameState.LevelComplete)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Time.time - _levelCompleteTimestamp >= TimeAfterLevelEndToEnableSpacebar)
                    RestartScene();
            }
        } 
        else if (_gameState == GameState.GameOver)
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
        return Instance._gameState;
    }

    public static void RecordShotFired()
    {
        Instance._levelStats.ShotsFired += 1;
    }

    public static void RecordHit()
    {
        Instance._levelStats.ShotsOnTarget += 1;
    }

    public static void RecordHeadshot()
    {
        Instance._levelStats.Headshots += 1;
    }

    private void ExecuteOnLevelCompleteProcedure()
    {
        _gameState = GameState.LevelComplete;
        ScreenFade.StartFade(ScreenFadeType.LevelComplete, ScreenFadeDuration);

        _levelCompleteTimestamp = Time.time;
        Instance._levelStats.Time = _levelCompleteTimestamp - _levelStartTimestamp;    
        LevelCompleteStatsDisplay.BeginLevelEndProcedure(Instance._levelStats);
    }

    private void ExecuteGameOverProcedure()
    {
        _gameState = GameState.GameOver;
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