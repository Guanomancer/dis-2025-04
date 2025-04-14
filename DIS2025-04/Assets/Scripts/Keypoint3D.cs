using System;

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