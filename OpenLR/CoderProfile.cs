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

using Itinero;
using Itinero.Attributes;
using Itinero.Profiles;
using OpenLR.Matching;
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
        private readonly float _maxSearch;

        private readonly RoutingSettings<float> _routingSettings;

        /// <summary>
        /// Creates a new coder profile.
        /// </summary>
        public CoderProfile(Profile profile, float scoreThreshold, float maxSearch)
        {
            if (profile == null) { throw new ArgumentNullException("profile"); }

            _profile = profile;
            _scoreThreshold = scoreThreshold;
            _maxSearch = maxSearch;

            this.MaxSettles = 65536;

            _routingSettings = new RoutingSettings<float>();
            _routingSettings.SetMaxSearch(profile.Name, _maxSearch);
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
        /// Gets the maximum search (time or distance depending on used profile).
        /// </summary>
        public float MaxSearch
        {
            get
            {
                return _maxSearch;
            }
        }
        
        /// <summary>
        /// Gets the routing settings.
        /// </summary>
        public RoutingSettings<float> RoutingSettings
        {
            get
            {
                return _routingSettings; 
            }
        }

        /// <summary>
        /// Gets more aggressive routing settings.
        /// </summary>
        public RoutingSettings<float> GetAggressiveRoutingSettings(float factor)
        {
            var routingSettings = new RoutingSettings<float>();
            routingSettings.SetMaxSearch(_profile.Name, _maxSearch * factor);

            return routingSettings;
        }

        /// <summary>
        /// Gets the max settles.
        /// </summary>
        public int MaxSettles { get; internal set; }

        /// <summary>
        /// Matches nwb/fow.
        /// </summary>
        public virtual float Match(IAttributeCollection attributes, FormOfWay fow, FunctionalRoadClass frc)
        {
            FormOfWay actualFow;
            FunctionalRoadClass actualFrc;
            if (this.Extract(attributes, out actualFrc, out actualFow))
            { // a mapping was found. match and score.
                return MatchScoring.MatchAndScore(frc, fow, actualFrc, actualFow);
            }
            return 0;
        }

        /// <summary>
        /// Tries to extract fow/frc from the given attributes.
        /// </summary>
        public abstract bool Extract(IAttributeCollection tags, out FunctionalRoadClass frc, out FormOfWay fow);
    }
}