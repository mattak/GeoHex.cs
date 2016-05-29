using System;
using NUnit.Framework;

namespace GeoHex
{
    [TestFixture]
    public class GeoHexPerformanceTest
    {
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
    }
}