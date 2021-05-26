using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeatNet : Model.BaseModel
{
    public override Model.BaseModel Copy()
    {
        throw new System.NotImplementedException();
    }

    public override List<float> Infer(List<float> inputs)
    {
        throw new System.NotImplementedException();
    }
}

/// <summary>
/// The type of a neat neuron
/// </summary>
public enum NEATNodeType
{
    Input,
    Output,
    Hidden,
}
