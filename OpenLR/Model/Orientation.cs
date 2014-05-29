namespace OpenLR.Model
{
    public enum Orientation
    {
        /// <summary>
        /// 0 - Point has no sense of SideOfRoad, or determination of SideOfRoad is not applicable (default).
        /// </summary>
        NoOrientation = 0,
        /// <summary>
        /// 1 - Point has SideOfRoad from first LRP towards second LRP.
        /// </summary>
        FirstToSecond = 1,
        /// <summary>
        /// 2 - Point has SideOfRoad from second LRP towards first LRP.
        /// </summary>
        SecondToFirst = 2,
        /// <summary>
        /// 3  - Point has SideOfRoad in both directions.
        /// </summary>
        BothDirections = 3
    }
}