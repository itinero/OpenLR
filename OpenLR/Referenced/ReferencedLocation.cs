namespace OpenLR.Referenced
{
    /// <summary>
    /// Represents an OsmSharp-specific location: a locatioin with an actual dependence on the routing network.
    /// </summary>
    public abstract class ReferencedLocation
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();
    }
}