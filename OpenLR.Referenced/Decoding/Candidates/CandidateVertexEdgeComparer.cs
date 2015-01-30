using OsmSharp.Routing.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Referenced.Decoding.Candidates
{
    /// <summary>
    /// A comparer for vertex edge candidates.
    /// </summary>
    public class CandidateVertexEdgeComparer<TEdge> : IComparer<CandidateVertexEdge<TEdge>>
        where TEdge : IGraphEdgeData
    {
        /// <summary>
        /// Compares the two given vertex-edge candidates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(CandidateVertexEdge<TEdge> x, CandidateVertexEdge<TEdge> y)
        {
            var comparison = y.Score.Value.CompareTo(x.Score.Value);
            if(comparison == 0)
            {
                return 1;
            }
            return comparison;
        }
    }
}
