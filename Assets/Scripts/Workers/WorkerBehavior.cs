using UnityEngine;
using Panda;

public class WorkerBehavior : MonoBehaviour
{
    WorkerMind mind;
    private void Awake()
    {
        mind = GetComponent<WorkerMind>();
    }

    [Task]
    void SetWorkerColor(float r, float g, float b)
    {
        this.GetComponent<Renderer>().material.color = new Color(r, g, b);
        Task.current.Succeed();
    }

    [Task]
    void GetCoffee()
    {
        mind.GetCoffee();
        Task.current.Complete(mind.isCloseEnough());
    }

    [Task]
    void Work()
    {
        mind.Work();
        // Task.current.Complete(mind.isCloseEnough());
    }
}