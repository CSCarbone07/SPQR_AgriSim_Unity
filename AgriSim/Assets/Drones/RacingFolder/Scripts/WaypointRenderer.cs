using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointRenderer : MonoBehaviour {

    private int amountOfFullPaths = 1;
    [Range(5,50)]
    [Tooltip("Smoothness of our line. (The higher the smoother)")]
    public int pointsInAPath = 5;
    private List<Transform> startingPoints;
    private List<Transform> middlePoints;
    private List<Transform> endingPoints;
    
    private LineRenderer lineRenderer;
    private Vector3[] positions;
    
    void Start () {
        FindLineRenderer();
        InitalizeLists();
        FindChildrenAndWaypointAnchors();
    }
    
    public void FindLineRenderer()
    {
        if (!lineRenderer)
        {
            if (GetComponent<LineRenderer>())
            {
                lineRenderer = GetComponent<LineRenderer>();
            }
            else
            {
                gameObject.AddComponent<LineRenderer>();
                lineRenderer = GetComponent<LineRenderer>();
            }
        }
    }
    public void InitalizeLists()
    {
        startingPoints = new List<Transform>();
        middlePoints = new List<Transform>();
        endingPoints = new List<Transform>();
    }
    public void FindChildrenAndWaypointAnchors()
    {
        foreach (Transform t in transform)
        {
            foreach (Transform child in t)
            {
                if (child.name == "p0")
                {
                    startingPoints.Add(child);
                }
                else if (child.name == "pm0")
                {
                    middlePoints.Add(child);
                }
                else if (child.name == "p1")
                {
                    endingPoints.Add(child);
                }
            }
        }

        if(startingPoints.Count > 1)
        {
            for (int i = 1; i < startingPoints.Count; i++)
            {
                startingPoints[i] = endingPoints[i - 1];
            }

            RemoveUnecessaryChildren();
        }
    }
    private void RemoveUnecessaryChildren()
    {
        // because we do not want to destroy first p0 child
        for(int i = 1; i < transform.childCount; i++)
        {
            Transform currentChild = transform.GetChild(i);
            for(int j = 0; j < currentChild.childCount; j++)
            {
                Transform subChild = currentChild.GetChild(j);
                if (subChild.name == "p0")
                {
                    subChild.gameObject.SetActive(false);
                }
            }
        }
    }

    private void Update()
    {
        DrawQuadraticCurve();
    }

    public void DrawQuadraticCurve()
    {
        amountOfFullPaths = startingPoints.Count;
        positions = new Vector3[amountOfFullPaths * pointsInAPath];
        lineRenderer.positionCount = amountOfFullPaths * pointsInAPath;

        for (int i = 0; i < amountOfFullPaths; i++)
        {
            {
                for (int j = 0; j < pointsInAPath; j++)
                {
                    float t = (j+ .5f) / (float)pointsInAPath;
                    positions[(pointsInAPath * i) + j] = CalculateQuadraticBezierPoint(t, startingPoints[i].position, middlePoints[i].position, endingPoints[i].position);
                }
            }
        }
         lineRenderer.SetPositions(positions);

    }
    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }

}


