using System;
using NUnit.Framework;

namespace GeoHex
{
    [TestFixture]
    public class GeoHexTest
    {
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
        public void ZoneGetCoordsTest()
        {
            // TODO: write test
        }

        [Test]
        public void ZoneGetHexSize()
        {
            // TODO: write test
        }

        [Test]
        public void LocationEquals()
        {
            // TODO: write test
        }

        [Test]
        public void LocationGetHashCode()
        {
            // TODO: write test
        }

        [Test]
        public void GetZoneByLocation()
        {
            // TODO: write test
        }

        [Test]
        public void GetXYByLocation()
        {
            // TODO: write test
        }

        [Test]
        public void GetZoneByCode()
        {
            // TODO: write test
        }

        [Test]
        public void GetZoneByXY()
        {
            // TODO: write test
        }

        [Test]
        public void AdjustXY()
        {
            // TODO: write test
        }
    }
}