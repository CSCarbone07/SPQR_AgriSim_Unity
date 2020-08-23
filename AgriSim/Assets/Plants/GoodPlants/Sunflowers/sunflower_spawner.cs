using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using Random = UnityEngine.Random;

public class sunflower_spawner : SpawnerAndSwitch
{

    public Material TestMaterial;

    public GameObject[] rootLeaf;// = new GameObject[] { beetLeaf1 };
    public GameObject[] rootLeaf_NIR;

    public GameObject[] headLeaf;// = new GameObject[] { beetLeaf1 };
    public GameObject[] headLeaf_NIR;

    public GameObject[] Leaf_TAG;


    private int maxLeafAmount = 2;
    public float plantDeltaHeight = 2;  //height between root leaves and head leaves
    public Vector3 leafScale = new Vector3(1, 1, 1);

    private GameObject[] createdPrefab_rootLeaves;
    private int[] createdPrefab_rootLeaves_Type;

    private GameObject[] createdPrefab_headLeaves;
    private int[] createdPrefab_headLeaves_Type;

    protected Vector3 randomRotationValue;
    protected Quaternion newRotation;
    public Vector3 plantRotation = new Vector3(200f, 0f, -90f);
    public Renderer[] rend;
    public Renderer thisRend;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        thisRend = GetComponent<Renderer>();
        //OnDrawGizmosSelected();
        /*
        rend = new Renderer[createdPrefabLeaves.Length];
        int n = 0;
        foreach(GameObject leaf in createdPrefabLeaves)
        {
            rend[n]=leaf.GetComponentInChildren<SkinnedMeshRenderer>();
            n++;
        }
        */
        //OnDrawGizmosSelected();
        //Debug.Log("gizmosss");

    }



    /*
    // Draws a wireframe sphere in the Scene view, fully enclosing
    // the object.
    void OnDrawGizmos()
    {
        // A sphere that fully encloses the bounding box.


        
        foreach(Renderer r in rend)
        {
            Vector3 center = r.bounds.center;
            float radius = r.bounds.extents.magnitude;

            Debug.Log(center.ToString("F4"));
            Debug.Log(radius.ToString("F4"));

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(center, radius);
        }


        Vector3 center = createdPrefabLeaves[0].GetComponentInChildren<SkinnedMeshRenderer>().bounds.center;
        float radius = createdPrefabLeaves[0].GetComponentInChildren<SkinnedMeshRenderer>().bounds.extents.magnitude;
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(center, radius);
    }
    */

    // Update is called once per frame
    public override void Update()
    {

    }

    public override void Spawn()
    {
        base.Spawn();
        //Debug.Log("I am a Good Plant");
        //GameObject createdPrefabStem = new GameObject();
        createdPrefab_rootLeaves = new GameObject[maxLeafAmount];
        createdPrefab_rootLeaves_Type = new int[maxLeafAmount];

        createdPrefab_headLeaves = new GameObject[maxLeafAmount];
        createdPrefab_headLeaves_Type = new int[maxLeafAmount];

        // Leaf Spawn
        //Vector3 tempPosition = position;
        Vector3 tempPosition = this.transform.position;
        tempPosition[1] += 0.8f;


        // first spawn root leaves
        for (int x = 0; x < maxLeafAmount; x++)
        {
            Debug.Log("spawning leaf: " + x);


            randomRotationValue = new Vector3(0f, x * 50f, 0f);
            plantRotation[1] += UnityEngine.Random.Range(-16.0f, 16.0f);
            //plantRotation[0] += UnityEngine.Random.Range(-5.0f, 5.0f);
            newRotation = Quaternion.Euler(plantRotation + randomRotationValue);
            GameObject createdPrefabLeaf;

            int typeOfLeaf = Random.Range(0, rootLeaf.Length - 1);
            createdPrefab_rootLeaves_Type[x] = typeOfLeaf;
            createdPrefabLeaf = Instantiate(rootLeaf[typeOfLeaf], tempPosition, newRotation);

            if (UnityEngine.Random.Range(0f, 10f) < 9.5f)
            {
                createdPrefabLeaf.transform.localScale = leafScale * UnityEngine.Random.Range(0.5f, 1f);
            }
            else
            {
                createdPrefabLeaf.transform.localScale = leafScale * 0;
            }
            createdPrefabLeaf.SetActive(true);
            createdPrefabLeaf.AddComponent<MeshRenderer>();
            createdPrefabLeaf.transform.SetParent(this.transform);
            createdPrefab_rootLeaves[x] = createdPrefabLeaf;



            //Debug.Log("lower limit: " + lowerLimit);
            //Debug.Log("upper limit: " + upperLimit);

            //createdPrefabLeaf.transform.SetParent(createdPrefabStem.transform);
        }
        //SwitchToNIR();


    }

    // calculate signed triangle area using a kind of "2D cross product":
    float Area(Vector2 p1, Vector2 p2, Vector2 p3) 
    {
    Vector2 v1 = p1 - p3;
    Vector2 v2 = p2 - p3;
        //return (v1.x* v2.y - v1.y* v2.x)/2;
        return ((p1.x * (p2.y - p3.y) + p2.x * (p3.y - p1.y) + p3.x * (p1.y - p2.y)) / 2);
    }

    public override void SwitchToNIR()
    {
        base.SwitchToNIR();
        for (int x = 0; x < maxLeafAmount; x++)
        {
            GameObject createdPrefabLeaf = Instantiate(rootLeaf_NIR[createdPrefab_rootLeaves_Type[x]], createdPrefab_rootLeaves[x].transform.position, createdPrefab_rootLeaves[x].transform.rotation);
            createdPrefabLeaf.transform.localScale = createdPrefab_rootLeaves[x].transform.localScale;
            //(beetLeaf_NIR[createdPrefabLeavesType[x]], createdPrefab_rootLeaves[x]);
            createdPrefabLeaf.transform.SetParent(this.transform);
            Destroy(createdPrefab_rootLeaves[x]);
            createdPrefab_rootLeaves[x] = createdPrefabLeaf;
            //Debug.Log(createdPrefabLeavesType[x]);
        }

    }

    public override void SwitchToTAG()
    {
        base.SwitchToNIR();
        for (int x = 0; x < maxLeafAmount; x++)
        {
            GameObject createdPrefabLeaf = Instantiate(Leaf_TAG[createdPrefab_rootLeaves_Type[x]], createdPrefab_rootLeaves[x].transform.position, createdPrefab_rootLeaves[x].transform.rotation);
            createdPrefabLeaf.transform.localScale = createdPrefab_rootLeaves[x].transform.localScale;
            //(beetLeaf_NIR[createdPrefabLeavesType[x]], createdPrefab_rootLeaves[x]);
            createdPrefabLeaf.transform.SetParent(this.transform);
            Destroy(createdPrefab_rootLeaves[x]);
            createdPrefab_rootLeaves[x] = createdPrefabLeaf;
            //Debug.Log(createdPrefabLeavesType[x]);
        }

    }
}
