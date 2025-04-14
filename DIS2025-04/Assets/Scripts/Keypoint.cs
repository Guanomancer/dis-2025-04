using System;
using UnityEngine;

public class Keypoint
{
    public double X { get; set; }
    public double Y { get; set; }
    public string Name { get; set; }

    public Keypoint() { }

    public Keypoint(string name, double x, double y)
    {
        Name = name;
        X = x;
        Y = y;
    }

    public Keypoint(string name, Vector2 vec)
    {
        Name = name;
        X = vec.x;
        Y = vec.y;
    }

    public Vector2 ToVector2()
    {
        return (new Vector2((float)X, (float)Y));
    }

    // Implicit conversion *to* Vector2 is fine
    public static implicit operator Vector2(Keypoint k) => new Vector2((float)k.X, (float)k.Y);

    public static Keypoint FromVector2(string name, Vector2 vec)
    {
        return new Keypoint(name, vec);
    }

    public override string ToString()
    {
        return $"(Name: {Name}, X: {X}, Y: {Y})";
    }

    // Equality comparison
    public override bool Equals(object obj)
    {
        if (obj is not Keypoint other)
            return false;

        return Name == other.Name && X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, X, Y);
    }

    // Clone method
    public Keypoint Clone()
    {
        return new Keypoint(Name, X, Y);
    }

    // Deconstruction
    public void Deconstruct(out string name, out double x, out double y)
    {
        name = Name;
        x = X;
        y = Y;
    }

}