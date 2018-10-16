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
    void StandStill()
    {
        print("Idle");
    }

    [Task]
    bool IsHungry()
    {
        return mind.hunger > 50;
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

    bool CoffeeDrinker()
    {
        return mind.coffeeDrinker;
    }

    [Task]
    void GetCoffee()
    {
        if (!CoffeeDrinker() || mind.thirst < 50)
        {
            Task.current.Fail();
        }

        if (Task.current.isStarting)
        {
            mind.shouldRefresh = true;
        }

        mind.GetCoffee();

        if (movement.closeEnough && movement.IsAtCoffeeMachine())
        {
            mind.thirst = 0;
            Task.current.Succeed();
        }
    }

    [Task]
    void GetSnack()
    {
        if (mind.hunger < 50)
        {
            Task.current.Fail();
        }

        if (Task.current.isStarting)
        {
            mind.shouldRefresh = true;
        }

        mind.GetSnack();

        if (movement.closeEnough && movement.IsAtSnackMachine())
        {
            mind.hunger = 25;
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

        if (movement.closeEnough)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    void GetLunch()
    {
        if (!IsLunch() && mind.hunger < 75)
        {
            Task.current.Fail();
        }

        if (Task.current.isStarting)
        {
            mind.shouldRefresh = true;
        }

        mind.GoToRestaurant();
    }

    [Task]
    void GoToBathroom()
    {
        if (mind.bladder < 75)
        {
            Task.current.Fail();
        }

        if (Task.current.isStarting)
        {
            mind.shouldRefresh = true;
        }

        mind.VisitBathroom();

        if (movement.IsAtToilet())
        {
            mind.bladder = 0;
            Task.current.Succeed();
        }
    }
}