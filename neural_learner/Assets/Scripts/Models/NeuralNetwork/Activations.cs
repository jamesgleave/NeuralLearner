using System;

namespace Activations
{
    public abstract class Activation
    {
        public string name;
        public abstract float activate(float value);
        public abstract float[] activate(float[] values);
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

        public override float[] activate(float[] values)
        {
            for (int i = 0; i < values.Length; i++)
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

        public override float[] activate(float[] values)
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

        public override float[] activate(float[] values)
        {
            for (int i = 0; i < values.Length; i++)
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

        public override float[] activate(float[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = activate(values[i]);
            }
            return values;
        }
    }
}

