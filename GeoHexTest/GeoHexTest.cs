using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Compatibility;

namespace GeoHex
{
    [TestFixture]
    public class GeoHexTest
    {
        private const double LOCATION_PRECISION = 0.0000000000001;

        [Test]
        public void Pow3Test()
        {
            for (int i = 0; i < 20; i++)
            {
                Assert.AreEqual(Pow3.Calc(i), (long) Math.Pow(3, i));
            }
        }

        [Test]
        public void ZoneEqualsTest()
        {
            var zone1 = new Zone(0.0, 0.0, 0, 0, "A");
            var zone2 = new Zone(0.0, 0.0, 0, 0, "A");
            var zone3 = new Zone(0.0, 0.0, 0, 0, "B");
            Assert.AreEqual(zone1, zone2);
            Assert.AreNotEqual(zone1, zone3);
        }

        [Test]
        public void ZoneHashCodeTest()
        {
            var zone1 = new Zone(0.0, 0.0, 0, 0, "A");
            var zone2 = new Zone(0.0, 0.0, 0, 0, "A");
            var zone3 = new Zone(0.0, 0.0, 0, 0, "B");
            Assert.AreEqual(zone1.GetHashCode(), zone2.GetHashCode());
            Assert.AreNotEqual(zone1.GetHashCode(), zone3.GetHashCode());
        }

        [Test]
        public void ZoneGetHexCoordsTest()
        {
            foreach (string[] v in ParseCsv("../../resources/GetHexCoords_v3.2.csv"))
            {
                double lat = double.Parse(v[0]);
                double lon = double.Parse(v[1]);
                int level = int.Parse(v[2]);
                Zone z = GEOHEX.GetZoneByLocation(lat, lon, level);

                double[][] expectedPolygon =
                {
                    new double[] {double.Parse(v[3]), double.Parse(v[4])},
                    new double[] {double.Parse(v[5]), double.Parse(v[6])},
                    new double[] {double.Parse(v[7]), double.Parse(v[8])},
                    new double[] {double.Parse(v[9]), double.Parse(v[10])},
                    new double[] {double.Parse(v[11]), double.Parse(v[12])},
                    new double[] {double.Parse(v[13]), double.Parse(v[14])},
                };

                AssertPolygon(expectedPolygon, z.GetHexCoords());
            }
        }

        [Test]
        public void ZoneGetHexSize()
        {
            foreach (String[] v in ParseCsv("../../resources/GetHexSize_v3.2.csv"))
            {
                double lat = double.Parse(v[0]);
                double lon = double.Parse(v[1]);
                int level = int.Parse(v[2]);
                double expected_hex_size = double.Parse(v[3]);
                Zone z = GEOHEX.GetZoneByLocation(lat, lon, level);
                Assert.AreEqual(expected_hex_size, z.GetHexSize(), LOCATION_PRECISION);
            }
        }

        [Test]
        public void LocationEquals()
        {
            Location location = new Location(1.0, 1.0);
            Location same = new Location(1.0, 1.0);
            Location other = new Location(1.0, 2.0);
            Assert.True(location.Equals(same));
            Assert.False(location.Equals(other));
        }

        [Test]
        public void LocationGetHashCode()
        {
            Location location = new Location(1.0, 1.0);
            Location same = new Location(1.0, 1.0);
            Location other = new Location(1.0, 2.0);
            Assert.AreEqual(location.GetHashCode(), same.GetHashCode());
            Assert.AreNotEqual(location.GetHashCode(), other.GetHashCode());
        }

        [Test]
        public void XYEquals()
        {
            XY xy = new XY(1, 1);
            XY same = new XY(1, 1);
            XY other = new XY(1, 2);
            Assert.True(xy.Equals(same));
            Assert.False(xy.Equals(other));
        }

        [Test]
        public void XYGetHashCode()
        {
            XY xy = new XY(1, 1);
            XY same = new XY(1, 1);
            XY other = new XY(1, 2);
            Assert.AreEqual(xy.GetHashCode(), same.GetHashCode());
            Assert.AreNotEqual(xy.GetHashCode(), other.GetHashCode());
        }

        [Test]
        public void GetZoneByLocation()
        {
            foreach (string[] v in ParseCsv("../../resources/GetZoneByLocation_v3.2.csv"))
            {
                double lat = double.Parse(v[0]);
                double lon = double.Parse(v[1]);
                int level = int.Parse(v[2]);
                String code = v[3];
                Zone zone = GEOHEX.GetZoneByLocation(lat, lon, level);
                Assert.AreEqual(code, zone.code);
            }
        }

        [Test]
        public void GetXYByLocation()
        {
            foreach (String[] v in ParseCsv("../../resources/GetXYByLocation_v3.2.csv"))
            {
                double lat = double.Parse(v[0]);
                double lon = double.Parse(v[1]);
                int level = int.Parse(v[2]);
                double x = double.Parse(v[3]);
                double y = double.Parse(v[4]);
                XY xy = GEOHEX.GetXYByLocation(lat, lon, level);
                Assert.AreEqual(x, xy.x, LOCATION_PRECISION);
                Assert.AreEqual(y, xy.y, LOCATION_PRECISION);
            }
        }

        [Test]
        public void GetZoneByCode()
        {
            foreach (string[] v in ParseCsv("../../resources/GetZoneByCode_v3.2.csv"))
            {
                double lat = double.Parse(v[0]);
                double lon = double.Parse(v[1]);
                int level = int.Parse(v[2]);
                string code = v[3];
                Zone zone = GEOHEX.GetZoneByCode(code);
                Assert.AreEqual(zone.code, code);
                AssertLatitude(zone.latitude, lat);
                AssertLongitude(zone.longitude, lon);
                Assert.AreEqual(zone.GetLevel(), level);
            }
        }

        [Test]
        public void GetZoneByXY()
        {
            foreach (string[] v in ParseCsv("../../resources/GetZoneByXY_v3.2.csv"))
            {
                double x = double.Parse(v[0]);
                double y = double.Parse(v[1]);
                double lat = double.Parse(v[2]);
                double lon = double.Parse(v[3]);
                int level = int.Parse(v[4]);
                String code = v[5];
                Zone zone = GEOHEX.GetZoneByXY(x, y, level);
                AssertLatitude(lat, zone.latitude);
                AssertLongitude(lon, zone.longitude);
                Assert.AreEqual(level, zone.GetLevel());
                Assert.AreEqual(code, zone.code);
            }
        }

        [Test]
        public void AdjustXY()
        {
            foreach (string[] v in ParseCsv("../../resources/AdjustXY_v3.2.csv"))
            {
                long x = long.Parse(v[0]);
                long y = long.Parse(v[1]);
                int level = int.Parse(v[2]);
                double ex = double.Parse(v[3]);
                double ey = double.Parse(v[4]);
                XY resultXY = GEOHEX.AdjustXY(x, y, level);
                Assert.AreEqual(ex, resultXY.x, 0);
                Assert.AreEqual(ey, resultXY.y, 0);
            }
        }

        private List<string[]> ParseCsv(string path)
        {
            List<string[]> wordsList = new List<string[]>();

            using (StreamReader reader = new StreamReader(path))
            {
                while (reader.Peek() >= 0)
                {
                    string lineString = reader.ReadLine();
                    if (lineString[0] == '#')
                    {
                        continue;
                    }

                    string[] words = lineString.Split(new Char[] {','});
                    wordsList.Add(words);
                }
            }

            return wordsList;
        }

        private void AssertPolygon(double[][] expectedPolygon, Location[] polygon)
        {
            for (int i = 0; i < expectedPolygon.Length; i++)
            {
                double[] latlon = expectedPolygon[i];
                AssertLatitude(latlon[0], polygon[i].latitude);
                AssertLongitude(latlon[1], polygon[i].longitude);
            }
        }

        private void AssertLatitude(double expectedLatitude, double latitude)
        {
            Assert.AreEqual(expectedLatitude, latitude, LOCATION_PRECISION);
        }

        private void AssertLongitude(double expectedLongitude, double longitude)
        {
            Assert.AreEqual(expectedLongitude, longitude, LOCATION_PRECISION);

            if (Math.Abs(expectedLongitude - longitude) + LOCATION_PRECISION >= 360.0)
            {
                if (longitude >= 0)
                {
                    Assert.AreEqual(expectedLongitude, longitude - 360.0, LOCATION_PRECISION);
                }
                else
                {
                    Assert.AreEqual(expectedLongitude, longitude + 360.0, LOCATION_PRECISION);
                }
            }
            else
            {
                Assert.AreEqual(expectedLongitude, longitude, LOCATION_PRECISION);
            }
        }
    }
}