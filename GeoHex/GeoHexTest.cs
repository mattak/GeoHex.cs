using System;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GeoHex;

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
    public void ZoneTest()
    {
        var zone1 = new Zone(0.0, 0.0, 0, 0, "A");
        var zone2 = new Zone(0.0, 0.0, 0, 0, "A");
        var zone3 = new Zone(0.0, 0.0, 0, 0, "B");
        Assert.AreEqual(zone1, zone2);
        Assert.AreNotEqual(zone1, zone3);
    }
}