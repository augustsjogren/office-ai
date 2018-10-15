using UnityEngine;
using Panda;

public class WorkerBehavior : MonoBehaviour
{
    WorkerMind mind;
    WorkerMovement movement;

    private void Awake()
    {
        mind = GetComponent<WorkerMind>();
        movement = GetComponent<WorkerMovement>();
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
        return movement.HasCoffee();
    }

    [Task]
    void GetCoffee()
    {
        if (Task.current.isStarting)
        {
            mind.shouldRefresh = true;
        }

        mind.GetCoffee();

        if(movement.IsAtCoffeeMachine() && movement.HasCoffee())
        {
            Task.current.Succeed();
        }
    }

    [Task]
    bool IsAtDesk()
    {
        return movement.IsAtDesk();
    }

    [Task]
    void Work()
    {
        if (Task.current.isStarting)
        {
            mind.shouldRefresh = true;
        }
        mind.Work();

        if (movement.IsAtDesk())
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