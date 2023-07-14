using UnityEditor;
using UnityEngine;

namespace BlazeAISpace 
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CoverShooterBehaviour))]
    public class CoverShooterBehaviourInspector : Editor
    {
        SerializedProperty moveSpeed,
        turnSpeed,
        idleAnim,
        moveAnim,
        idleMoveT,

        distanceFromEnemy,
        attackDistance,
        layersCheckOnAttacking,
        shootingAnim,
        shootingAnimT,
        shootEvent,
        shootEvery,
        singleShotDuration,
        delayBetweenEachShot,
        totalShootTime,

        firstSightDecision,
        coverBlownDecision,
        attackEnemyCover,

        braveMeter,
        changeCoverFrequency,

        callOthers,
        callRadius,
        showCallRadius,
        agentLayersToCall,
        callOthersTime,
        receiveCallFromOthers,

        moveBackwards,
        moveBackwardsDistance,
        moveBackwardsSpeed,
        moveBackwardsAnim,
        moveBackwardsAnimT,
        moveBackwardsAttack,

        strafe,
        strafeSpeed,
        strafeTime,
        strafeWaitTime,
        leftStrafeAnim,
        rightStrafeAnim,
        strafeAnimT,
        strafeLayersToAvoid,

        searchLocationRadius,
        timeToStartSearch,
        searchPoints,
        searchPointAnim,
        pointWaitTime,
        endSearchAnim,
        endSearchAnimTime,
        searchAnimsT,
        playAudioOnSearchStart,
        playAudioOnSearchEnd,

        returnPatrolAnim,
        returnPatrolAnimT,
        returnPatrolTime,
        playAudioOnReturnPatrol,

        playAudioOnChase,
        alwaysPlayOnChase,

        playAudioDuringShooting,
        alwaysPlayDuringShooting,

        playAudioOnMoveToShoot,
        alwaysPlayOnMoveToShoot;


        bool displayshootEvents = true;
        int spaceBetween = 20;

        string[] tabs = {"Movement", "Attack", "Attack-Idle", "Call Others", "Search & Return", "Audios"};
        int tabSelected = 0;
        int tabIndex = -1;


        void OnEnable()
        {
            GetSelectedTab(); 


            moveSpeed = serializedObject.FindProperty("moveSpeed");
            turnSpeed = serializedObject.FindProperty("turnSpeed");
            idleAnim = serializedObject.FindProperty("idleAnim");
            moveAnim = serializedObject.FindProperty("moveAnim");
            idleMoveT = serializedObject.FindProperty("idleMoveT");


            distanceFromEnemy = serializedObject.FindProperty("distanceFromEnemy");
            attackDistance = serializedObject.FindProperty("attackDistance");
            layersCheckOnAttacking = serializedObject.FindProperty("layersCheckOnAttacking");
            shootingAnim = serializedObject.FindProperty("shootingAnim");
            shootingAnimT = serializedObject.FindProperty("shootingAnimT");
            shootEvent = serializedObject.FindProperty("shootEvent");
            shootEvery = serializedObject.FindProperty("shootEvery");
            singleShotDuration = serializedObject.FindProperty("singleShotDuration");
            delayBetweenEachShot = serializedObject.FindProperty("delayBetweenEachShot");
            totalShootTime = serializedObject.FindProperty("totalShootTime");


            firstSightDecision = serializedObject.FindProperty("firstSightDecision");
            coverBlownDecision = serializedObject.FindProperty("coverBlownDecision");
            attackEnemyCover = serializedObject.FindProperty("attackEnemyCover");


            braveMeter = serializedObject.FindProperty("braveMeter");
            changeCoverFrequency = serializedObject.FindProperty("changeCoverFrequency");


            callOthers = serializedObject.FindProperty("callOthers");
            callRadius = serializedObject.FindProperty("callRadius");
            showCallRadius = serializedObject.FindProperty("showCallRadius");
            agentLayersToCall = serializedObject.FindProperty("agentLayersToCall");
            callOthersTime = serializedObject.FindProperty("callOthersTime");
            receiveCallFromOthers = serializedObject.FindProperty("receiveCallFromOthers");


            moveBackwards = serializedObject.FindProperty("moveBackwards");
            moveBackwardsDistance = serializedObject.FindProperty("moveBackwardsDistance");
            moveBackwardsSpeed = serializedObject.FindProperty("moveBackwardsSpeed");
            moveBackwardsAnim = serializedObject.FindProperty("moveBackwardsAnim");
            moveBackwardsAnimT = serializedObject.FindProperty("moveBackwardsAnimT");
            moveBackwardsAttack = serializedObject.FindProperty("moveBackwardsAttack");


            strafe = serializedObject.FindProperty("strafe");
            strafeSpeed = serializedObject.FindProperty("strafeSpeed");
            strafeTime = serializedObject.FindProperty("strafeTime");
            strafeWaitTime = serializedObject.FindProperty("strafeWaitTime");
            leftStrafeAnim = serializedObject.FindProperty("leftStrafeAnim");
            rightStrafeAnim = serializedObject.FindProperty("rightStrafeAnim");
            strafeAnimT = serializedObject.FindProperty("strafeAnimT");
            strafeLayersToAvoid = serializedObject.FindProperty("strafeLayersToAvoid");


            searchLocationRadius = serializedObject.FindProperty("searchLocationRadius");
            timeToStartSearch = serializedObject.FindProperty("timeToStartSearch");
            searchPoints = serializedObject.FindProperty("searchPoints");
            searchPointAnim = serializedObject.FindProperty("searchPointAnim");
            pointWaitTime = serializedObject.FindProperty("pointWaitTime");
            endSearchAnim = serializedObject.FindProperty("endSearchAnim");
            endSearchAnimTime = serializedObject.FindProperty("endSearchAnimTime");
            searchAnimsT = serializedObject.FindProperty("searchAnimsT");
            playAudioOnSearchStart = serializedObject.FindProperty("playAudioOnSearchStart");
            playAudioOnSearchEnd = serializedObject.FindProperty("playAudioOnSearchEnd");


            returnPatrolAnim = serializedObject.FindProperty("returnPatrolAnim");
            returnPatrolAnimT = serializedObject.FindProperty("returnPatrolAnimT");
            returnPatrolTime = serializedObject.FindProperty("returnPatrolTime");
            playAudioOnReturnPatrol = serializedObject.FindProperty("playAudioOnReturnPatrol");


            playAudioOnChase = serializedObject.FindProperty("playAudioOnChase");
            alwaysPlayOnChase = serializedObject.FindProperty("alwaysPlayOnChase");

            playAudioDuringShooting = serializedObject.FindProperty("playAudioDuringShooting");
            alwaysPlayDuringShooting = serializedObject.FindProperty("alwaysPlayDuringShooting");

            playAudioOnMoveToShoot = serializedObject.FindProperty("playAudioOnMoveToShoot");
            alwaysPlayOnMoveToShoot = serializedObject.FindProperty("alwaysPlayOnMoveToShoot");
        }
        
        public override void OnInspectorGUI () 
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.55f, 0.55f, 0.55f, 1f);
            
            DrawToolbar();
            EditorGUILayout.Space(10);
            
            GUI.backgroundColor = oldColor;
            CoverShooterBehaviour script = (CoverShooterBehaviour) target;

            tabIndex = -1;

            switch (tabSelected)
            {
                case 0:
                    DrawMovementTab();
                    break;
                case 1:
                    DrawAttackTab();
                    break;
                case 2:
                    DrawAttackIdleTab(script);
                    break;
                case 3:
                    DrawCallOthersTab(script);
                    break;
                case 4:
                    DrawSearchAndReturnTab(script);
                    break;
                case 5:
                    DrawAudiosTab(script);
                    break;
            }
            

            EditorPrefs.SetInt("ShooterTabSelected", tabSelected);
            serializedObject.ApplyModifiedProperties();
        }

        #region DRAWING

        void DrawToolbar()
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

                if (tabIndex == 3) {
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space(0.2f);
                    GUILayout.BeginHorizontal();
                }

                if (tabIndex == tabSelected) {
                    // selected button
                    GUILayout.Button(item, selectedStyle, GUILayout.MinWidth(105), GUILayout.Height(40));
                }
                else {
                    // unselected buttons
                    if (GUILayout.Button(item, unselectedStyle, GUILayout.MinWidth(105), GUILayout.Height(40))) {
                        // this will trigger when a button is pressed
                        tabSelected = tabIndex;
                    }
                }
            }

            GUILayout.EndHorizontal();
        }

        void DrawMovementTab()
        {
            EditorGUILayout.LabelField("SPEEDS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(moveSpeed);
            EditorGUILayout.PropertyField(turnSpeed);

            EditorGUILayout.Space(spaceBetween);

            EditorGUILayout.LabelField("ANIMATIONS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(idleAnim);
            EditorGUILayout.PropertyField(moveAnim);
            
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(idleMoveT);
        }

        void DrawAttackTab()
        {
            EditorGUILayout.LabelField("DISTANCES", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(distanceFromEnemy);
            EditorGUILayout.PropertyField(attackDistance);
            EditorGUILayout.Space(spaceBetween);

            EditorGUILayout.LabelField("FRIENDLY-FIRE LAYERS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(layersCheckOnAttacking);
            EditorGUILayout.Space(spaceBetween);

            EditorGUILayout.LabelField("SHOOT ANIMATION", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(shootingAnim);
            EditorGUILayout.PropertyField(shootingAnimT);
            EditorGUILayout.Space(spaceBetween);

            EditorGUILayout.LabelField("TIMING", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(shootEvery);
            EditorGUILayout.PropertyField(singleShotDuration);
            EditorGUILayout.PropertyField(delayBetweenEachShot);
            EditorGUILayout.PropertyField(totalShootTime);
            EditorGUILayout.Space(spaceBetween);

            displayshootEvents = EditorGUILayout.Toggle("Display Attack Events", displayshootEvents);
            if (displayshootEvents) {
                EditorGUILayout.PropertyField(shootEvent);
            }
            EditorGUILayout.Space(spaceBetween);

            EditorGUILayout.LabelField("DECISIONS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(firstSightDecision);
            EditorGUILayout.PropertyField(coverBlownDecision);
            EditorGUILayout.PropertyField(attackEnemyCover);
            EditorGUILayout.Space(spaceBetween);

            EditorGUILayout.LabelField("BRAVENESS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(braveMeter);
            EditorGUILayout.Space(spaceBetween);
            
            EditorGUILayout.LabelField("CHANGE COVER", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(changeCoverFrequency);
        }

        void DrawAttackIdleTab(CoverShooterBehaviour script)
        {
            EditorGUILayout.LabelField("MOVE BACKWARDS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(moveBackwards);
            if (script.moveBackwards) {
                EditorGUILayout.PropertyField(moveBackwardsDistance);
                EditorGUILayout.PropertyField(moveBackwardsSpeed);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(moveBackwardsAnim);
                EditorGUILayout.PropertyField(moveBackwardsAnimT);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(moveBackwardsAttack);
            }
            EditorGUILayout.Space(spaceBetween);
            

            EditorGUILayout.LabelField("STRAFING", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(strafe);
            if (script.strafe) {
                EditorGUILayout.PropertyField(strafeSpeed);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(strafeTime);
                EditorGUILayout.PropertyField(strafeWaitTime);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(leftStrafeAnim);
                EditorGUILayout.PropertyField(rightStrafeAnim);
                EditorGUILayout.PropertyField(strafeAnimT);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(strafeLayersToAvoid);
            }
        }

        void DrawCallOthersTab(CoverShooterBehaviour script)
        {
            EditorGUILayout.LabelField("CALL OTHERS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(callOthers);
            if (script.callOthers) {
                EditorGUILayout.PropertyField(callRadius);
                EditorGUILayout.PropertyField(showCallRadius);
                EditorGUILayout.PropertyField(agentLayersToCall);
                EditorGUILayout.PropertyField(callOthersTime);
                EditorGUILayout.PropertyField(receiveCallFromOthers);
            }
        }

        void DrawSearchAndReturnTab(CoverShooterBehaviour script)
        {
            EditorGUILayout.LabelField("SEARCHING LOCATION", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(searchLocationRadius);
            if (script.searchLocationRadius) {
                EditorGUILayout.PropertyField(timeToStartSearch);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(searchPoints);
                EditorGUILayout.PropertyField(searchPointAnim);
                EditorGUILayout.PropertyField(pointWaitTime);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(endSearchAnim);
                EditorGUILayout.PropertyField(endSearchAnimTime);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(searchAnimsT);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(playAudioOnSearchStart);
                EditorGUILayout.PropertyField(playAudioOnSearchEnd);

                return;
            }

            
            EditorGUILayout.Space(spaceBetween);
            EditorGUILayout.LabelField("RETURNING TO PATROL", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(returnPatrolAnim);
            EditorGUILayout.PropertyField(returnPatrolAnimT);
            EditorGUILayout.PropertyField(returnPatrolTime);
            EditorGUILayout.PropertyField(playAudioOnReturnPatrol);
        }

        void DrawAudiosTab(CoverShooterBehaviour script)
        {
            EditorGUILayout.LabelField("AUDIOS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(playAudioOnChase);
            if (script.playAudioOnChase) {
                EditorGUILayout.PropertyField(alwaysPlayOnChase);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(playAudioDuringShooting);
            if (script.playAudioDuringShooting) {
                EditorGUILayout.PropertyField(alwaysPlayDuringShooting);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(playAudioOnMoveToShoot);
            if (script.playAudioOnMoveToShoot) {
                EditorGUILayout.PropertyField(alwaysPlayOnMoveToShoot);
            }
        }

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

        void GetSelectedTab()
        {
            if (EditorPrefs.HasKey("ShooterTabSelected")) {
                tabSelected = EditorPrefs.GetInt("ShooterTabSelected");
            }
            else{
                tabSelected = 0;
            }   
        }

        #endregion
    }
}
