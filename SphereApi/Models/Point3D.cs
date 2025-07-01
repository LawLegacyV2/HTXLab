namespace SphereApi.Models;

public record Point3D(double X, double Y, double Z)
{
    public double DistanceSquaredTo(Point3D other) =>
        Math.Pow(X - other.X, 2) +
        Math.Pow(Y - other.Y, 2) +
        Math.Pow(Z - other.Z, 2);
}
