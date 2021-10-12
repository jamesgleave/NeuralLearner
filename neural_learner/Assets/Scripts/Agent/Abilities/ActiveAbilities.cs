using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : ActiveAbility
{

    private float boost_multiplier;
    private float boost_period;
    private float agent_original_speed;
    private float agent_boost_speed;

    public Boost(float cost, BaseAgent agent, float cooldown, float boost_period, float boost_multiplier) : base(cost, agent, cooldown)
    {
        this.boost_multiplier = boost_multiplier;
        this.boost_period = boost_period;

        this.agent_original_speed = agent.movement_speed;
        this.agent_boost_speed = agent.movement_speed * boost_multiplier;
    }

    public override void Activate(float desire)
    {
        base.Activate(desire);
        attached_agent.StartCoroutine(Run());
    }

    public IEnumerator Run()
    {
        attached_agent.movement_speed = agent_boost_speed;
        Debug.Log("Starting");
        yield return new WaitForSeconds(boost_period);
        attached_agent.movement_speed = agent_original_speed;
        Debug.Log("Ending");
    }
}


public class Magnet : ActiveAbility
{
    public Magnet(float cost, BaseAgent agent, float cooldown) : base(cost, agent, cooldown)
    {

    }
}
