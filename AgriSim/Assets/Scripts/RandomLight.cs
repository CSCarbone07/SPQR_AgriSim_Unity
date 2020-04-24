//using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using System.IO;

public class RandomLight : MonoBehaviour
{
    Light lt;
    Color color;
    Vector3 pos;
    public float lowest_intensity_multiplier = 0.8f;
    public float highest_intensity_multiplier = 1.2f;

    private float spawnDelay = 3f;
    private float nextSpawnTime = 0f;
    private float intensity;
    private float rotationX;
    private float rotationZ;

    // Start is called before the first frame update
    void Start()
    {
        lt = GetComponent<Light>();
        color = lt.color;
        pos = transform.position;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void changeLight()
    {
            intensity = Random.Range(lowest_intensity_multiplier, highest_intensity_multiplier);
            lt.color = color * intensity;

            rotationX = Random.Range(60f, 120f);
            rotationZ = Random.Range(60f, 120f);
            lt.transform.rotation = Quaternion.Euler(new Vector3(rotationX, 0f, rotationZ));

            nextSpawnTime = Time.time + spawnDelay;
    }

    private bool ShouldSpawn()
    {
        return Time.time >= nextSpawnTime;
    }
}
