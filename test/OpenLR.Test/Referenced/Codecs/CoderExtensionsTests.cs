using Itinero;
using Itinero.Network;
using Itinero.Snapping;
using NUnit.Framework;
using OpenLR.Model;
using OpenLR.Referenced.Codecs;

namespace OpenLR.Test.Referenced.Codecs;

public class CoderExtensionsTests
{
    [Test]
    public async Task FindCandidateSnapPointsForAsync_ExactVertex_ShouldFindVertex()
    {
        var coder = await TestCoders.Coder1();
        var vertex = coder.Network.GetVertices().First();
        var location = coder.Network.GetVertex(vertex);

        var result = await coder.FindCandidateSnapPointsForAsync(new LocationReferencePoint()
        {
            Bearing = null,
            Coordinate = new Coordinate() { Latitude = location.latitude, Longitude = location.longitude },
            DistanceToNext = 0
        }).ToListAsync();

        Assert.That(result, Has.Count.EqualTo(1));
        var snapPoint = result.First();
        Assert.That(snapPoint.Candidate.IsVertex, Is.True);
        var actualVertex = snapPoint.Candidate.GetVertex(coder.Network);
        Assert.Multiple(() =>
        {
            Assert.That(actualVertex.LocalId, Is.EqualTo(vertex.LocalId));
            Assert.That(actualVertex.TileId, Is.EqualTo(vertex.TileId));
        });
    }

    [Test]
    public async Task FindCandidateSnapPointsForAsync_CloseVertex_ShouldFindVertex()
    {
        var coder = await TestCoders.Coder1();
        (double longitude, double latitude, float? e) location = (4.461017014122632, 51.229683938101715, null);
        var vertex = (await coder.Network.Snap().ToVertexAsync(location)).Value;

        var result = await coder.FindCandidateSnapPointsForAsync(new LocationReferencePoint()
        {
            Bearing = null,
            Coordinate = new Coordinate() { Latitude = location.latitude, Longitude = location.longitude },
            DistanceToNext = 0
        }).ToListAsync();

        Assert.That(result, Has.Count.EqualTo(1));
        var snapPoint = result.First();
        Assert.That(snapPoint.Candidate.IsVertex, Is.True);
        var actualVertex = snapPoint.Candidate.GetVertex(coder.Network);
        Assert.Multiple(() =>
        {
            Assert.That(actualVertex.LocalId, Is.EqualTo(vertex.LocalId));
            Assert.That(actualVertex.TileId, Is.EqualTo(vertex.TileId));
        });
    }

    [Test]
    public async Task FindCandidateSnapPointsForAsync_NoCloseVertex_ShouldFindSnapPoint()
    {
        var coder = await TestCoders.Coder1();
        (double longitude, double latitude, float? e) location = (4.4622062896577575,
            51.22965207888018, null);
        var snapPoint = (await coder.Network.Snap().ToAsync(location)).Value;

        var result = await coder.FindCandidateSnapPointsForAsync(new LocationReferencePoint()
        {
            Bearing = null,
            Coordinate = new Coordinate() { Latitude = location.latitude, Longitude = location.longitude },
            DistanceToNext = 0
        }).ToListAsync();

        Assert.That(result, Has.Count.EqualTo(1));
        var actualSnapPoint = result.First();
        Assert.Multiple(() =>
        {
            Assert.That(actualSnapPoint.Candidate.IsVertex, Is.False);
            Assert.That(actualSnapPoint.Candidate.EdgeId.LocalId, Is.EqualTo(snapPoint.EdgeId.LocalId));
            Assert.That(actualSnapPoint.Candidate.EdgeId.TileId, Is.EqualTo(snapPoint.EdgeId.TileId));
        });
    }

    [Test]
    public async Task FindCandidateSnapPointsForAsync_CloseButNotCloseEnoughVertex_ShouldFindVertexAndSnapPoint()
    {
        var coder = await TestCoders.Coder1();
        (double longitude, double latitude, float? e) location = (
            4.461238967732044,
            51.22967870631666, null);

        var result = await coder.FindCandidateSnapPointsForAsync(new LocationReferencePoint()
        {
            Bearing = null,
            Coordinate = new Coordinate() { Latitude = location.latitude, Longitude = location.longitude },
            DistanceToNext = 0
        }).ToListAsync();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(result.Any(x => x.Candidate.IsVertex), Is.True);
            Assert.That(result.Any(x => !x.Candidate.IsVertex), Is.True);
        });
    }

    [Test]
    public async Task FindCandidateSnapPointAndDirection_()
    {
        var coder = await TestCoders.Coder1();
        var vertex = coder.Network.GetVertices().First();
        var location = coder.Network.GetVertex(vertex);

        var result = (await coder.FindCandidateSnapPointsForAsync(new LocationReferencePoint()
        {
            Bearing = null,
            Coordinate = new Coordinate() { Latitude = location.latitude, Longitude = location.longitude },
            DistanceToNext = 0
        }).ToListAsync()).First();

        var result2 =
            coder.FindCandidateSnapPointAndDirectionFor(result, true, FormOfWay.Motorway,
                FunctionalRoadClass.Frc2, 0);
    }
}
