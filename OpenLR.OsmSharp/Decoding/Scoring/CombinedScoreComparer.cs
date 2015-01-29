using OsmSharp.Routing.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.OsmSharp.Decoding.Scoring
{
    /// <summary>
    /// A combined score compared.
    /// </summary>
    internal class CombinedScoreComparer<TEdge> : IComparer<CombinedScore<TEdge>>
        where TEdge : IGraphEdgeData
    {
        /// <summary>
        /// Compares the two combine scores.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(CombinedScore<TEdge> x, CombinedScore<TEdge> y)
        {
            var comparison = y.Score.Value.CompareTo(x.Score.Value);
            if(comparison == 0)
            {
                if(y.Target.Vertex == x.Target.Vertex && x.Source.Vertex == y.Source.Vertex &&
                    y.Target.Edge.Equals(x.Target.Edge) && x.Source.Edge.Equals(y.Source.Edge))
                { // only return 0 on items that are actually equal.
                    return 0;
                }
                return 1;
            }
            return comparison;
        }
    }
}
