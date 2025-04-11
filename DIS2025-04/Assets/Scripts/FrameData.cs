using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class Keypoint
{
    public double x;
    public double y;
    public string name;

    public void SetX(double value)
    {
        x = value;
    }

    public void SetY(double value)
    {
        y = value;
    }

    public void SetName(string value)
    {
        name = value;
    }

    public double GetX()
    {
        return x;
    }

    public double GetY()
    {
        return y;
    }

    public string GetName()
    {
        return name;
    }
}