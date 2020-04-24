using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererShader : MonoBehaviour {

    [TextArea(3, 10)]
    public string description = "THIS SCRIPT GETS THE LINE RENDERER MATERIAL AND ADDS OUT MAIN CAMERA POSITION TO THE SHADER TO WORK PROPERLY"; 

    private Transform mainCamera;
    Renderer render;

    void Start()
    {
        render = gameObject.GetComponent<Renderer>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    void Update()
    {

        render.sharedMaterial.SetVector("_PlayerPosition", mainCamera.position);

    }
}
