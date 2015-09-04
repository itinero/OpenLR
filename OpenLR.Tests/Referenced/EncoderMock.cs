namespace OpenLR.Tests.Referenced
{
    /// <summary>
    /// A mockup for an encoder.
    /// </summary>
    class EncoderMock : OpenLR.Encoding.Encoder
    {

        public override Encoding.LocationEncoder<Locations.CircleLocation> CreateCircleLocationEncoder()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding.LocationEncoder<Locations.ClosedLineLocation> CreateClosedLineLocationEncoder()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding.LocationEncoder<Locations.GeoCoordinateLocation> CreateGeoCoordinateLocationEncoder()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding.LocationEncoder<Locations.GridLocation> CreateGridLocationEncoder()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding.LocationEncoder<Locations.LineLocation> CreateLineLocationEncoder()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding.LocationEncoder<Locations.PointAlongLineLocation> CreatePointAlongLineLocationEncoder()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding.LocationEncoder<Locations.PoiWithAccessPointLocation> CreatePoiWithAccessPointLocationEncoder()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding.LocationEncoder<Locations.PolygonLocation> CreatePolygonLocationEncoder()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding.LocationEncoder<Locations.RectangleLocation> CreateRectangleLocationEncoder()
        {
            throw new System.NotImplementedException();
        }
    }
}
