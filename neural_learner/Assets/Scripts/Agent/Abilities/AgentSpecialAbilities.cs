using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpecialAbilities
{
    /// <summary>
    /// The agent with this ability
    /// </summary>
    protected BaseAgent attached_agent;

    /// <summary>
    /// The cooldown period for the ability
    /// </summary>
    protected float cooldown_time;

    /// <summary>
    ///  The current cooldown remaining
    /// </summary>
    protected float cooldown;

    /// <summary>
    /// The cost of using the ability
    /// </summary>
    protected float cost;

    /// <summary>
    /// How much the agent wants to activate its ability
    /// </summary>
    protected float desire;

    /// <summary>
    /// The threshold for desire. To activate the ability: desire > activation_threshold
    /// </summary>
    protected float activation_threshold = 0.0f;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cost"></param>
    /// <param name="agent"></param>
    /// <param name="cooldown"></param>

    public AgentSpecialAbilities(float cost, BaseAgent agent, float cooldown)
    {
        Debug.LogWarning("Cost is not being used... Not implemented yet");
        this.cost = cost;
        this.cooldown = cooldown;
        this.attached_agent = agent;
    }

    public virtual void Activate(float desire)
    {
        // Update how much the agent wants to activate the ability
        this.desire = desire;

        // If the cooldown has not reached zero return before doing anything
        if (cooldown > 0)
            return;

        // TODO implement cost for agent using the ability
    }

    public void Update()
    {
        cooldown -= Time.deltaTime;
        cooldown = Mathf.Max(cooldown, 0);
        OnUpdate();
    }

    /// <summary>
    /// OnUpdate is called every time the ability is updated
    /// </summary>
    public virtual void OnUpdate()
    {
        // Does nothing initially
    }

    public float GetCooldown()
    {
        return cooldown;
    }
}


public class PassiveAbility : AgentSpecialAbilities
{
    // Passive abilities remain on once activated.
    // If deactivated, the passive ability will remain off until activated again.

    /// <summary>
    /// True if the passive ability is active
    /// </summary>
    protected bool active;

    public PassiveAbility(float cost, BaseAgent agent) : base(cost, agent, 0)
    {
        active = true;
    }

    public override void Activate(float desire)
    {
        base.Activate(desire);

        // If we desire to activate the effect, do it
        if (!active && this.desire > activation_threshold)
        {
            ApplyEffect();
            active = true;
        }
        // If we are active and dont want the effect, deactivate it
        else if (active && this.desire <= activation_threshold)
        {
            Deactivate();
            active = false;
        }
    }

    protected virtual void Deactivate()
    {

    }

    protected virtual void ApplyEffect()
    {

    }

}

public class ActiveAbility : AgentSpecialAbilities
{
    public ActiveAbility(float cost, BaseAgent agent, float cooldown) : base(cost, agent, cooldown)
    {

    }

    public override void Activate(float desire)
    {
        base.Activate(desire);

        // Return if we dont have enough desire
        if (this.desire <= activation_threshold)
            return;

        cooldown = cooldown_time;
    }
}

public class Help
{
    void test()
    {
        PassiveAbility passive = new PassiveAbility(1, null);
        ActiveAbility active = new ActiveAbility(1, null, 0);

        passive.Activate(0.5f);
        active.Activate(0.5f);

        AgentSpecialAbilities ability = passive;
        ability.Activate(0.5f);
    }
}