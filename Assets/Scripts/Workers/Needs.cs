using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Needs : MonoBehaviour
{

    public float hunger;
    public float thirst;
    public float bladder;

    private float time;

    Clock clock;

    float randomFactor;

    private void Awake()
    {
        clock = GameObject.Find("ClockText").GetComponent<Clock>();
    }

    // Use this for initialization
    void Start()
    {
        randomFactor = OfficeManager.instance.needsRand.Next(0, 50);
        //print(randomFactor);

        time += Time.deltaTime * clock.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNeeds();
    }

    void UpdateNeeds()
    {
        hunger += Time.deltaTime / 1.5f;
        thirst += Time.deltaTime;
        bladder += Time.deltaTime / 2.0f;
    }
}
