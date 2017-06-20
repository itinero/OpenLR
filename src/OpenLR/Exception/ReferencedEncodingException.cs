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

using System;

namespace OpenLR.Referenced
{
    /// <summary>
    /// Represents a referenced encoding exception.
    /// </summary>
    public class ReferencedEncodingException : Exception
    {
        private readonly ReferencedLocation _referencedLocation;

        /// <summary>
        /// Creates a new referenced encoding exception.
        /// </summary>
        public ReferencedEncodingException(ReferencedLocation location, string message, Exception innerException)
            : base(message, innerException)
        {
            _referencedLocation = location;
        }
        /// <summary>
        /// Creates a new referenced encoding exception.
        /// </summary>
        public ReferencedEncodingException(ReferencedLocation location, string message)
            : base(message)
        {
            _referencedLocation = location;
        }

        /// <summary>
        /// Returns the location that was being encoded.
        /// </summary>
        public ReferencedLocation Location
        {
            get
            {
                return _referencedLocation;
            }
        }
    }
}