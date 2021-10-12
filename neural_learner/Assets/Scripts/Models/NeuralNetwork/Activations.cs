using System.Collections;
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

        public float Derivative(float value)
        {
            if (value <= 0)
            {
                return 0;
            }
            else
            {
                return 1;
            }
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

        public float Derivative(float value)
        {
            return 1;
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

        public float Derivative(float value)
        {
            // 1-tanh(x)^2
            return 1 - Mathf.Pow(activate(value), 2);
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

        public float Derivative(float value)
        {
            if (value == 0)
            {
                return 0;
            }
            else if (value > 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
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

        public float Derivative(float value)
        {
            return activate(value) - (1 - activate(value));
        }
    }

    public class Invert : Activation
    {
        public Invert()
        {
            name = "Invert";
        }

        public override float activate(float value)
        {
            return -value;
        }

        public override List<float> activate(List<float> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                values[i] = activate(values[i]);
            }
            return values;
        }

        public float Derivative(float value)
        {
            return -1;
        }
    }


    public class IncorrectActivationException : Exception
    {

    }

    public static class ActivationHelper
    {
        // Create a list of names
        private static List<string> names = new List<string>();

        private static void AddNames()
        {
            // Add all of the codes
            names.Add("Tanh");
            names.Add("Linear");
            names.Add("Relu");
            names.Add("Abs");
            names.Add("Invert");
        }

        public static string GetRandomActivation()
        {
            // If this function is called for the first time, we add the names to the static name list
            if (names.Count == 0)
            {
                AddNames();
            }

            // Return a random name
            return names[UnityEngine.Random.Range(0, names.Count)];
        }

        public static Activation FromName(string activ)
        {
            // Return a linear activation
            if (activ.Equals("Linear"))
            {
                return new Activations.Linear();
            }

            // Return a Relu activation
            if (activ.Equals("Relu"))
            {
                return new Activations.Relu();
            }

            // Return a Tahnh activation
            if (activ.Contains("Tanh"))
            {
                return new Activations.Tanh();
            }

            // Return a absolute value activation
            if (activ.Contains("Abs"))
            {
                return new Activations.Abs();
            }

            // Return a Sigmoid activation
            if (activ.Contains("Sigmoid"))
            {
                return new Activations.Sigmoid();
            }

            // Return an Invert activation
            if (activ.Contains("Invert"))
            {
                return new Activations.Invert();
            }

            throw new IncorrectActivationException();
            //return new Activations.Linear();
        }
    }
}

