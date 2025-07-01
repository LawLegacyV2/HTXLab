namespace SphereApi.Models;

public class SphereRequest
{
    public Point3D SphereCenter { get; set; } = default!;
    public double Radius { get; set; }
    public Point3D Point { get; set; } = default!;
}
