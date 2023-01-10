using NetTopologySuite.Algorithm.Distance;
using NetTopologySuite.Geometries;
using NUnit.Framework;

namespace OpenLR.Test;

/// <summary>
/// Holds extra assert functionality.
/// </summary>
public static class AssertGeo
{
    /// <summary>
    /// Compares two geometries.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="actual"></param>
    /// <param name="delta"></param>
    public static void AreEqual(Geometry expected, Geometry actual, double delta)
    {
        if(expected is LineString lineString && actual is LineString s)
        {
            AssertGeo.AreEqual(lineString, s, delta);
        }
        //else if (expected is ILineString && actual is ILineString)
        //{
        //    Assert.AreEqual(expected as ILineString, actual as ILineString, delta);
        //}
    }

    /// <summary>
    /// Compares two line strings.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="actual"></param>
    /// <param name="delta"></param>
    public static void AreEqual(LineString expected, LineString actual, double delta)
    {
        var distance = new PointPairDistance();
        foreach(Coordinate actualCoordinate in actual.Coordinates)
        {
            DistanceToPoint.ComputeDistance(expected, actualCoordinate, distance);
            Assert.That(distance.Distance, Is.EqualTo(0.0).Within(delta));
        }
        foreach (Coordinate expectedCoordinate in expected.Coordinates)
        {
            DistanceToPoint.ComputeDistance(actual, expectedCoordinate, distance);
            Assert.That(distance.Distance, Is.EqualTo(0.0).Within(delta));
        }
    }

    // /// <summary>
    // /// Compares two line strings.
    // /// </summary>
    // /// <param name="geoJsonActual"></param>
    // /// <param name="geoJson"></param>
    // public static void AreEqual(string geoJsonActual, string geoJson, double delta)
    // {
    //     var geoJsonReader = new GeoJsonReader();
    //     var actual = geoJsonReader.Read<IGeometry>(geoJsonActual);
    //     var expected = geoJsonReader.Read<IGeometry>(geoJson);
    //
    //     AssertGeo.AreEqual(actual, expected, delta);
    // }
}
