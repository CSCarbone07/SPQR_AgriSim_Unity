using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomInput : MonoBehaviour
{
    public DroneMovement droneMovementScript;
    public float timer = 0f;
    public float desiredTime = 10f;


    public Vector3 inputSpeed = Vector3.zero;
    public Vector3 actualPos = Vector3.zero;
    public Vector3 actualOrientation = Vector3.zero;
    public Vector3 desiredPos = Vector3.zero;
    public Vector3 desiredVelocity = Vector3.zero;
    public Vector3 actualVelocity = Vector3.zero;
    private Vector3 displacement = Vector3.zero;
    private Vector3 dotDisplacement = Vector3.zero;

    public Vector3 Kp = new Vector3(.8f, .1f, .8f);
    public Vector3 Kd = new Vector3(.05f, .1f, .05f);
    //public Vector3 Ki = new Vector3(.1f, .1f, .1f);
    public Vector3 C = new Vector3(0, 0, 0);
    private float radius = 10f;
    public GameObject target;

    //public Matrix4x4 matrix;

    //#########################################################Working variable linear
    public Vector3 oldPosition = Vector3.zero;


    //#########################################################Working variable angular
    public float targetAngle;
    public float robotAngle;
    public Vector3 robotRobotAngle;
    public float angularSpeed = 0f;

    private float oldTargetAngle = 0f;
    public float desiredAngleVel = 0f;

    public float KpAngle = .05f;
    public float KdAngle = .005f;
    public float angleDisplacement;
    public float targetRobotAngle;
    //public float targetWorldAngle;

    //########################################################ON OFF
    public bool rotation = true;
    public bool movement = true;

    // Start is called before the first frame update
    void Start()
    {
        droneMovementScript = GetComponent<DroneMovement>();

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime % 60;
        //matrix = HomogeneusMatrix(transform);
        if (rotation)
        {
            RotateDrone();
        }

        if (movement)
        {
            MoveDrone();
        }

    }


    private void MoveDrone()
    {
        actualPos = transform.position;
        desiredPos = target.transform.position;
        displacement = desiredPos - actualPos;

        desiredVelocity = DesiredLinearVelocity();
        actualVelocity = droneMovementScript.ourDrone.velocity;
        dotDisplacement = desiredVelocity - actualVelocity;

        inputSpeed = transform.InverseTransformVector(Vector3.Scale(Kp, displacement + Vector3.Scale(Kd, dotDisplacement)));
        //inputSpeed = matrix.inverse.MultiplyVector(Vector3.Scale(Kp, displacement + Vector3.Scale(Kd, dotDisplacement)));

        ApplyMovement();

    }

    private Vector3 DesiredLinearVelocity()
    {
        Vector3 desiredVel = Vector3.zero;
        desiredVel.x = -(2 * Mathf.PI * radius / desiredTime) * Mathf.Sin(timer / desiredTime * 2 * Mathf.PI);
        desiredVel.z = (2 * Mathf.PI * radius / desiredTime) * Mathf.Cos(timer / desiredTime * 2 * Mathf.PI);
        return desiredVel;
    }


    private void ApplyMovement()
    {
        Vector3 relative = transform.InverseTransformPoint(target.transform.position);

        if (relative.z >= 0.0f)
        {
            Move_backward(0.0f);
            Move_forward(inputSpeed.z);
        }
        else
        {
            Move_forward(0.0f);
            Move_backward(-inputSpeed.z);
        }


        if (relative.x >= 0.0f)
        {
            Move_leftward(0.0f);
            Move_rightward(inputSpeed.x);
        }
        else
        {
            Move_rightward(0.0f);
            Move_leftward(-inputSpeed.x);
        }

        if (relative.y >= 0.0f)
        {
            Move_down(0.0f);
            Move_up(inputSpeed.y);
        }
        else
        {
            Move_up(0.0f);
            Move_down(-inputSpeed.y);
        }
    }

    public void RotateDrone()
    {
        robotAngle = transform.rotation.eulerAngles.y;
        targetAngle = target.transform.rotation.eulerAngles.y;

        angleDisplacement = Mathf.DeltaAngle(robotAngle, targetAngle);
        desiredAngleVel = (targetAngle - oldTargetAngle) / Time.deltaTime;
        float dotError = desiredAngleVel - droneMovementScript.ourDrone.angularVelocity.y;

        angularSpeed = (KpAngle * angleDisplacement) + (KdAngle * dotError);
        ApplyRotation();

        oldTargetAngle = targetAngle;
    }

    private void ApplyRotation()
    {
        if (angleDisplacement <= 0.0f)
        {
            //print("Rotating LEFT of: " + angularSpeed);
            Rotate_right(0f);
            Rotate_left(-angularSpeed);
        }

        if (angleDisplacement > 0.0f)
        {
            //print("Rotating RIGHT of: " + angularSpeed);
            Rotate_left(0f);
            Rotate_right(angularSpeed);
        }
    }

    private Matrix4x4 HomogeneusMatrix(Transform transformation)
    {
        Matrix4x4 transform_matrix = Matrix4x4.zero;
        transform_matrix.SetTRS(transformation.position, transformation.rotation, Vector3.one);
        return transform_matrix;
    }



    private void Stop_movement()
    {
        droneMovementScript.customFeed_forward = 0;
        droneMovementScript.customFeed_leftward = 0;
        //droneMovementScript.customFeed_rotateLeft = 0;
    }

    private void Move_forward(float fwd)
    {
        print("Move_forward");
        droneMovementScript.customFeed_forward = fwd;
    }

    private void Move_backward(float bwd)
    {
        print("Move_backward");
        droneMovementScript.customFeed_backward = bwd;
    }

    private void Move_up(float up)
    {
        print("Move_up");
        droneMovementScript.customFeed_upward = up;
    }

    private void Move_down(float down)
    {
        print("Move_down");
        droneMovementScript.customFeed_downward = down;
    }

    private void Move_leftward(float lwd)
    {
        print("Move_leftward");
        droneMovementScript.customFeed_leftward = lwd;
    }

    private void Move_rightward(float rwd)
    {
        print("Move_rightward");
        droneMovementScript.customFeed_rightward = rwd;
    }

    public void Rotate_left(float rotLeft)
    {
        print("Rotate_left");
        droneMovementScript.customFeed_rotateLeft = rotLeft;
    }

    public void Rotate_right(float rotRight)
    {
        print("Rotate_right");
        droneMovementScript.customFeed_rotateRight = rotRight;
    }



    private float EvaluateAngleY(float angle)
    {
        if (angle < 0)
        {
            return 360f + angle;
        }
        return angle;
    }

    //private float DesiredOrientation()
    //{
    //    desiredangY = -Mathf.Atan2(desiredPos.z, desiredPos.x) * Mathf.Rad2Deg;
    //    //target.transform.rotation = (new Quaternion(0f, desiredangY, 0f, 1f));
    //    target.transform.eulerAngles = new Vector3(0f, desiredangY, 0);
    //    return desiredangY;
    //}
}