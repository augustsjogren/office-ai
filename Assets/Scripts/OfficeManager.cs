using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeManager : MonoBehaviour
{
    public static OfficeManager instance = null;

    GameObject[] workers;

    float breakTime;

    public bool isBreak;
    public bool isLunch;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }


    // Use this for initialization
    void Start()
    {
        workers = GameObject.FindGameObjectsWithTag("Worker");
    }

    private void Update()
    {
        breakTime += Time.deltaTime;

        if (breakTime > 10)
        {
            isBreak = true;
        }

        if(breakTime > 30)
        {
            isBreak = false;
            breakTime = 0.0f;
        }
    }

    public void CoffeeBreak()
    {
        foreach (var worker in workers)
        {
            worker.GetComponent<WorkerMind>().GetCoffee();
        }
    }

    public void BackToWork()
    {
        foreach (var worker in workers)
        {
            worker.GetComponent<WorkerMind>().Work();
        }
    }
}
