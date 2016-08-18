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
    /// Represents a score with a single value and a single name.
    /// </summary>
    public class SimpleScore : Score
    {
        private readonly string _name; // Holds the name.
        private readonly string _description; // Holds the description.
        private readonly double _value; // Holds the value.
        private readonly double _reference; // Holds the reference.

        /// <summary>
        /// Creates a new simple score.
        /// </summary>
        internal SimpleScore(string name, string description, double value, double reference)
        {
            _name = name;
            _description = description;
            _value = value;
            _reference = reference;
        }

        /// <summary>
        /// Gets the name of this score.
        /// </summary>
        public override string Name { get { return _name; } }

        /// <summary>
        /// Gets the description of this score.
        /// </summary>
        public override string Description { get { return _description; } }

        /// <summary>
        /// Gets the value of this score.
        /// </summary>
        public override double Value { get { return _value; } }

        /// <summary>
        /// Gets the references of this score.
        /// </summary>
        public override double Reference { get { return _reference; } }

        /// <summary>
        /// Gets a score by name.
        /// </summary>
        public override Score GetByName(string key)
        {
            if(this.Name == key)
            {
                return this;
            }
            return null;
        }
    }
}