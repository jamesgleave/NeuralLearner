using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WideFOV : PassiveAbility
{

    private float width_factor;
    private float original_width;
    public WideFOV(float cost, BaseAgent agent, float width_factor) : base(cost, agent)
    {
        this.width_factor = width_factor;
    }

    protected override void Deactivate()
    {

    }

    protected override void ApplyEffect()
    {

    }
}

public class Sleep : PassiveAbility
{
    public Sleep(float cost, BaseAgent agent) : base(cost, agent)
    {

    }
}