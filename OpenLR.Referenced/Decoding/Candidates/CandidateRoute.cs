using OpenLR.Referenced.Locations;
using OpenLR.Referenced.Scoring;
using OsmSharp.Routing.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Referenced.Decoding.Candidates
{
    /// <summary>
    /// Represents a candiate route and associated score.
    /// </summary>
    public class CandidateRoute
    {
        /// <summary>
        /// Gets or sets the route.
        /// </summary>
        public ReferencedLine Route { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        public Score Score { get; set; }

        /// <summary>
        /// Returns a description of this candidate.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}",
                this.Route.ToString(), this.Score.ToString());
        }
    }
}
