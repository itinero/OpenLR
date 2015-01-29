using OpenLR.OsmSharp.Decoding.Candidates;
using OpenLR.OsmSharp.Scoring;
using OsmSharp.Routing.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.OsmSharp.Decoding.Scoring
{
    /// <summary>
    /// Represents a combined score.
    /// </summary>
    internal class CombinedScore<TEdge>
        where TEdge : IGraphEdgeData
    {
        /// <summary>
        /// Gets or sets the source candidate.
        /// </summary>
        public CandidateVertexEdge<TEdge> Source { get; set; }

        /// <summary>
        /// Gets or sets the target candidate.
        /// </summary>
        public CandidateVertexEdge<TEdge> Target { get; set; }

        /// <summary>
        /// Returns the score.
        /// </summary>
        public Score Score
        {
            get
            {
                return this.Source.Score + this.Target.Score;
            }
        }

        /// <summary>
        /// Determines whether this object is equal to the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = (obj as CombinedScore<TEdge>);
            return other != null && 
                other.Target.Equals(this.Target) && 
                other.Source.Equals(this.Source) && 
                other.Score.Equals(this.Score);
        }

        /// <summary>
        /// Serves as a hashfunction.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Score.GetHashCode() ^
                this.Target.GetHashCode() ^
                this.Source.GetHashCode();
        }
    }
}
