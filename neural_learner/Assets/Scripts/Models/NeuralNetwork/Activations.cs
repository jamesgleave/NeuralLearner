﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Activations
{
    public abstract class Activation
    {
        public string name;
        public abstract float activate(float value);
        public abstract List<float> activate(List<float> values);
    }

    public class Relu : Activation
    {
        public Relu()
        {
            name = "Relu";
        }

        public override float activate(float value)
        {
            // The ReLu function.
            if (value >= 0)
            {
                return value;
            }
            else
            {
                return 0;
            }
        }

        public override List<float> activate(List<float> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                values[i] = activate(values[i]);
            }
            return values;
        }
    }

    public class Linear : Activation
    {
        public Linear()
        {
            name = "Linear";
        }

        public override float activate(float value)
        {
            return value;
        }

        public override List<float> activate(List<float> values)
        {
            return values;
        }
    }

    public class LeakyRelu : Activation
    {
        float alpha;
        public LeakyRelu(float a)
        {
            name = "LeakyRelu" + a.ToString();
            alpha = 1 / a;
        }

        public override float activate(float value)
        {
            // The ReLu function.
            if (value >= 0)
            {
                return value;
            }
            else
            {
                return value * alpha;
            }
        }

        public override List<float> activate(List<float> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                values[i] = activate(values[i]);
            }
            return values;
        }
    }

    public class Tanh : Activation
    {
        public Tanh()
        {
            name = "Tanh";
        }

        public override float activate(float value)
        {
            return (float)Math.Tanh(value);
        }

        public override List<float> activate(List<float> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                values[i] = activate(values[i]);
            }
            return values;
        }
    }

    public class Softmax : Activation
    {
        public Softmax()
        {
            name = "Softmax";
        }

        public override float activate(float value)
        {
            return 0.0f;
        }

        public override List<float> activate(List<float> values)
        {
            float sum = 0;

            for (int i = 0; i < values.Count; i++)
            {
                sum += Mathf.Exp(values[i]);
            }

            for (int i = 0; i < values.Count; i++)
            {
                values[i] = Mathf.Exp(values[i]) / sum;
            }

            return values;
        }
    }

    public class Abs : Activation
    {
        public Abs()
        {
            name = "Abs";
        }

        public override float activate(float value)
        {
            return Mathf.Abs(value);
        }

        public override List<float> activate(List<float> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                values[i] = activate(values[i]);
            }
            return values;
        }
    }

    public class Sigmoid : Activation
    {
        public Sigmoid()
        {
            name = "Sigmoid";
        }

        public override float activate(float value)
        {
            return 1 / (Mathf.Exp(-value) + 1);
        }

        public override List<float> activate(List<float> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                values[i] = activate(values[i]);
            }
            return values;
        }
    }

    public static class ActivationHelper
    {
        public static string GetRandomActivation()
        {
            // Create a list of names
            List<string> names = new List<string>();

            // Add all of the codes
            //names.Add("Softmax");
            names.Add("Sigmoid");
            names.Add("Tanh");
            names.Add("Linear");
            names.Add("Relu");
            names.Add("Abs");

            // Return a random name
            return names[UnityEngine.Random.Range(0, names.Count)];
        }

        public static void FromCode()
        {

        }
    }

}

