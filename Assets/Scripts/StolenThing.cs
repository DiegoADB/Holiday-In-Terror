using System.Collections;
using System.Collections.Generic;

public class StolenThing
{
    int id;
    float value;
    string name;

    //Constructor
    public StolenThing(int _id, float _value, string _name)
    {
        id = _id;
        value = _value;
        name = _name;
    }
    
    public int GetId()
    {
        return id;
    }
    public float GetValue()
    {
        return value;
    }
    public string GetName()
    {
        return name;
    }
}
