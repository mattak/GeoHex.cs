#!/bin/bash -xe
rm -rf GeoHex{,Test}/{bin,obj}
rm GeoHex/*.nupkg

xbuild /p:Configuration=Release20 GeoHex.sln
xbuild /p:Configuration=Release35 GeoHex.sln
xbuild /p:Configuration=Release40 GeoHex.sln
xbuild /p:Configuration=Release45 GeoHex.sln

pushd GeoHex
nuget pack GeoHex.nuspec
popd
