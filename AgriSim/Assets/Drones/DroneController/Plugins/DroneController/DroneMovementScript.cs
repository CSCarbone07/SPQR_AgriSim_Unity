using UnityEngine;
using System.Collections;
using DroneController.CameraMovement;
using DroneController.CustomGUI;
using DroneController.Profiles;
namespace DroneController
{
    namespace Profiles
    {
        [System.Serializable]
        public class Profile
        {
		
            public int maxForwardSpeed;
            public int maxSidewaySpeed;
            public float slowDownTime;
            public float forceUpHover;
            public float forceDownHover;
            public float sideMovementSpeed;
            public float movementForwardSpeed;
            public float rotationAmount;
            public float tiltMovementSpeed;
            public float tiltNoMovementSpeed;
            public float wantedForwardTilt;
            public float wantedSideTilt;
            public float droneSoundAmplifier;
        }
    }
    namespace Physics
    {
        public enum JoystickDrivingAxis { none, forward, sideward, upward, rotation }
        public class DroneMovementScript : MonoBehaviour
        {

            #region PUBLIC VARIABLES - EDITED THROUGH CUSTOM INSPECTOR

            [HideInInspector] public int _profileIndex;
            [HideInInspector] public Profile[] profiles = new Profile[3]; // index = mode    ->      0 = advanced, 1 = intermediate, 2 = begginer

            [HideInInspector] public int inputEditorSelection;

            [HideInInspector] public bool idle = true; //used for idle animations 
            [Tooltip("Part of drone hierarchy that is animated, and will move drone according to the animations you made or default that you got with the package.")]
            [HideInInspector]
            public Animator animatedGameObject;
            [Header("Input INFO")]
            [Tooltip("Check this if you want to get phone contorls on screen")]
            [HideInInspector]
            public bool mobile_turned_on = false;
            [Tooltip("Check this if you want to use your joystick. (DON'T FORGET TO ADJUST THE INPUT SETTINGS!!)")]
            [HideInInspector]
            public bool joystick_turned_on = false;
            [HideInInspector] public CameraScript mainCamera;
            [Tooltip("Part of the drone hierarchy that is used to 'tilt'.")]
            [HideInInspector]
            public Transform droneObject;
            [Tooltip("Just a reading of current velocity")]
            [HideInInspector]
            public float velocity; //check for speed

            [Header("MAX SPEEDS")]
            [Tooltip("Max forward speed")]
            [HideInInspector]
            public float maxForwardSpeed = 10;
            [Tooltip("Max sideward speed")]
            [HideInInspector]
            public float maxSidewaySpeed = 5;
            [Header("Drone slowdown")]
            [Range(0.0f, 2.0f)]
            [Tooltip("The value that will tell how fast does our drone slowdown after letting go of controls. (Lower value faster it slows down)")]
            [HideInInspector]
            public float slowDownTime = 0.95f;

            [HideInInspector] public float droneSoundAmplifier = 1;

            [Tooltip("Upforce that will hold drone in place if i.e. idle, below add the force options for going upwards/downwards.")]
            [HideInInspector]
            public float upForce;
            [Header("Up & Down Forces")]
            [Tooltip("Value that will be applied when pressed the 'I' key(going upwards)")]
            [HideInInspector]
            public float forceUpHover = 450;
            [Tooltip("Value that will be applied when pressed the 'K' key(going downwards)")]
            [HideInInspector]
            public float forceDownHover = -200;

            [Header("Front & Side Movement Forces")]
            [Tooltip("Force that will be applied when moving to the sides.")]
            [HideInInspector]
            public float sideMovementAmount = 300.0f;
            [Tooltip("Force that will be applied when moving forwards/backwards.")]
            [HideInInspector]
            public float movementForwardSpeed = 500.0f;

            [HideInInspector] public float wantedSideTilt = 20;// the desired tilt value of our drone when moving

            [HideInInspector] public float currentYRotation;
            [Header("Rotation Amount Mulitplier.(When pressing J/L keys)")]
            [Tooltip("The multiplier of how fast our drone will rotate(higher the value, the faster it rotates)")]
            [HideInInspector]
            public float rotationAmount = 2.5f;

            [Header("Movement Tilt Speed")]
            [Range(0.0f, 1.0f)]
            [Tooltip("How fast will drone tilt to the side when moving. (The lower value the faster it will snap to the side)")]
            [HideInInspector]
            public float tiltMovementSpeed = 0.1f;
            [Range(0.0f, 1.0f)]
            [Tooltip("How fast will drone tilt to the original position when idle. (The lower value the faster it will snap to the side)")]
            [HideInInspector]
            public float tiltNoMovementSpeed = 0.3f;
            [HideInInspector] public float wantedForwardTilt = 20; // the desired tilt value of our drone when moving

			#endregion

			#region PUBLIC VARIABLES - INPUT SETTINGS

			[HideInInspector] public Rigidbody ourDrone;
			[HideInInspector] public bool FlightRecorderOverride;

			[Header("JOYSTICK AXIS INPUT")]
            [HideInInspector]
            public string left_analog_x = "Horizontal";
            [HideInInspector] public string left_analog_y = "Vertical";
            [HideInInspector] public string right_analog_x = "Horizontal_Right";
            [HideInInspector] public string right_analog_y = "Horizontal_UpDown";
            [HideInInspector] public KeyCode downButton = KeyCode.JoystickButton13;
            [HideInInspector] public KeyCode upButton = KeyCode.JoystickButton14;
            [HideInInspector] public bool analogUpDownMovement = false;
            [HideInInspector] public JoystickDrivingAxis left_analog_y_movement = JoystickDrivingAxis.forward;
            [HideInInspector] public JoystickDrivingAxis left_analog_x_movement = JoystickDrivingAxis.sideward;
            [HideInInspector] public JoystickDrivingAxis right_analog_y_movement = JoystickDrivingAxis.upward;
            [HideInInspector] public JoystickDrivingAxis right_analog_x_movement = JoystickDrivingAxis.rotation;

            [Header("INPUT TRANSLATED FOR MOBILE CONTROLS")]
            [HideInInspector]
            public bool W;
            [HideInInspector] public bool S;
            [HideInInspector] public bool A;
            [HideInInspector] public bool D;
            [HideInInspector] public bool I;
            [HideInInspector] public bool K;
            [HideInInspector] public bool J;
            [HideInInspector] public bool L;

            [HideInInspector] public Texture forward_button_texture;
            [HideInInspector] public Texture backward_button_texture;
            [HideInInspector] public Texture leftward_button_texture;
            [HideInInspector] public Texture rightward_button_texture;
            [HideInInspector] public Texture upwards_button_texture;
            [HideInInspector] public Texture downwards_button_texture;
            [HideInInspector] public Texture rotation_left_button_texture;
            [HideInInspector] public Texture rotation_right_button_texture;

            [Header("Keyboard Inputs")]
            [HideInInspector]
            public KeyCode forward = KeyCode.W;
            [HideInInspector] public KeyCode backward = KeyCode.S;
            [HideInInspector] public KeyCode rightward = KeyCode.D;
            [HideInInspector] public KeyCode leftward = KeyCode.A;
            [HideInInspector] public KeyCode upward = KeyCode.I;
            [HideInInspector] public KeyCode downward = KeyCode.K;
            [HideInInspector] public KeyCode rotateRightward = KeyCode.L;
            [HideInInspector] public KeyCode rotateLeftward = KeyCode.J;

            [Header("Tricks keyboard input")]
            [HideInInspector]
            public KeyCode barrelRollRight = KeyCode.O;
            [HideInInspector] public KeyCode barrelRollLeft = KeyCode.U;
            [HideInInspector] public KeyCode swingLeft = KeyCode.Q;
            [HideInInspector] public KeyCode swingRight = KeyCode.E;
            [HideInInspector] public KeyCode joystick_barrelRollRight = KeyCode.JoystickButton1;
            [HideInInspector] public KeyCode joystick_barrelRollLeft = KeyCode.JoystickButton3;
            [HideInInspector] public KeyCode joystick_swingLeft = KeyCode.JoystickButton2;
            [HideInInspector] public KeyCode joystick_swingRight = KeyCode.JoystickButton0;

			/// <summary>
			/// Custom Feed Input Variables
			/// </summary>
			[HideInInspector] public float customFeed_forward;
			[HideInInspector] public float customFeed_backward;
			[HideInInspector] public float customFeed_leftward;
			[HideInInspector] public float customFeed_rightward;
			[HideInInspector] public float customFeed_upward;
			[HideInInspector] public float customFeed_downward;
			[HideInInspector] public float customFeed_rotateLeft;
			[HideInInspector] public float customFeed_rotateRight;
			[HideInInspector] public bool customFeed; // do we use custom input for the inputs?

			#endregion

			#region PRIVATE VARIABLES

            AudioSource droneSound;

            private Vector3 velocityToSmoothDampToZero;

            private float mouseScrollWheelAmount;
            private float wantedHeight;
            private float currentHeight;
            private float heightVelocity;

            private float tiltAmountSideways = 0;
            private float tiltVelocitySideways;

            private float wantedYRotation;
            private float rotationYVelocity;

            private float tiltAmountForward = 0;
            private float tiltVelocityForward;

			[HideInInspector] public float Vertical_W = 0;
			[HideInInspector] public float Vertical_S = 0;
			[HideInInspector] public float Horizontal_A = 0;
			[HideInInspector] public float Horizontal_D = 0;
			[HideInInspector] public float Vertical_I = 0;
			[HideInInspector] public float Vertical_K = 0;
			[HideInInspector] public float Horizontal_J = 0;
			[HideInInspector] public float Horizontal_L = 0;

            private Rect wRect = new Rect(10, 55, 14, 20);
            private Rect sRect = new Rect(10, 80, 14, 20);
            private Rect aRect = new Rect(0, 67.5f, 14, 20);
            private Rect dRect = new Rect(20, 67.5f, 14, 20);

            private Rect iRect = new Rect(76, 55, 14, 20);
            private Rect jRect = new Rect(66, 67.5f, 14, 20);
            private Rect kRect = new Rect(76, 80, 14, 20);
            private Rect lRect = new Rect(86, 67.5f, 14, 20);

            private Vector2[] myTouchPosition = new Vector2[]{
                new Vector2(0,0),
                new Vector2(0,0)
            };

            #endregion
            
            #region Mono Behaviour METHODS

            public virtual void Awake()
            {
                ourDrone = GetComponent<Rigidbody>();

                StartCoroutine(FindMainCamera());
                SetStartingRotation();

                FindDroneSoundComponent();
            }

            /*
            void FixedUpdate(){
                GetVelocity ();
                ClampingSpeedValues();
                MovementUpDown();
                MovementLeftRight();
                Rotation();
                MovementForward();
                SettingControllerToInputSettings ();
                BasicDroneHoverAndRotation ();
            }

            void Update(){
                RotationUpdateLoop_TrickRotation();
                Animations();
                DroneSound();
                CameraCorrectPickAndTranslatingInputToWSAD ();
            }
            */

            /// <summary>
            /// Drawing button images on screen
            /// </summary>
            void OnGUI()
            {
                if (mobile_turned_on == true)
                {
                    DrawGUI.DrawTexture(wRect.x, wRect.y, wRect.width, wRect.height, forward_button_texture);
                    DrawGUI.DrawTexture(sRect.x, sRect.y, sRect.width, sRect.height, backward_button_texture);
                    DrawGUI.DrawTexture(aRect.x, aRect.y, aRect.width, aRect.height, leftward_button_texture);
                    DrawGUI.DrawTexture(dRect.x, dRect.y, dRect.width, dRect.height, rightward_button_texture);

                    DrawGUI.DrawTexture(iRect.x, iRect.y, iRect.width, iRect.height, upwards_button_texture);
                    DrawGUI.DrawTexture(jRect.x, jRect.y, jRect.width, jRect.height, rotation_left_button_texture);
                    DrawGUI.DrawTexture(kRect.x, kRect.y, kRect.width, kRect.height, downwards_button_texture);
                    DrawGUI.DrawTexture(lRect.x, lRect.y, lRect.width, lRect.height, rotation_right_button_texture);
                }
            }

            #endregion

            #region PRIVATE METHODS

            /// <summary>
            /// Fixes the starting rotation, sets the wanted and current rotation in the
            /// code so drone doesnt start with rotation of 0,0,0
            /// </summary>
            void SetStartingRotation()
            {
                wantedYRotation = transform.eulerAngles.y;
                currentYRotation = transform.eulerAngles.y;
            }

            /// <summary>
            /// Tries to find drone sound component upon script awake
            /// </summary>
            void FindDroneSoundComponent()
            {
                try
                {
                    if (gameObject.transform.Find("drone_sound").GetComponent<AudioSource>())
                    {
                        droneSound = gameObject.transform.Find("drone_sound").GetComponent<AudioSource>();
                    }
                    else
                    {
                        print("Found drone_sound but it has no AudioSource component.");
                    }
                }
                catch (System.Exception ex)
                {
                    print("No Sound Child GameObject ->" + ex.StackTrace.ToString());
                }
            }

            /// <summary>
            /// Translating Left Y Analog to WSAD, IJKL inputs
            /// </summary>
            void Left_Analog_Y_Translation()
            {
                if (left_analog_y_movement == JoystickDrivingAxis.forward)
                {
                    W = (Input.GetAxisRaw(left_analog_y) > 0) ? true : false;
                    S = (Input.GetAxisRaw(left_analog_y) < 0) ? true : false;
                }
                else if (left_analog_y_movement == JoystickDrivingAxis.sideward)
                {
                    D = (Input.GetAxisRaw(left_analog_y) > 0) ? true : false;
                    A = (Input.GetAxisRaw(left_analog_y) < 0) ? true : false;
                }
                else if (left_analog_y_movement == JoystickDrivingAxis.upward)
                {
                    if (analogUpDownMovement == true)
                    {
                        I = (Input.GetAxisRaw(left_analog_y) > 0) ? true : false;
                        K = (Input.GetAxisRaw(left_analog_y) < 0) ? true : false;
                    }
                }
                else if (left_analog_y_movement == JoystickDrivingAxis.rotation)
                {
                    J = (-Input.GetAxisRaw(left_analog_y) > 0) ? true : false;
                    L = (-Input.GetAxisRaw(left_analog_y) < 0) ? true : false;
                }
            }

            /// <summary>
            /// Translating Left X Analog to WSAD, IJKL inputs
            /// </summary>
            void Left_Analog_X_Translation()
            {
                if (left_analog_x_movement == JoystickDrivingAxis.forward)
                {
                    W = (Input.GetAxisRaw(left_analog_x) > 0) ? true : false;
                    S = (Input.GetAxisRaw(left_analog_x) < 0) ? true : false;
                }
                else if (left_analog_x_movement == JoystickDrivingAxis.sideward)
                {
                    D = (Input.GetAxisRaw(left_analog_x) > 0) ? true : false;
                    A = (Input.GetAxisRaw(left_analog_x) < 0) ? true : false;
                }
                else if (left_analog_x_movement == JoystickDrivingAxis.upward)
                {
                    if (analogUpDownMovement == true)
                    {
                        I = (Input.GetAxisRaw(left_analog_x) > 0) ? true : false;
                        K = (Input.GetAxisRaw(left_analog_x) < 0) ? true : false;
                    }
                }
                else if (left_analog_x_movement == JoystickDrivingAxis.rotation)
                {
                    J = (-Input.GetAxisRaw(left_analog_x) > 0) ? true : false;
                    L = (-Input.GetAxisRaw(left_analog_x) < 0) ? true : false;
                }
            }

            /// <summary>
            /// Translating Right Y Analog to WSAD, IJKL inputs
            /// </summary>
            void Right_Analog_Y_Translation()
            {
                if (right_analog_y_movement == JoystickDrivingAxis.forward)
                {
                    W = (-Input.GetAxisRaw(right_analog_y) > 0) ? true : false;
                    S = (-Input.GetAxisRaw(right_analog_y) < 0) ? true : false;
                }
                else if (right_analog_y_movement == JoystickDrivingAxis.sideward)
                {
                    D = (Input.GetAxisRaw(right_analog_y) > 0) ? true : false;
                    A = (Input.GetAxisRaw(right_analog_y) < 0) ? true : false;
                }
                else if (right_analog_y_movement == JoystickDrivingAxis.upward)
                {
                    if (analogUpDownMovement == true)
                    {
                        I = (-Input.GetAxisRaw(right_analog_y) > 0) ? true : false;
                        K = (-Input.GetAxisRaw(right_analog_y) < 0) ? true : false;
                    }
                }
                else if (right_analog_y_movement == JoystickDrivingAxis.rotation)
                {
                    J = (-Input.GetAxisRaw(right_analog_y) > 0) ? true : false;
                    L = (-Input.GetAxisRaw(right_analog_y) < 0) ? true : false;
                }
            }

            /// <summary>
            /// Translating Right X Analog to WSAD, IJKL inputs
            /// </summary>
            void Right_Analog_X_Translation()
            {
                if (right_analog_x_movement == JoystickDrivingAxis.forward)
                {
                    W = (-Input.GetAxisRaw(right_analog_x) > 0) ? true : false;
                    S = (-Input.GetAxisRaw(right_analog_x) < 0) ? true : false;
                }
                else if (right_analog_x_movement == JoystickDrivingAxis.sideward)
                {
                    D = (Input.GetAxisRaw(right_analog_x) > 0) ? true : false;
                    A = (Input.GetAxisRaw(right_analog_x) < 0) ? true : false;
                }
                else if (right_analog_x_movement == JoystickDrivingAxis.upward)
                {
                    if (analogUpDownMovement == true)
                    {
                        I = (Input.GetAxisRaw(right_analog_x) > 0) ? true : false;
                        K = (Input.GetAxisRaw(right_analog_x) < 0) ? true : false;
                    }
                }
                else if (right_analog_x_movement == JoystickDrivingAxis.rotation)
                {
                    J = (-Input.GetAxisRaw(right_analog_x) > 0) ? true : false;
                    L = (-Input.GetAxisRaw(right_analog_x) < 0) ? true : false;
                }
            }

            /// <summary>
            /// Faking smooth press on a mobile key or keyboard, faking a joystick sstick moving up...
            /// </summary>
            void Input_Mobile_Sensitvity_Calculation()
            {
                if (W == true)
                    Vertical_W = Mathf.LerpUnclamped(Vertical_W, 1, Time.deltaTime * 10);
                else Vertical_W = Mathf.LerpUnclamped(Vertical_W, 0, Time.deltaTime * 10);

                if (S == true)
                    Vertical_S = Mathf.LerpUnclamped(Vertical_S, -1, Time.deltaTime * 10);
                else Vertical_S = Mathf.LerpUnclamped(Vertical_S, 0, Time.deltaTime * 10);

                if (A == true)
                    Horizontal_A = Mathf.LerpUnclamped(Horizontal_A, -1, Time.deltaTime * 10);
                else Horizontal_A = Mathf.LerpUnclamped(Horizontal_A, 0, Time.deltaTime * 10);

                if (D == true)
                    Horizontal_D = Mathf.LerpUnclamped(Horizontal_D, 1, Time.deltaTime * 10);
                else Horizontal_D = Mathf.LerpUnclamped(Horizontal_D, 0, Time.deltaTime * 10);

                if (I == true)
                    Vertical_I = Mathf.LerpUnclamped(Vertical_I, 1, Time.deltaTime * 10);
                else Vertical_I = Mathf.LerpUnclamped(Vertical_I, 0, Time.deltaTime * 10);

                if (K == true)
                    Vertical_K = Mathf.LerpUnclamped(Vertical_K, -1, Time.deltaTime * 10);
                else Vertical_K = Mathf.LerpUnclamped(Vertical_K, 0, Time.deltaTime * 10);

                if (J == true)
                    Horizontal_J = Mathf.LerpUnclamped(Horizontal_J, 1, Time.deltaTime * 10);
                else Horizontal_J = Mathf.LerpUnclamped(Horizontal_J, 0, Time.deltaTime * 10);

                if (L == true)
                    Horizontal_L = Mathf.LerpUnclamped(Horizontal_L, -1, Time.deltaTime * 10);
                else Horizontal_L = Mathf.LerpUnclamped(Horizontal_L, 0, Time.deltaTime * 10);
            }

            /// <summary>
            /// Since we can change keys and bind joystick axis to any action, this method reads the input axis in original way so it knows what to do with it.
            /// Translating joystick input
            /// </summary>
            void Joystick_Input_Sensitivity_Calculation() //getting the input from joystick and determening where to move
            {
                //only I and K
                if (analogUpDownMovement == false)
                {
                    if (I == true)
                        Vertical_I = Mathf.LerpUnclamped(Vertical_I, 1, Time.deltaTime * 10);
                    else Vertical_I = Mathf.LerpUnclamped(Vertical_I, 0, Time.deltaTime * 10);

                    if (K == true)
                        Vertical_K = Mathf.LerpUnclamped(Vertical_K, -1, Time.deltaTime * 10);
                    else Vertical_K = Mathf.LerpUnclamped(Vertical_K, 0, Time.deltaTime * 10);
                }

                Left_Analog_Y_Movement();
                Left_Analog_X_Movement();
                Right_Analog_Y_Movement();
                Right_Analog_X_Movement();

            }

            //reads the inputs of the wanted axis to behave and moves it in that direction
            void Left_Analog_Y_Movement()
            {
                if (left_analog_y_movement == JoystickDrivingAxis.forward)
                {
                    Vertical_W = Input.GetAxis(left_analog_y);
                    Vertical_S = Input.GetAxis(left_analog_y);
                }
                else if (left_analog_y_movement == JoystickDrivingAxis.sideward)
                {
                    Horizontal_D = Input.GetAxis(left_analog_y);
                    Horizontal_A = Input.GetAxis(left_analog_y);
                }
                else if (left_analog_y_movement == JoystickDrivingAxis.upward)
                {
                    if (analogUpDownMovement == true)
                    {
                        Vertical_I = Input.GetAxis(left_analog_y);
                        Vertical_K = Input.GetAxis(left_analog_y);
                    }
                }
                else if (left_analog_y_movement == JoystickDrivingAxis.rotation)
                {
                    Horizontal_J = Input.GetAxis(left_analog_y);
                    Horizontal_L = Input.GetAxis(left_analog_y);
                }
            }
            void Left_Analog_X_Movement()
            {
                if (left_analog_x_movement == JoystickDrivingAxis.forward)
                {
                    Vertical_W = Input.GetAxis(left_analog_x);
                    Vertical_S = Input.GetAxis(left_analog_x);
                }
                else if (left_analog_x_movement == JoystickDrivingAxis.sideward)
                {
                    Horizontal_D = Input.GetAxis(left_analog_x);
                    Horizontal_A = Input.GetAxis(left_analog_x);
                }
                else if (left_analog_x_movement == JoystickDrivingAxis.upward)
                {
                    if (analogUpDownMovement == true)
                    {
                        Vertical_I = Input.GetAxis(left_analog_x);
                        Vertical_K = Input.GetAxis(left_analog_x);
                    }
                }
                else if (left_analog_x_movement == JoystickDrivingAxis.rotation)
                {
                    Horizontal_J = Input.GetAxis(left_analog_x);
                    Horizontal_L = Input.GetAxis(left_analog_x);
                }
            }
            void Right_Analog_Y_Movement()
            {
                if (right_analog_y_movement == JoystickDrivingAxis.forward)
                {
                    Vertical_W = -Input.GetAxis(right_analog_y);
                    Vertical_S = -Input.GetAxis(right_analog_y);
                }
                else if (right_analog_y_movement == JoystickDrivingAxis.sideward)
                {
                    Horizontal_D = Input.GetAxis(right_analog_y);
                    Horizontal_A = Input.GetAxis(right_analog_y);
                }
                else if (right_analog_y_movement == JoystickDrivingAxis.upward)
                {
                    if (analogUpDownMovement == true)
                    {
                        Vertical_I = -Input.GetAxis(right_analog_y);
                        Vertical_K = -Input.GetAxis(right_analog_y);
                    }
                }
                else if (right_analog_y_movement == JoystickDrivingAxis.rotation)
                {
                    Horizontal_J = Input.GetAxis(right_analog_y);
                    Horizontal_L = Input.GetAxis(right_analog_y);
                }
            }
            void Right_Analog_X_Movement()
            {
                if (right_analog_x_movement == JoystickDrivingAxis.forward)
                {
                    Vertical_W = -Input.GetAxis(right_analog_x);
                    Vertical_S = -Input.GetAxis(right_analog_x);
                }
                else if (right_analog_x_movement == JoystickDrivingAxis.sideward)
                {
                    Horizontal_D = Input.GetAxis(right_analog_x);
                    Horizontal_A = Input.GetAxis(right_analog_x);
                }
                else if (right_analog_x_movement == JoystickDrivingAxis.upward)
                {
                    if (analogUpDownMovement == true)
                    {
                        Vertical_I = Input.GetAxis(right_analog_x);
                        Vertical_K = Input.GetAxis(right_analog_x);
                    }
                }
                else if (right_analog_x_movement == JoystickDrivingAxis.rotation)
                {
                    Horizontal_J = Input.GetAxis(right_analog_x);
                    Horizontal_L = Input.GetAxis(right_analog_x);
                }
            }

            /// <summary>
            /// Calculating WSAD IJKL input for mobile to check if our touch is inside any of these keys in particular
            /// </summary>
            void CheckingIfInside()
            {
                W = (wRect.Contains(myTouchPosition[0]) || wRect.Contains(myTouchPosition[1])) ? true : false;
                S = (sRect.Contains(myTouchPosition[0]) || sRect.Contains(myTouchPosition[1])) ? true : false;
                A = (aRect.Contains(myTouchPosition[0]) || aRect.Contains(myTouchPosition[1])) ? true : false;
                D = (dRect.Contains(myTouchPosition[0]) || dRect.Contains(myTouchPosition[1])) ? true : false;

                I = (iRect.Contains(myTouchPosition[0]) || iRect.Contains(myTouchPosition[1])) ? true : false;
                J = (jRect.Contains(myTouchPosition[0]) || jRect.Contains(myTouchPosition[1])) ? true : false;
                K = (kRect.Contains(myTouchPosition[0]) || kRect.Contains(myTouchPosition[1])) ? true : false;
                L = (lRect.Contains(myTouchPosition[0]) || lRect.Contains(myTouchPosition[1])) ? true : false;
            }

            /// <summary>
            /// Getting touch positions on the screen, 2 touches max
            /// </summary>
            void TouchCalculations()
            {
                for (int i = 0; i < Input.touches.Length; i++)
                {
                    myTouchPosition[i] = DrawGUI.Percentages(new Vector2(Input.GetTouch(i).position.x, Screen.height - Input.GetTouch(i).position.y));
                }
                if (Input.touchCount == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        myTouchPosition[i] = Vector2.zero;
                    }
                }
                if (Input.touchCount == 1)
                {
                    myTouchPosition[1] = Vector2.zero;
                }
            }

            #endregion

            #region PUBLIC VARIABLES

            /// <summary>
            /// Just getting velocity from our rigidbody
            /// </summary>
            public void GetVelocity()
            {
                velocity = ourDrone.velocity.magnitude;
            }

            /// <summary>
            /// Applying upForce for hovering and keeping the drone in the air.
            /// Handles rotation and applies it here.
            /// Handles tilt values and applies it, gues where? here! :)
            /// </summary>
            public void BasicDroneHoverAndRotation()
            {
                ourDrone.AddRelativeForce(Vector3.up * upForce);

                ourDrone.rotation = Quaternion.Euler(
                    new Vector3(0, currentYRotation, 0)
                );

                ourDrone.angularVelocity = new Vector3(0, 0, 0);

                droneObject.localRotation = Quaternion.Euler(
                    new Vector3(tiltAmountForward, 0, tiltAmountSideways)
                );
            }

            /// <summary>
            /// Used to reset once we get control from photon again... to set drone in place without rotations
            /// </summary>
            public void ResetDroneObjectRotation()
            {
                tiltAmountForward = 0;
                currentYRotation = 0;
                wantedYRotation = 0;
                tiltAmountSideways = 0;
            }

            /// <summary>
            /// Used to reset once we get control from photon again... to set drone in place with starting rotation
            /// </summary>
            /// <param name="_startingYRotation"></param>
            public void ResetDroneObjectRotation(float _startingYRotation)
            {
                tiltAmountForward = 0;
                currentYRotation = _startingYRotation;
                wantedYRotation = _startingYRotation;
                tiltAmountSideways = 0;
            }

            /// <summary>
            /// Determining waether we are using joystick, mobile or a keyboard
            /// and does some calculations
            /// </summary>
            public void SettingControllerToInputSettings()
            {
				if(customFeed == false)
				{
					if (joystick_turned_on == false)
					{
						Input_Mobile_Sensitvity_Calculation();//returns lineary pressed WSAD keys for PC and mobile, joystick has sensitvity builtin by default
					}
					else
					{
						Joystick_Input_Sensitivity_Calculation();
					}

					if (mobile_turned_on == true)
					{
						TouchCalculations();
						CheckingIfInside();
					}
				}
				else
				{
					CustomInputFeed();
				}                
            }

			private void CustomInputFeed()
			{
				//forward
				Vertical_W = customFeed_forward;
				if (customFeed_forward > 0) W = true;
				else W = false;
				//backward
				Vertical_S = -customFeed_backward;
				if (customFeed_backward > 0) S = true;
				else S = false;
				//leftward
				Horizontal_A = -customFeed_leftward;
				if (customFeed_leftward > 0) A = true;
				else A = false;
				//rightward
				Horizontal_D = customFeed_rightward;
				if (customFeed_rightward > 0) D = true;
				else D = false;
				//upward
				Vertical_I = customFeed_upward;
				if (customFeed_upward > 0) I = true;
				else I = false;
				//downward
				Vertical_K = -customFeed_downward;
				if (customFeed_downward > 0) K = true;
				else K = false;
				//left rotation
				Horizontal_J = -customFeed_rotateLeft;
				if (customFeed_rotateLeft > 0) J = true;
				else J = false;
				//right rotation
				Horizontal_L = customFeed_rotateRight;
				if (customFeed_rotateRight > 0) L = true;
				else L = false;
			}

			/// <summary>
			/// Rotating the drone via barrel roll trick, adding 100 on the Y angle rotation
			/// </summary>
			public void RotationUpdateLoop_TrickRotation()
            {
                if (mainCamera.pickedMyDrone == true)
                {
                    if (mainCamera.ourDrone.transform == transform)
                    {
                        if ((Input.GetKeyDown(barrelRollLeft) || Input.GetKeyDown(joystick_barrelRollLeft)) && idle == false)
                        {
                            wantedYRotation -= 100;
                        }
                        if ((Input.GetKeyDown(barrelRollRight) || Input.GetKeyDown(joystick_barrelRollRight)) && idle == false)
                        {
                            wantedYRotation += 100;
                        }
                    }
                }
            }

            /// <summary>
            /// Translating all the input weather its joystick, keyboard or mobile to WSAD at the end
            /// so it works cross platforms and if we add a new controller typed
            /// </summary>
            public void CameraCorrectPickAndTranslatingInputToWSAD()
            {
				/*
                 * If we picked the drone we wish to control and if its the same one as this one
                 * control only this drone, else remain uncontrolled
                 */
				if (customFeed == true) return;

                if ((mainCamera.pickedMyDrone == true && mainCamera.ourDrone.transform == transform))
                {
                    if (mobile_turned_on == false && joystick_turned_on == false)
                    {
                        W = (Input.GetKey(forward)) ? true : false;
                        S = (Input.GetKey(backward)) ? true : false;
                        A = (Input.GetKey(leftward)) ? true : false;
                        D = (Input.GetKey(rightward)) ? true : false;

                        I = (Input.GetKey(upward)) ? true : false;
                        J = (Input.GetKey(rotateLeftward)) ? true : false;
                        K = (Input.GetKey(downward)) ? true : false;
                        L = (Input.GetKey(rotateRightward)) ? true : false;
                    }
                    if (mobile_turned_on == false && joystick_turned_on == true)
                    {

                        //only used for I and K
                        if (analogUpDownMovement == false)
                        {
                            K = (Input.GetKey(downButton)) ? true : false;
                            I = (Input.GetKey(upButton)) ? true : false;
                        }

                        Left_Analog_Y_Translation();
                        Left_Analog_X_Translation();
                        Right_Analog_Y_Translation();
                        Right_Analog_X_Translation();
                    }
                }
            }

            /// <summary>
            /// Triggering the animations
            /// </summary>
            public void Animations()
            {
                if (animatedGameObject != null)
                {
                    animatedGameObject.SetBool("idle", idle);

                    //BARREL ROLL ANIMS
                    if (Input.GetKeyDown(barrelRollLeft) || Input.GetKeyDown(joystick_barrelRollLeft) && idle == false)
                    {
                        StartCoroutine("Twirl_left_Method");
                    }
                    if (Input.GetKeyDown(barrelRollRight) || Input.GetKeyDown(joystick_barrelRollRight) && idle == false)
                    {
                        StartCoroutine("Twirl_right_Method");
                    }

                    //STAR WARS ANAKIN STYLE SWING, ~may the force be with you~
                    if (Input.GetKeyDown(swingLeft) || Input.GetKeyDown(joystick_swingLeft) && idle == false)
                    {
                        StartCoroutine("Left_Passage");
                    }
                    if (Input.GetKeyDown(swingRight) || Input.GetKeyDown(joystick_swingRight) && idle == false)
                    {
                        StartCoroutine("Right_Passage");
                    }
                }
            }

            /// <summary>
            /// Clamping speed values determined on what input is pressed
            /// </summary>
            public void ClampingSpeedValues()
            {
                if ((W || S) && (A || D))
                {
                    ourDrone.velocity = Vector3.ClampMagnitude(ourDrone.velocity, Mathf.Lerp(ourDrone.velocity.magnitude, maxForwardSpeed, Time.deltaTime * 5f));
                }
                if ((W || S) && (!A && !D))
                {
                    ourDrone.velocity = Vector3.ClampMagnitude(ourDrone.velocity, Mathf.Lerp(ourDrone.velocity.magnitude, maxForwardSpeed, Time.deltaTime * 5f));
                }
                if ((!W && !S) && (A || D))
                {
                    ourDrone.velocity = Vector3.ClampMagnitude(ourDrone.velocity, Mathf.Lerp(ourDrone.velocity.magnitude, maxSidewaySpeed, Time.deltaTime * 5f));
                }
                if (!W && !S && !A && !D)
                {
                    ourDrone.velocity = Vector3.SmoothDamp(ourDrone.velocity, Vector3.zero, ref velocityToSmoothDampToZero, slowDownTime);
                }
            }

            /// <summary>
            /// Drone Sound Managing
            /// </summary>
            public void DroneSound()
            {
                if (droneSound)
                {
                    droneSound.pitch = 1 + ((velocity / 100) * droneSoundAmplifier);

                    /*
                    [DEPRICATED - WILL BE UPDATED IN FURTHER UPDATES]
                    if (velocity > 1)
                    {
                        droneSound.spatialBlend = Mathf.Lerp(droneSound.spatialBlend, 0.0f, Time.deltaTime * 1);
                    }
                    else
                    {
                        droneSound.spatialBlend = Mathf.Lerp(droneSound.spatialBlend, 1.0f, Time.deltaTime * 1);
                    }
                    */
                }
            }

            /// <summary>
            /// Handling up down movement and applying needed force.
            /// </summary>
            public void MovementUpDown()
            {
                if ((W || S) || (A || D))
                {
                    idle = false;
                    if (I || K)
                    {
                        ourDrone.velocity = ourDrone.velocity;
                    }
                    if (!I && !K && !J && !L)
                    {
                        ourDrone.velocity = new Vector3(ourDrone.velocity.x, Mathf.Lerp(ourDrone.velocity.y, 0, Time.deltaTime * 5), ourDrone.velocity.z);
                        //upForce = 98.001f;//271
                        upForce = ourDrone.mass * 9.81001f;
                    }
                    if (!I && !K && (J || L))
                    {
                        ourDrone.velocity = new Vector3(ourDrone.velocity.x, Mathf.Lerp(ourDrone.velocity.y, 0, Time.deltaTime * 5), ourDrone.velocity.z);
                        //upForce = 98.005f;//110
                        upForce = ourDrone.mass * 9.81005f;
                    }
                }
                if ((!W || !S) && (A || D))
                {
                    idle = false;
                    //upForce = 98.002f;//136
                    upForce = ourDrone.mass * 9.81002f;
                }
                if ((W || S) && (A || D))
                {
                    idle = false;
                    if (J || L)
                    {
                        //upForce = 98.003f;//410
                        upForce = ourDrone.mass * 9.81003f;
                    }
                }
                if (I)
                {
                    idle = false;
                    upForce = forceUpHover * Vertical_I + ((1 - Vertical_I) * ourDrone.mass * 9.81003f); //450
                }
                else if (K)
                {
                    idle = false;
                    upForce = forceDownHover * -Vertical_K;//-200
                }
                else if (!I && !K && ((!W && !S) && (!A && !D)))
                {
                    upForce = ourDrone.mass * 9.81f;
                    idle = true;
                }
            }

            /// <summary>
            /// Handling left right movement and appying forces, also handling the titls
            /// </summary>
            public void MovementLeftRight()
            {
                if (A)
                {
                    ourDrone.AddRelativeForce(Vector3.right * Horizontal_A * sideMovementAmount);
                    tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, -wantedSideTilt * Horizontal_A, ref tiltVelocitySideways, tiltMovementSpeed);
                }
                if (D)
                {
                    ourDrone.AddRelativeForce(Vector3.right * Horizontal_D * sideMovementAmount);
                    tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, -wantedSideTilt * Horizontal_D, ref tiltVelocitySideways, tiltMovementSpeed);
                }
                if (!A && !D)
                {
                    tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 0, ref tiltVelocitySideways, tiltNoMovementSpeed);
                }
            }

            /// <summary>
            /// Handling rotations
            /// </summary>
            public void Rotation()
            {
				if(customFeed == false)
				{
					if (joystick_turned_on == false)
					{
						if (J)
						{
							wantedYRotation -= rotationAmount;
						}
						if (L)
						{
							wantedYRotation += rotationAmount;
						}
					}
					else
					{
						wantedYRotation += rotationAmount * Horizontal_J; // || Horizontal_L it doesnt matter since it's the same axis...
					}
				}
				else
				{
					if (J)
					{
						wantedYRotation += Horizontal_J * rotationAmount;
					}
					if (L)
					{
						wantedYRotation += Horizontal_L * rotationAmount;
					}
				}
                
                currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, 0.25f);
            }

            /// <summary>
            /// Movement forwards and backwars and tilting
            /// </summary>
            public void MovementForward()
            {
                if (W)
                {
                    ourDrone.AddRelativeForce(Vector3.forward * Vertical_W * movementForwardSpeed);
                    tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, wantedForwardTilt * Vertical_W, ref tiltVelocityForward, tiltMovementSpeed);
                }
                if (S)
                {
                    ourDrone.AddRelativeForce(Vector3.forward * Vertical_S * movementForwardSpeed);
                    tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, wantedForwardTilt * Vertical_S, ref tiltVelocityForward, tiltMovementSpeed);
                }
                if (!W && !S)
                {
                    tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 0, ref tiltVelocityForward, tiltNoMovementSpeed);
                }
            }

            #endregion

            #region PRIVATE Coroutine METHODS

            /// <summary>
            /// Keeps trying to find camera until we find it.
            /// </summary>
            IEnumerator FindMainCamera()
            {
                while (!mainCamera)
                {
                    try
                    {
                        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
                    }
                    catch (System.Exception e)
                    {
                        print("<color=red>Missing main camera! check the tags!</color> -> " + e);
                    }
                    yield return new WaitForEndOfFrame();
                }
            }

            /// <summary>
            /// Using animation trigger like this because, I missed up the animation creation,
            /// it should have been triggered and used like a trigger in the animation but,
            /// for some reasons I did it in this way, I will change it later, but it works for now
            /// </summary>
            IEnumerator Left_Passage()
            {
                animatedGameObject.SetBool("left_passage", true);
                yield return new WaitForSeconds(0.5f);
                animatedGameObject.SetBool("left_passage", false);
            }
            IEnumerator Right_Passage()
            {
                animatedGameObject.SetBool("right_passage", true);
                yield return new WaitForSeconds(0.5f);
                animatedGameObject.SetBool("right_passage", false);
            }
            IEnumerator Twirl_left_Method()
            {
                animatedGameObject.SetBool("twirl_left", true);
                yield return new WaitForSeconds(0.5f);
                animatedGameObject.SetBool("twirl_left", false);
            }
            IEnumerator Twirl_right_Method()
            {
                animatedGameObject.SetBool("twirl_right", true);
                yield return new WaitForSeconds(0.5f);
                animatedGameObject.SetBool("twirl_right", false);
            }

            #endregion
 
        }
    }
}