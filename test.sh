#!/bin/sh
xbuild /p:Configuration=Debug GeoHex.sln
mono ./packages/NUnit.ConsoleRunner.3.2.1/tools/nunit3-console.exe GeoHexTest/bin/Debug/GeoHexTest.dll
