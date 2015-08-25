using OpenLR.Model;
using OpenLR.Referenced.Scoring;
using OpenLR.Referenced;
using OsmSharp.Routing.Graph;
using OsmSharp.Units.Distance;

namespace OpenLR.Referenced.Locations
{
    /// <summary>
    /// Represents a referenced point along line location with a graph as a reference.
    /// </summary>
    public class ReferencedPointAlongLine : ReferencedLocation
    {
        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the route (vertex->edge->vertex->edge->vertex) associated with this location.
        /// </summary>
        public ReferencedLine Route { get; set; }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        public Orientation Orientation { get; set; }

        /// <summary>
        /// Holds an indication of accuracy.
        /// </summary>
        public Score Score { get; set; }

        /// <summary>
        /// Gets or sets the edge meta.
        /// </summary>
        public EdgeMeta EdgeMeta { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            if(this.Route == null)
            {
                return new ReferencedPointAlongLine()
                {
                    EdgeMeta = this.EdgeMeta,
                    Latitude = this.Latitude,
                    Longitude = this.Longitude,
                    Orientation = this.Orientation,
                    Score = this.Score
                };
            }
            return new ReferencedPointAlongLine()
            {
                EdgeMeta = this.EdgeMeta,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                Orientation = this.Orientation,
                Route = this.Route.Clone() as ReferencedLine,
                Score = this.Score
            };
        }
    }

    /// <summary>
    /// Represents edge meta data.
    /// </summary>
    public struct EdgeMeta
    {
        /// <summary>
        /// Gets or sets the edge idx.
        /// </summary>
        public int Idx { get; set; }

        /// <summary>
        /// Gets or sets the edge idx.
        /// </summary>
        public Meter Offset { get; set; }

        /// <summary>
        /// Gets or sets the edge length.
        /// </summary>
        public Meter Length { get; set; }
    }
}