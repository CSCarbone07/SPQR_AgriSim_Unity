using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using DroneController.Physics;

[CustomEditor(typeof(DroneMovement))]
[ExecuteInEditMode]
public class DroneMovementEditor : Editor {

	public override void OnInspectorGUI()
    {


        DrawDefaultInspector();
        var myScript = target as DroneMovementScript;
        EditorGUILayout.HelpBox("Hover NAME OF VARIABLES for properties to find out more about them. If you're not sure what they are used for feel free to contact me via e-mail or watch the youtube tutorials i prepared first.", MessageType.Info);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        DrawGameObjectPocketsAndVelocityReadings(myScript);

        EditorGUILayout.Space();

        DrawCustomProfilesWindow(myScript);

        EditorGUILayout.Space();

        DrawCustomInputTypeTabbedStyle(myScript);
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }

    void DrawGameObjectPocketsAndVelocityReadings(DroneMovementScript myScript)
    {
        myScript.animatedGameObject = (Animator)EditorGUILayout.ObjectField(new GUIContent("Animated GameObject Part", "Animated part of the drone that does the tricks. If you're not sure what to put here, check out already made prefabs or checkout my youtube tutorials for Drone Controller (located in the documentation)"), myScript.animatedGameObject, typeof(Animator), true);
        myScript.droneObject = (Transform)EditorGUILayout.ObjectField(new GUIContent("Drone Object", "Object part that is used to tilt. Ff you're not sure what to put here, check out already made prefabs or checkout my youtube tutorials for Drone Controller (located in the documentation)"), myScript.droneObject, typeof(Transform), true);
        GUI.enabled = false;
        EditorGUILayout.FloatField(new GUIContent("Velocity", "Getting our drone velocity."), myScript.velocity);
        GUI.enabled = true;
    }

    void DrawCustomProfilesWindow(DroneMovementScript myScript)
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Use this tab bar to customize flying settings. \n Edit to your own preferences. \n Selected one is applied to the drone and will drive with those settings.", EditorStyles.helpBox);

        myScript._profileIndex = GUILayout.Toolbar(myScript._profileIndex, new string[] { "Advanced", "Intermediate", "Begginer" });
        switch (myScript._profileIndex)
        {
            default:
                EditorGUILayout.LabelField("Max Speeds", EditorStyles.toolbarButton);
                myScript.profiles[myScript._profileIndex].maxForwardSpeed = EditorGUILayout.IntField(new GUIContent("Max Forward Speed", "Maximum forward speed for the drone to be allowed, ofcourse it's gonna go above that since you have to take c^2 = a^2 + b^2 in the calculation but this is a ballparked value at which will stop drone at that point of speed."), myScript.profiles[myScript._profileIndex].maxForwardSpeed);
                myScript.profiles[myScript._profileIndex].maxSidewaySpeed = EditorGUILayout.IntField(new GUIContent("Max Sideway Speed", "Same applies for the sideway speed.(Read forward speed attribute)"), myScript.profiles[myScript._profileIndex].maxSidewaySpeed);

                EditorGUILayout.LabelField("Front & Side Movement ", EditorStyles.toolbarButton);
                myScript.profiles[myScript._profileIndex].movementForwardSpeed = EditorGUILayout.FloatField(new GUIContent("Forward Movement Force", "Force that will be applied to move the drone forward."), myScript.profiles[myScript._profileIndex].movementForwardSpeed);
                myScript.profiles[myScript._profileIndex].sideMovementSpeed = EditorGUILayout.FloatField(new GUIContent("Side Movement Force", "Force that will be applied to move the drone sideway"), myScript.profiles[myScript._profileIndex].sideMovementSpeed);

                EditorGUILayout.LabelField("Up & Down Movement ", EditorStyles.toolbarButton);
                myScript.profiles[myScript._profileIndex].forceUpHover = EditorGUILayout.FloatField(new GUIContent("Up Movement Force", "Force that will be applied to move our drone upwards."), myScript.profiles[myScript._profileIndex].forceUpHover);
                myScript.profiles[myScript._profileIndex].forceDownHover = EditorGUILayout.FloatField(new GUIContent("Down Movement Force", "Force that will be applied to move our drone downwards."), myScript.profiles[myScript._profileIndex].forceDownHover);

                EditorGUILayout.LabelField("Tilt Values ", EditorStyles.toolbarButton);
                myScript.profiles[myScript._profileIndex].wantedForwardTilt = EditorGUILayout.Slider(new GUIContent("Wanted Forward Tilt", "The amount of rotation (around X axis) that will drone lean towards when moving forwards/backwards."), myScript.profiles[myScript._profileIndex].wantedForwardTilt, 0.0f, 45.0f);
                myScript.profiles[myScript._profileIndex].wantedSideTilt = EditorGUILayout.Slider(new GUIContent("Wanted Side Tilt", "The amount of rotation (around Z axis) that will be drone lean towards when moving sidewards."), myScript.profiles[myScript._profileIndex].wantedSideTilt, 0.0f, 45.0f);
                myScript.profiles[myScript._profileIndex].tiltMovementSpeed = EditorGUILayout.Slider(new GUIContent("Tilt Movement Speed", "The speed of how fast the drone will tilt. (Sidewards/Forwards)"), myScript.profiles[myScript._profileIndex].tiltMovementSpeed, 0.0f, 1.0f);
                myScript.profiles[myScript._profileIndex].tiltNoMovementSpeed = EditorGUILayout.Slider(new GUIContent("No Tilt Movement Speed", "The speed of how fast the drone will return to its original rotation."), myScript.profiles[myScript._profileIndex].tiltNoMovementSpeed, 0.0f, 1.0f);

                EditorGUILayout.LabelField("Rotation Speed ", EditorStyles.toolbarButton);
                myScript.profiles[myScript._profileIndex].rotationAmount = EditorGUILayout.FloatField(new GUIContent("Rotation Amount", "Rotation speed when rotating via J/L keys or via joystick."), myScript.profiles[myScript._profileIndex].rotationAmount);

                EditorGUILayout.LabelField("Drone Slowdown Speed ", EditorStyles.toolbarButton);
                myScript.profiles[myScript._profileIndex].slowDownTime = EditorGUILayout.Slider(new GUIContent("Slowdown Time", "How fast should our drone stabilize and hover in the air after driving it.(The slower the value, the faster it will stop and go idle)"), myScript.profiles[myScript._profileIndex].slowDownTime, 0.0f, 2.0f);

                EditorGUILayout.LabelField("Drone Sound Amplifier", EditorStyles.toolbarButton);
                myScript.profiles[myScript._profileIndex].droneSoundAmplifier = EditorGUILayout.Slider(new GUIContent("Drone Movement Sound Change", "tooltiptodo..."), myScript.profiles[myScript._profileIndex].droneSoundAmplifier, 0.0f, 2.0f);

                
                myScript.maxForwardSpeed = myScript.profiles[myScript._profileIndex].maxForwardSpeed;
                myScript.maxSidewaySpeed = myScript.profiles[myScript._profileIndex].maxSidewaySpeed;

                myScript.movementForwardSpeed = myScript.profiles[myScript._profileIndex].movementForwardSpeed;
                myScript.sideMovementAmount = myScript.profiles[myScript._profileIndex].sideMovementSpeed;

                myScript.forceUpHover = myScript.profiles[myScript._profileIndex].forceUpHover;
                myScript.forceDownHover = myScript.profiles[myScript._profileIndex].forceDownHover;

                myScript.wantedForwardTilt = myScript.profiles[myScript._profileIndex].wantedForwardTilt;
                myScript.wantedSideTilt = myScript.profiles[myScript._profileIndex].wantedSideTilt;
                myScript.tiltMovementSpeed = myScript.profiles[myScript._profileIndex].tiltMovementSpeed;
                myScript.tiltNoMovementSpeed = myScript.profiles[myScript._profileIndex].tiltNoMovementSpeed;

                myScript.rotationAmount = myScript.profiles[myScript._profileIndex].rotationAmount;
                myScript.slowDownTime = myScript.profiles[myScript._profileIndex].slowDownTime;

                myScript.droneSoundAmplifier = myScript.profiles[myScript._profileIndex].droneSoundAmplifier;
                break;
        }
        EditorGUILayout.EndVertical();
    }

    void DrawCustomInputTypeTabbedStyle(DroneMovementScript myScript)
    {
        //dodaj info tekst koji opisuje svaki%%%%%%%%%%%%%%%%%%%%%%%%%%%
        EditorGUILayout.BeginVertical("Box");
        myScript.inputEditorSelection = GUILayout.Toolbar(myScript.inputEditorSelection, new string[] { "Custom", "Keyboard", "Joystick", "Mobile" });

        switch (myScript.inputEditorSelection)
        {
			case 0:
				myScript.customFeed = true;

				EditorGUILayout.Space();

				EditorGUILayout.HelpBox("Hover on variable names to see their original name for custom code access.\nUse this if you want to feed your own movement values.\nIf you're not sure how to access these values, check the tutorial. Link from tutorial playlist is in the doc files.", MessageType.Info);

				EditorGUILayout.Space();
				myScript.customFeed_forward = EditorGUILayout.Slider(new GUIContent("Forward", "'customFeed_forward' is the variable name to acces, feed values from 0 to 1"), myScript.customFeed_forward, 0.0f, 1.0f);
				myScript.customFeed_backward = EditorGUILayout.Slider(new GUIContent("Backward", "'customFeed_backward' is the variable name to acces, feed values from 0 to 1"), myScript.customFeed_backward, 0.0f, 1.0f);
				myScript.customFeed_leftward = EditorGUILayout.Slider(new GUIContent("Leftward", "'customFeed_leftward' is the variable name to acces, feed values from 0 to 1"), myScript.customFeed_leftward, 0.0f, 1.0f);
				myScript.customFeed_rightward = EditorGUILayout.Slider(new GUIContent("Rightward", "'customFeed_rightward' is the variable name to acces, feed values from 0 to 1"), myScript.customFeed_rightward, 0.0f, 1.0f);
				myScript.customFeed_upward = EditorGUILayout.Slider(new GUIContent("Upward", "'customFeed_upward' is the variable name to acces, feed values from 0 to 1"), myScript.customFeed_upward, 0.0f, 1.0f);
				myScript.customFeed_downward = EditorGUILayout.Slider(new GUIContent("Downward", "'customFeed_downward' is the variable name to acces, feed values from 0 to 1"), myScript.customFeed_downward, 0.0f, 1.0f);
				myScript.customFeed_rotateLeft = EditorGUILayout.Slider(new GUIContent("Rotate Left", "'customFeed_rotateLeft' is the variable name to acces, feed values from 0 to 1"), myScript.customFeed_rotateLeft, 0.0f, 1.0f);
				myScript.customFeed_rotateRight = EditorGUILayout.Slider(new GUIContent("Rotate Right", "'customFeed_rotateRight' is the variable name to acces, feed values from 0 to 1"), myScript.customFeed_rotateRight, 0.0f, 1.0f);
				EditorGUILayout.Space();

				break;
            case 1:

				myScript.customFeed = false;
                myScript.mobile_turned_on = false;
                myScript.joystick_turned_on = false;

                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Setup wanted keyboard input to move your drone.", MessageType.Info);
                EditorGUILayout.Space();


                myScript.forward = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Forward", "Key press to move forward"), myScript.forward);
                myScript.backward = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Backward", "Key press to move backward"), myScript.backward);
                myScript.rightward = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Rightward", "Key press to move right (not rotating)"), myScript.rightward);
                myScript.leftward = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Leftward", "Key press to move left (not rotating)"), myScript.leftward);
                myScript.upward = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Upward", "Key press to 'Wingardium Leviosa' your drone"), myScript.upward);
                myScript.downward = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Downward", "Key press to move your drone down"), myScript.downward);
                myScript.rotateRightward = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Rotate Rightward", "Key press to rotate your drone right"), myScript.rotateRightward);
                myScript.rotateLeftward = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Rotate Leftward", "Key press to rotate your drone left"), myScript.rotateLeftward);

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                myScript.barrelRollRight = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Barrel-Roll Right", "Trick input for barrel-roll"), myScript.barrelRollRight);
                myScript.barrelRollLeft = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Barrel-Roll Left", "Trick input for barrel-roll"), myScript.barrelRollLeft);
                myScript.swingRight = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Swing Right", "Trick input for swing"), myScript.swingRight);
                myScript.swingLeft = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Swing Left", "Trick input for swing"), myScript.swingLeft);

                break;
            case 2:

				myScript.customFeed = false;
				myScript.mobile_turned_on = false;
                myScript.joystick_turned_on = true;

                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Setup wanted joystick control input to move your drone.", MessageType.Info);
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                    myScript.left_analog_x = EditorGUILayout.TextField(new GUIContent("Left Analog X Axis", "This is usually movement left/right (not rotating)"), myScript.left_analog_x);
                    myScript.left_analog_x_movement = (JoystickDrivingAxis)EditorGUILayout.EnumPopup(new GUIContent("", "tooltip"), myScript.left_analog_x_movement);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                    myScript.left_analog_y = EditorGUILayout.TextField(new GUIContent("Left Analog Y Axis", "This is usually movment forward/backward (not going up/down)"), myScript.left_analog_y);
                    myScript.left_analog_y_movement = (JoystickDrivingAxis)EditorGUILayout.EnumPopup(new GUIContent("", "tooltip"), myScript.left_analog_y_movement);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                    myScript.right_analog_x = EditorGUILayout.TextField(new GUIContent("Right Analog X Axis", "This is usually rotating left/right"), myScript.right_analog_x);
                    myScript.right_analog_x_movement = (JoystickDrivingAxis)EditorGUILayout.EnumPopup(new GUIContent("", "tooltip"), myScript.right_analog_x_movement);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                myScript.analogUpDownMovement = EditorGUILayout.Toggle(new GUIContent("Analog Movement Up/Down", "Use this if you want your joystick to use analog axis to move up or down. If you want to use buttons uncheck this."), myScript.analogUpDownMovement);

                //EditorGUILayout.Space();
                //EditorGUILayout.Space();
                if (myScript.analogUpDownMovement == true)
                {
                    EditorGUILayout.BeginHorizontal();
                        myScript.right_analog_y = EditorGUILayout.TextField(new GUIContent("Right Analog Y Axis", "This is usually hovering up/down"), myScript.right_analog_y);
                        myScript.right_analog_y_movement = (JoystickDrivingAxis)EditorGUILayout.EnumPopup(new GUIContent("", "tooltip"), myScript.right_analog_y_movement);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    myScript.upButton = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Up Button", "Key press to 'Wingardium Leviosa' your drone via joystick"), myScript.upButton);
                    myScript.downButton = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Down Button", "Key press to move your drone down via joystick"), myScript.downButton);
                }                

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                myScript.joystick_barrelRollRight = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Barrel-Roll Right", "Trick input for barrel-roll via joystick"), myScript.joystick_barrelRollRight);
                myScript.joystick_barrelRollLeft = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Barrel-Roll Left", "Trick input for barrel-roll via joystick"), myScript.joystick_barrelRollLeft);
                myScript.joystick_swingRight = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Swing Right", "Trick input for swing via joystick"), myScript.joystick_swingRight);
                myScript.joystick_swingLeft = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Swing Left", "Trick input for swing via joystick"), myScript.joystick_swingLeft);


                break;
            case 3:

				myScript.customFeed = false;
				myScript.mobile_turned_on = true;
                myScript.joystick_turned_on = false;

                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Mobile is hardcoded at the moment, but here you can see simulated pressed for the joystick/keyboard/phone input that user presses", MessageType.Info);
                EditorGUILayout.Space();

                myScript.forward_button_texture = (Texture)EditorGUILayout.ObjectField("Forward Button", myScript.forward_button_texture, typeof(Texture), false) as Texture;
                myScript.backward_button_texture = (Texture)EditorGUILayout.ObjectField("Backward Button", myScript.backward_button_texture, typeof(Texture), false) as Texture;
                myScript.leftward_button_texture = (Texture)EditorGUILayout.ObjectField("Leftward Button", myScript.leftward_button_texture, typeof(Texture), false) as Texture;
                myScript.rightward_button_texture = (Texture)EditorGUILayout.ObjectField("Rightward Button", myScript.rightward_button_texture, typeof(Texture), false) as Texture;
                myScript.upwards_button_texture = (Texture)EditorGUILayout.ObjectField("Upward Button", myScript.upwards_button_texture, typeof(Texture), false) as Texture;
                myScript.downwards_button_texture = (Texture)EditorGUILayout.ObjectField("Downward Button", myScript.downwards_button_texture, typeof(Texture), false) as Texture;
                myScript.rotation_left_button_texture = (Texture)EditorGUILayout.ObjectField("Left Rotation Button", myScript.rotation_left_button_texture, typeof(Texture), false) as Texture;
                myScript.rotation_right_button_texture = (Texture)EditorGUILayout.ObjectField("Right Rotation Button", myScript.rotation_right_button_texture, typeof(Texture), false) as Texture;

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                myScript.W = EditorGUILayout.Toggle(new GUIContent("W (simulated)", ""), myScript.W);
                myScript.S = EditorGUILayout.Toggle(new GUIContent("S (simulated)", ""), myScript.S);
                myScript.A = EditorGUILayout.Toggle(new GUIContent("A (simulated)", ""), myScript.A);
                myScript.D = EditorGUILayout.Toggle(new GUIContent("D (simulated)", ""), myScript.D);
                myScript.I = EditorGUILayout.Toggle(new GUIContent("I (simulated)", ""), myScript.I);
                myScript.J = EditorGUILayout.Toggle(new GUIContent("I (simulated)", ""), myScript.J);
                myScript.K = EditorGUILayout.Toggle(new GUIContent("J (simulated)", ""), myScript.K);
                myScript.L = EditorGUILayout.Toggle(new GUIContent("K (simulated)", ""), myScript.L);

                break;
        }
        EditorGUILayout.EndVertical();
    }
}
