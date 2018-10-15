using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeManager : MonoBehaviour
{
    public static OfficeManager instance = null;

    GameObject[] workers;

    public int lunchTime;

    public bool isBreak;
    public bool isLunch;
    public bool isMorning = true;

    public Clock clock;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        clock = GameObject.Find("ClockText").GetComponent<Clock>();
    }


    // Use this for initialization
    void Start()
    {
        workers = GameObject.FindGameObjectsWithTag("Worker");
    }

    private void Update()
    {

    }

    public bool IsBreak()
    {
        if (clock.hours >= 9.0f && clock.hours < 10.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsLunch()
    {
        if (clock.hours >= 12.0f && clock.hours < 13.30f)
        {
            return true;
        }
        else
        {
            return false;
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
