using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnovationGenerator
{
    private int current_innovation = 0;

    /// <summary>
    /// Returns and increments the current innovation number
    /// </summary>
    /// <returns></returns>
    public int GetInnovation()
    {
        return current_innovation++;
    }

    /// <summary>
    /// Returns the current innovation value but does not increment it
    /// </summary>
    /// <returns></returns>
    public int Query()
    {
        return current_innovation;
    }

    /// <summary>
    /// Set the innovation count
    /// </summary>
    /// <returns></returns>
    public void SetProgress(int progress)
    {
        current_innovation = progress;
    }
}
