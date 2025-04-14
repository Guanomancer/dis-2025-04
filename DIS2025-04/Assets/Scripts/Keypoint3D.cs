using System;
using UnityEngine;

public class Keypoint3D : Keypoint
{
    public double Z { get; set; }

    public Keypoint3D() { }

    public Keypoint3D(string name, double x, double y, double z)
    {
        Name = name;
        X = x;
        Y = y;
        Z = y;
    }

    public Keypoint3D(string name, Vector3 vec)
    {
        Name = name;
        X = vec.x;
        Y = vec.y;
        Z = vec.z;
    }

    public Vector2 ToVector3()
    {
        return (new Vector3((float)X, (float)Y, (float)Z));
    }

    public static implicit operator Vector3(Keypoint3D k) => new Vector3((float)k.X, (float)k.Y, (float)k.Z);

    public static Keypoint3D FromVector3(string name, Vector3 vec)
    {
        return new Keypoint3D(name, vec);
    }

    public override string ToString()
    {
        return $"(Name: {Name}, X: {X}, Y: {Y}, Z: {Z})";
    }

    // Equality comparison
    public override bool Equals(object obj)
    {
        if (obj is not Keypoint3D other)
            return false;

        return Name == other.Name && X == other.X && Y == other.Y && Z == other.Z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, X, Y, Z);
    }

    // Clone method
    public new Keypoint3D Clone()
    {
        return new Keypoint3D(Name, X, Y, Z);
    }

    // Deconstruction
    public void Deconstruct(out string name, out double x, out double y, out double z)
    {
        name = Name;
        x = X;
        y = Y;
        z = Z;
    }

}