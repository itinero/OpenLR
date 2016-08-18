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
    /// Represents a mulitplied score, a score that is the multiplication of two others.
    /// </summary>
    public class MultipliedScore : Score
    {
        private Score _left; // Holds the left score.
        private Score _right; // Holds the right score.

        /// <summary>
        /// Creates a new multiplied score.
        /// </summary>
        internal MultipliedScore(Score left, Score right)
        {
            _left = left;
            _right = right;
        }

        /// <summary>
        /// Returns the left-side.
        /// </summary>
        public Score Left
        {
            get
            {
                return _left;
            }
        }

        /// <summary>
        /// Returns the right side.
        /// </summary>
        public Score Right
        {
            get
            {
                return _right;
            }
        }

        /// <summary>
        /// Returns the name of this score.
        /// </summary>
        public override string Name
        {
            get { return "[" + this.Left.Name + "] * [" + this.Right.Name + "]"; }
        }

        /// <summary>
        /// Returns the description of this score.
        /// </summary>
        public override string Description
        {
            get { return "[" + this.Left.Description + "] * [" + this.Right.Description + "]"; }
        }

        /// <summary>
        /// Returns the value of this score.
        /// </summary>
        public override double Value
        {
            get { return this.Left.Value * this.Right.Value; }
        }

        /// <summary>
        /// Returns the reference value of this score.
        /// </summary>
        public override double Reference
        {
            get { return this.Left.Reference * this.Right.Reference; }
        }

        /// <summary>
        /// Gets a score by name.
        /// </summary>
        public override Score GetByName(string key)
        {
            var leftScore = this.Left.GetByName(key);
            var rightScore = this.Right.GetByName(key);
            if (leftScore != null && rightScore != null)
            {
                return leftScore + rightScore;
            }
            else if (leftScore != null)
            {
                return leftScore;
            }
            else if (rightScore != null)
            {
                return rightScore;
            }
            return null;
        }
    }
}