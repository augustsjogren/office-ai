using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeManager : MonoBehaviour
{
    public static OfficeManager instance = null;

    GameObject[] workers;

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
