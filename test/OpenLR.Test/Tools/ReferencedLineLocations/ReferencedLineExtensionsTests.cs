using Itinero;
using Itinero.Geo;
using Itinero.Network.Enumerators.Edges;
using Itinero.Profiles;
using Itinero.Routing;
using Itinero.Snapping;
using NUnit.Framework;
using OpenLR.Referenced.Locations;
using OpenLR.Tools.ReferencedLineLocations;

namespace OpenLR.Test.Tools.ReferencedLineLocations;

public class ReferencedLineExtensionsTests
{
    [Test]
    public async Task GetCoveredEdges_1Edge_WithOffsets_ShouldCover1PartialEdge()
    {
        // setup a routing network to test against.
        var network = await TestNetworks.Network3();

        // setup test location and data to verify this.
        (double longitude, double latitude, float? e) start = (
            4.791750729739011,
            51.266950597067655, null);
        (double longitude, double latitude, float? e) end = (
            4.795721368475275,
            51.26693716841704, null);
        
        // calculate shortest path.
        var path = (await network.Route(new DefaultProfile()).From(
            await network.Snap().ToAsync(start)).To(
            await network.Snap().ToAsync(end)).PathAsync()).Value;

        var lineLocation = ReferencedLine.FromPath(path).WithOffsets(25,25);
            
        var covered = lineLocation.GetCoveredEdges().ToList();
        Assert.That(covered, Has.Count.EqualTo(path.Count));
        foreach (var (edge, forward, _, _) in path)
        {
            var coveredEdge = covered.First(x => x.edge == edge);
            Assert.Multiple(() =>
            {
                Assert.That(coveredEdge.forward, Is.EqualTo(forward));
                Assert.That(coveredEdge.tailOffset, Is.EqualTo(0.25 * ushort.MaxValue).Within(2));
                Assert.That(coveredEdge.headOffset, Is.EqualTo(0.75 * ushort.MaxValue).Within(2));
            });
        }
    }
    
    [Test]
    public async Task GetCoveredEdges_3Edges_ShouldCover3Edges()
    {
        // setup a routing network to test against.
        var network = await TestNetworks.Network3();

        // setup test location and data to verify this.
        (double longitude, double latitude, float? e) start = (
            4.7957682609558105,
            51.268252139690674, null);
        (double longitude, double latitude, float? e) end = (
            4.791841506958008,
            51.268158160891474, null);
        
        // calculate shortest path.
        var path = (await network.Route(new DefaultProfile()).From(
            await network.Snap().ToAsync(start)).To(
            await network.Snap().ToAsync(end)).PathAsync()).Value;

        var lineLocation = ReferencedLine.FromPath(path);

        var covered = lineLocation.GetCoveredEdges().ToList();
        Assert.That(covered, Has.Count.EqualTo(path.Count));
        foreach (var (edge, forward, _, _) in path)
        {
            Assert.That(covered, Does.Contain((edge, forward, 0, ushort.MaxValue)));
        }
    }
    
    [Test]
    public async Task GetCoveredEdges_2Edge_WithOffsets_ShouldCover2PartialEdges()
    {
        // setup a routing network to test against.
        var network = await TestNetworks.Network3();

        // setup test location and data to verify this.
        (double longitude, double latitude, float? e) start = (
            4.791836581387362,
            51.268159175623616, null);
        (double longitude, double latitude, float? e) end = (
            4.795721368475275,
            51.26693716841704, null);
        
        // calculate shortest path.
        var path = (await network.Route(new DefaultProfile()).From(
            await network.Snap().ToAsync(start)).To(
            await network.Snap().ToAsync(end)).PathAsync()).Value;

        var lineLocation = ReferencedLine.FromPath(path).WithOffsets(10,10);
        var offset10Percent = lineLocation.GetCoordinates().DistanceEstimateInMeter() * 0.1;

        var edgeEnumerator = network.GetEdgeEnumerator();
        var covered = lineLocation.GetCoveredEdges().ToList();
        Assert.That(covered, Has.Count.EqualTo(path.Count));
        foreach (var (edge, forward, _, _) in path)
        {
            var coveredEdge = covered.First(x => x.edge == edge);
            Assert.That(edgeEnumerator.MoveTo(edge, forward), Is.True);
            var edgeLength = edgeEnumerator.GetCompleteShape().DistanceEstimateInMeter();
            var offset = (offset10Percent / edgeLength) * ushort.MaxValue;

            Assert.That(coveredEdge.forward, Is.EqualTo(forward));
            if (coveredEdge.tailOffset == 0)
            {
                Assert.That(coveredEdge.headOffset, Is.EqualTo(ushort.MaxValue - offset).Within(2));
            }
            else
            {
                Assert.That(coveredEdge.tailOffset, Is.EqualTo(offset).Within(2));
                Assert.That(coveredEdge.headOffset, Is.EqualTo(ushort.MaxValue));
            }
        }
    }
}
