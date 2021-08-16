using System.Collections.Generic;

namespace Model
{
    public abstract class BaseModel
    {
        public abstract List<float> Infer(List<float> inputs);
        public abstract BaseModel Copy();
        public abstract float GetComplexity();
    }
}