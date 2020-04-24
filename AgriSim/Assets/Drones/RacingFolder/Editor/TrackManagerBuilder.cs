using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrackManager))]
public class TrackManagerBuilder : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TrackManager trackManager = (TrackManager)target;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //  EditorGUILayout.HelpBox("Hover NAME OF VARIABLES for properties to find out more about them. If you're not sure what they are used for feel free to contact me via e-mail or watch the youtube tutorials i prepared first.", MessageType.Info);
        EditorGUILayout.LabelField("USE THIS TO CREATE CUSTOM WAYPOINTS", EditorStyles.toolbarButton);

        trackManager.waypointPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Waypoint Prefab","Your setup waypoint prefab here."), trackManager.waypointPrefab, typeof(GameObject), true);

        if (GUILayout.Button("Create waypoint"))
        {
            Selection.activeGameObject = trackManager.CreateWaypoint();
        }

        if(GUILayout.Button("Enable/Disable waypoint line"))
        {
            trackManager.gameObject.GetComponent<WaypointRenderer>().enabled = !trackManager.gameObject.GetComponent<WaypointRenderer>().enabled;
            trackManager.gameObject.GetComponent<LineRenderer>().enabled = !trackManager.gameObject.GetComponent<LineRenderer>().enabled;
            trackManager.gameObject.GetComponent<LineRendererShader>().enabled = !trackManager.gameObject.GetComponent<LineRendererShader>().enabled;
        }
    }

}
