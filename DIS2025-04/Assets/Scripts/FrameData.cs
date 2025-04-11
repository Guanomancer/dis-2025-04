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

public class Point3D: Keypoint
{
    public double X3D { get; set; }
    public double Y3D { get; set; }
    public double Z3D { get; set; }

}

public class HandTrackingData
{
    public string DeviceID { get; set; }
    public int FrameIndex { get; set; }
    public string Handedness { get; set; }
    public int Confidence { get; set; }

    public List<Keypoint> Keypoints;
    public List<Keypoint3D> Keypoints3D;
    public List<Point3D> Points3D;

}

