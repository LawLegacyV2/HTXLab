using SphereApi.Models;
using SphereApi.Services;
using Xunit;

namespace SphereApi.Tests;

public class SphereServiceTests
{
    private readonly SphereService _service = new();

    [Fact]
    public void PointInsideSphere_ReturnsInside()
    {
        // Arrange
        var request = new SphereRequest
        {
            SphereCenter = new Point3D(0, 0, 0),
            Radius = 5,
            Point = new Point3D(1, 1, 1)
        };

        // Act
        var result = _service.GetPointPosition(request);

        // Assert
        Assert.Equal("inside", result);
    }

    [Fact]
    public void PointOutsideSphere_ReturnsOutside()
    {
        var request = new SphereRequest
        {
            SphereCenter = new Point3D(0, 0, 0),
            Radius = 5,
            Point = new Point3D(10, 0, 0)
        };

        var result = _service.GetPointPosition(request);

        Assert.Equal("outside", result);
    }

    [Fact]
    public void PointOnSurface_ReturnsOnSurface()
    {
        var request = new SphereRequest
        {
            SphereCenter = new Point3D(0, 0, 0),
            Radius = 5,
            Point = new Point3D(3, 4, 0) // Distance = 5
        };

        var result = _service.GetPointPosition(request);

        Assert.Equal("on the surface", result);
    }
}
