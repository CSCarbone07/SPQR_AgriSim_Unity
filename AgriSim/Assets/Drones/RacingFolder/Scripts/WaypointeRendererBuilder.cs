using UnityEngine;

[ExecuteInEditMode]
public class WaypointeRendererBuilder : MonoBehaviour {

    private WaypointRenderer waypointRenderer;
    private TrackManager trackManager;

    void Update() {
        if (!waypointRenderer) {
            waypointRenderer = GetComponent<WaypointRenderer>();
        }
        else
        {
            waypointRenderer.FindLineRenderer();
            waypointRenderer.InitalizeLists();
            waypointRenderer.FindChildrenAndWaypointAnchors();
            waypointRenderer.DrawQuadraticCurve();
        }

        if (!trackManager)
        {
            trackManager = GameObject.Find("TrackManager").GetComponent<TrackManager>();
        }
        else
        {
            trackManager.FindWayPoints();
            trackManager.LightRangeUpdate();
            trackManager.WaypointSoundLevel();
            //trackManager.AddSound();
        }        
	}

    void OnDisable()
    {
        gameObject.GetComponent<WaypointeRendererBuilder>().enabled = true;
    }
}
