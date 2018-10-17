using UnityEngine;
using Panda;

public class WorkerBehavior : MonoBehaviour
{
    WorkerMind mind;
    WorkerMovement movement;
    Needs needs;

    private void Awake()
    {
        mind = GetComponent<WorkerMind>();
        movement = GetComponent<WorkerMovement>();
        needs = GetComponent<Needs>();
    }

    [Task]
    void StandStill()
    {
        print("Idle");
    }

    [Task]
    bool IsHungry()
    {
        return needs.hunger > 50;
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
        if (!CoffeeDrinker() || needs.thirst < 50)
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
        return needs.thirst > 50;
    }

    [Task]
    void DrinkCoffee()
    {
        if(needs.thirst < 50)
        {
            print("Fail");
            Task.current.Fail();
        }
        needs.thirst = 0;
        needs.bladder += 25;
        Task.current.Succeed();
    }

    [Task]
    void GoToSnackMachine()
    {
        if (needs.hunger < 50)
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
        needs.hunger -= 25;
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
        needs.hunger = 0;
    }

    [Task]
    void GoToRestaurant()
    {
        if (Task.current.isStarting)
        {
            mind.shouldRefresh = true;
        }

        if (IsLunch() && needs.hunger > 25)
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
        needs.hunger = 0;
        Task.current.Succeed();
    }

    [Task]
    bool NeedToPee()
    {
        return needs.bladder > 50;
    }

    [Task]
    void GoToBathroom()
    {
        // No need to wee
        if (needs.bladder < 75)
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
        needs.bladder = 0;
        Task.current.Succeed();
    }
}