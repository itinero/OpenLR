// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using OpenLR.Referenced.Scoring;

namespace OpenLR.Referenced.Codecs.Candidates
{
    /// <summary>
    /// Represents a candidate vertex/edge pair and associated score.
    /// </summary>
    public class CandidateVertexEdge
    {
        /// <summary>
        /// The combined score of vertex and edge.
        /// </summary>
        public Score Score { get; set; }

        /// <summary>
        /// Gets or sets the candidate edge.
        /// </summary>
        public uint EdgeId { get; set; }

        /// <summary>
        /// Gets or sets the candidate vertex.
        /// </summary>
        public uint VertexId { get; set; }

        ///// <summary>
        ///// Converts this referenced location to a geometry.
        ///// </summary>
        //public FeatureCollection ToFeatures(RouterDb db)
        //{
        //    var featureCollection = new FeatureCollection();

        //    // build linestring.
        //    featureCollection.Add(new Feature(db.Network.GetAsLineString(this.EdgeId), 
        //        new AttributesTable()));

        //    return featureCollection;
        //}

        /// <summary>
        /// Determines whether this object is equal to the given object.
        /// </summary>
        public override bool Equals(object obj)
        {
            var other = (obj as CandidateVertexEdge);
            return other != null && other.Score == this.Score && 
                other.EdgeId == this.EdgeId &&
                other.VertexId == this.VertexId;
        }

        /// <summary>
        /// Serves as a hashfunction.
        /// </summary>
        public override int GetHashCode()
        {
            return this.Score.GetHashCode() ^
                this.EdgeId.GetHashCode() ^
                this.VertexId.GetHashCode();
        }

        /// <summary>
        /// Returns a description of this candidate.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0} on {1}: {2}",
                this.VertexId.ToString(), 
                this.EdgeId.ToString(),
                this.Score.ToString());
        }
    }
}