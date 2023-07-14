using UnityEngine;
using UnityEditor;
using BlazeAISpace;

[CanEditMultipleObjects]
[CustomEditor(typeof(BlazeAI))]

public class BlazeAIEditor : Editor
{
    string[] tabs = {"General", "States", "Vision", "Surprised", "Distract", "Hit", "Death", "Companion"};
    int tabSelected = 0;
    int tabIndex = -1;


    // variables
    SerializedProperty useRootMotion,
    centerPosition,
    showCenterPosition,
    groundLayers,

    audioScriptable,
    agentAudio,
    
    waypoints,
    vision,
    
    checkEnemyContact,
    enemyContactRadius,
    showEnemyContactRadius,
    
    useNormalStateOnAwake,
    normalStateBehaviour,

    useAlertStateOnAwake,
    alertStateBehaviour,

    attackStateBehaviour,
    coverShooterMode,
    coverShooterBehaviour,
    goingToCoverBehaviour,

    useSurprisedState,
    surprisedStateBehaviour,

    canDistract,
    distractedStateBehaviour,
    priorityLevel,
    turnAlertOnDistract,
    playDistractedAudios,

    useHitCooldown,
    maxHitCount,
    hitCooldown,
    hitStateBehaviour,
    
    deathAnim,
    deathAnimT,
    playDeathAudio,
    deathCallRadius,
    agentLayersToDeathCall,
    showDeathCallRadius,
    deathEvent,
    useRagdoll,
    useNaturalVelocity,
    hipBone,
    deathRagdollForce,
    destroyOnDeath,
    timeBeforeDestroy,

    friendly,

    distanceCull,
    animToPlayOnCull,

    ignoreUnreachableEnemy,
    fallBackPoints,
    showPoints,
    
    warnEmptyBehavioursOnStart,
    warnEmptyAnimations,

    companionMode,
    companionTo,
    companionBehaviour;

    BlazeAI script;


    #region UNITY METHODS

    void OnEnable()
    {
        if (EditorPrefs.HasKey("TabSelected")) {
            tabSelected = EditorPrefs.GetInt("TabSelected");
        }
        else {
            tabSelected = 0;
        }   


        useRootMotion = serializedObject.FindProperty("useRootMotion");
        centerPosition = serializedObject.FindProperty("centerPosition");
        showCenterPosition = serializedObject.FindProperty("showCenterPosition");
        groundLayers = serializedObject.FindProperty("groundLayers");


        audioScriptable = serializedObject.FindProperty("audioScriptable");
        agentAudio = serializedObject.FindProperty("agentAudio");

        
        waypoints = serializedObject.FindProperty("waypoints");
        vision = serializedObject.FindProperty("vision");


        checkEnemyContact = serializedObject.FindProperty("checkEnemyContact");
        enemyContactRadius = serializedObject.FindProperty("enemyContactRadius");
        showEnemyContactRadius = serializedObject.FindProperty("showEnemyContactRadius");


        distanceCull = serializedObject.FindProperty("distanceCull");
        animToPlayOnCull = serializedObject.FindProperty("animToPlayOnCull");


        friendly = serializedObject.FindProperty("friendly");


        ignoreUnreachableEnemy = serializedObject.FindProperty("ignoreUnreachableEnemy");
        fallBackPoints = serializedObject.FindProperty("fallBackPoints");
        showPoints = serializedObject.FindProperty("showPoints");


        warnEmptyBehavioursOnStart = serializedObject.FindProperty("warnEmptyBehavioursOnStart");
        warnEmptyAnimations = serializedObject.FindProperty("warnEmptyAnimations");


        // STATES TAB
        useNormalStateOnAwake = serializedObject.FindProperty("useNormalStateOnAwake");
        normalStateBehaviour = serializedObject.FindProperty("normalStateBehaviour");


        useAlertStateOnAwake = serializedObject.FindProperty("useAlertStateOnAwake");
        alertStateBehaviour = serializedObject.FindProperty("alertStateBehaviour");


        attackStateBehaviour = serializedObject.FindProperty("attackStateBehaviour");
        coverShooterMode = serializedObject.FindProperty("coverShooterMode");
        coverShooterBehaviour = serializedObject.FindProperty("coverShooterBehaviour");
        goingToCoverBehaviour = serializedObject.FindProperty("goingToCoverBehaviour");


        // SURPRISED TAB
        useSurprisedState = serializedObject.FindProperty("useSurprisedState");
        surprisedStateBehaviour = serializedObject.FindProperty("surprisedStateBehaviour");


        // DISTRACT TAB
        canDistract = serializedObject.FindProperty("canDistract");
        distractedStateBehaviour = serializedObject.FindProperty("distractedStateBehaviour");
        priorityLevel = serializedObject.FindProperty("priorityLevel");
        turnAlertOnDistract = serializedObject.FindProperty("turnAlertOnDistract");
        playDistractedAudios = serializedObject.FindProperty("playDistractedAudios");


        // HIT TAB
        useHitCooldown = serializedObject.FindProperty("useHitCooldown");
        maxHitCount = serializedObject.FindProperty("maxHitCount");
        hitCooldown = serializedObject.FindProperty("hitCooldown");
        hitStateBehaviour = serializedObject.FindProperty("hitStateBehaviour");


        // DEATH TAB
        deathAnim = serializedObject.FindProperty("deathAnim");
        deathAnimT = serializedObject.FindProperty("deathAnimT");
        playDeathAudio = serializedObject.FindProperty("playDeathAudio");
        deathCallRadius = serializedObject.FindProperty("deathCallRadius");
        agentLayersToDeathCall = serializedObject.FindProperty("agentLayersToDeathCall");
        showDeathCallRadius = serializedObject.FindProperty("showDeathCallRadius");
        deathEvent = serializedObject.FindProperty("deathEvent");
        useRagdoll = serializedObject.FindProperty("useRagdoll");
        useNaturalVelocity = serializedObject.FindProperty("useNaturalVelocity");
        hipBone = serializedObject.FindProperty("hipBone");
        deathRagdollForce = serializedObject.FindProperty("deathRagdollForce");
        destroyOnDeath = serializedObject.FindProperty("destroyOnDeath");
        timeBeforeDestroy = serializedObject.FindProperty("timeBeforeDestroy");


        // COMPANION TAB
        companionMode = serializedObject.FindProperty("companionMode");
        companionTo = serializedObject.FindProperty("companionTo");
        companionBehaviour = serializedObject.FindProperty("companionBehaviour");
    }

    protected virtual void OnSceneGUI()
    {
        if (script == null) {
            return;
        }

        if (script.waypoints.showWaypoints) {
            DrawWaypointHandles();
        }

        if (script.showPoints) {
            DrawFallBackPointsHandles();
        }
    }

    public override void OnInspectorGUI () 
    {
        var oldColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0.55f, 0.55f, 0.55f, 1f);
        
        StyleToolbar();
        EditorGUILayout.LabelField("Hover on any property below for insights", EditorStyles.helpBox);

        // reset the tabs
        tabIndex = -1;


        GUI.backgroundColor = oldColor;
        script = (BlazeAI)target;
        

        // tab selection
        switch (tabSelected)
        {
            case 0:
                GeneralTab(script);
                break;
            case 1:
                StatesTab(script);
                break;
            case 2:
                VisionTab();
                break;
            case 3:
                SurprisedTab(script);
                break;
            case 4:
                DistractionsTab(script);
                break;
            case 5:
                HitTab(script);
                break;
            case 6:
                DeathTab(script);
                break;
            case 7:
                CompanionTab(script);
                break;
        }


        EditorPrefs.SetInt("TabSelected", tabSelected);
        serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #region DRAWING INSPECTOR

    void StyleToolbar()
    {   
        // unselected btns style
        var unselectedStyle = new GUIStyle(GUI.skin.button);
        unselectedStyle.normal.textColor = Color.red;

        
        // selected btn style
        var selectedStyle = new GUIStyle();
        selectedStyle.normal.textColor = Color.white;
        selectedStyle.active.textColor = Color.white;
        selectedStyle.margin = new RectOffset(4,4,2,2);
        selectedStyle.alignment = TextAnchor.MiddleCenter;

        selectedStyle.normal.background = MakeTex(1, 1, new Color(1f, 0f, 0.1f, 0.8f));
        

        // draw the toolbar
        GUILayout.BeginHorizontal();
        
        foreach (var item in tabs) {
            tabIndex++;

            if (tabIndex == 4) {
                GUILayout.EndHorizontal();
                EditorGUILayout.Space(0.2f);
                GUILayout.BeginHorizontal();
            }

            if (tabIndex == tabSelected) {
                // selected button
                GUILayout.Button(item, selectedStyle, GUILayout.MinWidth(80), GUILayout.Height(40));
            }
            else {
                // unselected buttons
                if (GUILayout.Button(item, unselectedStyle, GUILayout.MinWidth(80), GUILayout.Height(40))) {
                    // this will trigger when a button is pressed
                    tabSelected = tabIndex;
                }
            }
        }

        GUILayout.EndHorizontal();
    }


    // render the general tab properties
    void GeneralTab(BlazeAI script)
    {
        EditorGUILayout.PropertyField(useRootMotion);
        EditorGUILayout.PropertyField(centerPosition);
        EditorGUILayout.PropertyField(showCenterPosition);
        EditorGUILayout.PropertyField(groundLayers);


        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(audioScriptable);
        EditorGUILayout.PropertyField(agentAudio);
        
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(waypoints);


        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(checkEnemyContact);
        if (script.checkEnemyContact) {
            EditorGUILayout.PropertyField(enemyContactRadius);
            EditorGUILayout.PropertyField(showEnemyContactRadius);
        }


        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(friendly);


        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(distanceCull);
        if (script.distanceCull) {
            EditorGUILayout.PropertyField(animToPlayOnCull);
        }
        

        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(ignoreUnreachableEnemy);
        if (script.ignoreUnreachableEnemy) {
            EditorGUILayout.PropertyField(fallBackPoints);
            EditorGUILayout.PropertyField(showPoints);
        }


        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(warnEmptyBehavioursOnStart);
        EditorGUILayout.PropertyField(warnEmptyAnimations);
        

        EditorGUILayout.Space(10);
    }


    // render the states classes
    void StatesTab(BlazeAI script)
    {
        EditorGUILayout.PropertyField(useNormalStateOnAwake);
        EditorGUILayout.PropertyField(normalStateBehaviour);

        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(useAlertStateOnAwake);
        EditorGUILayout.PropertyField(alertStateBehaviour);   


        EditorGUILayout.Space(18);
        EditorGUILayout.LabelField("ATTACK STATE", EditorStyles.boldLabel);
        EditorGUI.BeginDisabledGroup(script.coverShooterMode);
            EditorGUILayout.PropertyField(attackStateBehaviour);
        EditorGUI.EndDisabledGroup();

        
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(coverShooterMode);

        EditorGUI.BeginDisabledGroup(!script.coverShooterMode);
            EditorGUILayout.PropertyField(coverShooterBehaviour);
            EditorGUILayout.PropertyField(goingToCoverBehaviour);
        EditorGUI.EndDisabledGroup();


        EditorGUILayout.Space(10);
        if (GUILayout.Button("Add Behaviours", GUILayout.Height(40))) {
            script.SetPrimeBehaviours();
        }


        EditorGUILayout.Space(10);
    }


    void VisionTab()
    {
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(vision);
    }


    void SurprisedTab(BlazeAI script)
    {
        EditorGUILayout.LabelField("Surprised State triggers when the AI finds a target in Normal State.", EditorStyles.textField);
        EditorGUILayout.PropertyField(useSurprisedState);
        EditorGUILayout.PropertyField(surprisedStateBehaviour);

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Add Behaviour", GUILayout.Height(40))) {
            script.SetSurprisedBehaviour();
        }

        EditorGUILayout.Space(10);
    }


    // render the distractions tab class
    void DistractionsTab(BlazeAI script)
    {
        EditorGUILayout.PropertyField(canDistract);
        EditorGUILayout.PropertyField(distractedStateBehaviour);
        EditorGUILayout.PropertyField(priorityLevel);

        EditorGUILayout.PropertyField(turnAlertOnDistract);

        EditorGUILayout.PropertyField(playDistractedAudios);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Add Behaviour", GUILayout.Height(40))) {
            script.SetDistractedBehaviour();
        }
        
        EditorGUILayout.Space(10);
    }


    // render the hits tab class
    void HitTab(BlazeAI script)
    {
        EditorGUILayout.PropertyField(useHitCooldown);
        if (script.useHitCooldown) {
            EditorGUILayout.PropertyField(maxHitCount);
            EditorGUILayout.PropertyField(hitCooldown);
        }
        EditorGUILayout.Space(10);


        EditorGUILayout.PropertyField(hitStateBehaviour);
        EditorGUILayout.Space(10);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Add Behaviour", GUILayout.Height(40))) {
            script.SetHitBehaviour();
        }
    }


    // render the death tab class
    void DeathTab(BlazeAI script)
    {
        EditorGUILayout.PropertyField(deathAnim);
        EditorGUILayout.PropertyField(deathAnimT);
        EditorGUILayout.PropertyField(playDeathAudio);

    
        EditorGUILayout.PropertyField(deathCallRadius);
        EditorGUILayout.PropertyField(agentLayersToDeathCall);
        EditorGUILayout.PropertyField(showDeathCallRadius);


        EditorGUILayout.PropertyField(deathEvent);
        

        EditorGUILayout.PropertyField(useRagdoll);
        if (script.useRagdoll) {
            EditorGUILayout.PropertyField(useNaturalVelocity);

            if (!script.useNaturalVelocity) {
                EditorGUILayout.PropertyField(hipBone);
                EditorGUILayout.PropertyField(deathRagdollForce);
            }
        }


        EditorGUILayout.PropertyField(destroyOnDeath);
        if (script.destroyOnDeath) {
            EditorGUILayout.PropertyField(timeBeforeDestroy);
        }


        EditorGUILayout.Space(10);
    }


    // render the companion tab
    void CompanionTab(BlazeAI script)
    {
        EditorGUILayout.PropertyField(companionMode);
        EditorGUILayout.PropertyField(companionTo);
        EditorGUILayout.PropertyField(companionBehaviour);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Add Companion Behaviour", GUILayout.Height(40))) {
            script.SetCompanionBehaviour();
        }
    }


    // create texture for buttons
    Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        

        for (int i = 0; i < pix.Length; ++i) {
            pix[i] = col;
        }


        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();


        return result;
    }

    #endregion

    #region MISC
    
    void DrawWaypointHandles()
    {
        if (!script.waypoints.showWaypoints || script.waypoints.randomize) {
            return;
        }

        int max = script.waypoints.waypoints.Count;

        for (int i=0; i<max; i++) 
        {
            EditorGUI.BeginChangeCheck();

            Vector3 currentWaypoint = script.waypoints.waypoints[i];
            Vector3 newTargetPosition = Handles.PositionHandle(currentWaypoint, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(script, "Change Waypoint");
                script.waypoints.waypoints[i] = newTargetPosition;
            }
        }
    }

    void DrawFallBackPointsHandles()
    {
        if (!script.ignoreUnreachableEnemy) return;
        if (!script.showPoints) return;

        EditorGUI.BeginChangeCheck();

        Vector3 currentPoint;
        int max = script.fallBackPoints.Length;

        for (int i=0; i<max; i++) {
            currentPoint = script.fallBackPoints[i];
            Vector3 newTargetPosition = Handles.PositionHandle(currentPoint, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(script, "Change Fallback Point");
                script.fallBackPoints[i] = newTargetPosition;
            }
        }
    }

    #endregion
}
