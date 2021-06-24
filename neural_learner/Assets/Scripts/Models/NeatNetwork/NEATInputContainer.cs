using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEATInputContainer
{
    private NEATInputType input_type;
    private float value;

    public NEATInputContainer()
    {
        input_type = NEATInputType.FeedForward;
        value = float.NaN;
    }

    /// <summary>
    /// Set the input type of the input
    /// </summary>
    /// <param name="t"></param>
    public void SetInputType(NEATInputType t)
    {
        input_type = t;
    }

    /// <summary>
    /// Set the value of the input
    /// </summary>
    /// <param name="v"></param>
    public void SetValue(float v)
    {
        // Throw an exception if the input type is not a recurrent and we try to set the value.
        // Recurrent connections are never reset so they may act as 'memory'
        if (float.IsNaN(value) == false && input_type != NEATInputType.Recurrent)
        {
            throw new System.Exception("Value can only be set if value is float.NaN! Reset before setting value.");
        }
        value = v;
    }

    /// <summary>
    /// Returns the type of this input (NEATInputType.FeedForward, NEATInputType.Recurrent, or NEATInputType.Unknown).
    /// </summary>
    /// <returns></returns>
    public NEATInputType GetInputType()
    {
        return input_type;
    }

    /// <summary>
    /// Returns the input's current value.
    /// </summary>
    /// <returns></returns>
    public float GetInputValue()
    {
        if (IsNaN() && input_type == NEATInputType.Recurrent)
        {
            return 0;
        }
        else
        {
            return value;
        }
    }

    /// <summary>
    /// Returns true if the internally stored 'value' is float.NaN
    /// </summary>
    /// <returns></returns>
    public bool IsNaN()
    {
        return float.IsNaN(value);
    }

    /// <summary>
    /// Reset the input value to float.NaN
    /// </summary>
    public void ResetValue()
    {
        // Only reset non-recurrent values
        if (input_type != NEATInputType.Recurrent)
        {
            value = float.NaN;
        }
    }

    public bool IsReady()
    {
        return (float.IsNaN(value) == false) || input_type == NEATInputType.Recurrent;
    }

    public override string ToString()
    {
        return "<Type: " + input_type + ", " + "Value: " + GetInputValue() + ">";
    }

    /// <summary>
    /// Represents the different types of inputs possible in the NEAT Network
    /// </summary>
    public enum NEATInputType
    {
        FeedForward, Recurrent, Unknown
    }
}
