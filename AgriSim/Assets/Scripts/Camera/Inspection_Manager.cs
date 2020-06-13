using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspection_Manager : MonoBehaviour
{

    public bool takeScreenshot = false;

    public float delayBetweenMoves = 1;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Inspection_Move>().Initialize();
        Loop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Loop()
    {
        if (takeScreenshot)
        {
            this.GetComponent<SaveImage>().TakeScreenshot();
        }
        this.GetComponent<Inspection_Move>().Rellocate();

        Invoke("Loop", delayBetweenMoves);

    }
    /*
    void Manage()
    {

        Loop();
    }
    */
}
