using UnityEngine;

public class WaypointDetector : MonoBehaviour {

    private TrackManager trackManager;
    private BoxCollider thisCollider;

    private AudioSource passSound;
    //initalized upon starting game
    private void Awake()
    {
        try
        {
            trackManager = GameObject.Find("TrackManager").GetComponent<TrackManager>(); //getting the track manager class component
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Did not found TrackManager by name, try checking your track manager name. ->" + ex);
        }

        try
        {
            passSound = transform.parent.Find("sound").GetComponent<AudioSource>();
        }
        catch(System.Exception ex)
        {
            Debug.LogError("Did not found sound gameobject. -> " + ex);
        }

        thisCollider = GetComponent<BoxCollider>(); //getting the box collider componente to turn this to trigger so we dont have to do it manually
        thisCollider.isTrigger = true; //turning trigger on
    }

    //checks for entring trigger
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.tag == "Player")//searches for the player tag in the root of the transform (all drones must be player tags)
        {
            if (passSound) passSound.Play();
            trackManager.PassedThroughThisPoint(transform.parent); //let track manager known which waypoint we just passed(returning parent of detector transform because that one is stored in the array)
        }
    }
}
