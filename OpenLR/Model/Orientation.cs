namespace OpenLR.Model
{
    /// <summary>
    /// Enumerates type of orientation.
    /// </summary>
    public enum Orientation
    {
        /// <summary>
        /// 0 - Point has no sense of orientation, or determination of orientation is not applicable (default).
        /// </summary>
        NoOrientation = 0,
        /// <summary>
        /// 1 - Point has orientation from first LRP towards second LRP.
        /// </summary>
        FirstToSecond = 1,
        /// <summary>
        /// 2 - Point has orientation from second LRP towards first LRP.
        /// </summary>
        SecondToFirst = 2,
        /// <summary>
        /// 3  - Point has orientation in both directions.
        /// </summary>
        BothDirections = 3
    }
}