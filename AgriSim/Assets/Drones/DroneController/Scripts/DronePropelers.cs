using DroneController.Propelers;

public class DronePropelers : ElisaScript {

    public override void Awake()
    {
        base.Awake(); //I would suggest you to put code below this line or in a Start() method
    }

    void Update()
    {
        RotationInputs();
        RotationDifferentials();
    }

}