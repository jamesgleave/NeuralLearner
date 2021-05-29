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
}
