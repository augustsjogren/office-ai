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
    public bool isMorning = true;

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
            //isBreak = true;
            //isLunch = true;
            //isMorning = false;
        }

        if (breakTime > 20)
        {
            
            //isBreak = false;
            //isLunch = false;
            breakTime = 0.0f;
        }
    }

    public void ToggleLunch()
    {
        isLunch = !isLunch;
    }

    public void ToggleBreak()
    {
        isBreak = !isBreak;
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
