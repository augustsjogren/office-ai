using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeMachine : MonoBehaviour
{
    public int cupsLeft;

    public bool CupsLeft()
    {
        return cupsLeft > 0;
    }

    public void DispenseCup()
    {
        if (CupsLeft())
        {
            cupsLeft--;
        }
    }
}
