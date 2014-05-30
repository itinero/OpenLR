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
        where TEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Compares the two combine scores.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(CombinedScore<TEdge> x, CombinedScore<TEdge> y)
        {
            return y.Score.CompareTo(x.Score);
        }
    }
}
