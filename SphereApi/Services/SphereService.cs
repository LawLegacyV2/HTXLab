using SphereApi.Models;

namespace SphereApi.Services;

public class SphereService
{
    public string GetPointPosition(SphereRequest request)
    {
        double radiusSquared = request.Radius * request.Radius;
        double distanceSquared = request.SphereCenter.DistanceSquaredTo(request.Point);

        const double TOLERANCE = 1e-9;

        if (Math.Abs(distanceSquared - radiusSquared) < TOLERANCE)
            return "on the surface";
        else if (distanceSquared < radiusSquared)
            return "inside";
        else
            return "outside";
    }
}
