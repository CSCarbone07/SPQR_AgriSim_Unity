using DroneController.CameraMovement;

public class DroneCamera : CameraScript {

    override public void Awake()
    {
        base.Awake(); //I would suggest you to put code below this line or in a Start() method
    }

	private void FixedUpdate()
	{
		FPVTPSCamera();
		ScrollMath();
	}

	void Update()
    {
	
		PickDroneToControl();
    }

}
