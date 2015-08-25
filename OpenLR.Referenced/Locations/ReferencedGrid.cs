using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OpenLR.Referenced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Referenced.Locations
{
    /// <summary>
    /// Represents a referenced grid location with a graph as a reference.
    /// </summary>
    /// <remarks>The reference graph play no part here, a grid is just a grid.</remarks>
    public class ReferencedGrid : ReferencedLocation
    {
        /// <summary>
        /// Gets or sets the lower left latitude.
        /// </summary>
        public double LowerLeftLatitude { get; set; }

        /// <summary>
        /// Gets or sets the lower left longitude.
        /// </summary>
        public double LowerLeftLongitude { get; set; }

        /// <summary>
        /// Gets or sets the upper right latitude.
        /// </summary>
        public double UpperRightLatitude { get; set; }

        /// <summary>
        /// Gets or sets the upper right longitude.
        /// </summary>
        public double UpperRightLongitude { get; set; }

        /// <summary>
        /// Gets or sets the number of rows.
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// Gets or sets the number of columns.
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new ReferencedGrid()
            {
                LowerLeftLatitude = this.LowerLeftLatitude,
                LowerLeftLongitude = this.LowerLeftLongitude,
                UpperRightLatitude = this.UpperRightLatitude,
                UpperRightLongitude = this.UpperRightLongitude,
                Rows = this.Rows,
                Columns = this.Columns
            };
        }

        /// <summary>
        /// Converts this referenced location to a geometry.
        /// </summary>
        /// <returns></returns>
        public FeatureCollection ToFeatures()
        {
            // create the geometry factory.
            var geometryFactory = new GeometryFactory();

            // create the feature collection.
            var featureCollection = new FeatureCollection();

            //// create a point feature at each point in the grid.
            //double lonDiff = (this.LowerLeftLongitude - this.UpperRightLongitude) / this.Columns;
            //double latDiff = (this.UpperRightLatitude - this.LowerLeftLatitude) / this.Rows;
            //for (int column = 0; column < this.Columns; column++)
            //{
            //    double longitude = this.LowerLeftLongitude - (column * lonDiff);
            //    for (int row = 0; row < this.Rows; row++)
            //    {
            //        double latitude = this.UpperRightLatitude - (row * latDiff);
            //        var point = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
            //        var pointAttributes = new AttributesTable();
            //        featureCollection.Add(new Feature(point, pointAttributes));
            //    }
            //}

            // create a lineair ring.
            var lineairRingAttributes = new AttributesTable();
            featureCollection.Add(new Feature(geometryFactory.CreateLinearRing(new Coordinate[] {
                new Coordinate(this.LowerLeftLongitude, this.LowerLeftLatitude),
                new Coordinate(this.LowerLeftLongitude, this.UpperRightLatitude),
                new Coordinate(this.UpperRightLongitude, this.UpperRightLatitude),
                new Coordinate(this.UpperRightLongitude, this.LowerLeftLatitude),
                new Coordinate(this.LowerLeftLongitude, this.LowerLeftLatitude)
            }), lineairRingAttributes));
            return featureCollection;
        }
    }
}