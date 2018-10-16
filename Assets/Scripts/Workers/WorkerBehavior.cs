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
    void GoToCoffeeMachine()
    {
        // Doesn't want coffee
        if (!CoffeeDrinker() || mind.thirst < 50)
        {
            Task.current.Fail();
        }

        if (Task.current.isStarting)
        {
            mind.shouldRefresh = true;
        }

        mind.GoToCoffeeMachine();

        if (movement.closeEnough && movement.IsAtCoffeeMachine())
        {
            Task.current.Succeed();
        }
    }

    [Task]
    bool IsThirsty()
    {
        return mind.thirst > 50;
    }

    [Task]
    void DrinkCoffee()
    {
        if(mind.thirst < 50)
        {
            print("Fail");
            Task.current.Fail();
        }
        mind.thirst = 0;
        Task.current.Succeed();
    }

    [Task]
    void GoToSnackMachine()
    {
        if (mind.hunger < 50)
        {
            Task.current.Fail();
        }

        if (Task.current.isStarting)
        {
            mind.shouldRefresh = true;
        }

        mind.GoToSnackMachine();

        if (movement.closeEnough && movement.IsAtSnackMachine())
        {
            Task.current.Succeed();
        }
    }

    [Task]
    void EatSnack()
    {
        mind.hunger = 25;
        Task.current.Succeed();
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
    void Eat()
    {
        mind.hunger = 0;
    }

    [Task]
    void GoToRestaurant()
    {
        

        if (Task.current.isStarting)
        {
            mind.shouldRefresh = true;
        }

        if (IsLunch() && mind.hunger > 50)
        {
            mind.GoToRestaurant();
        }
        else
        {
            Task.current.Fail();
        }

        if (movement.IsAtRestaurant())
        {
            Task.current.Succeed();
        }
    }

    [Task]
    void EatLunch()
    {
        mind.hunger = 0;
        Task.current.Succeed();
    }

    [Task]
    bool NeedToPee()
    {
        return mind.bladder > 50;
    }

    [Task]
    void GoToBathroom()
    {
        // No need to wee
        if (mind.bladder < 75)
        {
            Task.current.Fail();
        }

        if (Task.current.isStarting)
        {
            mind.shouldRefresh = true;
        }

        mind.GoToBathroom();

        if (movement.IsAtToilet())
        {
            Task.current.Succeed();
        }
    }

    [Task]
    void Wee()
    {
        mind.bladder = 0;
        Task.current.Succeed();
    }
}