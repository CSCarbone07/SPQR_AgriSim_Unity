using UnityEngine;
using DroneController.CameraMovement;
[ExecuteInEditMode]
public class UIEditorScript : MonoBehaviour
{

    #region PUBLIC VARIABLS

    [TextArea(4, 10)]
    public string description = "EDITOR SCRIPT. THIS SCRIPT IS USED TO DETERMINE HOW MANY DRONES ARE THERE ON THE SCENE, AND IF THERE IS MORE THAN 1 DRONE IT WILL ACTIVATE THE 'PICK MENU' AUTOMATICALLY.";
    public CameraScript cs;

    #endregion

    #region PRIVATE VARIABLES

    private bool goOnce = false;

    #endregion

    #region Mono Behaviour METHODS

    void Update()
    {
        if (!cs)
            cs = GetComponent<CameraScript>();

        if (cs)
        {
            CanvasDroneSelect();
            CanvasTrackTime();
        }
    }

    #endregion

    #region PRIVATE METHODS

    /// <summary>
    /// Checks how many drones are there in the scene, enables disables the UI buttons to select
    /// </summary>
    void CanvasDroneSelect()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length > 1)
        {
            if (goOnce == false)
            {
                foreach (GameObject g in cs.canvasSelectButtons)
                {
                    if (g)
                        g.SetActive(true);
                }
                foreach (GameObject g in cs.canvasExitButtons)
                {
                    if (g)
                        g.SetActive(true);
                }
                goOnce = true;
            }
        }
        else
        {
            foreach (GameObject g in cs.canvasSelectButtons)
            {
                if (g)
                    g.SetActive(false);
            }
            foreach (GameObject g in cs.canvasExitButtons)
            {
                if (g)
                    g.SetActive(false);
            }
            goOnce = false;
        }
    }

    /// <summary>
    /// Tries to find track manager to display time
    /// </summary>
    void CanvasTrackTime()
    {
        if (GameObject.Find("TrackManager"))
        {
            if (cs.canvasTimeTrack)
                cs.canvasTimeTrack.SetActive(true);
        }
        else
        {
            if (cs.canvasTimeTrack)
                cs.canvasTimeTrack.SetActive(false);
        }
    }

    #endregion

}
