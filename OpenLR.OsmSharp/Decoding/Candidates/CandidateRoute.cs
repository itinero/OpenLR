using OpenLR.OsmSharp.Locations;
using OpenLR.OsmSharp.Scoring;
using OsmSharp.Routing.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.OsmSharp.Decoding.Candidates
{
    /// <summary>
    /// Represents a candiate route and associated score.
    /// </summary>
    public class CandidateRoute<TEdge>
        where TEdge : IGraphEdgeData
    {
        /// <summary>
        /// Gets or sets the route.
        /// </summary>
        public ReferencedLine<TEdge> Route { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        public Score Score { get; set; }
    }
}
