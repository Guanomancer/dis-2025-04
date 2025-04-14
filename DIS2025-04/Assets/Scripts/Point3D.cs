using System;
using UnityEngine;

public class Point3D : Keypoint
{
    public double X3D { get; set; }
    public double Y3D { get; set; }
    public double Z3D { get; set; }

    public Point3D() { }

    public Point3D(string name, double x, double y, double x3D, double y3D, double z3D)
    {
        Name = name;
        X = x;
        Y = y;
        X3D = x3D;
        Y3D = y3D;
        Z3D = z3D;
    }

    public Point3D(string name, Vector2 vec2, Vector3 vec3)
    {
        Name = name;
        X = vec2.x;
        Y = vec2.y;
        X3D = vec3.x;
        Y3D = vec3.y;
        Z3D = vec3.z;
    }

    public new Vector2 ToVector2()
    {
        return new Vector2((float)X, (float)Y);
    }

    public Vector3 ToVector3()
    {
        return new Vector3((float)X3D, (float)Y3D, (float)Z3D);
    }

    public static implicit operator Vector2(Point3D p) => new Vector2((float)p.X, (float)p.Y);
    public static implicit operator Vector3(Point3D p) => new Vector3((float)p.X3D, (float)p.Y3D, (float)p.Z3D);

    public static Point3D FromVectors(string name, Vector2 vec2, Vector3 vec3)
    {
        return new Point3D(name, vec2, vec3);
    }

    public override string ToString()
    {
        return $"(Name: {Name}, X: {X}, Y: {Y}, X3D: {X3D}, Y3D: {Y3D}, Z3D: {Z3D})";
    }

    // Equality comparison
    public override bool Equals(object obj)
    {
        if (obj is not Point3D other)
            return false;

        return Name == other.Name &&
               X == other.X &&
               Y == other.Y &&
               X3D == other.X3D &&
               Y3D == other.Y3D &&
               Z3D == other.Z3D;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, X, Y, X3D, Y3D, Z3D);
    }

    // Clone method
    public new Point3D Clone()
    {
        return new Point3D(Name, X, Y, X3D, Y3D, Z3D);
    }

    // Deconstruction
    public void Deconstruct(out string name, out double x, out double y, out double x3D, out double y3D, out double z3D)
    {
        name = Name;
        x = X;
        y = Y;
        x3D = X3D;
        y3D = Y3D;
        z3D = Z3D;
    }
}
