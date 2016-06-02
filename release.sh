#!/bin/bash -xe
rm -rf GeoHex{,Test}/{bin,obj}

if [ "$(find GeoHex -type f -name '*.nupkg')" != "" ]; then
    rm GeoHex/*.nupkg
fi

xbuild /p:Configuration=Release20 GeoHex.sln
xbuild /p:Configuration=Release35 GeoHex.sln
xbuild /p:Configuration=Release40 GeoHex.sln
xbuild /p:Configuration=Release45 GeoHex.sln

pushd GeoHex
nuget pack GeoHex.nuspec
popd
