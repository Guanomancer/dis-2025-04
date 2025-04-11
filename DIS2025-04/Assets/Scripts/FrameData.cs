using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class Keypoint
{
    public double X {  get; set; }
    public double Y { get; set; }
    public string Name { get; set; }

}

public class Keypoint3D: Keypoint
{
    public double Z { get; set; }
}