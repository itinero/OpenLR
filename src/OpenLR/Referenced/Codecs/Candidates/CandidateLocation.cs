﻿using Itinero;
using Itinero.Snapping;
using OpenLR.Referenced.Scoring;

namespace OpenLR.Referenced.Codecs.Candidates;

/// <summary>
/// Represents a candidate point and it's score, a location on the network that could be an LRP.
/// </summary>
public class CandidateLocation
{
    /// <summary>
    /// Gets or sets the score.
    /// </summary>
    public Score Score { get; set; }

    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    public SnapPoint Location { get; set; }
        
    /// <summary>
    /// Determines whether this object is equal to the given object.
    /// </summary>
    public override bool Equals(object obj)
    {
        var other = (obj as CandidateLocation);
        return other != null && other.Score == this.Score &&
               other.Location.EdgeId == this.Location.EdgeId &&
               other.Location.Offset == this.Location.Offset;
    }

    /// <summary>
    /// Serves as a hash function.
    /// </summary>
    public override int GetHashCode()
    {
        return this.Score.GetHashCode() ^
               this.Location.EdgeId.GetHashCode() ^
               this.Location.Offset.GetHashCode();
    }

    /// <summary>
    /// Returns a description of this candidate.
    /// </summary>
    public override string ToString()
    {
        return $"{this.Location.ToString()}: {this.Score.ToString()}";
    }
}
