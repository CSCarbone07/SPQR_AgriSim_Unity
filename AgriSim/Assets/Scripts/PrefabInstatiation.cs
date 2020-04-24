using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class PrefabInstatiation : MonoBehaviour
{

    public GameObject prefab;
    //public int numberOfObjects = 20;
    //public float radius = 5f;
    private Vector3 myPosition;// = transform.position;
    //private GameObject createdPrefab;
    public float gridX = 5f;
    public float gridY = 5f;
    public float spacingX = 2f;
    public float spacingY = 2f;
    public Vector3 Rotation; //for rotating the mesh in case the mesh is upside down or something else
    public Vector3 Scale = new Vector3(1,1,1);
    private Quaternion newRotation;


    private Vector3 addRandomRotation;

    [Range(0.0f, 1.0f)]
    public float addRandomRotationX = 0;
    [Range(0.0f, 1.0f)]
    public float addRandomRotationY = 0;
    [Range(0.0f, 1.0f)]
    public float addRandomRotationZ = 0;
    [Range(0.0f, 100.0f)]
    public float Density = 100;

    private Vector3 randomRotationValue;

    public bool updateInstantiation = false;    // Click to update instances
    //private bool canUpdate = true;
    private bool regenerate = true;



    void Instantiate()
    {
        /*
        foreach (Transform child in transform) //this.gameObject.transform)
        {
            //DestroyImmediate(child.gameObject);
            GameObject.DestroyImmediate(child.gameObject);
        }
        */

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }

        myPosition = transform.position;
        newRotation = Quaternion.Euler(Rotation);
        if (regenerate)
        {
            for (int y = 0; y < gridY; y++)
            {
                for (int x = 0; x < gridX; x++)
                {
                    //randomRotationValue = new Vector3(Random.Range(-90.0f, 90.0f)* addRandomRotationX, Random.Range(-90.0f, 90.0f) * addRandomRotationY, Random.Range(-90.0f, 90.0f) * addRandomRotationZ));
                    addRandomRotation = new Vector3(addRandomRotationX, addRandomRotationY, addRandomRotationZ);
                    randomRotationValue = addRandomRotation * (Random.Range(-90.0f, 90.0f));
                    newRotation = Quaternion.Euler(Rotation + randomRotationValue);

                    if((Density/100) >= Random.Range(0.0f, 1.0f))
                    { 
                        Vector3 pos = new Vector3(x * spacingX, 0, y * spacingY) + myPosition;

                        GameObject createdPrefab = Instantiate(prefab, pos, newRotation);
                        createdPrefab.transform.SetParent(this.gameObject.transform); // = this.transform;
                                                                                      //prefab.transform.parent = transform;
                        createdPrefab.transform.localScale = Scale + new Vector3(Random.Range(0.0f, .5f), Random.Range(0.0f, .2f), Random.Range(0.0f, .5f));
                    }
                }
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    //void OnRenderObject()
    {


        if (updateInstantiation)
        {
            Instantiate();
            updateInstantiation = false;
        }


        /*
        if (updateInstantiation)
        {
            if (canUpdate)
            {
                canUpdate = false;
                foreach (Transform child in this.gameObject.transform)
                {
                    DestroyImmediate(child.gameObject);
                }
                Instantiate();

                updateInstantiation = false;
                canUpdate = true;
            }
        }
        */


        /*
        for (int i = 0; i < numberOfObjects; i++)
        {

            float angle = i * Mathf.PI * 2 / numberOfObjects;
            Vector3 pos = (new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius) + myPosition;
            Instantiate(prefab, pos, Quaternion.identity);
         }
        */


        //print(this.gameObject.name);
    }
}
