using System;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BooleanTrigger
{
    private bool boolean;
    
    private Delegate function;

    /// <summary>
    /// Create a trigger boolean which calls the lambda expression when the boolean is updated.
    /// </summary>
    /// <param name="boolean"></param>
    /// <param name="function"></param>
    public BooleanTrigger(bool boolean, Delegate function)
    {
        this.boolean = boolean;
        this.function = function;
    }

    /// <summary>
    /// Create a trigger boolean which calls the lambda expression when the boolean is updated.
    /// </summary>
    /// <param name="boolean"></param>
    /// <param name="function"></param>
    public BooleanTrigger(bool boolean)
    {
        this.boolean = boolean;
    }

    public void SetFunction(Delegate function){
        this.function = function;
    }

    /// <summary>
    /// Change the state
    /// </summary>
    /// <param name="b"></param>
    public virtual void Set(bool b){
        if(function == null){
            boolean = b;
        }else{
            // Check if we have an update
            if(b != this.boolean){
                // Update the boolean
                this.boolean = b;
                // Call the function
                function.DynamicInvoke();
            }
        }
    }

    public static bool operator true(BooleanTrigger x) => x.boolean == true;
    public static bool operator false(BooleanTrigger x) => x.boolean == false;
    public static bool operator !(BooleanTrigger x) => !x.boolean;
    public static bool operator &(BooleanTrigger x, BooleanTrigger y) => x.boolean && y.boolean;
    public static bool operator |(BooleanTrigger x, BooleanTrigger y) => x.boolean || y.boolean;
    public static bool operator ==(BooleanTrigger x, BooleanTrigger y) => x.boolean == y.boolean;
    public static bool operator !=(BooleanTrigger x, BooleanTrigger y) => x.boolean != y.boolean;
}
