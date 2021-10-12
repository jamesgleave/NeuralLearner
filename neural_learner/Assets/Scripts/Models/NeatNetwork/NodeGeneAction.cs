﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeCalculationMethod
{
    LinComb, Latch,
}

////////////////////////////////////////////////////////////////////////////////////////////////

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

    public static NodeCalculationMethod FromName(string name)
    {
        switch (name)
        {
            case "LinComb":
                return NodeCalculationMethod.LinComb;
            case "Latch":
                return NodeCalculationMethod.Latch;
        }
        return NodeCalculationMethod.LinComb;
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////

public abstract class CalculationMethod
{
    /// <summary>
    /// The kind of method the object uses
    /// </summary>
    public NodeCalculationMethod method;

    /// <summary>
    /// Calculates the output for each method
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public abstract float Calculate(float input, Activations.Activation activation);

    /// <summary>
    /// Returns the calculation method enum
    /// </summary>
    /// <returns></returns>
    public abstract NodeCalculationMethod GetMethodName();
}

////////////////////////////////////////////////////////////////////////////////////////////////
////////////     Subclasses of Calculation Methods     /////////////////
////////////////////////////////////////////////////////////////////////////////////////////////


public class LinComb : CalculationMethod
{
    public LinComb()
    {
        method = NodeCalculationMethod.LinComb;
    }

    public override float Calculate(float input, Activations.Activation activation)
    {
        return activation.activate(input);
    }

    public override NodeCalculationMethod GetMethodName()
    {
        return this.method;
    }
}


public class Latch : CalculationMethod
{

    // Latch stores the previous output value for later use
    private float previous_output = 0;

    public Latch()
    {
        method = NodeCalculationMethod.Latch;
    }

    public override float Calculate(float input, Activations.Activation activation)
    {
        // Activate the input
        input = activation.activate(input);

        // Latch Logic
        if (previous_output == 1 && input > 0 || input >= 0.80f)
        {
            previous_output = 1;
        }
        else
        {
            previous_output = 0;
        }

        // Return the output
        return previous_output;
    }

    public override NodeCalculationMethod GetMethodName()
    {
        return this.method;
    }
}