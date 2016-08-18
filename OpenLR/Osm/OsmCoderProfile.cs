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
using Itinero.Osm.Vehicles;
using OpenLR.Model;

namespace OpenLR.Osm
{
    /// <summary>
    /// A coder profile for OSM.
    /// </summary>
    public class OsmCoderProfile : CoderProfile
    {
        /// <summary>
        /// Creates a new coder profile.
        /// </summary>
        public OsmCoderProfile(float scoreThreshold)
            : base(Vehicle.Car.Shortest(), scoreThreshold)
        {

        }

        /// <summary>
        /// Tries to match the given attributes to the given fow/frc and returns a score of the match.
        /// </summary>
        public override float Match(IAttributeCollection attributes, FormOfWay fow, FunctionalRoadClass frc)
        {
            string highway;
            if (!attributes.TryGetValue("highway", out highway))
            { // not even a highway tag!
                return 0;
            }

            // TODO: take into account form of way? Maybe not for OSM-data?
            switch (frc)
            { // check there reference values against OSM: http://wiki.openstreetmap.org/wiki/Highway
                case FunctionalRoadClass.Frc0: // main road.
                    if (highway == "motorway" || highway == "motorway_link" ||
                        highway == "trunk" || highway == "trunk_link")
                    {
                        return 1;
                    }
                    break;
                case FunctionalRoadClass.Frc1: // first class road.
                    if (highway == "primary" || highway == "primary_link")
                    {
                        return 1;
                    }
                    break;
                case FunctionalRoadClass.Frc2: // second class road.
                    if (highway == "secondary" || highway == "secondary_link")
                    {
                        return 1;
                    }
                    break;
                case FunctionalRoadClass.Frc3: // third class road.
                    if (highway == "tertiary" || highway == "tertiary_link")
                    {
                        return 1;
                    }
                    break;
                case FunctionalRoadClass.Frc4:
                    if (highway == "road" || highway == "road_link" ||
                        highway == "unclassified" || highway == "residential")
                    {
                        return 1;
                    }
                    break;
                case FunctionalRoadClass.Frc5:
                    if (highway == "road" || highway == "road_link" ||
                        highway == "unclassified" || highway == "residential" ||
                        highway == "living_street")
                    {
                        return 1;
                    }
                    break;
                case FunctionalRoadClass.Frc6:
                    if (highway == "road" || highway == "track" ||
                        highway == "unclassified" || highway == "residential" ||
                        highway == "living_street")
                    {
                        return 1;
                    }
                    break;
                case FunctionalRoadClass.Frc7: // other class road.
                    if (highway == "footway" || highway == "bridleway" ||
                        highway == "steps" || highway == "path" ||
                        highway == "living_street")
                    {
                        return 1;
                    }
                    break;
            }

            if (highway != null && highway.Length > 0)
            { // for any other highway return a low match.
                return 0.2f;
            }
            return 0;
        }

        /// <summary>
        /// Tries to extract fow/frc from the given attributes.
        /// </summary>
        public override bool Extract(IAttributeCollection tags, out FunctionalRoadClass frc, out FormOfWay fow)
        {
            frc = FunctionalRoadClass.Frc7;
            fow = FormOfWay.Undefined;
            string highway;
            if (tags.TryGetValue("highway", out highway))
            {
                switch (highway)
                { // check there reference values against OSM: http://wiki.openstreetmap.org/wiki/Highway
                    case "motorway":
                    case "trunk":
                        frc = FunctionalRoadClass.Frc0;
                        break;
                    case "primary":
                    case "primary_link":
                        frc = FunctionalRoadClass.Frc1;
                        break;
                    case "secondary":
                    case "secondary_link":
                        frc = FunctionalRoadClass.Frc2;
                        break;
                    case "tertiary":
                    case "tertiary_link":
                        frc = FunctionalRoadClass.Frc3;
                        break;
                    case "road":
                    case "road_link":
                    case "unclassified":
                    case "residential":
                        frc = FunctionalRoadClass.Frc4;
                        break;
                    case "living_street":
                        frc = FunctionalRoadClass.Frc5;
                        break;
                    default:
                        frc = FunctionalRoadClass.Frc7;
                        break;
                }
                switch (highway)
                { // check there reference values against OSM: http://wiki.openstreetmap.org/wiki/Highway
                    case "motorway":
                    case "trunk":
                        fow = FormOfWay.Motorway;
                        break;
                    case "primary":
                    case "primary_link":
                        fow = FormOfWay.MultipleCarriageWay;
                        break;
                    case "secondary":
                    case "secondary_link":
                    case "tertiary":
                    case "tertiary_link":
                        fow = FormOfWay.SingleCarriageWay;
                        break;
                    default:
                        fow = FormOfWay.SingleCarriageWay;
                        break;
                }
                return true; // should never fail on a highway tag.
            }
            return false;
        }
    }
}