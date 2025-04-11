using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class Keypoint
{
    public double x {  get; set; }
    public double y { get; set; }
    public string name { get; set; }

}

public class Keypoint3D: Keypoint
{
    public double z { get; set; }
}