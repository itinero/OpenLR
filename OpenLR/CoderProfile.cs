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

using Itinero.Attributes;
using Itinero.Profiles;
using OpenLR.Model;
using System;

namespace OpenLR
{
    /// <summary>
    /// A profile that can be used by the coder to implmenent network-specifics.
    /// </summary>
    public abstract class CoderProfile
    {
        private readonly Profile _profile;
        private readonly float _scoreThreshold;

        /// <summary>
        /// Creates a new coder profile.
        /// </summary>
        public CoderProfile(Profile profile, float scoreThreshold)
        {
            if (profile == null) { throw new ArgumentNullException("profile"); }

            _profile = profile;
            _scoreThreshold = scoreThreshold;

            this.MaxSettles = 65536;
        }

        /// <summary>
        /// Gets the get factor function.
        /// </summary>
        public Profile Profile
        {
            get
            {
                return _profile;
            }
        }

        /// <summary>
        /// Gets the score threshold.
        /// </summary>
        public float ScoreThreshold
        {
            get
            {
                return _scoreThreshold;
            }
        }

        /// <summary>
        /// Gets the max settles.
        /// </summary>
        public int MaxSettles { get; internal set; }

        /// <summary>
        /// Tries to match the given attributes to the given fow/frc and returns a score of the match.
        /// </summary>
        public abstract float Match(IAttributeCollection attributes, FormOfWay fow, FunctionalRoadClass frc);

        /// <summary>
        /// Tries to extract fow/frc from the given attributes.
        /// </summary>
        public abstract bool Extract(IAttributeCollection tags, out FunctionalRoadClass frc, out FormOfWay fow);
    }
}