using OsmSharp.Routing.Osm.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.OsmSharp.Router
{
    public class PathSegment
    {
        public PathSegment(long vertex)
        {
            this.Vertex = vertex;
        }

        public PathSegment(long vertex, double weight, LiveEdge edge, PathSegment from)
        {
            this.Vertex = vertex;
            this.Edge = edge;
            this.Weight = weight;
            this.From = from;
        }

        public long Vertex { get; private set; }

        public LiveEdge Edge { get; private set; }

        public double Weight { get; set; }

        public PathSegment From { get; private set; }

        internal PathSegment[] ToArray()
        {
            var segments = new List<PathSegment>();
            var current = this;
            while(current != null)
            {
                segments.Add(current);
                current = current.From;
            }
            segments.Reverse();
            return segments.ToArray();
        }
    }
}