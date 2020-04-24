using System.Collections;
using UnityEngine;

public class TrackManager : MonoBehaviour
{

    private int waypointCounter;
    [Tooltip("Array of our waypoints, they get dynamically added and you can re-position them in order if something goes wrong.(Probably will not)")]
    public Transform[] waypointArray; //put them here in order you wish to circuit or click on a button

    private void Awake()
    {
        //check if we have anywaypoints
        if (transform.childCount == 0)
            gameObject.SetActive(false);
    }
    private void Start()
    {
        FindWayPoints(); //finds allwaypoints if the inital list is 0, must be childed to this manager
        DisableTriggers();
    }

    public void FindWayPoints()
    {
        waypointArray = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            waypointArray[i] = transform.GetChild(i);
        }
    }

    void DisableTriggers()
    {
        foreach (Transform _t in waypointArray)
        {
            _t.Find("Detector").gameObject.SetActive(false);
        }

        ActivateNextWayPoint();
    }

    public void PassedThroughThisPoint(Transform _waypoint)
    {
        DeactivateCurrentWayPoint();

        if (waypointCounter == waypointArray.Length - 1)
        {
            waypointCounter = 0;
        }
        else
        {
            waypointCounter++;
        }

        LapTimerMethod();
        ActivateNextWayPoint();
    }

    [Tooltip("Color of waypoints that will be emmitting when they are not the next in the row to be passed through.")]
    public Color inactiveWaypointColor;
    [Tooltip("Color of single next waypoint that is second to come.")]
    public Color nextWaypointColor;
    [Tooltip("Color of current waypoint we need to pass through.")]
    public Color activeWaypointColor;
    [Tooltip("Range of light emitting from waypoints.")]
    [Range(0, 20)]
    public float lightRange = 10;
    [Tooltip("Volume of sound effect when passing through waypoint.")]
    [Range(0.0f, 1.0f)]
    public float waypointSound = 0.2f;
    void ActivateNextWayPoint()
    {
        //finding child objects
        Transform neonPipes = waypointArray[waypointCounter].Find("NeonPipes");
        Transform detectorsChild = waypointArray[waypointCounter].Find("Detector");
        Transform lights = waypointArray[waypointCounter].Find("lights");

        //activate trigger
        detectorsChild.gameObject.SetActive(true);

        //paint current waypoint gate
        MeshRenderer meshRenderer_NeonPipes = neonPipes.GetComponent<MeshRenderer>();
        meshRenderer_NeonPipes.materials[0].SetColor("_Color", activeWaypointColor);
        meshRenderer_NeonPipes.materials[0].SetColor("_EmissionColor", activeWaypointColor);
        //paint lights
        foreach (Transform lightChild in lights)
        {
            lightChild.GetComponent<Light>().color = activeWaypointColor;
        }


        //actiavte seoncd waypoint and paint
        ActivateSecondNextWayPoint();
    }

    public void LightRangeUpdate()
    {
        foreach (Transform t in waypointArray)
        {
            Transform lights = t.Find("lights");
            if (lights)
                foreach (Transform l in lights)
                {
                    if (l)
                        l.GetComponent<Light>().range = lightRange;
                }
        }
    }

    public void WaypointSoundLevel()
    {
        foreach (Transform t in waypointArray)
        {
            Transform sound = t.Find("sound");
            if (sound)
                sound.GetComponent<AudioSource>().volume = waypointSound;

        }
    }

    /*DONT USE THIS!
    public AudioClip blipSound;
    public bool goOnce = false;
    public void AddSound()
    {
        if(goOnce == false)
        foreach (Transform t in waypointArray)
        {

            if (t.Find("sound"))
                print("gggggg");
            else
            {
                if (blipSound)
                {
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    DestroyImmediate(go.GetComponent<MeshFilter>());
                    DestroyImmediate(go.GetComponent<BoxCollider>());
                    DestroyImmediate(go.GetComponent<MeshRenderer>());
                    go.AddComponent<AudioSource>();
                    go.GetComponent<AudioSource>().clip = blipSound;
                        go.GetComponent<AudioSource>().playOnAwake = false;
                        go.GetComponent<AudioSource>().spatialBlend = 1.0f;
                        go.GetComponent<AudioSource>().volume = 0.1f;
                    go.name = "sound";
                    go.transform.SetParent(t);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.Euler(Vector3.zero);
                }
                else
                    print("no sound");

            }
        }
        goOnce = true;
    }
    */

    void ActivateSecondNextWayPoint()
    {
        int secondNextWayPoint = 0;
        if (waypointCounter + 1 == waypointArray.Length)
        {
            secondNextWayPoint = 0;
        }
        else
        {
            secondNextWayPoint = waypointCounter + 1;
        }

        //finding child objects
        Transform neonPipes = waypointArray[secondNextWayPoint].Find("NeonPipes");
        Transform detectorsChild = waypointArray[secondNextWayPoint].Find("Detector");
        Transform lights = waypointArray[secondNextWayPoint].Find("lights");

        //deactivate trigger
        detectorsChild.gameObject.SetActive(false);

        //paint current waypoint gate
        MeshRenderer meshRenderer_NeonPipes = neonPipes.GetComponent<MeshRenderer>();
        meshRenderer_NeonPipes.materials[0].SetColor("_Color", nextWaypointColor);
        meshRenderer_NeonPipes.materials[0].SetColor("_EmissionColor", nextWaypointColor);
        //paint lights
        foreach (Transform lightChild in lights)
        {
            lightChild.GetComponent<Light>().color = nextWaypointColor;
        }

    }

    void DeactivateCurrentWayPoint()
    {
        //finding child objects
        Transform neonPipes = waypointArray[waypointCounter].Find("NeonPipes");
        Transform detectorsChild = waypointArray[waypointCounter].Find("Detector");
        Transform lights = waypointArray[waypointCounter].Find("lights");

        //deactivate trigger
        detectorsChild.gameObject.SetActive(false);

        //paint current waypoint gate
        MeshRenderer meshRenderer_NeonPipes = neonPipes.GetComponent<MeshRenderer>();
        meshRenderer_NeonPipes.materials[0].SetColor("_Color", inactiveWaypointColor);
        meshRenderer_NeonPipes.materials[0].SetColor("_EmissionColor", inactiveWaypointColor);
        //paint lights
        foreach (Transform lightChild in lights)
        {
            lightChild.GetComponent<Light>().color = inactiveWaypointColor;
        }
    }

    //IM A BIG Batman FAN - "Anakin Skywalker"
    //p.s. don't feed the troll
    void OnePaintMethodToRuleThemAll()
    {

    }

    [Tooltip("Max time so we can set scores.")]
    public float bestLapTime = 999;
    [Tooltip("Current lap time.")]
    public float lapTime;
    IEnumerator myMethod;
    [Tooltip("UI Text to connect score times if you like.")]
    public UnityEngine.UI.Text bestLapTime_UI;
    [Tooltip("UI Text to connect score times if you like.")]
    public UnityEngine.UI.Text lapTime_UI;
    void LapTimerMethod()
    {
        if (waypointCounter == 1)
        {
            //print("Lap started!");
            myMethod = TrackLapTime();
            StartCoroutine(myMethod);
        }
        else if (waypointCounter == 0)
        {
            //print("Lap ended");
            if(onTrackManagerFinished != null)
                onTrackManagerFinished();

            StopCoroutine(myMethod);
            if (lapTime < bestLapTime)
            {
                // print("Congratulations! New lap time!");
                bestLapTime = lapTime;
                if (bestLapTime_UI)
                {
                    bestLapTime_UI.text = lapTime.ToString("f2") + " sec";
                }
            }
        }
    }

    IEnumerator TrackLapTime()
    {
        lapTime = 0;

        while (true)
        {
            yield return new WaitForEndOfFrame();
            lapTime += Time.deltaTime * 1;
            if (lapTime_UI)
            {
                lapTime_UI.text = lapTime.ToString("f2") + " sec";
            }
        }
    }

    //called from editor inspector
    [Header("Use this to create waypoints.")]
    [HideInInspector]
    public GameObject waypointPrefab;
    public GameObject CreateWaypoint()
    {
        GameObject newWaypoint = Instantiate(waypointPrefab, transform);
        newWaypoint.transform.rotation = Quaternion.Euler(Vector3.zero);
        newWaypoint.transform.localScale = Vector3.one;
        newWaypoint.transform.localPosition = Vector3.zero;

        return newWaypoint;
    }

    private void OnDrawGizmos()
    {
        GameObject p0;
        GameObject pm0;
        GameObject p1;
        foreach (Transform t in waypointArray)
        {
            p0 = t.Find("p0").gameObject;
            pm0 = t.Find("pm0").gameObject;
            p1 = t.Find("p1").gameObject;

            if (p0.activeSelf == true)
            {
                Gizmos.color = new Color(1, 0, 0, 0.6f);
                Gizmos.DrawSphere(p0.transform.position, 0.5f);
            }
            if (pm0.activeSelf == true)
            {
                Gizmos.color = new Color(1, 0, 0, 0.6f);
                Gizmos.DrawSphere(pm0.transform.position, 0.5f);
            }
            if (p1.activeSelf == true)
            {
                Gizmos.color = new Color(1, 0, 0, 0.6f);
                Gizmos.DrawSphere(p1.transform.position, 0.5f);
            }
        }
    }

    public delegate void TrackManagerStatus();
    public static TrackManagerStatus onTrackManagerFinished;

}