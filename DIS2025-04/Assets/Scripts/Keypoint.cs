using System;

public class Keypoint
{
    public double X { get; set; }
    public double Y { get; set; }
    public string Name { get; set; }

    public Keypoint() { }

    public Keypoint(string name, double x, double y, double? z = null)
    {
        Name = name;
        X = x;
        Y = y;
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