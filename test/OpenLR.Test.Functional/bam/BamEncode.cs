using System;
using System.IO;
using Itinero;
using Itinero.Algorithms;
using Itinero.Profiles;
using OpenLR.Codecs;
using OpenLR.Codecs.Binary;
using OpenLR.Model.Locations;
using OpenLR.Osm;
using OpenLR.Referenced;
using OpenLR.Referenced.Codecs;
using OpenLR.Referenced.Locations;
using Serilog;

namespace OpenLR.Test.Functional.bam
{
    public class BamEncode
    {
        public static void TestEncodeAll()
        {
            Log.Information("Encode all edges in a network.");
            TestEncodeAll("bam/data.routerdb");
            TestEncodeAll("bam/bam2.routerdb");
        }

        public static void TestEncodeAll(string router)
        {
            RouterDb routerDb;
            using (var stream = File.OpenRead(router))
            {
                routerDb = RouterDb.Deserialize(stream);
            }

            var coderProfile = new OsmCoderProfile();
            var coder = new Coder(routerDb, coderProfile);

            var getFactor = coder.Router.GetDefaultGetFactor(coder.Profile.Profile);

            var enumerator = routerDb.Network.GetEdgeEnumerator();
            for (uint v = 0; v < routerDb.Network.VertexCount; v++)
            {
                    if (!enumerator.MoveTo(v)) continue;

                    while (enumerator.MoveNext())
                    {
                        if (enumerator.To < v) continue;
                        
                        var factor = getFactor(enumerator.Data.Profile);
                    
                        try
                        {
                            var encoding = coder.BuildLineLocation(new DirectedEdgeId(enumerator.Id, true), true);
                        }
                        catch (Exception e)
                        {
                            Log.Warning($"Edge {enumerator.Id}: {e}");
                        }
                    }
            }
        }
    }

    class Coder : OpenLR.Coder
    {
        private readonly Router _router;
        private readonly CodecBase _rawCodec;
        private readonly CoderProfile _profile;

        public Coder(RouterDb routerDb, CoderProfile profile)
            : this(routerDb, profile, new BinaryCodec())
        {
        }

        public Coder(RouterDb routerDb, CoderProfile profile, CodecBase rawCodec) : base(routerDb, profile, rawCodec)
        {
            _router = new Router(routerDb);
            _rawCodec = rawCodec;
            _profile = profile;
            if (!_router.SupportsAll((IProfileInstance) profile.Profile))
                throw new ArgumentException(
                    "The router db does not support the profile in the coder profile. Are you using the correct vehicle profile?");
        }

        public Router Router
        {
            get { return _router; }
        }

        public CoderProfile Profile
        {
            get { return _profile; }
        }

        public CodecBase RawCodec
        {
            get { return _rawCodec; }
        }


        public string Encode(ReferencedLocation location, EncodingSettings settings)
        {
            if (location is ReferencedCircle)
                return _rawCodec.Encode(ReferencedCircleCodec.Encode(location as ReferencedCircle));
            if (location is ReferencedGeoCoordinate)
                return _rawCodec.Encode(ReferencedGeoCoordinateCodec.Encode(location as ReferencedGeoCoordinate));
            if (location is ReferencedGrid)
                return _rawCodec.Encode(ReferencedGridCodec.Encode(location as ReferencedGrid));
            if (location is ReferencedLine)
                return _rawCodec.Encode(ReferencedLineCodec.Encode(location as ReferencedLine, this, settings));
            if (location is ReferencedPointAlongLine)
                return _rawCodec.Encode(
                    ReferencedPointAlongLineCodec.Encode(location as ReferencedPointAlongLine, this));
            if (location is ReferencedPolygon)
                return _rawCodec.Encode(ReferencedPolygonCodec.Encode(location as ReferencedPolygon));
            if (location is ReferencedRectangle)
                return _rawCodec.Encode(ReferencedRectangleCodec.Encode(location as ReferencedRectangle));
            throw new ArgumentOutOfRangeException(nameof(location), "Unknown location type.");
        }


        /// <summary>
        /// Builds a line location object.
        /// </summary>
        /// <param name="coder">The coder.</param>
        /// <param name="edge">The edge.</param>
        /// <param name="correctDirection">Correct the direction if the edge cannot be traversed.</param>
        /// <returns>The line that represents the edge.</returns>
        public LineLocation BuildLineLocation(DirectedEdgeId edge,
            bool correctDirection = false)
        {
            var referenced = this.BuildReferencedLine(edge, correctDirection);

            if (referenced == null)
            {
                return null;
            }
            return ReferencedLineCodec.Encode(referenced, this);
        }

        /// <summary>
        /// Builds a referenced line representing a single edge.
        /// </summary>
        /// <param name="coder">The coder.</param>
        /// <param name="edge">The edge.</param>
        /// <param name="correctDirection">Correct the direction if the edge cannot be traversed.</param>
        /// <returns>The referenced line that represents the edge.</returns>
        private ReferencedLine BuildReferencedLine(DirectedEdgeId edge,
            bool correctDirection = false)
        {
            try
            {
                var edgeDetails = Router.Db.Network.GetEdge(edge.EdgeId);

                // check if this edge is ok with this profile.
                var factor = Router.GetDefaultGetFactor(Profile.Profile)(edgeDetails.Data.Profile);
                if (factor.Value <= 0)
                {
                    // not traversable in any direction.
                    return null;
                }

                // reverse direction if requested and needed.
                if (correctDirection)
                {
                    if ((edge.Forward && factor.Direction == 2) ||
                        (!edge.Forward && factor.Direction == 1))
                    {
                        edge = new DirectedEdgeId(edge.EdgeId, !edge.Forward);
                        return BuildReferencedLine(edge, false);
                    }
                }

                // build the line location.
                var vertex1 = edgeDetails.From;
                var vertex2 = edgeDetails.To;
                var vertex1RouterPoint = Router.Db.Network.CreateRouterPointForVertex(vertex1,
                    vertex2);
                var vertex2RouterPoint = Router.Db.Network.CreateRouterPointForVertex(vertex2,
                    vertex1);
                
                
                if (!edge.Forward)
                {
                    return new ReferencedLine()
                    {
                        Edges = new[] {edge.SignedDirectedId},
                        Vertices = new[] {vertex2, vertex1},
                        StartLocation = vertex2RouterPoint,
                        EndLocation = vertex1RouterPoint
                    };
                }

                return new ReferencedLine()
                {
                    Edges = new[] {edge.SignedDirectedId},
                    Vertices = new[] {vertex1, vertex2},
                    StartLocation = vertex1RouterPoint,
                    EndLocation = vertex2RouterPoint
                };
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Failed to build line location for edge: {0} - {1}",
                    edge, ex.ToString()));
            }

            return null;
        }
    }
}