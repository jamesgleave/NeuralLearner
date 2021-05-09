using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Brain : MonoBehaviour
{
    public abstract List<float> GetAction(List<float> obs);
    public abstract void Setup();
    public abstract void Setup(Model.BaseModel model);
    public abstract Model.BaseModel GetModel();
}
