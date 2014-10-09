namespace OpenLR.Referenced
{
    /// <summary>
    /// Represents an OsmSharp-specific location: a locatioin with an actual dependence on the routing network.
    /// </summary>
    public class ReferencedLocation
    {
        /// <summary>
        /// Holds an indication of accuracy [0-1].
        /// </summary>
        public float Score { get; set; }
    }
}