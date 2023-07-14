using UnityEditor;
using UnityEngine;

namespace BlazeAISpace
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AttackStateBehaviour))]

    public class AttackStateBehaviourInspector : Editor
    {
        SerializedProperty moveSpeed,
        turnSpeed,

        idleAnim,
        moveAnim,
        idleMoveT,

        ranged,
        distanceFromEnemy,
        attackDistance,
        layersCheckOnAttacking,
        attacks,
        attackEvent,

        attackInIntervals,
        attackInIntervalsTime,

        callOthers,
        callRadius,
        agentLayersToCall,
        callOthersTime,
        showCallRadius,
        receiveCallFromOthers,

        moveBackwards,
        moveBackwardsDistance,
        moveBackwardsSpeed,
        moveBackwardsAnim,
        moveBackwardsAnimT,

        turnToTarget,
        turnSensitivity,
        useTurnAnims,

        strafe,
        strafeDirection,
        strafeSpeed,
        strafeTime,
        strafeWaitTime,
        leftStrafeAnim,
        rightStrafeAnim,
        strafeAnimT,
        strafeLayersToAvoid,

        onAttackRotate,
        onAttackRotateSpeed,

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

        playAttackIdleAudio,
        attackIdleAudioTime,

        playAudioOnChase,
        alwaysPlayOnChase,

        playAudioOnMoveToAttack,
        alwaysPlayOnMoveToAttack;


        bool displayAttackEvents = true;
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

            ranged = serializedObject.FindProperty("ranged");
            distanceFromEnemy = serializedObject.FindProperty("distanceFromEnemy");
            attackDistance = serializedObject.FindProperty("attackDistance");
            layersCheckOnAttacking = serializedObject.FindProperty("layersCheckOnAttacking");
            attacks = serializedObject.FindProperty("attacks");
            attackEvent = serializedObject.FindProperty("attackEvent");

            attackInIntervals = serializedObject.FindProperty("attackInIntervals");
            attackInIntervalsTime = serializedObject.FindProperty("attackInIntervalsTime");

            callOthers = serializedObject.FindProperty("callOthers");
            callRadius = serializedObject.FindProperty("callRadius");
            agentLayersToCall = serializedObject.FindProperty("agentLayersToCall");
            callOthersTime = serializedObject.FindProperty("callOthersTime");
            showCallRadius = serializedObject.FindProperty("showCallRadius");
            receiveCallFromOthers = serializedObject.FindProperty("receiveCallFromOthers");

            moveBackwards = serializedObject.FindProperty("moveBackwards");
            moveBackwardsDistance = serializedObject.FindProperty("moveBackwardsDistance");
            moveBackwardsSpeed = serializedObject.FindProperty("moveBackwardsSpeed");
            moveBackwardsAnim = serializedObject.FindProperty("moveBackwardsAnim");
            moveBackwardsAnimT = serializedObject.FindProperty("moveBackwardsAnimT");

            turnToTarget = serializedObject.FindProperty("turnToTarget");
            turnSensitivity = serializedObject.FindProperty("turnSensitivity");
            useTurnAnims = serializedObject.FindProperty("useTurnAnims");

            strafe = serializedObject.FindProperty("strafe");
            strafeDirection = serializedObject.FindProperty("strafeDirection");
            strafeSpeed = serializedObject.FindProperty("strafeSpeed");
            strafeTime = serializedObject.FindProperty("strafeTime");
            strafeWaitTime = serializedObject.FindProperty("strafeWaitTime");
            leftStrafeAnim = serializedObject.FindProperty("leftStrafeAnim");
            rightStrafeAnim = serializedObject.FindProperty("rightStrafeAnim");
            strafeAnimT = serializedObject.FindProperty("strafeAnimT");
            strafeLayersToAvoid = serializedObject.FindProperty("strafeLayersToAvoid");

            onAttackRotate = serializedObject.FindProperty("onAttackRotate");
            onAttackRotateSpeed = serializedObject.FindProperty("onAttackRotateSpeed");

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

            playAttackIdleAudio = serializedObject.FindProperty("playAttackIdleAudio");
            attackIdleAudioTime = serializedObject.FindProperty("attackIdleAudioTime");

            playAudioOnChase = serializedObject.FindProperty("playAudioOnChase");
            alwaysPlayOnChase = serializedObject.FindProperty("alwaysPlayOnChase");

            playAudioOnMoveToAttack = serializedObject.FindProperty("playAudioOnMoveToAttack");
            alwaysPlayOnMoveToAttack = serializedObject.FindProperty("alwaysPlayOnMoveToAttack");
        }

        public override void OnInspectorGUI () 
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.55f, 0.55f, 0.55f, 1f);
            
            DrawToolbar();
            EditorGUILayout.Space(10);
            
            GUI.backgroundColor = oldColor;
            AttackStateBehaviour script = (AttackStateBehaviour) target;

            tabIndex = -1;

            switch (tabSelected)
            {
                case 0:
                    DrawMovementTab();
                    break;
                case 1:
                    DrawAttackTab(script);
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

            EditorPrefs.SetInt("AttackTabSelected", tabSelected);
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

        void DrawAttackTab(AttackStateBehaviour script)
        {
            EditorGUILayout.LabelField("ATTACKING & DISTANCES", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(ranged);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(distanceFromEnemy);
            EditorGUILayout.PropertyField(attackDistance);
            EditorGUILayout.Space(20);

            EditorGUILayout.LabelField("FRIENDLY-FIRE LAYERS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(layersCheckOnAttacking);
            EditorGUILayout.Space(20);

            EditorGUILayout.LabelField("ATTACK ANIMATIONS & TIMING", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(attacks);
            EditorGUILayout.Space(20);


            displayAttackEvents = EditorGUILayout.Toggle("Display Attack Events", displayAttackEvents);
            if (displayAttackEvents) {
                EditorGUILayout.PropertyField(attackEvent);
            }
            EditorGUILayout.Space(spaceBetween);


            EditorGUILayout.LabelField("INTERVAL ATTACKS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(attackInIntervals);
            if (script.attackInIntervals) {
                EditorGUILayout.PropertyField(attackInIntervalsTime);
            }
            EditorGUILayout.Space(spaceBetween);


            EditorGUILayout.LabelField("ATTACK ROTATE", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(onAttackRotateSpeed);
            EditorGUILayout.PropertyField(onAttackRotate);
        }

        void DrawAttackIdleTab(AttackStateBehaviour script)
        {
            EditorGUILayout.LabelField("MOVE BACKWARDS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(moveBackwards);
            if (script.moveBackwards) {
                EditorGUILayout.PropertyField(moveBackwardsDistance);
                EditorGUILayout.PropertyField(moveBackwardsSpeed);
                EditorGUILayout.Space();
                
                EditorGUILayout.PropertyField(moveBackwardsAnim);
                EditorGUILayout.PropertyField(moveBackwardsAnimT);
            }


            EditorGUILayout.Space(spaceBetween);
            EditorGUILayout.LabelField("TURNING TO TARGET", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(turnToTarget);
            if (script.turnToTarget) {
                EditorGUILayout.PropertyField(turnSensitivity);
                EditorGUILayout.PropertyField(useTurnAnims);
            }


            EditorGUILayout.Space(spaceBetween);
            EditorGUILayout.LabelField("STRAFING", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(strafe);
            if (script.strafe) {
                EditorGUILayout.PropertyField(strafeDirection);
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

        void DrawCallOthersTab(AttackStateBehaviour script)
        {
            EditorGUILayout.LabelField("CALL OTHERS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(callOthers);
            if (script.callOthers) {
                EditorGUILayout.PropertyField(callRadius);
                EditorGUILayout.PropertyField(agentLayersToCall);
                EditorGUILayout.PropertyField(callOthersTime);
                EditorGUILayout.PropertyField(showCallRadius);
                EditorGUILayout.PropertyField(receiveCallFromOthers);
            }
        }

        void DrawSearchAndReturnTab(AttackStateBehaviour script)
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

        void DrawAudiosTab(AttackStateBehaviour script)
        {
            EditorGUILayout.LabelField("AUDIOS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(playAttackIdleAudio);
            if (script.playAttackIdleAudio) {
                EditorGUILayout.PropertyField(attackIdleAudioTime);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(playAudioOnChase);
            if (script.playAudioOnChase) {
                EditorGUILayout.PropertyField(alwaysPlayOnChase);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(playAudioOnMoveToAttack);
            if (script.playAudioOnMoveToAttack) {
                EditorGUILayout.PropertyField(alwaysPlayOnMoveToAttack);
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
            if (EditorPrefs.HasKey("AttackTabSelected")) {
                tabSelected = EditorPrefs.GetInt("AttackTabSelected");
            }
            else{
                tabSelected = 0;
            }   
        }

        #endregion
    }
}
