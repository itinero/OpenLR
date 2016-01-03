//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace OpenLR.Referenced.Router
//{
//    /// <summary>
//    /// Represents a path segment.
//    /// </summary>
//    public class PathSegment
//    {
//        /// <summary>
//        /// Creates a new path segment without a previous one.
//        /// </summary>
//        /// <param name="vertex"></param>
//        public PathSegment(long vertex)
//        {
//            this.Vertex = vertex;
//        }

//        /// <summary>
//        /// Creates a new path segment referring to the previous one.
//        /// </summary>
//        public PathSegment(long vertex, double weight, LiveEdge edge, PathSegment from)
//        {
//            this.Vertex = vertex;
//            this.Edge = edge;
//            this.Weight = weight;
//            this.From = from;
//        }

//        /// <summary>
//        /// Gets the vertex.
//        /// </summary>
//        public long Vertex { get; private set; }

//        /// <summary>
//        /// Gets the edge.
//        /// </summary>
//        public LiveEdge Edge { get; private set; }

//        /// <summary>
//        /// Gets the weight.
//        /// </summary>
//        public double Weight { get; set; }

//        /// <summary>
//        /// Gets the previous segment.
//        /// </summary>
//        public PathSegment From { get; private set; }

//        /// <summary>
//        /// Converts all the segments an array.
//        /// </summary>
//        /// <returns></returns>
//        internal PathSegment[] ToArray()
//        {
//            var segments = new List<PathSegment>();
//            var current = this;
//            while(current != null)
//            {
//                segments.Add(current);
//                current = current.From;
//            }
//            segments.Reverse();
//            return segments.ToArray();
//        }

//        /// <summary>
//        /// Returns true if this path contains the given vertex.
//        /// </summary>
//        /// <param name="vertex"></param>
//        /// <returns></returns>
//        internal bool Contains(long vertex)
//        {
//            var current = this;
//            while(current != null)
//            {
//                if (current.Vertex == vertex)
//                {
//                    return true;
//                }
//                current = current.From;
//            }
//            return false;
//        }
//    }
//}