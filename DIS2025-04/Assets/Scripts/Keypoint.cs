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

}