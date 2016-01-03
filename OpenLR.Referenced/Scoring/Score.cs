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

namespace OpenLR.Referenced.Scoring
{
    /// <summary>
    /// Represents a score for a specific purpose.
    /// </summary>
    public abstract class Score
    {
        /// <summary>
        /// Holds the candidate route score name.
        /// </summary>
        public static string CANDIDATE_ROUTE = "candidate_route";

        /// <summary>
        /// Holds the distance comparison score name.
        /// </summary>
        public static string DISTANCE_COMPARISON = "distance_comparison";

        /// <summary>
        /// Holds the vertex distance score name.
        /// </summary>
        public static string VERTEX_DISTANCE = "vertex_distance";

        /// <summary>
        /// Holds the match arc score name.
        /// </summary>
        public static string MATCH_ARC = "match_arc";

        /// <summary>
        /// Holds the bearing difference.
        /// </summary>
        public static string BEARING_DIFF = "bearing_diff";

        /// <summary>
        /// Gets the name of this score.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the description of this score.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets or the score of this score.
        /// </summary>
        public abstract double Value { get; }

        /// <summary>
        /// Gets or sets the reference value of this score.
        /// </summary>
        /// <remarks>If value is max 1, this contains 1.</remarks>
        public abstract double Reference { get; }

        /// <summary>
        /// Gets a score by name.
        /// </summary>
        public abstract Score GetByName(string key);

        /// <summary>
        /// Creates a new score.
        /// </summary>
        public static SimpleScore New(string name, string description, double value, double reference)
        {
            return new SimpleScore(name, description, value, reference);
        }

        /// <summary>
        /// Multiplies the two given scores.
        /// </summary>
        public static MultipliedScore operator *(Score left, Score right)
        {
            return new MultipliedScore(left, right);
        }

        /// <summary>
        /// Adds the two given scores.
        /// </summary>
        public static AddedScore operator +(Score left, Score right)
        {
            return new AddedScore(left, right);
        }

        /// <summary>
        /// Returns a description of this score.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0} {1}/{2}", this.Name, this.Value, this.Reference);
        }
        
        /// <summary>
        /// Returns the hashcode.
        /// </summary>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^
                this.Description.GetHashCode() ^
                this.Value.GetHashCode() ^
                this.Reference.GetHashCode();
        }

        /// <summary>
        /// Returns true if the given object equals this object.
        /// </summary>
        public override bool Equals(object obj)
        {
            var otherScore = obj as Score;
            if(otherScore != null)
            {
                return otherScore.Name.Equals(this.Name) &&
                    otherScore.Description.Equals(this.Description) &&
                    otherScore.Value.Equals(this.Value) &&
                    otherScore.Reference.Equals(this.Reference);
            }
            return false;
        }
    }
}