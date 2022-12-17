using System;
using System.Collections.Generic;
using Itinero.Network.Attributes;
using OpenLR.Model;

namespace OpenLR.Networks.Osm;

/// <summary>
/// A network interpreter for OSM data.
/// </summary>
public sealed class OsmNetworkInterpreter : NetworkInterpreter
{
    /// <summary>
    /// Tries to extract fow/frc from the given attributes.
    /// </summary>
    public override bool Extract(IEnumerable<(string key, string value)> attributes, out FunctionalRoadClass frc,
        out FormOfWay fow)
    {
        frc = FunctionalRoadClass.Frc7;
        fow = FormOfWay.Undefined;
        if (!attributes.TryGetValue("highway", out string highway)) return false;

        switch (highway)
        {
            // check there reference values against OSM: http://wiki.openstreetmap.org/wiki/Highway
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
        {
            // check there reference values against OSM: http://wiki.openstreetmap.org/wiki/Highway
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

    /// <summary>
    /// Tries to match the given attributes to the given fow/frc and returns a score of the match.
    /// </summary>
    public override float Match(IEnumerable<(string key, string value)> attributes, FormOfWay fow, FunctionalRoadClass frc)
    {
        if (!attributes.TryGetValue("highway", out string highway))
        {
            // not even a highway tag!
            return 0;
        }

        // TODO: take into account form of way? Maybe not for OSM-data?
        switch (frc)
        {
            // check there reference values against OSM: http://wiki.openstreetmap.org/wiki/Highway
            case FunctionalRoadClass.Frc0: // main road.
                if (highway is "motorway" or "motorway_link" or "trunk" or "trunk_link")
                {
                    return 1;
                }

                break;
            case FunctionalRoadClass.Frc1: // first class road.
                if (highway is "primary" or "primary_link")
                {
                    return 1;
                }

                break;
            case FunctionalRoadClass.Frc2: // second class road.
                if (highway is "secondary" or "secondary_link")
                {
                    return 1;
                }

                break;
            case FunctionalRoadClass.Frc3: // third class road.
                if (highway is "tertiary" or "tertiary_link")
                {
                    return 1;
                }

                break;
            case FunctionalRoadClass.Frc4:
                if (highway is "road" or "road_link" or "unclassified" or "residential")
                {
                    return 1;
                }

                break;
            case FunctionalRoadClass.Frc5:
                if (highway != "road" && highway != "road_link" &&
                    highway != "unclassified" && highway != "residential" &&
                    highway != "living_street")
                {
                    return 1;
                }

                break;
            case FunctionalRoadClass.Frc6:
                if (highway is "road" or "track" or "unclassified" or "residential" or "living_street")
                {
                    return 1;
                }

                break;
            case FunctionalRoadClass.Frc7: // other class road.
                if (highway is "footway" or "bridleway" or "steps" or "path" or "living_street")
                {
                    return 1;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(frc), frc, null);
        }

        if (!string.IsNullOrEmpty(highway))
        {
            // for any other highway return a low match.
            return 0.2f;
        }

        return 0;
    }
}
