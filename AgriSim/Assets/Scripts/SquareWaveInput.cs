using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquareWaveInput : MonoBehaviour
{
    public DroneMovement droneMovementScript;
    public float timeLeft = 5f;
    public float desiredTime = 10f;

    //################################UI
    public Text fuelText;
    public Image fuelImage;
    public float currentFuel = 5f;
    public float maxFuel = 20f;

    public GameObject gameOverPanel;
    public Text gameOverText;


    //################################Linear
    public Vector3 actualVelocity;
    private Vector3 displacement;
    private Vector3 dotDisplacement;
    private Vector3 linearInput;
    public Vector3 Kp = new Vector3(.8f, .1f, .8f);
    public Vector3 Kd = new Vector3(.05f, .1f, .05f);
    public Vector3 Ki = new Vector3(.1f, .1f, .1f);

    //################################Target variable
    public GameObject target;
    public Vector3 target_old_pos;
    public Vector3 target_new_pos;
    public float target_old_ang;
    public float target_new_ang;


    //################################Angular
    public float robotAngle;
    public float angularSpeed = 0f;
    public float KpAngle = .05f;
    public float KdAngle = .005f;
    public float angleDisplacement;
    public float desiredAngleVel;
    public Vector3 actualAngularVelocity;
    public float dotAngleError;

    //########################################################ON OFF
    public bool rotate = true;
    public bool move = true;

    //##########################################Camera shaking
    //public bool shaking;

    //// How long the object should shake for.
    //public float shakeDuration = 10f;

    //// Amplitude of the shake. A larger value shakes the camera harder.
    //public float shakeAmount = 0.7f;
    //public float decreaseFactor = 1.0f;

    //public Vector3 originalCameraPos;
    //public Quaternion originalCameraRot;


    void Awake()
    {
        gameOverPanel.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {


        droneMovementScript = GetComponent<DroneMovement>();


        robotAngle = transform.rotation.eulerAngles.y;
        target_old_pos = target.transform.position;
        target_old_ang = target.transform.rotation.eulerAngles.y;

        //shaking = false;
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        //RotateDrone();
        if (currentFuel >= 0)
        {
            MoveDrone();
            UpdateUIFuel();

            if (droneMovementScript.ourDrone.velocity.magnitude > 0.2f)
            {
                currentFuel -= .5f * Time.deltaTime * 1 / droneMovementScript.ourDrone.velocity.magnitude;
            }

            else
            {
                currentFuel -= .5f * Time.deltaTime;
            }
        }

        else
        {
            ApplyMovement(new Vector3(0f, 0f, 0f));
            UpdateUIGame();
        }

        //Vector3 movement = new Vector3(droneMovementScript.tiltAmountSideways, 0f, droneMovementScript.tiltAmountForward);
        //print("tilt: " + movement);
    }

    private void MoveDrone()
    {
        robotAngle = transform.rotation.eulerAngles.y;
        displacement = target.transform.position - transform.position;
        Vector3 approximatedVelocity = ApproximatedLinearVelocity();
        actualVelocity = droneMovementScript.ourDrone.velocity;
        dotDisplacement = approximatedVelocity - actualVelocity;
        linearInput = Vector3.Scale(Kp, displacement) + Vector3.Scale(Kd, dotDisplacement); //multiply component by component of a vector

        linearInput = transform.InverseTransformVector(linearInput);
        ApplyMovement(linearInput);

    }



    public void RotateDrone()
    {
        robotAngle = transform.rotation.eulerAngles.y;
        target_new_ang = target.transform.rotation.eulerAngles.y;

        angleDisplacement = Mathf.DeltaAngle(robotAngle, target_new_ang);
        //float desiredAngleVel = Mathf.DeltaAngle(target_new_ang, target_old_ang) / Time.deltaTime;
        desiredAngleVel = (target_new_ang - target_old_ang) / Time.deltaTime;


        //NOTE
        //NOT WORKING ANGULAR VELOCITY
        //NOT WORKING ANGULAR VELOCITY
        //NOT WORKING ANGULAR VELOCITY
        actualAngularVelocity = droneMovementScript.ourDrone.angularVelocity;
        //NOT WORKING ANGULAR VELOCITY
        //NOT WORKING ANGULAR VELOCITY
        //NOT WORKING ANGULAR VELOCITY

        dotAngleError = desiredAngleVel; //- actualAngularVelocity.y;
        angularSpeed = (KpAngle * angleDisplacement) + (KdAngle * dotAngleError);
        ApplyRotation();

        target_old_ang = target_new_ang;
    }





    private void ApplyMovement(Vector3 inputMovement)
    {
        Vector3 relative = transform.InverseTransformPoint(target.transform.position);

        if (relative.z >= 0.0f)
        {
            Move_backward(0.0f);
            Move_forward(inputMovement.z);
            //ApplyShaking('z');
        }
        else
        {
            Move_forward(0.0f);
            Move_backward(-inputMovement.z);
        }


        if (relative.x >= 0.0f)
        {
            Move_leftward(0.0f);
            Move_rightward(inputMovement.x);
        }
        else
        {
            Move_rightward(0.0f);
            Move_leftward(-inputMovement.x);
        }

        if (relative.y >= 0.0f)
        {
            Move_down(0.0f);
            Move_up(inputMovement.y);
        }
        else
        {
            Move_up(0.0f);
            Move_down(-inputMovement.y);
        }
    }

    public Vector3 ApproximatedLinearVelocity()
    {
        target_new_pos = target.transform.position;
        Vector3 velocity = (target_new_pos - target_old_pos) / Time.deltaTime;
        target_old_pos = target_new_pos;
        return velocity;
    }


    private void ApplyRotation()
    {
        if (angleDisplacement <= 0.0f)
        {
            Rotate_right(0.0f);
            Rotate_left(-angularSpeed);
        }

        if (angleDisplacement > 0.0f)
        {
            Rotate_left(0.0f);
            Rotate_right(angularSpeed);
        }
    }

    private void Move_forward(float fwd)
    {
        droneMovementScript.customFeed_forward = fwd;
    }

    private void Move_backward(float bwd)
    {
        droneMovementScript.customFeed_backward = bwd;
    }

    private void Move_up(float up)
    {
        droneMovementScript.customFeed_upward = up;
    }

    private void Move_down(float down)
    {
        droneMovementScript.customFeed_downward = down;
    }

    private void Move_leftward(float lwd)
    {
        droneMovementScript.customFeed_leftward = lwd;
    }

    private void Move_rightward(float rwd)
    {
        droneMovementScript.customFeed_rightward = rwd;
    }

    private void Rotate_left(float angle)
    {
        droneMovementScript.customFeed_rotateLeft = angle;
    }

    private void Rotate_right(float angle)
    {
        droneMovementScript.customFeed_rotateRight = angle;
    }


    public void UpdateUIFuel()
    {
        fuelImage.fillAmount = currentFuel / maxFuel;
        fuelText.text = "Fuel: " + ((int)(currentFuel / maxFuel * 100)).ToString() + "%";
    }

    public void UpdateUIGame()
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = "GAME OVER!";
    }
}