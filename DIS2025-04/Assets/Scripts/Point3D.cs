using System;

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
