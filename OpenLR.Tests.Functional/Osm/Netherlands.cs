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
using Itinero.LocalGeo;
using NetTopologySuite.Algorithm.Distance;
using NUnit.Framework;
using OpenLR.Geo;
using OpenLR.Osm;
using OpenLR.Referenced.Locations;
using OpenLR.Tests.Functional.NWB;

namespace OpenLR.Tests.Functional.Osm
{
    /// <summary>
    /// Contains tests for the netherlands.
    /// </summary>
    public static class Netherlands
    {
        /// <summary>
        /// Tests encoding/decoding on OSM-data.
        /// </summary>
        public static void TestAll(RouterDb routerDb)
        {
            TestEncodeDecodePointAlongLine(routerDb);
            TestEncodeDecodeRoutes(routerDb);
        }

        /// <summary>
        /// Tests encoding/decoding short routes.
        /// </summary>
        public static void TestEncodeDecodeRoutes(RouterDb routerDb)
        {
            var coder = new Coder(routerDb, new OsmCoderProfile(0.3f), new OpenLR.Codecs.Binary.BinaryCodec());

            var features = Extensions.FromGeoJsonFile(@".\Data\line_locations.geojson");

            var i = 0;
            foreach (var feature in features.Features)
            {
                var points = new Coordinate[2];
                var coordinates = (feature.Geometry as NetTopologySuite.Geometries.LineString).Coordinates;

                points[0] = new Coordinate((float)coordinates[0].Y, (float)coordinates[0].X);
                points[1] = new Coordinate((float)coordinates[1].Y, (float)coordinates[1].X);

                System.Console.WriteLine("Testing line location {0}/{1} @ {2}->{3}", i + 1, features.Features.Count, 
                    points[0].ToInvariantString(), points[1].ToInvariantString());
                TestEncodeDecoderRoute(coder, points);

                i++;
            }
        }

        /// <summary>
        /// Tests encoding/decoding a route.
        /// </summary>
        public static void TestEncodeDecoderRoute(Coder coder, Coordinate[] points)
        {
            var referencedLine = coder.BuildLine(points[0], points[1]);
            var referencedLineJson = referencedLine.ToFeatures(coder.Router.Db).ToGeoJson();

            var encoded = coder.Encode(referencedLine);

            var decodedReferencedLine = coder.Decode(encoded) as ReferencedLine;
            var decodedReferencedLineJson = decodedReferencedLine.ToFeatures(coder.Router.Db).ToGeoJson();

            var distance = DiscreteHausdorffDistance.Distance(referencedLine.ToLineString(coder.Router.Db),
                decodedReferencedLine.ToLineString(coder.Router.Db));

            Assert.IsTrue(distance < .1);
        }

        /// <summary>
        /// Tests encoding/decoding point along line locations.
        /// </summary>
        public static void TestEncodeDecodePointAlongLine(RouterDb routerDb)
        {
            var coder = new Coder(routerDb, new OsmCoderProfile(0.3f), new OpenLR.Codecs.Binary.BinaryCodec());
            
            var locations = Extensions.PointsFromGeoJsonFile(@".\Data\locations.geojson");

            //var locations = new List<Coordinate>();
            //locations.Add(new Coordinate(51.4498f, 5.44964f));
            //locations.Add(new Coordinate(51.3654f, 5.29525f));
            //locations.Add(new Coordinate(51.8501f, 4.31797f));
            //locations.Add(new Coordinate(52.5568f, 5.90423f)); // 4

            //locations.Add(new Coordinate(52.8746f, 5.98115f));
            //locations.Add(new Coordinate(51.9477f, 6.2842f));
            //locations.Add(new Coordinate(51.0621f, 5.82019f));
            //locations.Add(new Coordinate(51.5303f, 4.26908f)); // 8

            //locations.Add(new Coordinate(52.3747f, 5.76982f));
            //locations.Add(new Coordinate(51.2443f, 5.69533f));
            //////locations.Add(new Coordinate(51.8901f, 4.17898f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.5131f, 5.47854f));

            //locations.Add(new Coordinate(52.4127f, 6.60098f)); // 12
            //locations.Add(new Coordinate(52.763f, 6.90822f));
            //locations.Add(new Coordinate(51.251f, 5.71381f));
            //locations.Add(new Coordinate(51.9604f, 4.17883f));

            //locations.Add(new Coordinate(52.5569f, 5.88497f)); // 16
            //locations.Add(new Coordinate(52.1642f, 4.45662f));
            //locations.Add(new Coordinate(51.9482f, 6.03063f));
            //locations.Add(new Coordinate(52.6433f, 6.20174f));

            //locations.Add(new Coordinate(50.8762f, 5.98055f)); // 20
            //locations.Add(new Coordinate(52.7806f, 6.11608f));
            //locations.Add(new Coordinate(51.2383f, 5.70004f));
            //locations.Add(new Coordinate(52.6668398676016f, 6.75843159955385f));

            //locations.Add(new Coordinate(52.2417171648453f, 6.76068902214294f)); // 24
            ////locations.Add(new Coordinate(52.2649281481956f, 6.79163185935606f)); // Street doesn't allow car traffic: Could not build point along line: Could not find an edge close to the given location.
            //locations.Add(new Coordinate(52.2849958097548f, 6.82090288400939f));
            //locations.Add(new Coordinate(52.2874338908226f, 6.83455925036983f));

            //locations.Add(new Coordinate(52.2255065307597f, 6.84294875514722f));
            //locations.Add(new Coordinate(53.3438742228396f, 6.90324560503569f)); //28 
            //locations.Add(new Coordinate(52.4081097131379f, 6.87420189159591f));
            //locations.Add(new Coordinate(52.1908575845447f, 6.8728231939004f));

            //locations.Add(new Coordinate(52.7517359438934f, 6.89204843070648f));
            ////locations.Add(new Coordinate(52.311292669198f, 6.88650216012963f)); // TODO: Could not build point along line: Could not find an edge close to the given location.
            ////locations.Add(new Coordinate(52.2216828725473f, 6.88986576006998f)); // TODO: Could not build point along line: Could not find an edge close to the given location.
            //locations.Add(new Coordinate(52.2157241982115f, 6.89015062279853f)); // 32

            //locations.Add(new Coordinate(52.3174899907749f, 6.90375101987198f));
            //locations.Add(new Coordinate(52.788739229037f, 6.9279679252794f));
            //locations.Add(new Coordinate(52.2219808449457f, 6.91208543880988f));
            //locations.Add(new Coordinate(52.2874026392575f, 6.92158255694133f)); // 36

            //locations.Add(new Coordinate(53.1756794106863f, 6.97176783559564f));
            //locations.Add(new Coordinate(52.2165225156316f, 6.9410299706219f));
            //locations.Add(new Coordinate(52.9861347983993f, 6.98787732451992f));
            //locations.Add(new Coordinate(52.221696027067f, 6.97341739877845f)); // 40

            //locations.Add(new Coordinate(53.1035603672337f, 7.00781270448309f));
            //locations.Add(new Coordinate(51.508832344031f, 3.87358806392666f));
            //locations.Add(new Coordinate(51.7474248439024f, 4.1091581488686f));
            //locations.Add(new Coordinate(51.5615076810109f, 3.50095171788601f)); // 44

            //locations.Add(new Coordinate(50.8764958779182f, 5.83299994468689f));
            //locations.Add(new Coordinate(50.8774978023329f, 5.829244852066039f));
            //locations.Add(new Coordinate(50.88243602716634f, 5.879353880882263f));
            //locations.Add(new Coordinate(50.98669327261579f, 5.805244445800781f)); // 48

            //locations.Add(new Coordinate(50.98969327870194f, 5.784392755091851f));
            //locations.Add(new Coordinate(50.99880219389809f, 5.799281419715974f));
            //locations.Add(new Coordinate(50.998913284652595f, 5.798812012734767f));
            //locations.Add(new Coordinate(51.00150877020324f, 5.7903313636779785f)); // 52

            //locations.Add(new Coordinate(51.019367364425136f, 5.809208204120425f));
            //locations.Add(new Coordinate(51.0306302054113f, 5.821887880219136f));
            //locations.Add(new Coordinate(51.04456433902737f, 5.844683647155762f));
            //locations.Add(new Coordinate(51.069975261279964f, 5.822976323551818f)); // 56

            //locations.Add(new Coordinate(51.19301822209427f, 5.995362417665828f));
            //locations.Add(new Coordinate(51.19344097091757f, 5.995565659694641f));
            ////locations.Add(new Coordinate(51.19350349484122f, 5.984800749775194f)); // Street doesn't allow car traffic: The point in the ReferencedPointAlongLine could not be projected on the referenced edge.
            ////locations.Add(new Coordinate(51.19378761316388f, 5.984652042388916f)); // Street doesn't allow car traffic: Could not build point along line: Could not find an edge close to the given location.

            ////locations.Add(new Coordinate(51.194090056911485f, 5.984622848724452f)); // Street doesn't allow car traffic: Could not build point along line: Could not find an edge close to the given location.
            ////locations.Add(new Coordinate(51.19411042962273f, 5.984651696278578f)); // Street doesn't allow car traffic: Could not build point along line: Could not find an edge close to the given location.
            //locations.Add(new Coordinate(51.19295819286386f, 5.995333557827781f));
            //locations.Add(new Coordinate(51.21517053608611f, 5.89940071105957f)); // 60

            //locations.Add(new Coordinate(51.21769066040184f, 5.8908605575561515f));
            //locations.Add(new Coordinate(51.25164008209833f, 5.686159093619339f));
            //locations.Add(new Coordinate(51.252114084586374f, 5.706900507377988f));
            //locations.Add(new Coordinate(51.25297546386719f, 5.7227702140808105f)); // 64

            ////locations.Add(new Coordinate(51.2548427360395f, 5.715597635081922f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.26953147394792f, 5.721618320920545f));
            //locations.Add(new Coordinate(51.27033414432933f, 5.725992171289633f));
            //locations.Add(new Coordinate(51.4498f, 5.44964f));

            //locations.Add(new Coordinate(51.3654f, 5.29525f));
            //locations.Add(new Coordinate(51.8501f, 4.31797f));
            //locations.Add(new Coordinate(52.5568f, 5.90423f));
            //locations.Add(new Coordinate(52.8746f, 5.98115f));

            //locations.Add(new Coordinate(51.9477f, 6.2842f));
            //locations.Add(new Coordinate(51.0621f, 5.82019f));
            //locations.Add(new Coordinate(51.5303f, 4.26908f));
            //locations.Add(new Coordinate(52.3747f, 5.76982f));

            //locations.Add(new Coordinate(51.2443f, 5.69533f));
            ////locations.Add(new Coordinate(51.8901f, 4.17898f)); // // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.5131f, 5.47854f));
            //locations.Add(new Coordinate(52.4127f, 6.60098f));

            //locations.Add(new Coordinate(52.763f, 6.90822f));
            //locations.Add(new Coordinate(51.251f, 5.71381f));
            //locations.Add(new Coordinate(51.9604f, 4.17883f));
            //locations.Add(new Coordinate(52.5569f, 5.88497f));

            //locations.Add(new Coordinate(52.1642f, 4.45662f));
            //locations.Add(new Coordinate(51.9482f, 6.03063f));
            //locations.Add(new Coordinate(52.6433f, 6.20174f));
            //locations.Add(new Coordinate(50.8762f, 5.98055f));

            //locations.Add(new Coordinate(52.7806f, 6.11608f));
            //locations.Add(new Coordinate(51.2383f, 5.70004f));
            //locations.Add(new Coordinate(51.56372983f, 3.552754182f));
            //locations.Add(new Coordinate(51.47051437f, 3.816054197f));

            //locations.Add(new Coordinate(51.34056004f, 3.824364554f));
            //locations.Add(new Coordinate(51.3257767f, 3.825693161f));
            //locations.Add(new Coordinate(51.4370814f, 3.85682696f));
            //locations.Add(new Coordinate(51.50883234f, 3.873588064f));

            //locations.Add(new Coordinate(51.79171808f, 3.882509989f));
            //locations.Add(new Coordinate(51.65258875f, 3.903109591f));
            //locations.Add(new Coordinate(51.66739669f, 4.093991017f));
            ////locations.Add(new Coordinate(51.75521481f, 4.16095086f)); // Street doesn't allow car traffic: 

            //locations.Add(new Coordinate(51.54717757f, 4.182867221f));
            ////locations.Add(new Coordinate(52.08823266f, 4.251664804f)); // Street doesn't allow car traffic: Could not build point along line: Could not find an edge close to the given location.
            //locations.Add(new Coordinate(51.50275787f, 4.272298422f));
            //locations.Add(new Coordinate(52.02688639f, 4.26393405f));

            //locations.Add(new Coordinate(51.47526808f, 4.285166192f));
            //locations.Add(new Coordinate(51.4929897f, 4.300415331f));
            //locations.Add(new Coordinate(52.04560804f, 4.287458295f));
            //locations.Add(new Coordinate(52.1174354f, 4.287207609f));

            //locations.Add(new Coordinate(51.49332866f, 4.30430339f));
            //locations.Add(new Coordinate(51.90916661f, 4.315260159f));
            //locations.Add(new Coordinate(51.90500883f, 4.316973287f));
            //locations.Add(new Coordinate(52.06579914f, 4.318500203f));

            //locations.Add(new Coordinate(51.86610882f, 4.355169838f));
            //locations.Add(new Coordinate(52.07559827f, 4.357522134f));
            ////locations.Add(new Coordinate(51.86082877f, 4.366463233f)); // Street doesn't allow car traffic: .
            ////locations.Add(new Coordinate(52.10444144f, 4.363680142f)); // Street doesn't allow car traffic: .

            ////locations.Add(new Coordinate(52.12849749f, 4.365883786f)); // Street doesn't allow car traffic: Could not build point along line: Could not find an edge close to the given location.
            //locations.Add(new Coordinate(51.91048937f, 4.375295839f));
            ////locations.Add(new Coordinate(52.07073903f, 4.378035671f)); // Street doesn't allow car traffic: Could not build point along line: Could not find an edge close to the given location.
            ////locations.Add(new Coordinate(52.04445109f, 4.390736778f));  // Street doesn't allow car traffic: .

            //locations.Add(new Coordinate(51.86114259f, 4.395414416f));
            //locations.Add(new Coordinate(52.2016627f, 4.396433988f));
            //locations.Add(new Coordinate(51.93650977f, 4.428547288f));
            //locations.Add(new Coordinate(52.18361019f, 4.425566424f));

            //locations.Add(new Coordinate(51.51365732f, 4.440748533f));
            ////locations.Add(new Coordinate(51.87945683f, 4.434884701f)); // Street doesn't allow car traffic: Could not build point along line: Could not find an edge close to the given location.
            //locations.Add(new Coordinate(51.52080847f, 4.448897481f));
            ////locations.Add(new Coordinate(51.52979392f, 4.451449917f)); // Street doesn't allow car traffic: Could not build point along line: Could not find an edge close to the given location.

            //locations.Add(new Coordinate(52.01472555f, 4.444525597f));
            //locations.Add(new Coordinate(52.19208008f, 4.441838636f));
            //locations.Add(new Coordinate(51.52384593f, 4.459021195f));
            //locations.Add(new Coordinate(51.91505427f, 4.464650802f));

            //locations.Add(new Coordinate(51.52282419f, 4.473730447f));
            //locations.Add(new Coordinate(51.89720665f, 4.468611872f));
            //locations.Add(new Coordinate(51.87419448f, 4.469435397f));
            ////locations.Add(new Coordinate(52.13276212f, 4.465134396f)); // Street doesn't allow car traffic: .

            //locations.Add(new Coordinate(52.17701342f, 4.469076182f));
            //locations.Add(new Coordinate(51.99400258f, 4.478028035f));
            //locations.Add(new Coordinate(52.1427354f, 4.475616643f));
            //locations.Add(new Coordinate(52.19415243f, 4.478677714f));

            ////locations.Add(new Coordinate(51.87731067f, 4.494497816f)); // Street doesn't allow car traffic: .
            //locations.Add(new Coordinate(51.89708262f, 4.513336354f));
            //locations.Add(new Coordinate(52.14964877f, 4.509222206f));
            //locations.Add(new Coordinate(52.05620732f, 4.511233838f));

            //locations.Add(new Coordinate(52.17371071f, 4.510741244f));
            ////locations.Add(new Coordinate(52.05628069f, 4.520773852f)); // Street doesn't allow car traffic: Could not build point along line: Could not find an edge close to the given location.
            ////locations.Add(new Coordinate(52.21602533f, 4.517672816f)); // Street doesn't allow car traffic: .
            //locations.Add(new Coordinate(51.88048444f, 4.529008103f));

            ////locations.Add(new Coordinate(51.92100634f, 4.534018899f)); // Street doesn't allow car traffic: Could not build point along line: Could not find an edge close to the given location.
            //locations.Add(new Coordinate(51.87927874f, 4.546318915f));
            ////locations.Add(new Coordinate(51.88688419f, 4.546536842f)); // Street doesn't allow car traffic: failed.
            //locations.Add(new Coordinate(51.95197756f, 4.570934597f));

            //locations.Add(new Coordinate(51.91477346f, 4.58944108f));
            //locations.Add(new Coordinate(51.8641307f, 4.600381837f));
            //locations.Add(new Coordinate(51.9415922f, 4.601048374f));
            //locations.Add(new Coordinate(52.27413421f, 4.620021f));

            //locations.Add(new Coordinate(51.8269901f, 4.627957343f));
            //locations.Add(new Coordinate(51.84664662f, 4.636642945f));
            //locations.Add(new Coordinate(52.26467098f, 4.636750136f));
            //locations.Add(new Coordinate(52.47416366f, 4.640814628f));

            //locations.Add(new Coordinate(52.37575626f, 4.653126454f));
            //locations.Add(new Coordinate(51.79123831f, 4.663918663f));
            //locations.Add(new Coordinate(52.15185756f, 4.658257673f));
            //locations.Add(new Coordinate(52.30424744f, 4.656544608f));

            //locations.Add(new Coordinate(52.35683184f, 4.662358028f));
            //locations.Add(new Coordinate(52.12980324f, 4.667092485f));
            //locations.Add(new Coordinate(51.98296456f, 4.670400212f));
            //locations.Add(new Coordinate(52.35360973f, 4.666783103f));

            //locations.Add(new Coordinate(52.49874574f, 4.680235354f));
            //locations.Add(new Coordinate(52.63634049f, 4.679916243f));
            //locations.Add(new Coordinate(51.49006819f, 4.739378594f));
            //locations.Add(new Coordinate(52.01426289f, 4.740479614f));

            //locations.Add(new Coordinate(51.60923799f, 4.75459351f));
            //locations.Add(new Coordinate(52.50554962f, 4.750986851f));
            //locations.Add(new Coordinate(52.89500429f, 4.750403903f));
            //locations.Add(new Coordinate(51.60759023f, 4.7694678f));

            //locations.Add(new Coordinate(52.5003827f, 4.76745208f));
            //locations.Add(new Coordinate(52.96453846f, 4.764245106f));
            //locations.Add(new Coordinate(52.36712548f, 4.783815349f));
            //locations.Add(new Coordinate(52.46204715f, 4.791426821f));

            //locations.Add(new Coordinate(53.05681002f, 4.785026664f));
            //locations.Add(new Coordinate(51.5944741f, 4.80873069f));
            //locations.Add(new Coordinate(51.67264536f, 4.812237561f));
            //locations.Add(new Coordinate(52.35677289f, 4.806206819f));

            //locations.Add(new Coordinate(52.67473867f, 4.835484717f));
            //locations.Add(new Coordinate(52.42395333f, 4.8440937f));
            //locations.Add(new Coordinate(51.65921221f, 4.862902756f));
            //locations.Add(new Coordinate(52.3142589f, 4.860210563f));

            //locations.Add(new Coordinate(52.30841929f, 4.864315295f));
            //locations.Add(new Coordinate(52.33259883f, 4.869244853f));
            //locations.Add(new Coordinate(52.4414277f, 4.875569025f));
            ////locations.Add(new Coordinate(52.34072094f, 4.879018289f)); // Street doesn't allow car traffic: Could not build point along line: Could not find an edge close to the given location.

            //locations.Add(new Coordinate(52.40644664f, 4.91929707f));
            //locations.Add(new Coordinate(52.3049591f, 4.922397581f));
            //locations.Add(new Coordinate(52.22137887f, 4.936179545f));
            //locations.Add(new Coordinate(52.51095241f, 4.944444566f));

            //locations.Add(new Coordinate(52.52303653f, 4.956581728f));
            ////locations.Add(new Coordinate(52.50285686f, 4.965854647f)); // Street doesn't allow car traffic: 
            //locations.Add(new Coordinate(51.59540173f, 5.026970279f));
            //locations.Add(new Coordinate(52.09912387f, 5.03544428f));

            ////locations.Add(new Coordinate(52.4691524f, 5.067603126f));  // TODO: investigate.
            //locations.Add(new Coordinate(52.49997446f, 5.079615773f));
            //locations.Add(new Coordinate(52.03321233f, 5.082886603f));
            //locations.Add(new Coordinate(52.02843719f, 5.086759022f));

            //locations.Add(new Coordinate(51.59061289f, 5.092330928f));
            //locations.Add(new Coordinate(52.02746234f, 5.091004576f));
            //locations.Add(new Coordinate(52.07726253f, 5.097013861f));
            ////locations.Add(new Coordinate(52.01545308f, 5.099642758f)); // Street doesn't allow car traffic: 

            //locations.Add(new Coordinate(52.09308651f, 5.105896007f));
            //locations.Add(new Coordinate(52.11762486f, 5.117496955f));
            //locations.Add(new Coordinate(52.23078671f, 5.159554256f));
            //locations.Add(new Coordinate(52.27946986f, 5.16023229f));

            //locations.Add(new Coordinate(52.02829867f, 5.162315018f));
            //locations.Add(new Coordinate(52.39438837f, 5.171088987f));
            //locations.Add(new Coordinate(52.09531849f, 5.180483798f));
            //locations.Add(new Coordinate(52.6733354f, 5.182693846f));

            //locations.Add(new Coordinate(52.35745941f, 5.190538674f));
            //locations.Add(new Coordinate(52.37132135f, 5.216612326f));
            ////locations.Add(new Coordinate(52.37509099f, 5.217526206f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.94881386f, 5.243606319f));

            //locations.Add(new Coordinate(52.69564154f, 5.242183938f));
            ////locations.Add(new Coordinate(52.07629418f, 5.262712158f)); // TODO: check this issue!
            //locations.Add(new Coordinate(51.66012603f, 5.287244635f));
            //locations.Add(new Coordinate(52.21726264f, 5.301312991f));

            //locations.Add(new Coordinate(51.50810903f, 5.312320546f));
            ////locations.Add(new Coordinate(51.63793692f, 5.349219173f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.48998399f, 5.384288849f));
            //locations.Add(new Coordinate(51.27034867f, 5.398490108f));

            //locations.Add(new Coordinate(51.68431033f, 5.400896406f));
            ////locations.Add(new Coordinate(51.89223488f, 5.412933459f)); // Street doesn't allow car traffic: failed.
            //locations.Add(new Coordinate(51.40587692f, 5.41877211f));
            //locations.Add(new Coordinate(51.67947953f, 5.419519335f));

            //locations.Add(new Coordinate(52.16912018f, 5.426097128f));
            //locations.Add(new Coordinate(52.1935144f, 5.430970393f));
            //locations.Add(new Coordinate(51.89111473f, 5.445112195f));
            //locations.Add(new Coordinate(51.42734169f, 5.451880011f));

            //locations.Add(new Coordinate(51.47360684f, 5.45256234f));
            //locations.Add(new Coordinate(51.36207998f, 5.454003189f));
            //locations.Add(new Coordinate(52.51754728f, 5.460103178f));
            //locations.Add(new Coordinate(52.58068555f, 5.462881897f));

            //locations.Add(new Coordinate(52.22164297f, 5.468964993f));
            //locations.Add(new Coordinate(51.55017608f, 5.486927501f));
            //locations.Add(new Coordinate(52.12103387f, 5.502109303f));
            //locations.Add(new Coordinate(51.77185527f, 5.526265938f));

            //locations.Add(new Coordinate(51.42733794f, 5.537065135f));
            //locations.Add(new Coordinate(51.62148079f, 5.547132255f));
            //locations.Add(new Coordinate(51.63493867f, 5.556676628f));
            //locations.Add(new Coordinate(51.62455006f, 5.55846704f));

            //locations.Add(new Coordinate(51.43479081f, 5.558964584f));
            //locations.Add(new Coordinate(52.89632271f, 5.568718464f));
            //locations.Add(new Coordinate(52.04072728f, 5.565503291f));
            //locations.Add(new Coordinate(53.14608469f, 5.571106107f));

            //locations.Add(new Coordinate(51.9590452f, 5.567253661f));
            //locations.Add(new Coordinate(52.0234651f, 5.569120411f));
            ////locations.Add(new Coordinate(51.65912938f, 5.611854729f)); // Street doesn't allow car traffic: failed.
            //locations.Add(new Coordinate(51.65495125f, 5.619264927f));

            //locations.Add(new Coordinate(53.1787399f, 5.635037316f));
            //locations.Add(new Coordinate(53.02147862f, 5.643256698f));
            //locations.Add(new Coordinate(50.84257514f, 5.649526285f));
            //locations.Add(new Coordinate(50.8542436f, 5.657991117f));

            ////locations.Add(new Coordinate(52.02848656f, 5.672092208f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.99854413f, 5.674286722f));
            //locations.Add(new Coordinate(51.56491535f, 5.693276877f));
            //locations.Add(new Coordinate(51.38465611f, 5.72844761f));

            //locations.Add(new Coordinate(52.86512206f, 5.749191503f));
            //locations.Add(new Coordinate(51.21411189f, 5.757837364f));
            //locations.Add(new Coordinate(51.46454665f, 5.768989658f));
            //locations.Add(new Coordinate(51.83884496f, 5.805907365f));

            //locations.Add(new Coordinate(51.84925108f, 5.815778238f));
            //locations.Add(new Coordinate(51.46512439f, 5.814914267f));
            //locations.Add(new Coordinate(50.98805414f, 5.823664203f));
            //locations.Add(new Coordinate(53.20580263f, 5.849517827f));

            //locations.Add(new Coordinate(52.44827563f, 5.84965673f));
            //locations.Add(new Coordinate(51.00231566f, 5.852517736f));
            //locations.Add(new Coordinate(52.41922619f, 5.872307495f));
            //locations.Add(new Coordinate(51.84138201f, 5.866522167f));

            //locations.Add(new Coordinate(51.95398142f, 5.871891054f));
            //locations.Add(new Coordinate(51.06392871f, 5.871607359f));
            //locations.Add(new Coordinate(51.00989218f, 5.881090892f));
            //locations.Add(new Coordinate(52.95835288f, 5.909007996f));

            //locations.Add(new Coordinate(51.9080027f, 5.921084978f));
            //locations.Add(new Coordinate(51.96048609f, 5.922294979f));
            //locations.Add(new Coordinate(52.18185771f, 5.945521881f));
            //locations.Add(new Coordinate(51.66329702f, 5.940404383f));

            ////locations.Add(new Coordinate(52.18374788f, 5.964574367f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(52.1846867f, 5.967010676f));
            //locations.Add(new Coordinate(51.98173629f, 5.96656784f));
            //locations.Add(new Coordinate(51.98542981f, 5.977889951f));

            //locations.Add(new Coordinate(50.92660588f, 5.967339029f));
            //locations.Add(new Coordinate(51.17829231f, 5.975224744f));
            //locations.Add(new Coordinate(52.20367648f, 5.990276487f));
            //locations.Add(new Coordinate(52.18782345f, 5.992749296f));

            //locations.Add(new Coordinate(52.22307756f, 5.996442269f));
            //locations.Add(new Coordinate(52.20794567f, 5.996430294f));
            ////locations.Add(new Coordinate(50.92364848f, 5.980858021f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(52.21268623f, 5.999269007f));

            //locations.Add(new Coordinate(51.66460761f, 5.994953482f));
            //locations.Add(new Coordinate(51.20314257f, 5.988896676f));
            //locations.Add(new Coordinate(52.39389645f, 6.043222745f));
            //locations.Add(new Coordinate(51.35753851f, 6.048795189f));

            //locations.Add(new Coordinate(51.12294803f, 6.045684716f));
            //locations.Add(new Coordinate(52.53039807f, 6.07815481f));
            //locations.Add(new Coordinate(53.09638301f, 6.104354464f));
            //locations.Add(new Coordinate(52.50934255f, 6.096690169f));

            //locations.Add(new Coordinate(52.52802379f, 6.098381483f));
            //locations.Add(new Coordinate(51.39129733f, 6.098778271f));
            //locations.Add(new Coordinate(53.10925365f, 6.142605109f));
            //locations.Add(new Coordinate(52.49135651f, 6.1406423f));

            //locations.Add(new Coordinate(52.25879314f, 6.150641575f));
            ////locations.Add(new Coordinate(53.4787861f, 6.174752156f)); // TODO: check oneway and dead-end stuff.
            //locations.Add(new Coordinate(51.90764385f, 6.150761601f));
            //locations.Add(new Coordinate(51.36562996f, 6.151823076f));

            //locations.Add(new Coordinate(52.23609373f, 6.176598435f));
            //locations.Add(new Coordinate(52.06467075f, 6.180986989f));
            //locations.Add(new Coordinate(52.71499808f, 6.194237852f));
            //locations.Add(new Coordinate(51.35053074f, 6.173613342f));

            //locations.Add(new Coordinate(52.23824185f, 6.210903474f));
            //locations.Add(new Coordinate(52.13453627f, 6.235173553f));
            //locations.Add(new Coordinate(52.34188034f, 6.267908947f));
            ////locations.Add(new Coordinate(53.01289436f, 6.321036036f));  // Street doesn't allow car traffic: failed

            //locations.Add(new Coordinate(51.94790622f, 6.313454825f));
            //locations.Add(new Coordinate(53.1819825f, 6.37899202f));
            //locations.Add(new Coordinate(51.90370011f, 6.367420155f));
            //locations.Add(new Coordinate(52.16472628f, 6.403725679f));

            ////locations.Add(new Coordinate(52.52604537f, 6.422070914f));  // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(52.28269254f, 6.422741014f));
            //locations.Add(new Coordinate(52.37413967f, 6.470551377f));
            //locations.Add(new Coordinate(52.36124757f, 6.509971281f));

            //locations.Add(new Coordinate(53.00837164f, 6.545681294f));
            //locations.Add(new Coordinate(52.11705711f, 6.527042725f));
            //locations.Add(new Coordinate(52.45994005f, 6.571509273f));
            //locations.Add(new Coordinate(53.27931695f, 6.600781944f));

            //locations.Add(new Coordinate(52.35762571f, 6.591526364f));
            //locations.Add(new Coordinate(52.34358958f, 6.642909881f));
            ////locations.Add(new Coordinate(52.36572737f, 6.67862434f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(52.82908155f, 6.694436712f));

            ////locations.Add(new Coordinate(52.34642419f, 6.684570358f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(53.17301219f, 6.756284775f));
            ////locations.Add(new Coordinate(52.99645898f, 6.758298257f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(53.01040422f, 6.760862346f));

            ////locations.Add(new Coordinate(52.1682897f, 6.740753613f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(52.66683987f, 6.7584316f));
            //locations.Add(new Coordinate(52.24171716f, 6.760689022f));
            ////locations.Add(new Coordinate(52.26492815f, 6.791631859f)); // Street doesn't allow car traffic: failed

            //locations.Add(new Coordinate(52.40810971f, 6.874201892f));
            //locations.Add(new Coordinate(52.19085758f, 6.872823194f));
            //locations.Add(new Coordinate(52.75173594f, 6.892048431f));
            ////locations.Add(new Coordinate(52.22168287f, 6.88986576f)); // Street doesn't allow car traffic: failed

            //locations.Add(new Coordinate(52.31748999f, 6.90375102f));
            //locations.Add(new Coordinate(52.22198084f, 6.912085439f));
            //locations.Add(new Coordinate(52.28740264f, 6.921582557f));
            //locations.Add(new Coordinate(53.17567941f, 6.971767836f));

            //locations.Add(new Coordinate(52.21652252f, 6.941029971f));
            //locations.Add(new Coordinate(52.22169603f, 6.973417399f));
            //locations.Add(new Coordinate(53.10356037f, 7.007812704f));
            //locations.Add(new Coordinate(51.563729831293f, 3.55275418234898f));

            //locations.Add(new Coordinate(51.4705143650449f, 3.81605419748636f));
            //locations.Add(new Coordinate(51.3405600356253f, 3.82436455426515f));
            //locations.Add(new Coordinate(51.3257767039377f, 3.8256931613602f));
            //locations.Add(new Coordinate(51.4370813962264f, 3.85682696024089f));

            //locations.Add(new Coordinate(51.508832344031f, 3.87358806392666f));
            //locations.Add(new Coordinate(51.7917180840328f, 3.88250998927826f));
            //locations.Add(new Coordinate(51.6525887537122f, 3.90310959052383f));
            //locations.Add(new Coordinate(51.6673966887632f, 4.09399101684123f));

            ////locations.Add(new Coordinate(51.7552148079694f, 4.16095085983675f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.5471775718201f, 4.18286722147375f));
            ////locations.Add(new Coordinate(52.0882326620573f, 4.25166480421198f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.5027578708182f, 4.27229842181587f));

            //locations.Add(new Coordinate(52.0268863920622f, 4.26393404973391f));
            //locations.Add(new Coordinate(51.4752680826043f, 4.28516619153359f));
            //locations.Add(new Coordinate(51.4929897042935f, 4.30041533096572f));
            ////locations.Add(new Coordinate(52.0456080365624f, 4.28745829481848f)); // Street doesn't allow car traffic: failed

            //locations.Add(new Coordinate(52.1174353972545f, 4.28720760903592f));
            //locations.Add(new Coordinate(51.4933286564293f, 4.30430338954614f));
            //locations.Add(new Coordinate(51.9091666146737f, 4.31526015945274f));
            //locations.Add(new Coordinate(51.9050088316544f, 4.31697328734405f));

            //locations.Add(new Coordinate(52.0657991364996f, 4.31850020276214f));
            //locations.Add(new Coordinate(51.8661088181781f, 4.35516983846154f));
            //locations.Add(new Coordinate(52.0755982747493f, 4.35752213416407f));
            ////locations.Add(new Coordinate(51.8608287694928f, 4.36646323262661f)); // Street doesn't allow car traffic: failed

            ////locations.Add(new Coordinate(52.128497491626f, 4.36588378649293f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.9104893656212f, 4.37529583908562f));
            ////locations.Add(new Coordinate(52.0707390261386f, 4.37803567137929f)); // Street doesn't allow car traffic: failed
            ////locations.Add(new Coordinate(52.0444510945626f, 4.39073677829253f)); // Street doesn't allow car traffic: failed

            //locations.Add(new Coordinate(51.8611425888183f, 4.39541441646128f));
            //locations.Add(new Coordinate(52.2016626984203f, 4.39643398777683f));
            //locations.Add(new Coordinate(51.9365097694435f, 4.42854728800315f));
            //locations.Add(new Coordinate(52.1836101869893f, 4.42556642405603f));

            //locations.Add(new Coordinate(51.5136573168075f, 4.44074853322156f));
            ////locations.Add(new Coordinate(51.8794568314641f, 4.43488470063439f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.5208084736544f, 4.44889748124757f));
            ////locations.Add(new Coordinate(51.5297939240114f, 4.45144991712208f)); // Street doesn't allow car traffic: failed

            //locations.Add(new Coordinate(52.0147255464849f, 4.44452559747424f));
            //locations.Add(new Coordinate(52.1920800804858f, 4.44183863575172f));
            //locations.Add(new Coordinate(51.5238459316361f, 4.45902119525924f));
            //locations.Add(new Coordinate(51.9150542747988f, 4.46465080235518f));

            //locations.Add(new Coordinate(51.5228241887611f, 4.473730447188f));
            //locations.Add(new Coordinate(51.8972066464358f, 4.46861187176796f));
            //locations.Add(new Coordinate(51.8741944809287f, 4.46943539723876f));
            ////locations.Add(new Coordinate(52.1327621190071f, 4.46513439647832f)); // Street doesn't allow car traffic: failed

            //locations.Add(new Coordinate(52.177013417293f, 4.46907618218066f));
            //locations.Add(new Coordinate(51.9940025832687f, 4.47802803474858f));
            //locations.Add(new Coordinate(52.1427353969003f, 4.47561664293413f));
            //locations.Add(new Coordinate(52.1941524289914f, 4.47867771384714f));

            ////locations.Add(new Coordinate(51.8773106734569f, 4.49449781563314f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.8970826178193f, 4.5133363536793f));
            //locations.Add(new Coordinate(52.1496487660397f, 4.50922220612092f));
            //locations.Add(new Coordinate(52.0562073208634f, 4.51123383779991f));

            //locations.Add(new Coordinate(52.1737107055531f, 4.51074124362659f));
            ////locations.Add(new Coordinate(52.056280688276f, 4.52077385244267f)); // Street doesn't allow car traffic: failed
            ////locations.Add(new Coordinate(52.2160253252616f, 4.51767281643094f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.8804844404514f, 4.52900810254166f));

            ////locations.Add(new Coordinate(51.921006339764f, 4.53401889909516f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.8792787400512f, 4.54631891506012f));
            ////locations.Add(new Coordinate(51.8868841876135f, 4.54653684160814f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.9519775610962f, 4.57093459728524f));

            //locations.Add(new Coordinate(51.9147734555286f, 4.58944108007123f));
            //locations.Add(new Coordinate(51.8641307046617f, 4.60038183655572f));
            //locations.Add(new Coordinate(51.9415922049585f, 4.60104837431431f));
            //locations.Add(new Coordinate(52.2741342093619f, 4.62002099979192f));

            //locations.Add(new Coordinate(51.8269901039973f, 4.62795734342995f));
            //locations.Add(new Coordinate(51.8466466206489f, 4.6366429445765f));
            //locations.Add(new Coordinate(52.2646709833979f, 4.63675013574146f));
            //locations.Add(new Coordinate(52.4741636557722f, 4.64081462828997f));

            //locations.Add(new Coordinate(52.3757562615982f, 4.6531264536051f));
            //locations.Add(new Coordinate(51.7912383146789f, 4.66391866250637f));
            //locations.Add(new Coordinate(52.1518575637663f, 4.65825767338785f));
            //locations.Add(new Coordinate(52.3042474417539f, 4.65654460756634f));

            //locations.Add(new Coordinate(52.3568318412512f, 4.66235802849867f));
            //locations.Add(new Coordinate(52.1298032350558f, 4.66709248534592f));
            //locations.Add(new Coordinate(51.9829645623349f, 4.67040021249576f));
            //locations.Add(new Coordinate(52.3536097301133f, 4.66678310333575f));

            //locations.Add(new Coordinate(52.4987457370052f, 4.68023535430022f));
            //locations.Add(new Coordinate(52.6363404942514f, 4.67991624329538f));
            //locations.Add(new Coordinate(51.4900681859263f, 4.73937859369871f));
            //locations.Add(new Coordinate(52.0142628865469f, 4.74047961416205f));

            //locations.Add(new Coordinate(51.609237991083f, 4.75459351021089f));
            //locations.Add(new Coordinate(52.5055496191764f, 4.75098685137045f));
            //locations.Add(new Coordinate(52.8950042926721f, 4.75040390300744f));
            //locations.Add(new Coordinate(51.6075902253105f, 4.76946780021014f));

            //locations.Add(new Coordinate(52.5003827039214f, 4.76745208019189f));
            //locations.Add(new Coordinate(52.9645384587543f, 4.76424510643932f));
            //locations.Add(new Coordinate(52.3671254793025f, 4.78381534929832f));
            //locations.Add(new Coordinate(52.4620471479101f, 4.79142682057847f));

            //locations.Add(new Coordinate(51.5944741033001f, 4.80873068979403f));
            //locations.Add(new Coordinate(51.67264535632f, 4.81223756108259f));
            //locations.Add(new Coordinate(52.6747386682715f, 4.83548471695039f));
            //locations.Add(new Coordinate(52.4239533338544f, 4.84409369999578f));

            //locations.Add(new Coordinate(51.6592122130299f, 4.86290275608419f));
            //locations.Add(new Coordinate(52.3142589013458f, 4.86021056329912f));
            //locations.Add(new Coordinate(52.308419294888f, 4.86431529494027f));
            //locations.Add(new Coordinate(52.3325988299922f, 4.86924485325601f));

            //locations.Add(new Coordinate(52.441427697235f, 4.87556902543322f));
            ////locations.Add(new Coordinate(52.3407209425313f, 4.87901828854187f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(52.4064466407207f, 4.91929707029637f));
            //locations.Add(new Coordinate(52.3049590955861f, 4.92239758080038f));

            //locations.Add(new Coordinate(52.2213788672461f, 4.9361795451301f));
            //locations.Add(new Coordinate(52.5230365285966f, 4.95658172823013f));
            ////locations.Add(new Coordinate(52.5028568634169f, 4.9658546472752f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.5790608693337f, 5.02622474557878f));

            //locations.Add(new Coordinate(51.5954017326863f, 5.02697027911881f));
            //locations.Add(new Coordinate(52.0991238719383f, 5.03544427998566f));
            ////locations.Add(new Coordinate(52.4691524020984f, 5.06760312593819f)); // TODO: check this one!
            //locations.Add(new Coordinate(52.4999744556017f, 5.07961577257201f));

            //locations.Add(new Coordinate(52.0332123262203f, 5.08288660318986f));
            //locations.Add(new Coordinate(52.0284371944383f, 5.08675902212432f));
            //locations.Add(new Coordinate(51.5906128897953f, 5.0923309275563f));
            //locations.Add(new Coordinate(52.0274623387417f, 5.09100457589878f));

            //locations.Add(new Coordinate(52.077262534655f, 5.09701386081996f));
            //// locations.Add(new Coordinate(52.0154530798453f, 5.09964275829296f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(52.093086510913f, 5.10589600715391f));
            //locations.Add(new Coordinate(52.1176248582664f, 5.11749695535551f));

            //locations.Add(new Coordinate(52.2307867072627f, 5.15955425619725f));
            //locations.Add(new Coordinate(52.279469859349f, 5.16023229029885f));
            //locations.Add(new Coordinate(52.0282986673713f, 5.16231501817521f));
            //locations.Add(new Coordinate(52.3943883651209f, 5.17108898653391f));

            //locations.Add(new Coordinate(52.0953184935833f, 5.18048379794206f));
            //locations.Add(new Coordinate(52.6733354009031f, 5.18269384553779f));
            //locations.Add(new Coordinate(52.3574594131722f, 5.19053867437484f));
            //locations.Add(new Coordinate(52.3713213524652f, 5.21661232627892f));

            ////locations.Add(new Coordinate(52.3750909880937f, 5.2175262063451f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.9488138555106f, 5.2436063189251f));
            //locations.Add(new Coordinate(52.6956415350805f, 5.24218393762625f));
            ////locations.Add(new Coordinate(52.0762941797675f, 5.2627121578818f)); // TODO: check this one!

            //locations.Add(new Coordinate(51.6601260330373f, 5.28724463451229f));
            //locations.Add(new Coordinate(52.3988175719495f, 5.29513805891597f));
            //locations.Add(new Coordinate(52.2172626350128f, 5.30131299101593f));
            //locations.Add(new Coordinate(51.5081090258226f, 5.31232054583341f));

            ////locations.Add(new Coordinate(51.6379369187721f, 5.34921917276022f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.4899839879105f, 5.38428884894828f));
            //locations.Add(new Coordinate(51.2703486719698f, 5.39849010751468f));
            //locations.Add(new Coordinate(51.6843103252173f, 5.4008964061922f));

            //locations.Add(new Coordinate(51.8780072133106f, 5.41312518080964f));
            //locations.Add(new Coordinate(51.4058769187664f, 5.4187721096788f));
            //locations.Add(new Coordinate(51.6794795344445f, 5.41951933475107f));
            //locations.Add(new Coordinate(52.1691201805928f, 5.42609712778071f));

            //locations.Add(new Coordinate(52.1935144039048f, 5.43097039313578f));
            //locations.Add(new Coordinate(51.8911147298923f, 5.44511219450233f));
            //locations.Add(new Coordinate(51.4273416855176f, 5.45188001098565f));
            //locations.Add(new Coordinate(51.4736068444085f, 5.45256233956684f));

            //locations.Add(new Coordinate(51.3620799769146f, 5.45400318878235f));
            //locations.Add(new Coordinate(52.5175472808679f, 5.46010317808526f));
            //locations.Add(new Coordinate(52.58068555347f, 5.46288189659904f));
            //locations.Add(new Coordinate(52.221642968099f, 5.46896499265992f));

            //locations.Add(new Coordinate(51.5501760794319f, 5.48692750055501f));
            //locations.Add(new Coordinate(52.1210338680939f, 5.50210930266252f));
            //locations.Add(new Coordinate(51.771855271745f, 5.52626593840005f));
            //locations.Add(new Coordinate(51.4273379401605f, 5.53706513548397f));

            //locations.Add(new Coordinate(51.6214807937381f, 5.54713225485515f));
            //locations.Add(new Coordinate(51.6349386707925f, 5.55667662834161f));
            //locations.Add(new Coordinate(51.6245500586463f, 5.5584670397119f));
            //locations.Add(new Coordinate(51.4347908070715f, 5.55896458385053f));

            //locations.Add(new Coordinate(52.8963227113481f, 5.56871846358763f));
            //locations.Add(new Coordinate(52.0407272803493f, 5.56550329113018f));
            //locations.Add(new Coordinate(53.1460846885728f, 5.57110610658765f));
            //locations.Add(new Coordinate(51.959045201728f, 5.56725366122533f));

            //locations.Add(new Coordinate(52.0234651036824f, 5.56912041093975f));
            ////locations.Add(new Coordinate(51.659129382076f, 5.61185472946271f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.6549512483766f, 5.61926492657508f));
            //locations.Add(new Coordinate(53.1787399003623f, 5.63503731570147f));

            //locations.Add(new Coordinate(53.021478620633f, 5.64325669753244f));
            //locations.Add(new Coordinate(50.8425751390842f, 5.64952628513405f));
            //locations.Add(new Coordinate(50.8542435988198f, 5.65799111734601f));
            ////locations.Add(new Coordinate(52.0284865637708f, 5.67209220825719f)); // Street doesn't allow car traffic: failed

            //locations.Add(new Coordinate(51.9985441311422f, 5.67428672231132f));
            //locations.Add(new Coordinate(51.5649153503637f, 5.69327687666277f));
            //locations.Add(new Coordinate(51.3846561113897f, 5.7284476097383f));
            //locations.Add(new Coordinate(52.8651220556404f, 5.74919150293958f));

            //locations.Add(new Coordinate(51.2141118883542f, 5.75783736351718f));
            //locations.Add(new Coordinate(51.4645466518433f, 5.76898965799321f));
            //locations.Add(new Coordinate(51.8388449574935f, 5.80590736505856f));
            //locations.Add(new Coordinate(51.8492510833143f, 5.81577823772863f));

            //locations.Add(new Coordinate(51.4651243894088f, 5.81491426679475f));
            //locations.Add(new Coordinate(50.9880541391735f, 5.82366420331404f));
            //locations.Add(new Coordinate(53.2058026338445f, 5.84951782716405f));
            //locations.Add(new Coordinate(52.4482756327289f, 5.84965672963174f));

            //locations.Add(new Coordinate(51.0023156597146f, 5.85251773607731f));
            //locations.Add(new Coordinate(52.4192261902465f, 5.87230749491164f));
            //locations.Add(new Coordinate(51.8413820078189f, 5.8665221671461f));
            //locations.Add(new Coordinate(51.0639287107378f, 5.87160735867429f));

            //locations.Add(new Coordinate(51.0098921832427f, 5.88109089178501f));
            //locations.Add(new Coordinate(52.9583528795504f, 5.90900799608934f));
            //locations.Add(new Coordinate(51.9080026970741f, 5.92108497835333f));
            //locations.Add(new Coordinate(51.9604860885074f, 5.92229497910847f));

            //locations.Add(new Coordinate(52.1818577093218f, 5.94552188104727f));
            //locations.Add(new Coordinate(51.6632970215377f, 5.94040438309244f));
            ////locations.Add(new Coordinate(52.1837478757249f, 5.96457436656975f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(52.1846866981361f, 5.96701067595723f));

            //locations.Add(new Coordinate(51.9817362928879f, 5.96656783959875f));
            //locations.Add(new Coordinate(51.9854298087586f, 5.97788995109583f));
            //locations.Add(new Coordinate(50.9266058769463f, 5.96733902928824f));
            //locations.Add(new Coordinate(51.1782923093502f, 5.97522474371109f));

            //locations.Add(new Coordinate(52.2036764841865f, 5.99027648667866f));
            //locations.Add(new Coordinate(52.1878234490261f, 5.99274929635985f));
            //locations.Add(new Coordinate(52.2230775578896f, 5.99644226893289f));
            //locations.Add(new Coordinate(52.2079456685516f, 5.99643029419045f));

            ////locations.Add(new Coordinate(50.9236484827494f, 5.9808580205005f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(52.212686229265f, 5.99926900690086f));
            //locations.Add(new Coordinate(51.6646076127722f, 5.99495348183499f));
            //locations.Add(new Coordinate(51.2031425712085f, 5.98889667570833f));

            //locations.Add(new Coordinate(52.3938964470039f, 6.04322274487732f));
            //locations.Add(new Coordinate(51.3575385065498f, 6.04879518879185f));
            //locations.Add(new Coordinate(52.5303980749083f, 6.07815481045921f));
            //locations.Add(new Coordinate(53.0963830111012f, 6.10435446385282f));

            //locations.Add(new Coordinate(52.5093425520689f, 6.09669016906031f));
            //locations.Add(new Coordinate(51.3912973312615f, 6.09877827059061f));
            //locations.Add(new Coordinate(53.1092536540137f, 6.14260510889319f));
            //locations.Add(new Coordinate(52.491356511886f, 6.14064230010144f));

            //locations.Add(new Coordinate(52.2587931446175f, 6.15064157539082f));
            ////locations.Add(new Coordinate(53.4787861030761f, 6.1747521564305f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.907643848406f, 6.15076160107651f));
            //locations.Add(new Coordinate(51.365629964504f, 6.15182307626683f));

            //locations.Add(new Coordinate(52.236093727349f, 6.17659843473745f));
            //locations.Add(new Coordinate(52.0646707454328f, 6.18098698890509f));
            //locations.Add(new Coordinate(52.7149980775275f, 6.19423785199596f));
            //locations.Add(new Coordinate(51.3505307364357f, 6.17361334167817f));

            //locations.Add(new Coordinate(52.2382418452137f, 6.21090347356604f));
            //locations.Add(new Coordinate(52.1345362720616f, 6.2351735527517f));
            //locations.Add(new Coordinate(52.3418803425697f, 6.26790894692625f));
            //locations.Add(new Coordinate(53.3118961259645f, 6.29767418137786f));

            ////locations.Add(new Coordinate(53.0128943581221f, 6.32103603588998f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(51.9479062188112f, 6.31345482517034f));
            //locations.Add(new Coordinate(53.1819825035149f, 6.37899201996084f));
            //locations.Add(new Coordinate(51.9037001076747f, 6.36742015469923f));

            //locations.Add(new Coordinate(52.1647262825431f, 6.40372567896385f));
            ////locations.Add(new Coordinate(52.5260453693715f, 6.42207091438668f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(52.2826925354459f, 6.42274101355686f));
            //locations.Add(new Coordinate(52.3741396686497f, 6.47055137700957f));

            //locations.Add(new Coordinate(52.3612475689542f, 6.50997128144003f));
            //locations.Add(new Coordinate(53.0083716406179f, 6.54568129435359f));
            //locations.Add(new Coordinate(52.1170571077943f, 6.52704272517736f));
            //locations.Add(new Coordinate(52.4599400524097f, 6.57150927300529f));

            //locations.Add(new Coordinate(53.2793169457619f, 6.60078194363641f));
            //locations.Add(new Coordinate(52.3576257051074f, 6.59152636359631f));
            //locations.Add(new Coordinate(52.3435895765147f, 6.64290988069899f));
            ////locations.Add(new Coordinate(52.3657273689638f, 6.67862433986395f)); // Street doesn't allow car traffic: failed

            //locations.Add(new Coordinate(52.8290815514676f, 6.6944367120555f));
            ////locations.Add(new Coordinate(52.3464241891806f, 6.68457035803287f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(53.1730121868811f, 6.75628477451471f));
            ////locations.Add(new Coordinate(52.9964589755564f, 6.75829825735671f)); // Street doesn't allow car traffic: failed

            //locations.Add(new Coordinate(53.0104042219775f, 6.76086234559778f));
            ////locations.Add(new Coordinate(52.1682897042589f, 6.74075361349117f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(52.6668398676016f, 6.75843159955385f));
            //locations.Add(new Coordinate(52.2417171648453f, 6.76068902214294f));

            ////locations.Add(new Coordinate(52.2649281481956f, 6.79163185935606f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(52.2206929699446f, 6.83728066095978f));
            //locations.Add(new Coordinate(52.4081097131379f, 6.87420189159591f));
            //locations.Add(new Coordinate(52.1908575845447f, 6.8728231939004f));

            //locations.Add(new Coordinate(52.7517359438934f, 6.89204843070648f));
            ////locations.Add(new Coordinate(52.2216828725473f, 6.88986576006998f)); // Street doesn't allow car traffic: failed
            //locations.Add(new Coordinate(52.3174899907749f, 6.90375101987198f));
            //locations.Add(new Coordinate(52.2874026392575f, 6.92158255694133f));

            //locations.Add(new Coordinate(53.1756794106863f, 6.97176783559564f));
            //locations.Add(new Coordinate(52.2165225156316f, 6.9410299706219f));
            //locations.Add(new Coordinate(52.221696027067f, 6.97341739877845f));
            //locations.Add(new Coordinate(53.1035603672337f, 7.00781270448309f));
            
            for (var i = 0; i < locations.Length; i++)
            {
                System.Console.WriteLine("Testing location {0}/{1} @ {2}", i + 1, locations.Length, locations[i].ToInvariantString());
                Netherlands.TestEncodeDecodePointAlongLine(coder, locations[i].Item1.Latitude, locations[i].Item1.Longitude, 30);
            }
        }

        /// <summary>
        /// Tests encoding/decoding a point along line location.
        /// </summary>
        public static void TestEncodeDecodePointAlongLine(Coder coder, float latitude, float longitude, float tolerance)
        {
            var decoded = EncodeDecodePointAlongLine(coder, latitude, longitude);

            var distance = Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(latitude, longitude, decoded.Latitude, decoded.Longitude);

            Assert.IsTrue(distance < tolerance);
        }

        /// <summary>
        /// Encode/decode a point along line location.
        /// </summary>
        private static ReferencedPointAlongLine EncodeDecodePointAlongLine(Coder coder, float latitude, float longitude)
        {
            RouterPoint resolvedPoint;
            var location = coder.BuildPointAlongLine(latitude, longitude, out resolvedPoint);
            var locationGeoJson = location.ToFeatures(coder.Router.Db).ToGeoJson();
            var resolvedLocation = resolvedPoint.LocationOnNetwork(coder.Router.Db);
            
            var encoded = coder.Encode(location);

            var decoded = coder.Decode(encoded) as ReferencedPointAlongLine;
            var decodedGeoJson = decoded.ToFeatures(coder.Router.Db).ToGeoJson();
            return decoded;
        }
    }
}