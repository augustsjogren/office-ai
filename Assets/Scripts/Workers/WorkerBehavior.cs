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
        mind.GetCoffee();
        Task.current.Complete(!mind.IsAtCoffeeMachine() && !mind.HasCoffee());
    }


    [Task]
    bool IsAtDesk()
    {
        return mind.IsAtDesk();
    }

    [Task]
    void Work()
    {
        mind.Work();
        Task.current.Complete(!mind.IsAtDesk());
    }

    [Task]
    void GetLunch()
    {
        mind.GoToRestaurant();

    }
}