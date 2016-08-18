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

using OpenLR.Referenced.Codecs.Candidates;
using OpenLR.Referenced.Scoring;

namespace OpenLR.Referenced.Codecs.Scoring
{
    /// <summary>
    /// Represents a combined score.
    /// </summary>
    internal class CombinedScore
    {
        /// <summary>
        /// Gets or sets the source candidate.
        /// </summary>
        public CandidateVertexEdge Source { get; set; }

        /// <summary>
        /// Gets or sets the target candidate.
        /// </summary>
        public CandidateVertexEdge Target { get; set; }

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
        public override bool Equals(object obj)
        {
            var other = (obj as CombinedScore);
            return other != null && 
                other.Target.Equals(this.Target) && 
                other.Source.Equals(this.Source) && 
                other.Score.Equals(this.Score);
        }

        /// <summary>
        /// Serves as a hashfunction.
        /// </summary>
        public override int GetHashCode()
        {
            return this.Score.GetHashCode() ^
                this.Target.GetHashCode() ^
                this.Source.GetHashCode();
        }
    }
}