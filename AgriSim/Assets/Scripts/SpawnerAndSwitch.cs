using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerAndSwitch : MonoBehaviour
{


    // Start is called before the first frame update
    public virtual void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public virtual void Spawn()
    {

    }

    public virtual void SwitchToNIR()
    {
        /*
        foreach (GameObject leaf in createdPrefabLeaf)
        {


        }
        */       
    }

    public virtual void SwitchToTAG()
    {
        /*
        foreach (GameObject leaf in createdPrefabLeaf)
        {


        }
        */
    }
}
