using System;
using NUnit.Framework;

namespace GeoHex
{
    [TestFixture]
    public class GeoHexPerformanceTest
    {
        [Test]
        public void CeilTest()
        {
            const int MaxRepeat = 10000000;
            int result = 0;
            {
                TimeWatch.Reset();
                TimeWatch.Resume();
                for (int i=0; i<MaxRepeat; i++) {
                    result = (int)Math.Ceiling((double)i * 0.5);
                }
                TimeWatch.Pause(MaxRepeat);
                TimeWatch.OutputResult("Math.Ceiling" + result.ToString());
            }
            {
                TimeWatch.Reset();
                TimeWatch.Resume();
                for (int i=0; i<MaxRepeat; i++) {
                    result = (i>>1) + (i&0x1);
                }
                TimeWatch.Pause(MaxRepeat);
                TimeWatch.OutputResult("CustomCeiling" + result.ToString());
            }
        }

        [Test]
        public void Pow3Test()
        {
            const int MaxRepeat = 1000000;
            const int MaxLevel = 19;

            {
                TimeWatch.Reset();
                TimeWatch.Resume();

                for (int repeat = 0; repeat < MaxRepeat; repeat++)
                {
                    for (int i = 0; i < MaxLevel; i++)
                    {
                        Pow3.Calc(i);
                    }
                }

                TimeWatch.Pause(MaxLevel*MaxRepeat);
                TimeWatch.OutputResult("Pow3");
            }

            {
                TimeWatch.Reset();
                TimeWatch.Resume();

                for (int repeat = 0; repeat < MaxRepeat; repeat++)
                {
                    for (int i = 0; i < MaxLevel; i++)
                    {
                        Math.Pow(3,i);
                    }
                }

                TimeWatch.Pause(MaxLevel*MaxRepeat);
                TimeWatch.OutputResult("Math.Pow3");
            }
        }

        [Test]
        public void ZoneGetHexCoordsTest()
        {
            TimeWatch.Reset();
            const int MaxReat = 1000000;

            {
                double lat = 33.35137950146622;
                double lon = 135.6104480957031;
                int level = 0;
                Zone z = GEOHEX.GetZoneByLocation(lat, lon, level);

                TimeWatch.Resume();
                for (int repeat = 0; repeat < MaxReat; repeat++)
                {
                    z.GetHexCoords();
                }
                TimeWatch.Pause(MaxReat);
            }

            TimeWatch.OutputResult("GetHexCoords");
        }

        [Test]
        public void ZoneGetHexSize()
        {
            const int MaxRepeat = 10000000;
            TimeWatch.Reset();

            {
                double lat = 33.35137950146622;
                double lon = 135.6104480957031;
                int level = 0;
                Zone z = GEOHEX.GetZoneByLocation(lat, lon, level);

                TimeWatch.Resume();
                for (int repeat = 0; repeat < MaxRepeat; repeat++)
                {
                    z.GetHexSize();
                }
                TimeWatch.Pause(MaxRepeat);
            }

            TimeWatch.OutputResult("GetHexSize");
        }

        [Test]
        public void GetZoneByLocation()
        {
            const int MaxRepeat = 100000;
            TimeWatch.Reset();

            {
                double lat = 33.35137950146622;
                double lon = 135.6104480957031;
                int level = 0;

                TimeWatch.Resume();
                for (int repeat = 0; repeat < MaxRepeat; repeat++)
                {
                    GEOHEX.GetZoneByLocation(lat, lon, level);
                }
                TimeWatch.Pause(MaxRepeat);
            }

            TimeWatch.OutputResult("GetZoneByLocation");
        }

        [Test]
        public void GetXYByLocation()
        {
            const int MaxRepeat = 1000000;
            TimeWatch.Reset();

            {
                double lat = 33.35137950146622;
                double lon = 135.6104480957031;
                int level = 0;

                TimeWatch.Resume();
                for (int repeat = 0; repeat < MaxRepeat; repeat++)
                {
                    GEOHEX.GetXYByLocation(lat, lon, level);
                }
                TimeWatch.Pause(MaxRepeat);
            }

            TimeWatch.OutputResult("GetXYByLocation");
        }

        [Test]
        public void GetZoneByCode()
        {
            const int MaxRepeat = 100000;
            TimeWatch.Reset();

            {
                string code = "XM";

                TimeWatch.Resume();
                for (int repeat = 0; repeat < MaxRepeat; repeat++)
                {
                    GEOHEX.GetZoneByCode(code);
                }
                TimeWatch.Pause(MaxRepeat);
            }

            TimeWatch.OutputResult("GetZoneByCode");
        }

        [Test]
        public void GetZoneByXY()
        {
            const int MaxRepeat = 100000;
            TimeWatch.Reset();

            {
                double x = 5;
                double y = -2;
                int level = 0;

                TimeWatch.Resume();
                for (int repeat = 0; repeat < MaxRepeat; repeat++)
                {
                    GEOHEX.GetZoneByXY(x, y, level);
                }
                TimeWatch.Pause(MaxRepeat);
            }

            TimeWatch.OutputResult("GetZoneByXY");
        }

        [Test]
        public void AdjustXY()
        {
            const int MaxRepeat = 10000000;
            TimeWatch.Reset();

            {
                int x = -10;
                int y = -10;
                int level = 0;

                TimeWatch.Resume();
                for (int repeat = 0; repeat < MaxRepeat; repeat++)
                {
                    GEOHEX.AdjustXY(x, y, level);
                }
                TimeWatch.Pause(MaxRepeat);
            }

            TimeWatch.OutputResult("AdjustXY");
        }
    }
}
