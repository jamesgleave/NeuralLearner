using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeCalculationMethod
{
    LinComb, Latch,
}

public static class MethodHelp
{
    public static NodeCalculationMethod GetRandomMethod()
    {
        int choise = Random.Range(0, 2);
        switch (choise)
        {
            case 0:
                return NodeCalculationMethod.LinComb;
            case 1:
                return NodeCalculationMethod.Latch;
        }

        return NodeCalculationMethod.LinComb;
    }
}

