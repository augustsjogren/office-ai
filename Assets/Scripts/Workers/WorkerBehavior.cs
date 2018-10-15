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
    void Idle()
    {
        
    }

    [Task]
    void StandStill()
    {
        print("Idle");
    }

    [Task]
    bool IsBreak()
    {
        return OfficeManager.instance.IsBreak();
    }

    [Task]
    bool IsLunch()
    {
        return OfficeManager.instance.IsLunch();
    }

    [Task]
    bool HasCoffee()
    {
        return mind.HasCoffee();
    }

    [Task]
    void GetCoffee()
    {
        if (Task.current.isStarting)
        {
            mind.shouldRefresh = true;
        }

        mind.GetCoffee();

        if(mind.IsAtCoffeeMachine() && mind.HasCoffee())
        {
            Task.current.Succeed();
        }
    }

    [Task]
    bool IsAtDesk()
    {
        return mind.IsAtDesk();
    }

    [Task]
    void Work()
    {
        if (Task.current.isStarting)
        {
            mind.shouldRefresh = true;
        }
        mind.Work();

        if (mind.IsAtDesk())
        {
            Task.current.Succeed();
        }
    }

    [Task]
    void GetLunch()
    {
        if (Task.current.isStarting)
        {
            mind.shouldRefresh = true;
        }

        mind.GoToRestaurant();

    }
}