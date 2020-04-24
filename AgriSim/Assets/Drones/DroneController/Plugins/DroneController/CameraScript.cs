using UnityEngine;
using DroneController.Physics;
using System.Collections;
namespace DroneController
{
    namespace CameraMovement
    {
        public class CameraScript : MonoBehaviour
        {

			#region PUBLIC VARIABLES - EDITED THROUGH CUSTOM INSPECTOR EDITOR

			[HideInInspector] public int inputEditorFPS;

            [HideInInspector] public bool FPS;
            [HideInInspector] public LayerMask fpsViewMask;
            [HideInInspector] public Vector3 positionInsideDrone;
            [HideInInspector] public Vector3 rotationInsideDrone;
            [HideInInspector] public float fpsFieldOfView = 90;

            [HideInInspector] public GameObject[] dronesToControl; //used to pick  between more drones in one scene
            [HideInInspector] public bool pickedMyDrone = false; // if we picked our drone or not
            public GameObject ourDrone; //our drone game object

            [Header("Position of the camera behind the drone.")]
            [HideInInspector] public Vector3 positionBehindDrone = new Vector3(0, 2, -4);

            [Tooltip("How fast the camera will follow drone position. (The lower the value the faster it will follow)")]
            [Range(0.0f, 0.1f)]
            [HideInInspector] public float cameraFollowPositionTime = 0.1f;

            [Tooltip("Value where if the camera/drone is moving upwards will raise the camera view upward to get a better look at what is above, same goes when going downwards.")]
            [HideInInspector] public float extraTilt = 10;
            [Tooltip("Parts of drone we wish to see in the third person.")]
            [HideInInspector] public LayerMask tpsLayerMask;
            [HideInInspector] public float tpsFieldOfView = 60;

            [Header("Mouse movement variables")]
            [Tooltip("Allows to freely view around the drone with your mouse and not depending on drone look rotation.")]
            [HideInInspector] public bool freeMouseMovement = false;
			[HideInInspector] public bool useJoystickFreeMovementOnly = false;
			[HideInInspector] public string mouse_X_axisName = "Mouse X", mouse_Y_axisName = "Mouse Y";
			[HideInInspector] public string dPad_X_axisName = "dPad_X", dPad_Y_axisName = "dPad_Y";
			[Tooltip("Value that will determine how fast your free look mouse will behave.")]
            [HideInInspector] public float mouseSensitvity = 100;
            [Tooltip("Value that will follow the camera view behind the mouse movement.(The lower the value, the faster it will follow mouse movement)")]
            [HideInInspector] public float mouseFollowTime = 0.2f;

            #endregion

            #region PRIVATE VARIABLES

            private int counterToControl = 0; //counter which determins what drone we are following
            private Vector3 velocitiCameraFollow;

            private float cameraYVelocity;
            private float previousFramePos;

            private float currentXPos, currentYPos;
            private float xVelocity, yVelocity;

            private float mouseXwanted, mouseYwanted;

            private float zScrollAmountSensitivity = 1, yScrollAmountSensitivity = -0.5f;
            private float zScrollValue, yScrollValue;

            #endregion

            #region PUBLIC VARIABLES

            [Header("Canvas drone selection UI variables")]
            [Tooltip("UI Canvas buttons that are used to select wanting drone.")]
            public GameObject[] canvasSelectButtons;
            [Tooltip("UIU Canvas button that is used to exit the current drone selection.")]
            public GameObject[] canvasExitButtons;
            [Tooltip("Track timer canvas")]
            public GameObject canvasTimeTrack;

            #endregion

            #region MONO BEHAVIOUR METHODS

            public virtual void Awake()
            {
                Drone_Pick_Initialization();
            }

			/*
            void FixedUpdate()
            {       
                FPVTPSCamera();
                ScrollMath();
            }

            void Update()
            {
                PickDroneToControl();
            }
            */

			#endregion

			#region PRIVATE METHODS

			/// <summary>
			/// Checking the scene if there are multiple drones to control, to follow the currently selected one or the last found one, if not
			/// try to found our single drone on the scene
			/// </summary>
			void Drone_Pick_Initialization()
            {
				if (ourDrone)
				{
					pickedMyDrone = true;
					return;
				}

                //resseting sheit
                pickedMyDrone = false;
                ourDrone = null;
				
                dronesToControl = GameObject.FindGameObjectsWithTag("Player");

                //added pick the drone before you fly
                if (dronesToControl.Length > 1)
                {
                    pickedMyDrone = false;
                    ourDrone = dronesToControl[counterToControl].gameObject;
                }
                else
                {
                    StartCoroutine(KeepTryingToFindOurDrone());
                }
            }
            
            //only when FPS is toggled ON
            void FPSCameraPositioning()
            {
                //if (transform.parent == null)
                //    transform.SetParent(ourDrone.GetComponent<DroneMovementScript>().animatedGameObject.transform);

                //if (GetComponent<Camera>().cullingMask != fpsViewMask)
                //    GetComponent<Camera>().cullingMask = fpsViewMask;

                //if (GetComponent<Camera>().fieldOfView != fpsFieldOfView)
                    //GetComponent<Camera>().fieldOfView = fpsFieldOfView;

                transform.localPosition = positionInsideDrone;
                transform.localEulerAngles = rotationInsideDrone;
            }

            //only when FPS is toggled OFF (Third person view)
            void TPSCameraPositioning()
            {
                if (transform.parent != null)
                    transform.SetParent(null);

                if (GetComponent<Camera>().cullingMask != tpsLayerMask)
                    GetComponent<Camera>().cullingMask = tpsLayerMask;

                if (GetComponent<Camera>().fieldOfView != tpsFieldOfView)
                    GetComponent<Camera>().fieldOfView = tpsFieldOfView;

                FollowDroneMethod();
                TiltCameraUpDown();
                FreeMouseMovementView();
            }

            void FollowDroneMethod()
            {
                if (pickedMyDrone && ourDrone)
                    transform.position = Vector3.SmoothDamp(transform.position, ourDrone.transform.TransformPoint(positionBehindDrone + new Vector3(0, yScrollValue, zScrollValue)), ref velocitiCameraFollow, cameraFollowPositionTime);
                else if (pickedMyDrone == false && dronesToControl.Length > 0)
                    transform.position = Vector3.SmoothDamp(transform.position, dronesToControl[counterToControl].transform.TransformPoint(positionBehindDrone + new Vector3(0, yScrollValue, zScrollValue)), ref velocitiCameraFollow, cameraFollowPositionTime);
            }
            
            void TiltCameraUpDown()
            {
                cameraYVelocity = Mathf.Lerp(cameraYVelocity, (transform.position.y - previousFramePos) * -extraTilt, Time.deltaTime * 10);
                previousFramePos = transform.position.y;
            }

            void FreeMouseMovementView()
            {
                if (freeMouseMovement == true)
                {
					float wantedXMouseFreeMovement = ((useJoystickFreeMovementOnly == false) ? Input.GetAxis(mouse_Y_axisName) : 0) * Time.deltaTime * mouseSensitvity;
					float wantedYMouseFreeMovement = ((useJoystickFreeMovementOnly == false) ? Input.GetAxis(mouse_X_axisName) : 0)* Time.deltaTime * mouseSensitvity;

					if(wantedXMouseFreeMovement == 0)
					{
						wantedXMouseFreeMovement = Input.GetAxis(dPad_Y_axisName) * Time.deltaTime * mouseSensitvity;
					}
					if(wantedYMouseFreeMovement == 0)
					{
						wantedYMouseFreeMovement = Input.GetAxis(dPad_X_axisName) * Time.deltaTime * mouseSensitvity;
					}

					mouseXwanted -= wantedXMouseFreeMovement;
                    mouseYwanted += wantedYMouseFreeMovement;

                    currentXPos = Mathf.SmoothDamp(currentXPos, mouseXwanted, ref xVelocity, mouseFollowTime);
                    currentYPos = Mathf.SmoothDamp(currentYPos, mouseYwanted, ref yVelocity, mouseFollowTime);

                    transform.rotation = Quaternion.Euler(new Vector3(14, ourDrone.GetComponent<DroneMovementScript>().currentYRotation, 0)) *
                        Quaternion.Euler(currentXPos, currentYPos, 0);
                }
                else
                {
					if (pickedMyDrone && ourDrone)
						transform.rotation = Quaternion.Euler(new Vector3(14 + cameraYVelocity, ourDrone.transform.rotation.eulerAngles.y, 0));
                    else if (pickedMyDrone == false && dronesToControl.Length > 0)
                        transform.rotation = Quaternion.Euler(new Vector3(14, dronesToControl[counterToControl].transform.rotation.eulerAngles.y, 0));
                }
            }

            #endregion

            #region PUBLIC METHODS

            /// <summary>
            /// Checking which values for camera positioning should we use.
            /// If we are picking the drone, use third person view, and after we select it will switch to first person view
            /// </summary>
            public void FPVTPSCamera()
            {
                if (FPS == true && pickedMyDrone == true)
                {
                    FPSCameraPositioning();
                }
                else
                {
                    TPSCameraPositioning();
                }
            }

            /// <summary>
            /// Handles drone pick in the runtime of the game. (EDITOR SCRIPT IS DIFFERENT, WHEN IT HIDES OR SHOWS UI)
            /// </summary>
            public void PickDroneToControl()
            {
                if (dronesToControl.Length > 1)
                {
                    if (pickedMyDrone == false)
                    {

                        //freeMouseMovement = true;
                        if (canvasSelectButtons.Length != 0)
                        {
                            foreach (GameObject go in canvasSelectButtons)
                            {
                                go.SetActive(true);
                            }
                            foreach (GameObject go in canvasExitButtons)
                            {
                                go.SetActive(false);
                            }
                        }

                        if (Input.GetKeyDown(KeyCode.Return))
                        {
                            Select();
                        }
                        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            PressedLeft();
                        }
                        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            PressedRight();
                        }
                    }
                    else
                    {
                        //freeMouseMovement = false;
                        //mouseXwanted = 0;
                        //	mouseYwanted = 0;
                        if (canvasSelectButtons.Length != 0)
                        {
                            foreach (GameObject go in canvasSelectButtons)
                            {
                                if (go)
                                    go.SetActive(false);
                            }
                            foreach (GameObject go in canvasExitButtons)
                            {
                                if (go)
                                    go.SetActive(true);
                            }
                        }

                        if (Input.GetKeyDown(KeyCode.Escape))
                        {
                            ReturnToPick();
                        }
                    }
                }
                else
                {
                    if (canvasSelectButtons.Length != 0)
                    {
                        foreach (GameObject go in canvasSelectButtons)
                        {
                            if (go)
                                go.SetActive(false);
                        }
                        foreach (GameObject go in canvasExitButtons)
                        {
                            if (go)
                                go.SetActive(false);
                        }
                    }
                }
            }

            /// <summary>
            /// LInked to ESC button and the UI button.
            /// </summary>
            public void ReturnToPick()
            {
                pickedMyDrone = false;

            }

            /// <summary>
            /// LINKED to Enter button and the UI button.
            /// </summary>
            public void Select()
            {
                ourDrone = dronesToControl[counterToControl].gameObject;
                pickedMyDrone = true;
            }

            /// <summary>
            /// Linked to left arrow and the UI button
            /// </summary>
            public void PressedLeft()
            {
                if (counterToControl >= 1)
                {
                    counterToControl--;
                }
                else
                {
                    counterToControl = dronesToControl.Length - 1;
                }
            }

            /// <summary>
            /// Linked to left arrow and the UI button
            /// </summary>
            public void PressedRight()
            {
                if (counterToControl < dronesToControl.Length - 1)
                {
                    counterToControl++;
                }
                else
                {
                    counterToControl = 0;
                }
            }

            /// <summary>
            /// Handles scrolling of mouse wheel to zoomin the camera
            /// </summary>
            public void ScrollMath()
            {
                if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                {
                    zScrollValue += Input.GetAxis("Mouse ScrollWheel") * zScrollAmountSensitivity;
                    yScrollValue += Input.GetAxis("Mouse ScrollWheel") * yScrollAmountSensitivity;
                }
            }

            #endregion

            #region PRIVATE Coroutine METHODS

            /// <summary>
            /// Keep trying to find our drone.
            /// This is needed if you want to connect your drones to network, so first you have your camera, and after you create your drone, you want it to be found by the camera.
            /// So this checks every frame if there is a new drone created.
            /// </summary>
            IEnumerator KeepTryingToFindOurDrone()
            {
                while (ourDrone == null)
                {
                    yield return null;
                    try
                    {
                        ourDrone = GameObject.FindGameObjectWithTag("Player").gameObject;

                        if (ourDrone)
                            pickedMyDrone = true;
                    }
                    catch (System.Exception e)
                    {
                        print("Are you supposed to have only one drone on the scene? <color=red>I can't find it!</color> -> " + e);
                    }
                }
            }

            #endregion

        }
    }
}
